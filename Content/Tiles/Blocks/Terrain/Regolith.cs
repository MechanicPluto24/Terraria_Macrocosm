using Macrocosm.Content.Dusts;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Tiles.Blocks.Terrain
{
    public class Regolith : ModTile
    {
        /// <summary> Types that merge with this. Use instead of <c>Main.tileFrame[][]</c> </summary>
        public static bool[] TileMerge { get; set; } = TileID.Sets.Factory.CreateBoolSet();

        public override void SetStaticDefaults()
        {
            Main.tileSolid[Type] = true;
            Main.tileBlockLight[Type] = true;
            Main.tileLighted[Type] = true;

            TileID.Sets.ChecksForMerge[Type] = true;

            TileID.Sets.CanBeClearedDuringOreRunner[Type] = true;

            MinPick = 225;
            MineResist = 3f;

            AddMapEntry(new Color(220, 220, 220));

            HitSound = SoundID.Dig;
            DustType = ModContent.DustType<RegolithDust>();
        }

        public override bool HasWalkDust() => true;

        public override void WalkDust(ref int dustType, ref bool makeDust, ref Color color)
        {
            dustType = ModContent.DustType<RegolithDust>();
        }

        public override void ModifyFrameMerge(int i, int j, ref int up, ref int down, ref int left, ref int right, ref int upLeft, ref int upRight, ref int downLeft, ref int downRight)
        {
            WorldGen.TileMergeAttemptFrametest(i, j, Type, TileMerge, ref up, ref down, ref left, ref right, ref upLeft, ref upRight, ref downLeft, ref downRight);
        }
    }
}