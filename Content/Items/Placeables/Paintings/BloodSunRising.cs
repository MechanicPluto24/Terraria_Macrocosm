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
            item.width = 32;
            item.height = 22;
            item.maxStack = 999;
            item.value = 0;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.useTurn = true;
            item.useAnimation = 15;
            item.useTime = 10;
            item.autoReuse = true;
            item.consumable = true;
            item.createTile = TileType<Tiles.Paintings.BloodSunRising>();
            item.placeStyle = 0;
        }

        public override void AddRecipes()
        {

        }
    }
}