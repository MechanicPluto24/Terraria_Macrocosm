using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace Macrocosm.Content.Items.Placeable.Paintings
{
	public class Apollo115 : ModItem
	{
		public override void SetStaticDefaults()
		{
		}

		public override void SetDefaults()
		{
			Item.width = 50;
			Item.height = 34;
			Item.maxStack = Item.CommonMaxStack;
			Item.value = 0;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.useTurn = true;
			Item.useAnimation = 15;
			Item.useTime = 10;
			Item.autoReuse = true;
			Item.consumable = true;
			Item.createTile = TileType<Tiles.Paintings.Apollo115>();
			Item.placeStyle = 0;
		}

		public override void AddRecipes()
		{

		}
	}
}