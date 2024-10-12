using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Macrocosm.Content.Players;
using Terraria;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Accessories
{
    public class StalwartTowerShield : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 34;
            Item.height = 52;
            Item.accessory = true;
            Item.defense = 10;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<StalwartPlayer>().StalwartShield = true;
        }
    }
}
