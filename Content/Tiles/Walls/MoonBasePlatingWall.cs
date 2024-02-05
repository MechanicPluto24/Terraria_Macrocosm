using Macrocosm.Common.Utils;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace Macrocosm.Content.Tiles.Walls
{
    public class MoonBasePlatingWall : ModWall
    {
        private static int[][] wallFrameNumberLookup;

        public override void SetStaticDefaults()
        {
            Main.wallHouse[Type] = true;
            AddMapEntry(new Color(71, 71, 74));

            // Adaptation of stone slab framing style (wallLargeFrames = 1),
            // but without the interlocked pattern (2 fewer repeats on Y)
            wallFrameNumberLookup = new int[2][] {
                new int[3] { 2, 4, 2 },
                new int[3] { 1, 3, 1 },
            };
        }

        public override bool WallFrame(int i, int j, bool randomizeFrame, ref int style, ref int frameNumber)
        {
            if (Utility.CoordinatesOutOfBounds(i, j))
                return false;

            frameNumber = wallFrameNumberLookup[j % 2][i % 3] - 1;
            return true;
        }
    }
}