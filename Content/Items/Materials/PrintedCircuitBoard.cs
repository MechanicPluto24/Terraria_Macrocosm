using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Materials
{
	public class PrintedCircuitBoard : ModItem
	{
		public override void SetStaticDefaults()
		{
			Item.ResearchUnlockCount = 5;
		}

		public override void SetDefaults()
		{
            Item.DefaultToPlaceableTile(ModContent.TileType<Tiles.Furniture.PrintedCircuitBoard>());
            Item.width = 20;
			Item.height = 20;
			Item.maxStack = Item.CommonMaxStack;
			Item.value = 100;
			Item.rare = ItemRarityID.Green;
			Item.material = true;
		}

		public override void AddRecipes()
		{
		}
	}
}