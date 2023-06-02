using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace Macrocosm.Content.Items.Placeable.Blocks
{
	public class ProtolithWall : ModItem
	{
		public override void SetStaticDefaults()
		{
		}

		public override void SetDefaults()
		{
			Item.width = 12;
			Item.height = 12;
			Item.maxStack = 9999;
			Item.useTurn = true;
			Item.autoReuse = true;
			Item.useAnimation = 15;
			Item.useTime = 7;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.consumable = true;
			Item.createWall = WallType<Tiles.Walls.ProtolithWall>();
		}

		public override void AddRecipes()
		{

			Recipe recipe = Recipe.Create(Type, 4);
			recipe.AddIngredient<Protolith>();
			recipe.AddTile(TileID.WorkBenches);
			recipe.Register();
		}
	}
}