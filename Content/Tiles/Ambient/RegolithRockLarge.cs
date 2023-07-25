using Macrocosm.Content.Dusts;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;
using Terraria.GameContent;
using Macrocosm.Content.Items.Placeable.Blocks;

namespace Macrocosm.Content.Tiles.Ambient
{
	public abstract class RegolithRockLargeBase : ModTile
	{
		// We want both tiles to use the same texture
		public override string Texture => "Macrocosm/Content/Tiles/Ambient/RegolithRockLarge";

		public override void SetStaticDefaults() {
			Main.tileSolid[Type] = false;
			Main.tileSolidTop[Type] = false;
			Main.tileTable[Type] = false;
			Main.tileNoAttach[Type] = true;
			Main.tileFrameImportant[Type] = true;
			Main.tileLavaDeath[Type] = true;

			DustType = ModContent.DustType<RegolithDust>();

			TileObjectData.newTile.CopyFrom(TileObjectData.Style3x2);

			TileObjectData.newTile.StyleHorizontal = true;
			TileObjectData.addTile(Type);

			TileID.Sets.DisableSmartCursor[Type] = true;
			AddMapEntry(new Color(220, 220, 220));
		}
	}

	// This is the fake tile that will be placed by the Rubblemaker.
	public class RegolithRockLargeFake : RegolithRockLargeBase
	{
		public override void SetStaticDefaults() {
			// Call to base SetStaticDefaults. Must inherit static defaults from base type 
			base.SetStaticDefaults();

			// Add rubble variant, all existing styles, to Rubblemaker, allowing to place this tile by consuming ExampleBlock
			FlexibleTileWand.RubblePlacementSmall.AddVariations(ModContent.ItemType<Regolith>(), Type, 0, 1, 2, 3, 4, 5);

			// Tiles placed by Rubblemaker drop the item used to place them.
			RegisterItemDrop(ModContent.ItemType<Regolith>());
		}
	}

	// This is the natural tile, this version is placed during world generation 
	public class RegolithRockLargeNatural : RegolithRockLargeBase
	{
		public override void SetStaticDefaults() {
			base.SetStaticDefaults();
		}
	}
}