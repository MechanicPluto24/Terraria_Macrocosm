using Macrocosm.Common.Players;
using Macrocosm.Content.Rarities;
using Terraria;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Accessories
{
    public class LaserSight : ModItem
    {
        public override void SetStaticDefaults()
        {
        }
        public override void SetDefaults()
        {
            Item.width = 32;
            Item.height = 30;
            Item.value = Item.sellPrice(gold: 5);
            Item.rare = ModContent.RarityType<MoonRarityT1>();
            Item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<MacrocosmPlayer>().ShootSpreadReduction += 0.5f;
        }
    }
}