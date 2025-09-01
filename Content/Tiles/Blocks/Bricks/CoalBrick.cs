using Macrocosm.Common.TileFrame;
using Macrocosm.Content.Dusts;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Tiles.Blocks.Bricks
{
    public class CoalBrick : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileSolid[Type] = true;
            Main.tileBrick[Type] = true;
            Main.tileBlockLight[Type] = true;

            AddMapEntry(new Color(38, 38, 39));

            DustType = ModContent.DustType<CoalDust>();
            HitSound = SoundID.Tink;
        }

        public override bool TileFrame(int i, int j, ref bool resetFrame, ref bool noBreak)
        {
            TileFraming.CommonFraming(i, j, customVariation: i % 3);
            Main.tile[i, j].TileFrameY += (short)(90 * (j % 3));
            return false;
        }
    }
}