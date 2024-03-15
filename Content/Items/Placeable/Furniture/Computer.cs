using Terraria;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Placeable.Furniture
{
	public class Computer : ModItem
	{
		public override void SetStaticDefaults()
		{
		}

		public override void SetDefaults()
		{
			Item.DefaultToPlaceableTile(ModContent.TileType<Tiles.Furniture.Computer>());
			Item.width = 32;
			Item.height = 30;
			Item.value = 500;
		}

        public override void AddRecipes()
        {
        }
    }
}