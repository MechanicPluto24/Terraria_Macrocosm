using Macrocosm.Common.Utils;
using Macrocosm.Content.Dusts;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace Macrocosm.Content.Tiles.Walls
{
    [LegacyName("MoonBasePlatingWall")]
    public class IndustrialPlatingWall : ModWall
    {
        private static int[][] wallFrameNumberLookup;

        public override void SetStaticDefaults()
        {
            Main.wallHouse[Type] = true;
            AddMapEntry(new Color(71, 71, 74));

            DustType = ModContent.DustType<IndustrialPlatingDust>();

            // Adaptation of stone slab framing style (wallLargeFrames = 1),
            // but without the interlocked pattern (2 fewer repeats on Y)
            wallFrameNumberLookup = [
                [2, 4, 2],
                [1, 3, 1],
            ];
        }

        public override bool WallFrame(int i, int j, bool randomizeFrame, ref int style, ref int frameNumber)
        {
            if (Utility.CoordinatesOutOfBounds(i, j))
                return false;

            frameNumber = wallFrameNumberLookup[j % 2][i % 3] - 1;
            return true;
        }
    }

    [LegacyName("MoonBasePlatingWallNatural")]
    public class IndustrialPlatingWallNatural : IndustrialPlatingWall
    {
        public override string Texture => base.Texture.Replace("Natural", "");

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            Main.wallHouse[Type] = false;
            RegisterItemDrop(ModContent.ItemType<Items.Walls.IndustrialPlatingWall>());
        }
    }

    [LegacyName("MoonBasePlatingWallUnsafe")]
    public class IndustrialPlatingWallUnsafe : IndustrialPlatingWall
    {
        public override string Texture => base.Texture.Replace("Unsafe", "");

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            Main.wallHouse[Type] = false;
        }
    }
}