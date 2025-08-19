using Macrocosm.Common.Bases.Walls;
using Macrocosm.Common.Enums;
using Macrocosm.Common.Utils;
using Macrocosm.Content.Dusts;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace Macrocosm.Content.Walls;

public class IndustrialPlatingWall : VariantWall
{
    private static int[][] wallFrameNumberLookup;

    public override void SetVariantStaticDefaults(WallSafetyType variant)
    {
        AddMapEntry(new Color(71, 71, 74));
        DustType = ModContent.DustType<IndustrialPlatingDust>();

        if (variant == WallSafetyType.Unsafe)
            RegisterItemDrop(ModContent.ItemType<Items.Walls.IndustrialPlatingWallUnsafe>());
        else
            RegisterItemDrop(ModContent.ItemType<Items.Walls.IndustrialPlatingWall>());

        // Adaption of stone slab framing style (wallLargeFrames = 1), but without the interlocked pattern (2 fewer repeats on Y)
        wallFrameNumberLookup = [
            [2, 4, 2],
            [1, 3, 1],
        ];
    }

    public override bool WallFrame(int i, int j, bool randomizeFrame, ref int style, ref int frameNumber)
    {
        if (!WorldGen.InWorld(i, j))
            return false;

        frameNumber = wallFrameNumberLookup[j % 2][i % 3] - 1;
        return true;
    }
}