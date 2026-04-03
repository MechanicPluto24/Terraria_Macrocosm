using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.UI.Elements;
using Terraria.Localization;
using Terraria.UI;
using Terraria.GameContent.Creative;
using Terraria.DataStructures;
using Terraria.ID;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Macrocosm.Common.UI.Themes;

namespace Macrocosm.Common.UI;

/// <summary>
/// Reusable item browser with search, category filter tabs, and a scrollable item grid.
/// Modeled after Terraria's Journey/Creative mode item list.
/// <para>
/// Layout: filter tabs sit at the top, visually touching the top edge of a content panel
/// that contains both the search bar and the scrollable item grid.
/// </para>
/// <para>
/// Usage:
/// <code>
/// var browser = new UIItemBrowser();
/// browser.SetEntryFilter(item => myCustomCheck(item));
/// browser.SetEntries(myItems);
/// browser.OnEntrySelected += (index, item) => HandleSelection(index, item);
/// </code>
/// </para>
/// </summary>
public class UIItemBrowser : UIElement
{
    private readonly List<Item> allEntries = new();
    private List<(int originalIndex, Item item)> displayedEntries = new();

    private readonly EntryFilterer<Item, IItemEntryFilter> filterer;
    private PredicateFilter entryFilter;

    private UISearchBar searchBar;
    private UIPanel searchBoxPanel;
    private UICreativeItemsInfiniteFilteringOptions filterTabs;
    private UIPanel contentPanel;
    private UIScrollableListPanel gridPanel;

    private bool didClickSomething;
    private bool didClickSearchBar;

    /// <summary>
    /// Fired when an entry is clicked. Parameters: (original index in the entries list, the Item).
    /// </summary>
    public event Action<int, Item> OnEntrySelected;

    /// <summary>Items per row in the grid.</summary>
    public int IconsPerRow { get; set; } = 10;

    /// <summary>Size of each item slot in pixels.</summary>
    public float IconSize { get; set; } = 48f;

    /// <param name="filters">
    /// Category filters to show as tabs. If null, uses the full set from the Creative menu
    /// (Weapon, Armor, Vanity, BuildingBlock, Furniture, Accessories, MiscAccessories,
    /// Consumables, Tools, Materials, Misc).
    /// A <see cref="ItemFilters.MiscFallback"/> is appended automatically.
    /// </param>
    public UIItemBrowser(List<IItemEntryFilter> filters = null)
    {
        filterer = new EntryFilterer<Item, IItemEntryFilter>();

        var baseFilters = filters ?? DefaultFilters();
        var allFilters = new List<IItemEntryFilter>(baseFilters)
        {
            new ItemFilters.MiscFallback(baseFilters)
        };

        filterer.AddFilters(allFilters);
        filterer.SetSearchFilterObject(new ItemFilters.BySearch());
    }

    private static List<IItemEntryFilter> DefaultFilters() =>
    [
        new ItemFilters.Weapon(),
        new ItemFilters.Armor(),
        new ItemFilters.Vanity(),
        new ItemFilters.BuildingBlock(),
        new ItemFilters.Furniture(),
        new ItemFilters.Accessories(),
        new ItemFilters.MiscAccessories(),
        new ItemFilters.Consumables(),
        new ItemFilters.Tools(),
        new ItemFilters.Materials(),
    ];

    /// <summary>
    /// Set a per-entry filter predicate. Items for which the predicate returns false
    /// are excluded from display, on top of search and category filters.
    /// Pass null to clear.
    /// </summary>
    public void SetEntryFilter(Predicate<Item> predicate)
    {
        if (entryFilter != null)
            filterer.AlwaysActiveFilters.Remove(entryFilter);

        if (predicate != null)
        {
            entryFilter = new PredicateFilter(predicate);
            filterer.AlwaysActiveFilters.Add(entryFilter);
        }
        else
        {
            entryFilter = null;
        }
    }

    /// <summary>
    /// Provide the full list of items to browse. Each entry is tracked by its index
    /// so that <see cref="OnEntrySelected"/> returns the correct original index
    /// even after filtering.
    /// </summary>
    public void SetEntries(List<Item> items)
    {
        allEntries.Clear();
        allEntries.AddRange(items);
        RefreshGrid();
    }

    /// <summary>
    /// Force a refresh of the displayed grid (e.g. after changing the entry filter).
    /// </summary>
    public void Refresh() => RefreshGrid();

    public override void OnInitialize()
    {
        Width.Set(0f, 1f);
        Height.Set(0f, 1f);
        SetPadding(0f);

        // Height reserved at the top for filter tabs (they overlap the panel's top edge)
        const float filterTabHeight = 38f;

        // Content panel: contains search bar + item grid
        contentPanel = new UIPanel()
        {
            Width = new(0f, 1f),
            Height = new(-filterTabHeight, 1f),
            Top = new(filterTabHeight, 0f),
            BackgroundColor = UITheme.Current.PanelStyle.BackgroundColor,
            BorderColor = UITheme.Current.PanelStyle.BorderColor,
        };
        contentPanel.SetPadding(6f);
        Append(contentPanel);

        // Filter tabs: sit at the top, touching the content panel
        filterTabs = new UICreativeItemsInfiniteFilteringOptions(filterer, "ItemBrowserFilter");
        filterTabs.OnClickingOption += RefreshGrid;
        filterTabs.Top = new(2f, 0f);
        filterTabs.HAlign = 0.5f;
        Append(filterTabs);

        // Search bar (inside content panel, pushed down for tab overhang)
        const float searchAreaTop = 10f;
        const float searchAreaHeight = 28f;
        UIElement searchArea = new()
        {
            Width = new(0f, 1f),
            Height = new(searchAreaHeight, 0f),
            Top = new(searchAreaTop, 0f),
        };
        searchArea.SetPadding(0f);
        contentPanel.Append(searchArea);
        BuildSearchBar(searchArea);

        // Scrollable item grid (inside content panel, just below search)
        const float searchToGridGap = 2f;
        float gridTop = searchAreaTop + searchAreaHeight + searchToGridGap;
        gridPanel = new UIScrollableListPanel()
        {
            Width = new(0, 1f),
            Height = new(-gridTop, 1f),
            Top = new(gridTop, 0f),
            ListPadding = 0f,
            ListOuterPadding = -4f,
            HideScrollbarIfNotScrollable = true,
            ScrollbarHAlign = 1f,
            BackgroundColor = Color.Transparent,
            BorderColor = Color.Transparent,
            PaddingLeft = 8f,
            PaddingRight = 0f,
        };
        contentPanel.Append(gridPanel);

        RefreshGrid();
    }

    private void BuildSearchBar(UIElement searchArea)
    {
        // Search icon button (left side)
        UIImageButton searchButton = new(Main.Assets.Request<Texture2D>("Images/UI/Bestiary/Button_Search"))
        {
            VAlign = 0.5f,
            HAlign = 0f,
        };
        searchButton.OnLeftClick += ClickSearchArea;
        searchButton.SetHoverImage(Main.Assets.Request<Texture2D>("Images/UI/Bestiary/Button_Search_Border"));
        searchButton.SetVisibility(1f, 1f);
        searchArea.Append(searchButton);

        // Search box panel (right of search icon)
        searchBoxPanel = new UIPanel()
        {
            Width = new(-searchButton.Width.Pixels - 3f, 1f),
            Height = new(0f, 1f),
            VAlign = 0.5f,
            HAlign = 1f,
            BackgroundColor = UITheme.Current.PanelStyle.BackgroundColor,
            BorderColor = UITheme.Current.PanelStyle.BorderColor,
        };
        searchBoxPanel.SetPadding(0f);
        searchArea.Append(searchBoxPanel);

        // Search bar text (grayed-out placeholder handled by UISearchBar)
        searchBar = new UISearchBar(Language.GetText("UI.PlayerNameSlot"), 0.8f)
        {
            Width = new(0f, 1f),
            Height = new(0f, 1f),
            HAlign = 0f,
            VAlign = 0.5f,
            IgnoresMouseInteraction = true,
        };

        searchBoxPanel.OnLeftClick += ClickSearchArea;
        searchBar.OnContentsChanged += OnSearchContentsChanged;
        searchBar.OnStartTakingInput += OnStartTakingInput;
        searchBar.OnEndTakingInput += OnEndTakingInput;
        searchBar.OnCanceledTakingInput += OnCanceledInput;
        searchBoxPanel.Append(searchBar);

        // Cancel/clear button (right side, inside the panel)
        UIImageButton cancelButton = new(Main.Assets.Request<Texture2D>("Images/UI/SearchCancel"))
        {
            HAlign = 1f,
            VAlign = 0.5f,
            Left = new(-2f, 0f),
        };
        cancelButton.OnLeftClick += SearchCancelButton_OnClick;
        cancelButton.OnMouseOver += (_, _) => SoundEngine.PlaySound(SoundID.MenuTick);
        searchBoxPanel.Append(cancelButton);
    }

    private void ClickSearchArea(UIMouseEvent evt, UIElement listeningElement)
    {
        if (evt.Target.Parent != searchBoxPanel)
        {
            searchBar.ToggleTakingText();
            didClickSearchBar = true;
        }
    }

    private void SearchCancelButton_OnClick(UIMouseEvent evt, UIElement listeningElement)
    {
        if (searchBar.HasContents)
        {
            searchBar.SetContents(null, forced: true);
            SoundEngine.PlaySound(SoundID.MenuClose);
        }
        else
        {
            SoundEngine.PlaySound(SoundID.MenuTick);
        }
    }

    private void OnStartTakingInput()
    {
        searchBoxPanel.BorderColor = Main.OurFavoriteColor;
    }

    private void OnEndTakingInput()
    {
        searchBoxPanel.BorderColor = UITheme.Current.PanelStyle.BorderColor;
    }

    private void OnCanceledInput()
    {
        searchBar.ToggleTakingText();
    }

    public override void LeftClick(UIMouseEvent evt)
    {
        base.LeftClick(evt);
        didClickSomething = true;
    }

    public override void RightClick(UIMouseEvent evt)
    {
        base.RightClick(evt);
        didClickSomething = true;
    }

    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);

        // Dismiss search bar when clicking elsewhere (same logic as Creative menu)
        if (didClickSomething && !didClickSearchBar && searchBar.IsWritingText)
            searchBar.ToggleTakingText();

        didClickSomething = false;
        didClickSearchBar = false;
    }

    private void RefreshGrid()
    {
        if (gridPanel is null)
            return;

        gridPanel.ClearList();

        displayedEntries = Enumerable.Range(0, allEntries.Count)
            .Where(i => filterer.FitsFilter(allEntries[i]))
            .Select(i => (i, allEntries[i]))
            .OrderBy(e => e.Item2.Name)
            .ToList();

        int iconsPerRow = IconsPerRow;
        float iconSize = IconSize;
        float iconOffsetLeft = displayedEntries.Count <= iconsPerRow * 5 ? 8f : 2f;
        int rowCount = (displayedEntries.Count + iconsPerRow - 1) / iconsPerRow;

        for (int row = 0; row < rowCount; row++)
        {
            int startIdx = row * iconsPerRow;
            int endIdx = Math.Min(startIdx + iconsPerRow, displayedEntries.Count);

            UIElement rowElement = new()
            {
                Width = new(64f, 1f),
                Height = new(iconSize, 0f),
            };
            rowElement.SetPadding(0f);

            for (int i = startIdx; i < endIdx; i++)
            {
                var (originalIndex, item) = displayedEntries[i];
                Item displayItem = item.Clone();

                UIInventorySlot slot = new(ref displayItem)
                {
                    Width = new(iconSize, 0f),
                    Height = new(iconSize, 0f),
                    Left = new((i - startIdx) * iconSize + iconOffsetLeft, 0f),
                    Top = new(0f, 0f),
                    CanInteractWithItem = false,
                    HoverHighlightColor = Color.Gold
                };

                int capturedIndex = originalIndex;
                Item capturedItem = item;
                slot.OnLeftClick += (_, _) => OnEntrySelected?.Invoke(capturedIndex, capturedItem);
                rowElement.Append(slot);
            }

            gridPanel.Add(rowElement);
        }
    }

    private void OnSearchContentsChanged(string searchText)
    {
        filterer.SetSearchFilter(searchText);
        RefreshGrid();
    }

    /// <summary>
    /// Internal adapter that wraps a <see cref="Predicate{Item}"/> as an <see cref="IItemEntryFilter"/>
    /// for use with <see cref="EntryFilterer{T,U}.AlwaysActiveFilters"/>.
    /// </summary>
    private class PredicateFilter : IItemEntryFilter, IEntryFilter<Item>
    {
        private readonly Predicate<Item> predicate;
        public PredicateFilter(Predicate<Item> predicate) => this.predicate = predicate;
        public bool FitsFilter(Item entry) => predicate(entry);
        public string GetDisplayNameKey() => "";
        public UIElement GetImage() => new();
    }
}
