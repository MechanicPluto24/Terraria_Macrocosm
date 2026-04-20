using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.UI;
using Macrocosm.Content.Machines.Consumers.Autocrafters;

namespace Macrocosm.Common.UI.Machines;

public class UIAutocrafterRecipeBrowser : UIElement
{
    private UIItemBrowser itemBrowser;

    private readonly List<Recipe> availableRecipes = new();
    private readonly AutocrafterTEBase autocrafter;

    public Action<Recipe> OnRecipeClicked { get; set; }

    public UIAutocrafterRecipeBrowser(AutocrafterTEBase machine)
    {
        autocrafter = machine;
    }

    public override void OnInitialize()
    {
        Width.Set(0f, 1f);
        Height.Set(0f, 1f);
        SetPadding(0f);

        itemBrowser = new UIItemBrowser()
        {
            Width = new(0f, 1f),
            Height = new(0f, 1f),
        };
        itemBrowser.OnEntrySelected += OnItemSelected;
        Append(itemBrowser);

        PopulateAvailableRecipes();
    }

    private void PopulateAvailableRecipes()
    {
        availableRecipes.Clear();
        foreach (var recipe in Main.recipe)
        {
            if (recipe?.createItem is Item item && item.type > ItemID.None && autocrafter.RecipeAllowed(recipe))
                availableRecipes.Add(recipe);
        }

        var items = new List<Item>(availableRecipes.Count);
        foreach (var recipe in availableRecipes)
            items.Add(recipe.createItem);

        itemBrowser.SetEntries(items);
    }

    private void OnItemSelected(int index, Item item)
    {
        if (index >= 0 && index < availableRecipes.Count)
            OnRecipeClicked?.Invoke(availableRecipes[index]);
    }
}
