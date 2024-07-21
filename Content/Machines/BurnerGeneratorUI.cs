using Macrocosm.Common.Loot;
using Macrocosm.Common.Netcode;
using Macrocosm.Common.Storage;
using Macrocosm.Common.Systems.Power;
using Macrocosm.Common.UI;
using Macrocosm.Common.UI.Themes;
using Macrocosm.Common.Utils;
using Macrocosm.Content.Rockets;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Linq;
using Terraria.GameContent.ItemDropRules;
using Terraria.GameContent.UI.Elements;


namespace Macrocosm.Content.Machines
{
    public class BurnerGeneratorUI : MachineUI
    {
        public BurnerGeneratorTE BurnerGenerator => MachineTE as BurnerGeneratorTE;

        private UIPanel inventoryPanel;

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
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            Inventory.ActiveInventory = BurnerGenerator.Inventory;
        }
    }
}
