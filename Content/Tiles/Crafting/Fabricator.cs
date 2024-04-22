using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Enums;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace Macrocosm.Content.Tiles.Crafting
{
    public class Fabricator : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            Main.tileNoAttach[Type] = true;
            Main.tileLavaDeath[Type] = true;

            TileObjectData.newTile.CopyFrom(TileObjectData.Style3x3);
            TileObjectData.newTile.Width = 3;
            TileObjectData.newTile.Height = 3;
            TileObjectData.newTile.CoordinateWidth = 16;
            TileObjectData.newTile.CoordinateHeights = [16, 16, 16];
            TileObjectData.newTile.CoordinatePadding = 2;
            TileObjectData.newTile.StyleHorizontal = false;
            TileObjectData.addTile(Type);

            HitSound = SoundID.Dig;
            DustType = -1;

            AddMapEntry(new Color(128, 128, 128), CreateMapEntryName());
        }

        public override void KillMultiTile(int i, int j, int frameX, int frameY)
        {
        }
    }
}
