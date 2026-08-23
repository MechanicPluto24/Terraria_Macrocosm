using Macrocosm.Common.Storage;
using Macrocosm.Common.Systems.Power;
using Macrocosm.Common.UI.Themes;
using Macrocosm.Content.Machines.Consumers.Autocrafters;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;

namespace Macrocosm.Common.UI.Machines;

public class AutocrafterUI : MachineUI
{
    public AutocrafterTEBase AutocrafterTE => MachineTE as AutocrafterTEBase;

    private UIPanel topPanel;
    private UIPanel bottomPanel;
    private UIPanel recipeBrowserPanel;
    private UIAutocrafterRecipeBrowser recipeBrowser;
    private UIPanel inputInventoryPanel;
    private UITextPanel<string> sharedPanelToggle;
    private UIPanel infoPanel;

    private bool showingInputInventory;

    private readonly Dictionary<int, UIPanel> outputSlotPanels = new();
    private readonly List<Recipe> selectedRecipeOptions = new();
    private Item selectedRecipeResult;
    private int selectedOutputSlot = -1;

    private Asset<Texture2D> applyRecipeIcon;
    private Asset<Texture2D> clearRecipeIcon;
    private Asset<Texture2D> clearRecipeDisabledIcon;

    public override void OnInitialize()
    {
        base.OnInitialize();

        applyRecipeIcon = ModContent.Request<Texture2D>(Macrocosm.UISymbolsPath + "CheckmarkGreen", AssetRequestMode.ImmediateLoad);
        clearRecipeIcon = ModContent.Request<Texture2D>(Macrocosm.UISymbolsPath + "CrossmarkRed", AssetRequestMode.ImmediateLoad);
        clearRecipeDisabledIcon = ModContent.Request<Texture2D>(Macrocosm.UISymbolsPath + "CrossmarkGray", AssetRequestMode.ImmediateLoad);

        Width.Set(850f, 0f);
        Height.Set(700f, 0f);
        title.Top.Pixels -= 6f;
        HAlign = 0.5f;
        VAlign = 0.5f;

        float topHeightPercent = AutocrafterTE.OutputSlots switch
        {
            1 => 0.16f,
            2 => 0.2585f,
            3 => 0.44f,
            4 => 0.48f,
            _ => 0.42f
        };
        float bottomHeightPercent = 1f - topHeightPercent;

        topPanel = new UIPanel()
        {
            Width = new(0f, 1f),
            Height = new(0f, topHeightPercent),
            Top = new(0f, 0f),
            BackgroundColor = UITheme.Current.PanelStyle.BackgroundColor,
            BorderColor = UITheme.Current.PanelStyle.BorderColor
        };
        Append(topPanel);

        bottomPanel = new UIPanel()
        {
            Width = new(0f, 1f),
            Height = new(0f, bottomHeightPercent),
            Top = new(4f, topHeightPercent),
            BackgroundColor = UITheme.Current.PanelStyle.BackgroundColor,
            BorderColor = UITheme.Current.PanelStyle.BorderColor
        };
        Append(bottomPanel);

        recipeBrowserPanel = new UIPanel()
        {
            Width = new(0f, 0.69f),
            Height = new(0f, 1f),
            Left = new(0f, 0f),
            BackgroundColor = UITheme.Current.PanelStyle.BackgroundColor,
            BorderColor = UITheme.Current.PanelStyle.BorderColor
        };
        bottomPanel.Append(recipeBrowserPanel);

        recipeBrowser = new UIAutocrafterRecipeBrowser(AutocrafterTE)
        {
            Width = new(0f, 1f),
            Height = new(0f, 1f),
            Top = new(0f, 0f),
            OnResultClicked = OnResultClicked
        };

        inputInventoryPanel = AutocrafterTE.Inventory.ProvideUI(
            start: AutocrafterTE.OutputSlots,
            end: AutocrafterTE.Inventory.Size,
            iconsPerRow: 10,
            rowsWithoutScrollbar: 5
        );
        inputInventoryPanel.Width = new(0f, 1f);
        inputInventoryPanel.Height = new(0f, 1f);
        inputInventoryPanel.Top = new(0f, 0f);
        inputInventoryPanel.BackgroundColor = Color.Transparent;
        inputInventoryPanel.BorderColor = Color.Transparent;
        inputInventoryPanel.Activate();

        sharedPanelToggle = new UITextPanel<string>("Inventory", 0.75f)
        {
            Width = new(90f, 0f),
            Height = new(26f, 0f),
            HAlign = 1f,
            Left = new(-8f, 0f),
            Top = new(-30f, 0f),
            BackgroundColor = UITheme.Current.PanelButtonStyle.BackgroundColor,
            BorderColor = UITheme.Current.PanelButtonStyle.BorderColor
        };
        sharedPanelToggle.OnLeftClick += (_, _) => ToggleSharedPanel();
        RefreshSharedPanel();

        infoPanel = new UIPanel()
        {
            Width = new(0f, 0.3f),
            Height = new(0f, 1f),
            Left = new(0f, 0.70f),
            BackgroundColor = UITheme.Current.PanelStyle.BackgroundColor,
            BorderColor = UITheme.Current.PanelStyle.BorderColor
        };
        infoPanel.SetPadding(8f);
        bottomPanel.Append(infoPanel);
        PopulateSlots();
        PopulateInfoPanel();
    }

    private void OnResultClicked(Item result, IReadOnlyList<Recipe> recipes)
    {
        selectedRecipeResult = result?.Clone();
        selectedRecipeOptions.Clear();

        if (recipes is not null)
            selectedRecipeOptions.AddRange(recipes.Where(recipe => recipe is not null));

        PopulateInfoPanel();
    }

    private void PopulateSlots()
    {
        if (AutocrafterTE?.Inventory is null)
            return;

        outputSlotPanels.Clear();
        topPanel.RemoveAllChildren();

        if (selectedOutputSlot >= AutocrafterTE.OutputSlots)
            selectedOutputSlot = -1;

        int outputs = AutocrafterTE.OutputSlots;
        for (int outputIndex = 0; outputIndex < outputs; outputIndex++)
        {
            var recipe = AutocrafterTE.SelectedRecipes?[outputIndex];

            List<Item> requiredItems = recipe?.requiredItem
                .Where(item => item.type > ItemID.None && item.stack > 0)
                .Select(item => item.Clone())
                .ToList() ?? new();

            int inputs = requiredItems.Count;
            float slotSize = 48f;
            float slotSpacing = 6f;
            float arrowWidth = 56f;
            float arrowSpacing = 10f;
            float clearButtonArea = 42f;
            float rowHeight = slotSize + 14f;
            float rowSpacing = 8f;

            UIPanel rowPanel = new()
            {
                Width = new(-20f, 1f),
                HAlign = 0.5f,
                Top = new(8f + outputIndex * (rowHeight + rowSpacing), 0f),
                Height = new(rowHeight, 0f),
                BackgroundColor = UITheme.Current.PanelStyle.BackgroundColor,
                BorderColor = UITheme.Current.PanelStyle.BorderColor * 0.5f
            };
            rowPanel.OnMouseOut += (_, element) => (element as UIPanel).BorderColor = UITheme.Current.PanelStyle.BorderColor * 0.5f;
            rowPanel.OnMouseOver += (_, element) => (element as UIPanel).BorderColor = UITheme.Current.PanelButtonStyle.BorderColorHighlight * 0.8f;
            int capturedOutputIndex = outputIndex;
            rowPanel.OnLeftClick += (_, _) =>
            {
                selectedOutputSlot = capturedOutputIndex;
                PopulateInfoPanel();
            };
            rowPanel.SetPadding(2f);
            rowPanel.PaddingTop = 4f;
            outputSlotPanels[outputIndex] = rowPanel;

            float inputSlotsWidth = inputs > 0 ? inputs * slotSize + (inputs - 1) * slotSpacing : 0f;
            float totalRowWidth = recipe is null
                ? slotSize
                : inputSlotsWidth + (inputs > 0 ? arrowSpacing : 0f) + arrowWidth + arrowSpacing + slotSize;
            float rowContentWidth = Width.Pixels - 20f - rowPanel.PaddingLeft - rowPanel.PaddingRight - clearButtonArea;
            float startX = (rowContentWidth - totalRowWidth) / 2f;

            for (int inputIndex = 0; inputIndex < inputs; inputIndex++)
            {
                Item requiredItem = requiredItems[inputIndex];
                int requiredType = requiredItem.type;
                int requiredStack = requiredItem.stack;
                int available = AutocrafterTE.Inventory.CountItems(requiredType, startIndex: AutocrafterTE.OutputSlots);
                requiredItem.stack = 1;

                UIInventorySlot inputSlot = CreateDisplayOnlySlot(ref requiredItem);
                inputSlot.DrawIndexNumber = true;
                inputSlot.CustomIndexLabel = $"{available}/{requiredStack}";
                inputSlot.OnUpdate += element =>
                {
                    int currentAvailable = AutocrafterTE.Inventory.CountItems(requiredType, startIndex: AutocrafterTE.OutputSlots);
                    ((UIInventorySlot)element).CustomIndexLabel = $"{currentAvailable}/{requiredStack}";
                };

                inputSlot.SetPadding(0f);
                inputSlot.Top.Set(0f, 0f);
                inputSlot.Left.Set(startX + (slotSize + slotSpacing) * inputIndex, 0f);

                rowPanel.Append(inputSlot);
            }

            float outputLeft = startX;
            if (recipe is not null)
            {
                float arrowLeft = startX + inputSlotsWidth + (inputs > 0 ? arrowSpacing : 0f);
                UITextureProgressBar extractArrowProgressBar = new(
                   ModContent.Request<Texture2D>(Macrocosm.UIButtonsPath + "LongArrowBorder", AssetRequestMode.ImmediateLoad),
                   ModContent.Request<Texture2D>(Macrocosm.UIButtonsPath + "LongArrowPlain", AssetRequestMode.ImmediateLoad),
                   ModContent.Request<Texture2D>(Macrocosm.UIButtonsPath + "LongArrowPlain", AssetRequestMode.ImmediateLoad)
                )
                {
                    BorderColor = UITheme.Current.PanelStyle.BorderColor,
                    BackgroundColor = UITheme.Current.PanelStyle.BackgroundColor,
                    FillColors = [Color.Black],
                    Left = new(arrowLeft, 0f),
                    VAlign = 0.52f
                };

                rowPanel.Append(extractArrowProgressBar);
                outputLeft = arrowLeft + arrowWidth + arrowSpacing;
            }

            var outputSlot = AutocrafterTE.Inventory.ProvideItemSlot(outputIndex);
            outputSlot.GrayscaleReservedItem = true;
            outputSlot.ReservedItemOpacity = 0.5f;
            outputSlot.SetPadding(0f);
            outputSlot.Top.Set(0f, 0f);
            outputSlot.Left.Set(outputLeft, 0f);

            rowPanel.Append(outputSlot);
            rowPanel.Append(CreateClearRecipeButton(outputIndex));

            topPanel.Append(rowPanel);
        }
    }

    private void ToggleSharedPanel()
    {
        showingInputInventory = !showingInputInventory;
        RefreshSharedPanel();
    }

    private void RefreshSharedPanel()
    {
        recipeBrowserPanel.RemoveAllChildren();
        recipeBrowserPanel.Append(showingInputInventory ? inputInventoryPanel : recipeBrowser);
        sharedPanelToggle.SetText(showingInputInventory ? "Recipes" : "Inventory");
        recipeBrowserPanel.Append(sharedPanelToggle);
    }

    private UIPanelIconButton CreateClearRecipeButton(int outputIndex)
    {
        var clearButton = new UIPanelIconButton(clearRecipeIcon)
        {
            HAlign = 1f,
            VAlign = 0.5f,
            Left = new(-4f, 0f),
            CheckInteractible = () => CanClearRecipeSlot(outputIndex)
        };

        clearButton.OnUpdate += element =>
        {
            var button = (UIPanelIconButton)element;
            button.SetImage(CanClearRecipeSlot(outputIndex) ? clearRecipeIcon : clearRecipeDisabledIcon);
        };
        clearButton.OnLeftClick += (_, _) => ClearRecipeSlot(outputIndex);

        return clearButton;
    }

    private bool CanClearRecipeSlot(int outputSlot)
        => AutocrafterTE.SelectedRecipes is not null
        && outputSlot >= 0
        && outputSlot < AutocrafterTE.SelectedRecipes.Length
        && AutocrafterTE.SelectedRecipes[outputSlot] is not null
        && AutocrafterTE.CanOverwriteRecipeAt(outputSlot);

    private void ClearRecipeSlot(int outputSlot)
    {
        if (!AutocrafterTE.ClearRecipeSlot(outputSlot))
            return;

        PopulateSlots();
        PopulateInfoPanel();
    }

    private void PopulateInfoPanel()
    {
        infoPanel.RemoveAllChildren();

        if (selectedRecipeResult is null || selectedRecipeOptions.Count <= 0)
            return;

        Item resultItem = selectedRecipeResult.Clone();
        UIText resultNameText = new(resultItem.Name, 0.8f)
        {
            Width = new(0f, 1f),
            Height = new(22f, 0f),
            TextColor = GetItemNameColor(resultItem),
            DynamicallyScaleDownToWidth = true
        };
        resultNameText.OnUpdate += element =>
        {
            if (selectedRecipeResult is not null)
                ((UIText)element).TextColor = GetItemNameColor(selectedRecipeResult);
        };
        infoPanel.Append(resultNameText);

        UIInventorySlot resultSlot = CreateDisplayOnlySlot(ref resultItem);
        resultSlot.HAlign = 0.5f;
        resultSlot.Top = new(30f, 0f);
        infoPanel.Append(resultSlot);

        UIScrollableListPanel recipeList = new()
        {
            Width = new(0f, 1f),
            Height = new(-88f, 1f),
            Top = new(88f, 0f),
            BackgroundColor = Color.Transparent,
            BorderColor = Color.Transparent,
            ListPadding = 4f,
            ListOuterPadding = 0f,
            HideScrollbarIfNotScrollable = true,
            ScrollbarHAlign = 1f,
            PaddingLeft = 0f,
            PaddingRight = 0f,
            PaddingTop = 0f,
            PaddingBottom = 0f
        };
        recipeList.SetPadding(0f);

        foreach (Recipe recipe in selectedRecipeOptions)
            recipeList.Add(CreateRecipeOptionRow(recipe));

        infoPanel.Append(recipeList);
        recipeList.Activate();
    }

    private static UIInventorySlot CreateDisplayOnlySlot(ref Item item)
        => new(ref item)
        {
            CanInteractWithItem = false,
            IgnoresMouseInteraction = true
        };

    private static Color GetItemNameColor(Item item)
    {
        float pulse = Main.mouseTextColor / 255f;
        int alpha = Main.mouseTextColor;
        int rarity = item?.rare ?? ItemRarityID.White;
        Color color = new((byte)(255f * pulse), (byte)(255f * pulse), (byte)(255f * pulse), alpha);

        switch (rarity)
        {
            case ItemRarityID.Quest:
                color = new((byte)(255f * pulse), (byte)(175f * pulse), 0, alpha);
                break;
            case ItemRarityID.Gray:
                color = new((byte)(130f * pulse), (byte)(130f * pulse), (byte)(130f * pulse), alpha);
                break;
            case ItemRarityID.Blue:
                color = new((byte)(150f * pulse), (byte)(150f * pulse), (byte)(255f * pulse), alpha);
                break;
            case ItemRarityID.Green:
                color = new((byte)(150f * pulse), (byte)(255f * pulse), (byte)(150f * pulse), alpha);
                break;
            case ItemRarityID.Orange:
                color = new((byte)(255f * pulse), (byte)(200f * pulse), (byte)(150f * pulse), alpha);
                break;
            case ItemRarityID.LightRed:
                color = new((byte)(255f * pulse), (byte)(150f * pulse), (byte)(150f * pulse), alpha);
                break;
            case ItemRarityID.Pink:
                color = new((byte)(255f * pulse), (byte)(150f * pulse), (byte)(255f * pulse), alpha);
                break;
            case ItemRarityID.LightPurple:
                color = new((byte)(210f * pulse), (byte)(160f * pulse), (byte)(255f * pulse), alpha);
                break;
            case ItemRarityID.Lime:
                color = new((byte)(150f * pulse), (byte)(255f * pulse), (byte)(10f * pulse), alpha);
                break;
            case ItemRarityID.Yellow:
                color = new((byte)(255f * pulse), (byte)(255f * pulse), (byte)(10f * pulse), alpha);
                break;
            case ItemRarityID.Cyan:
                color = new((byte)(5f * pulse), (byte)(200f * pulse), (byte)(255f * pulse), alpha);
                break;
            case ItemRarityID.Red:
                color = new((byte)(255f * pulse), (byte)(40f * pulse), (byte)(100f * pulse), alpha);
                break;
            case ItemRarityID.Purple:
                color = new((byte)(180f * pulse), (byte)(40f * pulse), (byte)(255f * pulse), alpha);
                break;
        }

        if (rarity > ItemRarityID.Purple && RarityLoader.GetRarity(rarity) is ModRarity modRarity)
            color = modRarity.RarityColor * pulse;

        if (item is not null && (item.expert || rarity == ItemRarityID.Expert))
            color = new((byte)(Main.DiscoR * pulse), (byte)(Main.DiscoG * pulse), (byte)(Main.DiscoB * pulse), alpha);

        if (item is not null && (item.master || rarity == ItemRarityID.Master))
            color = new((byte)(255f * pulse), (byte)(Main.masterColor * 200f * pulse), 0, alpha);

        return color;
    }

    private UIElement CreateRecipeOptionRow(Recipe recipe)
    {
        List<Item> requiredItems = recipe.requiredItem
            .Where(item => item.type > ItemID.None && item.stack > 0)
            .Select(item => item.Clone())
            .ToList();

        const float slotSize = 42f;
        const float slotSpacing = 4f;
        const float rowPadding = 6f;
        const int slotsPerRow = 3;

        int slotRows = Math.Max(1, (requiredItems.Count + slotsPerRow - 1) / slotsPerRow);
        float rowHeight = rowPadding * 2f + slotRows * slotSize + (slotRows - 1) * slotSpacing;

        UIPanel rowPanel = new()
        {
            Width = new(-6f, 1f),
            Height = new(rowHeight, 0f),
            BackgroundColor = UITheme.Current.PanelStyle.BackgroundColor,
            BorderColor = UITheme.Current.PanelStyle.BorderColor * 0.5f
        };
        rowPanel.SetPadding(0f);

        for (int i = 0; i < requiredItems.Count; i++)
        {
            Item displayItem = requiredItems[i];
            UIInventorySlot slot = CreateDisplayOnlySlot(ref displayItem);
            slot.Width = new(slotSize, 0f);
            slot.Height = new(slotSize, 0f);
            slot.Left = new(rowPadding + (i % slotsPerRow) * (slotSize + slotSpacing), 0f);
            slot.Top = new(rowPadding + (i / slotsPerRow) * (slotSize + slotSpacing), 0f);

            rowPanel.Append(slot);
        }

        var applyButton = new UIPanelIconButton(applyRecipeIcon)
        {
            HAlign = 1f,
            VAlign = 0.5f,
            Left = new(-rowPadding, 0f),
            CheckInteractible = () => CanApplyRecipe(recipe)
        };
        applyButton.OnLeftClick += (_, _) => ApplyRecipe(recipe);
        rowPanel.Append(applyButton);

        return rowPanel;
    }

    private bool CanApplyRecipe(Recipe recipe)
        => recipe is not null
        && selectedOutputSlot >= 0
        && outputSlotPanels.ContainsKey(selectedOutputSlot)
        && AutocrafterTE.CanOverwriteRecipeAt(selectedOutputSlot);

    private void ApplyRecipe(Recipe recipe)
    {
        if (!CanApplyRecipe(recipe))
            return;

        if (AutocrafterTE.SelectRecipeInSlot(selectedOutputSlot, recipe))
        {
            PopulateSlots();
            PopulateInfoPanel();
        }
    }

    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);

        foreach (var (outputIndex, panel) in outputSlotPanels)
        {
            if (outputIndex == selectedOutputSlot)
                panel.BorderColor = UITheme.Current.PanelButtonStyle.BorderColorHighlight;
            else if (panel.IsMouseHovering)
                panel.BorderColor = UITheme.Current.PanelButtonStyle.BorderColorHighlight * 0.8f;
            else
                panel.BorderColor = UITheme.Current.PanelStyle.BorderColor * 0.5f;
        }

        Inventory.ActiveInventory = AutocrafterTE.Inventory;
    }
}
