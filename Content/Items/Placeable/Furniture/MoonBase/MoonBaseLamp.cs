using Terraria.ModLoader;
using Terraria;
using Terraria.ID;
using Macrocosm.Content.Items.Placeable.Blocks;

namespace Macrocosm.Content.Items.Placeable.Furniture.MoonBase
{
	public class MoonBaseLamp : ModItem
	{
		public override void SetDefaults()
		{
			Item.DefaultToPlaceableTile(ModContent.TileType<Tiles.Furniture.MoonBase.MoonBaseLamp>());
			Item.width = 10;
			Item.height = 36;
			Item.value = 500;
		}

		public override void AddRecipes()
		{
			CreateRecipe()
			.AddIngredient<MoonBasePlating>(3)
			.AddTile(TileID.WorkBenches)
			.Register();
		}
	}
}
