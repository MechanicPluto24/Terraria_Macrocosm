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

        private UIPanel consumedItemPanel;
        private UIInventoryItemIcon itemIcon;

        public BurnerGeneratorUI()
        {
        }

        public override void OnInitialize()
        {
            base.OnInitialize();

            Width.Set(745f, 0f);
            Height.Set(394f, 0f);

            Recalculate();

            if (BurnerGenerator.Inventory is not null)
            {
                inventoryPanel = BurnerGenerator.Inventory.ProvideUI(iconsPerRow: 10, rowsWithoutScrollbar: 5, buttonMenuTopPercent: 0.765f);
                inventoryPanel.Width = new(0, 0.69f);
                inventoryPanel.BorderColor = UITheme.Current.ButtonStyle.BorderColor;
                inventoryPanel.BackgroundColor = UITheme.Current.PanelStyle.BackgroundColor;
                inventoryPanel.Activate();
                Append(inventoryPanel);
            }

            consumedItemPanel = new()
            {
                Width = new(48, 0),
                Height = new(48, 0),
                Left = new(0, 0.8f),
                VAlign = 0.5f,
                BorderColor = UITheme.Current.ButtonStyle.BorderColor,
                BackgroundColor = UITheme.Current.PanelStyle.BackgroundColor
            };
            Append(consumedItemPanel);

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

            if(itemIcon.Item.type != BurnerGenerator.ConsumedItem.type)
                itemIcon.Item = BurnerGenerator.ConsumedItem;
        }
    }
}
