using Macrocosm.Content.Dusts;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Macrocosm.Common.TileFrame;

namespace Macrocosm.Content.Tiles.Blocks.Bricks
{
    public class SeleniteBrick : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileSolid[Type] = true;
            Main.tileBrick[Type] = true;
            Main.tileBlockLight[Type] = true;
            Main.tileLighted[Type] = true;

            MinPick = 225;
            MineResist = 3f;

            AddMapEntry(new Color(190, 190, 200));

            DustType = ModContent.DustType<SeleniteDust>();
            HitSound = SoundID.Tink;
        }
        public override bool Slope(int i, int j)
        {
            WorldGen.TileFrame(i + 1, j + 1);
            WorldGen.TileFrame(i + 1, j - 1);
            WorldGen.TileFrame(i - 1, j + 1);
            WorldGen.TileFrame(i - 1, j - 1);
            return true;
        }
        public override bool TileFrame(int i, int j, ref bool resetFrame, ref bool noBreak)
        {
            TileFraming.PlatingStyle(i, j, countHalfBlocks: true);
            return false;
        }
    }
}