using Macrocosm.Common.TileFrame;
using Macrocosm.Content.Dusts;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Tiles.Blocks.Bricks;

public class ProtolithBrick : ModTile
{
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
        TileFraming.PlatingStyle(i, j, customVariation: i % 3);
        return true;
    }

    public override void PostTileFrame(int i, int j, int up, int down, int left, int right, int upLeft, int upRight, int downLeft, int downRight)
    {
        if (j % 2 == 0)
        {
            Tile t = Main.tile[i, j];
            t.TileFrameY += 270;
        }
    }
}