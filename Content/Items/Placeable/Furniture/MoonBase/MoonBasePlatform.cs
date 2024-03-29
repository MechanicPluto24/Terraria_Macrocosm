using Terraria;
using Terraria.ModLoader;
using Macrocosm.Content.Items.Placeable.Blocks;

namespace Macrocosm.Content.Items.Placeable.Furniture.MoonBase
{
	public class MoonBasePlatform : ModItem
	{
		public override void SetStaticDefaults() 
		{
			Item.ResearchUnlockCount = 200;
		}

		public override void SetDefaults() 
		{
			Item.DefaultToPlaceableTile(ModContent.TileType<Tiles.Furniture.MoonBase.MoonBasePlatform>());
			Item.width = 24;
			Item.height = 16;
		}

		public override void AddRecipes() 
		{
			CreateRecipe(2)
				.AddIngredient<MoonBasePlating>()
				.Register();
		}
	}
}