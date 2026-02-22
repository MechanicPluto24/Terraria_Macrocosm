using Macrocosm.Common.DataStructures;
using Macrocosm.Common.TileFrame;
using Macrocosm.Common.Utils;
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
        //Main.tileBrick[Type] = true; keep false
        Main.tileBlockLight[Type] = true;
        Main.tileBlendAll[Type] = true;

        TileID.Sets.HasSlopeFrames[Type] = true;
        TileID.Sets.GemsparkFramingTypes[Type] = Type;

        MinPick = 225;
        MineResist = 3f;

        AddMapEntry(new Color(38, 38, 39));

        DustType = ModContent.DustType<ProtolithDust>();
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
        int index = (i + 3 * (j % 2)) % 6;
        TileFraming.GemsparkFraming(i, j, resetFrame, customVariation: index % 3);
        TileFraming.SlopeFraming(i, j);
        return false;
    }

    public override void SetDrawPositions(int i, int j, ref int width, ref int offsetY, ref int height, ref short tileFrameX, ref short tileFrameY)
    {
        Tile tile = Main.tile[i, j];
        if (tile.IsSloped())
        {
            tileFrameY += 90;
        }
        else if (new TileNeighbourInfo(i, j).GetPredicateNeighbourInfo((t) => WorldGen.SolidTile(t) && t.TileType != Type || TileID.Sets.NotReallySolid[t.TileType] || Utility.IsPlatform(t.TileType)).Count4Way > 0)
        {
            tileFrameY += 90;
        }
        else if (Utility.HasInnerFrame(i, j))
        {
            int index = (i + 3 * (j % 2)) % 6;
            if (index >= 3)
                tileFrameY += 90;
        }
    }
}