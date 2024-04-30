using Macrocosm.Common.Bases.Items;
using Macrocosm.Common.Loot;
using Macrocosm.Common.Storage;
using Macrocosm.Common.Systems.Power;
using Macrocosm.Common.UI;
using Macrocosm.Common.UI.Themes;
using Macrocosm.Common.Utils;
using Macrocosm.Content.Items.Materials.Tech;
using Macrocosm.Content.Rockets;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.GameContent.UI.Elements;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;


namespace Macrocosm.Content.Machines
{
    internal class OilRefineryUI : MachineUI
    {
        public OilRefineryTE OilRefinery => MachineTE as OilRefineryTE;

        private UIPanel backgroundPanel;
        private UIInventorySlot sourceSlot;
        private UIInventorySlot resultSlot;

        public OilRefineryUI()
        {
        }

        public override void OnInitialize()
        {
            base.OnInitialize();

            Width.Set(745f, 0f);
            Height.Set(394f, 0f);

            Recalculate();

            backgroundPanel = new()
            {
                Width = new(0, 1f),
                Height = new(0, 1f),
                BorderColor = UITheme.Current.PanelStyle.BorderColor,
                BackgroundColor = UITheme.Current.PanelStyle.BackgroundColor,
            };
            backgroundPanel.SetPadding(0);
            Append(backgroundPanel);

            if (OilRefinery.Inventory is not null)
            {
                sourceSlot = OilRefinery.Inventory.ProvideItemSlot(0);
                sourceSlot.Top = new(0, 0.45f);
                sourceSlot.Left = new(0, 0.27f);
                backgroundPanel.Append(sourceSlot);

                resultSlot = OilRefinery.Inventory.ProvideItemSlot(1);
                resultSlot.Top = new(0, 0.45f);
                resultSlot.Left = new(0, 0.68f);
                resultSlot.AddReserved(
                    (item) => item.ModItem is FuelCanister,
                    Lang.GetItemName(ModContent.ItemType<FuelCanister>()),
                    ModContent.Request<Texture2D>(ContentSamples.ItemsByType[ModContent.ItemType<FuelCanister>()].ModItem.Texture + "_Blueprint")
                );
                backgroundPanel.Append(resultSlot);
            }
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            Inventory.ActiveInventory = OilRefinery.Inventory;

            if(resultSlot.Item.ModItem is FuelCanister fuelCanister && !fuelCanister.Full)
            {
                fuelCanister.FuelPercent += 0.01f;
            }
        }
    }
}
