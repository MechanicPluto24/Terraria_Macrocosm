using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace Macrocosm.Content.Items.Placeable.BlocksAndWalls
{
	public class IrradiatedBrickWall : ModItem
	{
		public override void SetStaticDefaults()
		{

		}

		public override void SetDefaults()
		{
			Item.width = 12;
			Item.height = 12;
			Item.maxStack = 999;
			Item.useTurn = true;
			Item.autoReuse = true;
			Item.useAnimation = 15;
			Item.useTime = 7;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.consumable = true;
			Item.createWall = WallType<Walls.IrradiatedBrickWall>();
		}

		public override void AddRecipes()
		{

			Recipe recipe = Recipe.Create(Type, 4);
			recipe.AddIngredient<IrradiatedBrick>(1);
			recipe.AddTile(TileID.WorkBenches);
			recipe.Register();
		}
	}
}