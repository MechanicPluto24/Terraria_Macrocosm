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
using Macrocosm.Common.Storage;

namespace Macrocosm.Common.UI.Machines
{
    public class UIAutocrafterRecipeBrowser : UIElement
    {
        private UISearchBar searchBar;

        private EntryFilterer<Item, IItemEntryFilter> filterer; // TODO (create a filter by crafting station)
        private EntrySorter<int, ICreativeItemSortStep> sorter; // TODO 

        private readonly List<Recipe> availableRecipes = new();
        private List<Recipe> filteredRecipes = new();

        private readonly AutocrafterTEBase autocrafter;
        private UIScrollableListPanel recipeListPanel;

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

            recipeListPanel = new UIScrollableListPanel()
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
            recipeListPanel.ClearList();

            filteredRecipes = availableRecipes
                .Where(r => filterer.FitsFilter(r.createItem))
                .OrderBy(r => r.createItem.Name)
                .ToList();

            int iconsPerRow = 9;
            float iconSize = 48f;
            float iconOffsetTop = 0f;
            float iconOffsetLeft = filteredRecipes.Count <= iconsPerRow * 5 ? 12f : 2f;

            int rowCount = (filteredRecipes.Count + iconsPerRow - 1) / iconsPerRow;

            UIElement itemSlotContainer = new()
            {
                Width = new(0f, 1f),
                Height = new(iconSize * rowCount, 0f),
            };
            itemSlotContainer.SetPadding(0f);

            for (int i = 0; i < filteredRecipes.Count; i++)
            {
                Recipe recipe = filteredRecipes[i];
                Item item = recipe.createItem.Clone();

                UIInventorySlot slot = new(ref item)
                {
                    Width = new(iconSize, 0f),
                    Height = new(iconSize, 0f),
                    Left = new(i % iconsPerRow * iconSize + iconOffsetLeft, 0f),
                    Top = new(i / iconsPerRow * iconSize + iconOffsetTop, 0f),
                    CanInteractWithItem = false,
                    HoverHighlightColor = Color.Gold
                };

                slot.OnLeftClick += (_, _) => OnRecipeClicked(recipe);
                itemSlotContainer.Append(slot);
            }


            recipeListPanel.Add(itemSlotContainer);
        }

        private void OnSearchChanged(string searchText)
        {
            filterer.SetSearchFilter(searchText);
            RefreshRecipeList();
        }
    }
}
