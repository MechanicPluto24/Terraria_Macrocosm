using Macrocosm.Common.TileFrame;
using Macrocosm.Content.Dusts;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Tiles.Blocks.Bricks
{
    public class ProtolithBrick : ModTile
    {
        public override bool IsLoadingEnabled(Mod mod) => false;

        public override void SetStaticDefaults()
        {
            Main.tileSolid[Type] = true;
            Main.tileBrick[Type] = true;
            Main.tileBlockLight[Type] = true;

            MinPick = 225;
            MineResist = 3f;

            AddMapEntry(new Color(38, 38, 39));

            DustType = ModContent.DustType<ProtolithDust>();
            HitSound = SoundID.Tink;
        }

        public override bool TileFrame(int i, int j, ref bool resetFrame, ref bool noBreak)
        {
            int xVariation = (j % 2);
            int yVariation = (i) % 2;
            TileFraming.PlatingStyle(i, j, customVariation: xVariation);
            Main.tile[i, j].TileFrameY += (short)(90 * yVariation);
            return false;
        }
    }
}