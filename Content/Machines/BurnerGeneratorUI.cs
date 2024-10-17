using Macrocosm.Common.Storage;
using Macrocosm.Common.Systems.Power;
using Macrocosm.Common.UI;
using Macrocosm.Common.UI.Themes;
using Microsoft.Xna.Framework;
using Terraria.GameContent.UI.Elements;


namespace Macrocosm.Content.Machines
{
    public class BurnerGeneratorUI : MachineUI
    {
        public BurnerGeneratorTE BurnerGenerator => MachineTE as BurnerGeneratorTE;

        private UIPanel inventoryPanel;


        private UIPanel progressBackgroundPanel;
        private UIPanelProgressBar progressBar;
        private UIPanel consumedItemPanel;
        private UIInventoryItemIcon itemIcon;

        public BurnerGeneratorUI()
        {
        }

        public override void OnInitialize()
        {
            base.OnInitialize();

            Width.Set(645f, 0f);
            Height.Set(394f, 0f);

            Recalculate();

            if (BurnerGenerator.Inventory is not null)
            {
                inventoryPanel = BurnerGenerator.Inventory.ProvideUI(iconsPerRow: 10, rowsWithoutScrollbar: 5, buttonMenuTopPercent: 0.765f);
                inventoryPanel.Width = new(0, 0.795f);
                inventoryPanel.BorderColor = UITheme.Current.ButtonStyle.BorderColor;
                inventoryPanel.BackgroundColor = UITheme.Current.PanelStyle.BackgroundColor;
                inventoryPanel.Activate();
                Append(inventoryPanel);
            }

            progressBackgroundPanel = new()
            {
                Width = new(0, 0.195f),
                Left = new(0, 0.805f),
                Height = new(0, 1f),
                Top = new(0, 0),
                BorderColor = UITheme.Current.ButtonStyle.BorderColor,
                BackgroundColor = UITheme.Current.PanelStyle.BackgroundColor
            };
            Append(progressBackgroundPanel);

            progressBar = new()
            {
                Width = new(24, 0),
                Height = new(0, 0.8f),
                Left = new(0, 0.1f),
                VAlign = 0.5f,
                FillColor = new Color(255, 255, 0),
                FillColorEnd = new Color(255, 0, 0),
                IsVertical = true,
                BorderColor = UITheme.Current.ButtonStyle.BorderColor,
                BackgroundColor = UITheme.Current.PanelStyle.BackgroundColor
            };
            progressBackgroundPanel.Append(progressBar);

            consumedItemPanel = new()
            {
                Width = new(48, 0),
                Height = new(48, 0),
                Left = new(0, 0.49f),
                VAlign = 0.5f,
                BorderColor = UITheme.Current.ButtonStyle.BorderColor,
                BackgroundColor = UITheme.Current.PanelStyle.BackgroundColor
            };
            progressBackgroundPanel.Append(consumedItemPanel);

            itemIcon = new()
            {
                HAlign = 0.5f,
                VAlign = 0.5f
            };
            consumedItemPanel.Append(itemIcon);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            Inventory.ActiveInventory = BurnerGenerator.Inventory;

            progressBar.Progress = BurnerGenerator.BurnProgress;

            if (itemIcon.Item.type != BurnerGenerator.ConsumedItem.type)
                itemIcon.Item = BurnerGenerator.ConsumedItem;
        }
    }
}
