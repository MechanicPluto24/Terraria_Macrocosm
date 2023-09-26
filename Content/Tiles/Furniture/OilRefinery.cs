using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace Macrocosm.Content.Tiles.Furniture
{
	public class OilRefinery : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            Main.tileNoAttach[Type] = true;
            Main.tileLavaDeath[Type] = true;

            TileObjectData.newTile.CopyFrom(TileObjectData.Style5x4);
			TileObjectData.newTile.Width = 4;
            TileObjectData.newTile.Height = 4;
            TileObjectData.newTile.CoordinateWidth = 16;
            TileObjectData.newTile.CoordinateHeights = new int[]{16, 16, 16, 16};
            TileObjectData.newTile.CoordinatePadding = 2;
            TileObjectData.newTile.Direction = TileObjectDirection.PlaceRight;
			TileObjectData.newTile.StyleHorizontal = false;
            TileObjectData.addTile(Type);

            HitSound = SoundID.Dig;
            DustType = -1;

            LocalizedText name = CreateMapEntryName();
            AddMapEntry(new Color(121, 107, 91), name);
        }

        public override void KillMultiTile(int i, int j, int frameX, int frameY)
        {
        }

    }

}
