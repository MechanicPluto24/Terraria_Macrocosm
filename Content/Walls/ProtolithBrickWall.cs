using Macrocosm.Common.Utils;
using Macrocosm.Content.Dusts;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace Macrocosm.Content.Walls
{
    public class ProtolithBrickWall : ModWall
    {
        public override bool IsLoadingEnabled(Mod mod) => false;

        public override void SetStaticDefaults()
        {
            Main.wallHouse[Type] = true;
            AddMapEntry(new Color(145, 145, 145));

            DustType = ModContent.DustType<ProtolithDust>();
        }

        public override bool WallFrame(int i, int j, bool randomizeFrame, ref int style, ref int frameNumber)
        {
            if (Utility.CoordinatesOutOfBounds(i, j))
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

    public class ProtolithBrickWallNatural : ProtolithBrickWall
    {
        public override string Texture => base.Texture.Replace("Natural", "");

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            Main.wallHouse[Type] = false;
            RegisterItemDrop(ModContent.ItemType<Items.Walls.ProtolithBrickWall>());
        }
    }

    public class ProtolithBrickWallUnsafe : ProtolithBrickWall
    {
        public override string Texture => base.Texture.Replace("Unsafe", "");

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            Main.wallHouse[Type] = false;
        }
    }
}