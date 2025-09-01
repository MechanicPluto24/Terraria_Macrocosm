using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace Macrocosm.Content.Tiles.Furniture
{
    public class XFLR6 : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            Main.tileNoAttach[Type] = true;
            Main.tileLavaDeath[Type] = true;

            TileObjectData.newTile.Width = 3;
            TileObjectData.newTile.Height = 5;
            TileObjectData.newTile.Origin = new Point16(1, 3);

            TileObjectData.newTile.CoordinateWidth = 16;
            TileObjectData.newTile.CoordinateHeights = [16, 16, 16, 16, 16];
            TileObjectData.newTile.CoordinatePadding = 2;

            TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile | AnchorType.SolidWithTop | AnchorType.SolidSide, TileObjectData.newTile.Width, 0);
            TileObjectData.newTile.UsesCustomCanPlace = true;
            TileObjectData.addTile(Type);

            HitSound = SoundID.Dig;
            DustType = -1;

            AddMapEntry(new Color(211, 0, 0), CreateMapEntryName());
        }
    }
}
