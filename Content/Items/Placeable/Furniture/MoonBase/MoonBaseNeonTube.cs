using Terraria.ModLoader;
using Terraria;
using Terraria.ID;
using Macrocosm.Content.Items.Placeable.Blocks;

namespace Macrocosm.Content.Items.Placeable.Furniture.MoonBase
{
	public class MoonBaseNeonTube : ModItem
	{
		public override void SetDefaults()
		{
			Item.DefaultToPlaceableTile(ModContent.TileType<Tiles.Furniture.MoonBase.MoonBaseNeonTube>());
			Item.width = 30;
			Item.height = 8;
			Item.value = 500;
		}

		public override void AddRecipes()
		{
			CreateRecipe()
			.AddIngredient<MoonBasePlating>(4)
			.AddTile(TileID.WorkBenches)
			.Register();
		}
	}
}
