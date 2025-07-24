using Macrocosm.Common.Bases.Walls;
using Macrocosm.Common.Enums;
using Macrocosm.Common.Utils;
using Macrocosm.Content.Dusts;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace Macrocosm.Content.Walls;

public class ProtolithBrickWall : VariantWall
{
    public override void SetVariantStaticDefaults(WallSafetyType variant)
    {
        AddMapEntry(new Color(145, 145, 145));
        DustType = ModContent.DustType<ProtolithDust>();

        if (variant == WallSafetyType.Unsafe)
            RegisterItemDrop(ModContent.ItemType<Items.Walls.ProtolithBrickWallUnsafe>());
        else
            RegisterItemDrop(ModContent.ItemType<Items.Walls.ProtolithBrickWall>());
    }

    public override bool WallFrame(int i, int j, bool randomizeFrame, ref int style, ref int frameNumber)
    {
        if (!WorldGen.InWorld(i, j))
            return false;

        int[][] wallFrameNumberLookup = [
            [1, 2, 3, 2],
            [2, 3, 2, 1],
            [3, 2, 1, 2],
        ];

        frameNumber = wallFrameNumberLookup[j % 3][i % 4] - 1;
        return true;
    }
}