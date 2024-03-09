using Terraria;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Placeable.Furniture
{
	public class EarthGlobe : ModItem
	{
		public override void SetStaticDefaults()
		{
		}

		public override void SetDefaults()
		{
			Item.DefaultToPlaceableTile(ModContent.TileType<Tiles.Furniture.EarthGlobe>());
			Item.width = 24;
			Item.height = 28;
			Item.value = 500;
		}

        public override void AddRecipes()
        {
        }
    }
}