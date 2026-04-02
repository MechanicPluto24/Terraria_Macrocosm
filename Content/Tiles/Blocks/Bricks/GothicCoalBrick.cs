using Macrocosm.Common.TileFrame;
using Macrocosm.Content.Dusts;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Tiles.Blocks.Bricks;

public class GothicCoalBrick : ModTile
{
    public override void SetStaticDefaults()
    {
        Main.tileSolid[Type] = true;
        Main.tileBlockLight[Type] = true;

        TileID.Sets.GemsparkFramingTypes[Type] = Type;

        //TileID.Sets.AllBlocksWithSmoothBordersToResolveHalfBlockIssue[Type] = false;
        //TileID.Sets.IgnoresNearbyHalfbricksWhenDrawn[Type] = true;

        AddMapEntry(new Color(38, 38, 39));

        DustType = ModContent.DustType<CoalDust>();
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

    public override void KillTile(int i, int j, ref bool fail, ref bool effectOnly, ref bool noItem)
    {
    }

    public override bool TileFrame(int i, int j, ref bool resetFrame, ref bool noBreak)
    {
        TileFraming.GemsparkFraming(i, j, resetFrame);
        return false;
    }
}