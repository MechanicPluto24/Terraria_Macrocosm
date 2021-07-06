using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;
using Terraria.ID;

namespace Macrocosm.Content.Items.Placeables.BlocksAndWalls
{
	public class SpaceshipWindowWall : ModItem
	{
		public override void SetStaticDefaults()
		{
			
		}

		public override void SetDefaults()
		{
			item.width = 12;
			item.height = 12;
			item.maxStack = 999;
			item.useTurn = true;
			item.autoReuse = true;
			item.useAnimation = 15;
			item.useTime = 7;
			item.useStyle = ItemUseStyleID.SwingThrow;
			item.consumable = true;
			item.createWall = WallType<Walls.SpaceshipWindowWall>();
		}

		public override void AddRecipes()
		{

			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(mod, "SpaceshipWindow", 1);
			recipe.AddTile(TileID.WorkBenches);
			recipe.SetResult(this, 4);
			recipe.AddRecipe();
		}
	}
}