using Macrocosm.Content.Items.Materials;
using Macrocosm.Content.Items.Placeable.Blocks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Placeable.Furniture.MoonBase
{
	public class MoonBaseLocker : ModItem
	{
		public override void SetStaticDefaults()
		{
		}

		public override void SetDefaults()
		{
			Item.DefaultToPlaceableTile(ModContent.TileType<Tiles.Furniture.MoonBase.MoonBaseLocker>());
			Item.width = 12;
			Item.height = 32;
			Item.value = 500;
		}

		public override void AddRecipes()
		{
			CreateRecipe()
			.AddIngredient<MoonBasePlating>(8)
			.AddTile(TileID.WorkBenches)
			.Register();
		}
	}
}