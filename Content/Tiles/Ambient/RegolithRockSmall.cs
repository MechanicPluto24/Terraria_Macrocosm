using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace Macrocosm.Content.Tiles.Ambient
{
	public class RegolithRockSmall : ModTile
	{
		public override void SetStaticDefaults()
		{
			Main.tileSolid[Type] = false;
			Main.tileSolidTop[Type] = false;
			Main.tileTable[Type] = false;
			Main.tileNoAttach[Type] = true;
			Main.tileFrameImportant[Type] = true;
			Main.tileLavaDeath[Type] = true;

			TileObjectData.newTile.CopyFrom(TileObjectData.Style1x1);
			TileObjectData.newTile.RandomStyleRange = 10;
			TileObjectData.newTile.StyleHorizontal = true;
			TileObjectData.addTile(Type);

			TileID.Sets.DisableSmartCursor[Type] = true;
			AddMapEntry(new Color(220, 220, 220));
		}
	}
}