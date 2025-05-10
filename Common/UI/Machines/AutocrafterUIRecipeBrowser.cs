using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.Localization;
using Terraria.UI;
using Terraria.GameContent.Creative;
using Macrocosm.Content.Machines.Consumers.Autocrafters;
using Terraria.DataStructures;
using System;
using Terraria.ID;
using Microsoft.Xna.Framework;
using Macrocosm.Common.UI.Themes;

namespace Macrocosm.Common.UI.Machines
{
    public class UIAutocrafterRecipeBrowser : UIElement
    {
        private UISearchBar searchBar;
        private UIList recipeList;
        private UIScrollbar scrollbar;

        private EntryFilterer<Item, IItemEntryFilter> filterer;
        private EntrySorter<int, ICreativeItemSortStep> sorter;

        private readonly List<Recipe> availableRecipes = new();
        private List<Recipe> filteredRecipes = new();

        private readonly AutocrafterTEBase autocrafter;
        private UIListScrollablePanel recipeListPanel;
        private UIElement recipeGridRoot;

        public Action<Recipe> OnRecipeClicked { get; set; }

        public UIAutocrafterRecipeBrowser(AutocrafterTEBase machine)
        {
            autocrafter = machine;

            filterer = new();
            sorter = new();

            filterer.AddFilters(new List<IItemEntryFilter>
            {
                new ItemFilters.Weapon(),
                new ItemFilters.Armor(),
                new ItemFilters.Tools(),
                new ItemFilters.Consumables(),
                new ItemFilters.Materials(),
                new ItemFilters.MiscFallback(new List<IItemEntryFilter>())
            });
            filterer.SetSearchFilterObject(new ItemFilters.BySearch());

            sorter.AddSortSteps([new SortingSteps.Alphabetical()]);
        }

        public override void OnInitialize()
        {
            Width.Set(0f, 1f);
            Height.Set(0f, 1f);
            SetPadding(2f);

            var searchPanel = new UIPanel()
            {
                Width = new(0f, 1f),
                Height = new(32f, 0f),
                Top = new(0f, 0f),
                BackgroundColor = UITheme.Current.PanelStyle.BackgroundColor,
                BorderColor = UITheme.Current.PanelStyle.BorderColor
            };
            searchPanel.SetPadding(4f);
            Append(searchPanel);

            searchBar = new UISearchBar(Language.GetText("UI.Search"), 0.8f)
            {
                Width = new(0f, 1f),
                Height = new(0f, 1f),
            };
            searchBar.OnContentsChanged += OnSearchChanged;
            searchPanel.Append(searchBar);

            recipeListPanel = new UIListScrollablePanel()
            {
                Width = new(0f, 1f),
                Height = new(-40f, 1f),
                Top = new(40f, 0f),
                ListPadding = 0f,
                ListOuterPadding = 0f,
                HideScrollbarIfNotScrollable = true,
                ScrollbarHAlign = 0.98f,
                BackgroundColor = UITheme.Current.PanelStyle.BackgroundColor,
                BorderColor = UITheme.Current.PanelStyle.BorderColor,
                PaddingLeft = 12f,
                PaddingRight = 2f,
            };
            Append(recipeListPanel);
            recipeGridRoot = new UIElement()
            {
                Width = new(0f, 1f)
            };
            recipeGridRoot.SetPadding(0f);
            recipeListPanel.Add(recipeGridRoot);

            PopulateAvailableRecipes();
            RefreshRecipeList();
        }

        private void PopulateAvailableRecipes()
        {
            availableRecipes.Clear();
            foreach (var recipe in Main.recipe)
            {
                if (recipe?.createItem is Item item && item.type > ItemID.None && autocrafter.RecipeAllowed(recipe))
                    availableRecipes.Add(recipe);
            }
        }

        private void RefreshRecipeList()
        {
            recipeGridRoot.RemoveAllChildren();

            filteredRecipes = availableRecipes.Where(r => filterer.FitsFilter(r.createItem)).ToList();
            filteredRecipes.Sort((a, b) => sorter.Compare(a.createItem.type, b.createItem.type));

            int itemsPerRow = 9;
            float slotSize = 48;
            float slotPadding = 2;
            for (int i = 0; i < filteredRecipes.Count; i++)
            {
                Recipe recipe = filteredRecipes[i];
                Item resultItem = recipe.createItem.Clone();

                var slot = new UIInventorySlot(ref resultItem);
                slot.Width.Set(slotSize, 0f);
                slot.Height.Set(slotSize, 0f);
                slot.CanInteractWithItem = false;
                slot.HoverHighlightColor = Color.Gold;

                int column = i % itemsPerRow;
                int row = i / itemsPerRow;

                slot.Left.Set(column * (slotSize + slotPadding), 0f);
                slot.Top.Set(row * (slotSize + slotPadding), 0f);

                slot.OnLeftClick += (_, _) => OnRecipeClicked(recipe);
                recipeGridRoot.Append(slot);
            }

            int rows = (filteredRecipes.Count + itemsPerRow - 1) / itemsPerRow;
            recipeGridRoot.Height.Set(rows * (slotSize + slotPadding), 0f);
        }

        private void OnSearchChanged(string searchText)
        {
            filterer.SetSearchFilter(searchText);
            RefreshRecipeList();
        }
    }
}
