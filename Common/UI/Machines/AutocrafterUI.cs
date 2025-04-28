using Macrocosm.Common.Storage;
using Macrocosm.Common.Systems.Power;
using Macrocosm.Common.UI;
using Macrocosm.Common.UI.Themes;
using Macrocosm.Content.Machines.Consumers.Autocrafters;
using Microsoft.Xna.Framework;
using System.Linq;
using Terraria.GameContent.UI.Elements;
using Terraria.UI;

namespace Macrocosm.Common.UI.Machines
{
    public class AutocrafterUI : MachineUI
    {
        public AutocrafterTEBase AutocrafterTE => MachineTE as AutocrafterTEBase;

        private UIPanel topPanel;
        private UIListScrollablePanel craftingSlotsPanel;
        private UIPanel bottomPanel;
        private UIPanel recipeBrowserPanel;
        private UIAutocrafterRecipeBrowser recipeBrowser;
        private UIPanel infoPanel;

        public override void OnInitialize()
        {
            base.OnInitialize();

            Width.Set(850f, 0f);
            HAlign = 0.5f;
            VAlign = 0.5f;
            switch (AutocrafterTE.OutputSlots)
            {
                case 1: Height.Set(300f + 400f * 0.25f, 0f); break;
                case 2: Height.Set(300f + 400f * 0.4f, 0f); break;
                case 3: Height.Set(300f + 400f * 0.6f, 0f);  break;
                case 4: Height.Set(300f + 400f * 0.8f, 0f);  break;
                default: Height.Set(700f, 0f); break;
            }

            topPanel = new UIPanel()
            {
                Width = new(0f, 1f),
                Height = new(0f, 0.4f),
                Top = new(0f, 0f),
                BackgroundColor = UITheme.Current.PanelStyle.BackgroundColor,
                BorderColor = UITheme.Current.PanelStyle.BorderColor
            };
            Append(topPanel);

            craftingSlotsPanel = new UIListScrollablePanel()
            {
                Width = new(0f, 1f),
                Height = new(0f, 1f),
                ListPadding = 8f,
                ListOuterPadding = 8f,
                ScrollbarHeight = new(0f, 0.9f),
                ScrollbarHAlign = 0.98f
            };
            topPanel.Append(craftingSlotsPanel);

            bottomPanel = new UIPanel()
            {
                Width = new(0f, 1f),
                Height = new(0f, 0.6f),
                Top = new(0f, 0.4f),
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
                OnRecipeClicked = (recipe) =>
                {
                    AutocrafterTE.SelectRecipe(0, recipe);
                    PopulateSlots();
                }
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

        private void PopulateSlots()
        {
            if (AutocrafterTE?.Inventory is null)
                return;

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
                float slotSize = 48f;
                float slotSpacing = 6f;

                UIElement slotRow = new()
                {
                    Width = new(0f, 1f),
                    Height = new(slotSize, 0f)
                };
                slotRow.SetPadding(0f);

                float totalRowWidth = (slotSize + slotSpacing) * totalSlots - slotSpacing;
                float startX = (Width.Pixels - totalRowWidth) / 2f;

                for (int inputIndex = 0; inputIndex < inputs; inputIndex++)
                {
                    int inventoryIndex = inputSlots[inputIndex];
                    var inputSlot = AutocrafterTE.Inventory.ProvideItemSlot(inventoryIndex);

                    inputSlot.Top.Set(0f, 0f);
                    inputSlot.Left.Set(startX + (slotSize + slotSpacing) * inputIndex, 0f);

                    slotRow.Append(inputSlot);
                }

                var outputSlot = AutocrafterTE.Inventory.ProvideItemSlot(outputIndex);
                outputSlot.Top.Set(0f, 0f);
                outputSlot.Left.Set(startX + (slotSize + slotSpacing) * inputs, 0f);

                slotRow.Append(outputSlot);

                craftingSlotsPanel.Add(slotRow);
            }
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            Inventory.ActiveInventory = AutocrafterTE.Inventory;
        }
    }
}
