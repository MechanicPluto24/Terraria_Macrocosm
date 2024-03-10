using Macrocosm.Content.Items.Placeable.Blocks;
using Macrocosm.Content.Tiles.Furniture.MoonBase;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Placeable.Furniture.MoonBase
{
	public class MoonBaseBulkhead : ModItem
	{
		public override void SetDefaults() {
			Item.DefaultToPlaceableTile(ModContent.TileType<MoonBaseBulkheadClosed>());
			Item.width = 12;
			Item.height = 42;
			Item.value = 150;
		}

		public override void AddRecipes() {
			CreateRecipe()
			.AddIngredient<MoonBasePlating>(10)
			.AddTile(TileID.WorkBenches)
			.Register();
		}
	}
}