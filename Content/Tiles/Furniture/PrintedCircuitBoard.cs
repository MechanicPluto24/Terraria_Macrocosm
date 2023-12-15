using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Enums;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace Macrocosm.Content.Tiles.Furniture
{
	public class PrintedCircuitBoard : ModTile
	{
		public override void SetStaticDefaults()
		{
			Main.tileFrameImportant[Type] = true;
			Main.tileNoAttach[Type] = true;
			Main.tileLavaDeath[Type] = true;

			TileObjectData.newTile.CopyFrom(TileObjectData.Style1x1);
			TileObjectData.newTile.CoordinateWidth = 16;
			TileObjectData.newTile.CoordinateHeights = new int[] { 18 };
			TileObjectData.newTile.CoordinatePadding = 2;
            //TileObjectData.newTile.DrawYOffset = 2;
			TileObjectData.addTile(Type);

			HitSound = SoundID.Dig;
			DustType = -1;

			AddMapEntry(new Color(0, 97, 45), CreateMapEntryName());
		}
	}
}
