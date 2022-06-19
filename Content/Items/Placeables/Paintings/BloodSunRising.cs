using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace Macrocosm.Content.Items.Placeables.Paintings
{
    public class BloodSunRising : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Blood Sun Rising");
            Tooltip.SetDefault("'L. Reda'");
        }

        public override void SetDefaults()
        {
            Item.width = 32;
            Item.height = 22;
            Item.maxStack = 999;
            Item.value = 0;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useTurn = true;
            Item.useAnimation = 15;
            Item.useTime = 10;
            Item.autoReuse = true;
            Item.consumable = true;
            Item.createTile = TileType<Tiles.Paintings.BloodSunRising>();
            Item.placeStyle = 0;
        }

        public override void AddRecipes()
        {

        }
    }
}