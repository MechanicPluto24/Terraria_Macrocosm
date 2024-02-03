using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Placeable.Tombstones
{
	public class MoonTombstone : ModItem
	{
		public override void SetStaticDefaults()
		{
			Item.ResearchUnlockCount = 2;
            FlexibleTileWand.RubblePlacementLarge.AddVariation(Type, ModContent.TileType<Tiles.Tombstones.MoonTombstone>(), 1);
        }

        public override void SetDefaults()
		{
			Item.width = 26;
			Item.height = 30;
			Item.useTurn = true;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.useAnimation = 15;
			Item.useTime = 10;
			Item.maxStack = Item.CommonMaxStack;
			Item.consumable = true;
			Item.createTile = ModContent.TileType<Tiles.Tombstones.MoonTombstone>();
			Item.placeStyle = 0;
			Item.rare = ItemRarityID.Purple;
		}
	}
}