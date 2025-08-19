using Macrocosm.Common.Storage;
using Macrocosm.Common.Systems.Power;
using Macrocosm.Common.UI.Themes;
using Macrocosm.Content.Machines.Consumers.Autocrafters;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader;

namespace Macrocosm.Common.UI.Machines;

public class AutocrafterUI : MachineUI
{
    public AutocrafterTEBase AutocrafterTE => MachineTE as AutocrafterTEBase;

    private UIPanel topPanel;
    private UIScrollableListPanel craftingSlotsPanel;
    private UIPanel bottomPanel;
    private UIPanel recipeBrowserPanel;
    private UIAutocrafterRecipeBrowser recipeBrowser;
    private UIPanel infoPanel;

    List<UIPanel> outputSlotPanels = new();
    private int selectedOutputSlot = -1;

    public override void OnInitialize()
    {
        base.OnInitialize();

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
            4 => 0.442f,
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

        craftingSlotsPanel = CreateSlotsPanel();
        topPanel.Append(craftingSlotsPanel);

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
            Width = new(0f, 0.65f),
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
            OnRecipeClicked = OnRecipeClicked
        };
        recipeBrowserPanel.Append(recipeBrowser);

        infoPanel = new UIPanel()
        {
            Width = new(0f, 0.35f),
            Height = new(0f, 1f),
            Left = new(0f, 0.65f),
            BackgroundColor = UITheme.Current.PanelStyle.BackgroundColor,
            BorderColor = UITheme.Current.PanelStyle.BorderColor
        };
        infoPanel.SetPadding(8f);
        bottomPanel.Append(infoPanel);
        PopulateSlots();
    }

    private void OnRecipeClicked(Recipe recipe)
    {
        if (!AutocrafterTE.SelectRecipeInFreeSlot(recipe))
        {
            if (selectedOutputSlot != -1 && selectedOutputSlot < outputSlotPanels.Count)
                AutocrafterTE.SelectRecipeInSlot(selectedOutputSlot, recipe);
        }

        PopulateSlots();
    }

    private UIScrollableListPanel CreateSlotsPanel()
    {
        craftingSlotsPanel = new UIScrollableListPanel()
        {
            Width = new(0f, 1f),
            Height = new(0f, 1f),
            ListPadding = 4f,
            ListOuterPadding = 0f,
            ListWidthWithScrollbar = new(0, 0.96f),
            ListWidthWithoutScrollbar = new(0, 1f),
            ScrollbarHeight = new(0f, 0.9f),
            ScrollbarHAlign = 1f,
            HideScrollbarIfNotScrollable = true,
            BackgroundColor = UITheme.Current.PanelStyle.BackgroundColor,
            BorderColor = UITheme.Current.PanelStyle.BorderColor
        };
        PopulateSlots();
        return craftingSlotsPanel;
    }

    private void PopulateSlots()
    {
        if (AutocrafterTE?.Inventory is null)
            return;

        outputSlotPanels = new();
        craftingSlotsPanel.ClearList();

        int outputs = AutocrafterTE.OutputSlots;
        for (int outputIndex = 0; outputIndex < outputs; outputIndex++)
        {
            var recipe = AutocrafterTE.SelectedRecipes?[outputIndex];
            if (recipe is null)
                continue;

            if (!AutocrafterTE.InputSlotAllocation.TryGetValue(outputIndex, out var inputSlots))
                continue;

            int inputs = inputSlots.Count;
            int totalSlots = inputs + 1;
            float slotSize = 42f;
            float slotSpacing = 6f;

            UIPanel rowPanel = new()
            {
                Width = new(0f, 1f),
                HAlign = 0.5f,
                Height = new(slotSize + 14f, 0f),
                BackgroundColor = UITheme.Current.PanelStyle.BackgroundColor,
                BorderColor = UITheme.Current.PanelStyle.BorderColor * 0.5f
            };
            rowPanel.OnMouseOut += (_, element) => (element as UIPanel).BorderColor = UITheme.Current.PanelButtonStyle.BorderColorHighlight;
            rowPanel.OnMouseOver += (_, element) => (element as UIPanel).BorderColor = UITheme.Current.PanelStyle.BorderColor * 0.5f;
            rowPanel.OnLeftClick += (_, element) => selectedOutputSlot = outputSlotPanels.IndexOf(element as UIPanel);
            rowPanel.SetPadding(2f);
            rowPanel.PaddingTop = 4f;
            outputSlotPanels.Add(rowPanel);

            float totalRowWidth = (slotSize + slotSpacing) * totalSlots - slotSpacing;
            float startX = (Width.Pixels - totalRowWidth) / 2f;
            for (int inputIndex = 0; inputIndex < inputs; inputIndex++)
            {
                int inventoryIndex = inputSlots[inputIndex];
                var inputSlot = AutocrafterTE.Inventory.ProvideItemSlot(inventoryIndex);

                inputSlot.SetPadding(0f);
                inputSlot.Top.Set(0f, 0f);
                inputSlot.Left.Set(startX + (slotSize + slotSpacing) * inputIndex, 0f);

                rowPanel.Append(inputSlot);
            }

            UITextureProgressBar extractArrowProgressBar = new(
               ModContent.Request<Texture2D>(Macrocosm.UIButtonsPath + "LongArrowBorder", AssetRequestMode.ImmediateLoad),
               ModContent.Request<Texture2D>(Macrocosm.UIButtonsPath + "LongArrowPlain", AssetRequestMode.ImmediateLoad),
               ModContent.Request<Texture2D>(Macrocosm.UIButtonsPath + "LongArrowPlain", AssetRequestMode.ImmediateLoad)
            )
            {
                BorderColor = UITheme.Current.PanelStyle.BorderColor,
                BackgroundColor = UITheme.Current.PanelStyle.BackgroundColor,
                FillColors = [Color.Black],
                Left = new(startX + (slotSize + slotSpacing) * inputs, 0f),
                VAlign = 0.52f
            };

            rowPanel.Append(extractArrowProgressBar);

            var outputSlot = AutocrafterTE.Inventory.ProvideItemSlot(outputIndex);
            outputSlot.SetPadding(0f);
            outputSlot.Top.Set(0f, 0f);
            outputSlot.Left.Set(startX + 60 + (slotSize + slotSpacing) * inputs, 0f);

            rowPanel.Append(outputSlot);

            craftingSlotsPanel.Add(rowPanel);
        }
    }

    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);

        for (int outputIndex = 0; outputIndex < (outputSlotPanels?.Count ?? 0); outputIndex++)
        {
            UIPanel panel = outputSlotPanels[outputIndex];
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
