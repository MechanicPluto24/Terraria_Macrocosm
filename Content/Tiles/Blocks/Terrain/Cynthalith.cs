using Macrocosm.Common.Bases.Tiles;
using Macrocosm.Common.Utils;
using Macrocosm.Content.Dusts;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Tiles.Blocks.Terrain
{
    public class Cynthalith : ModTile, IModifyTileFrame
    {
        public override void SetStaticDefaults()
        {
            Main.tileSolid[Type] = true;
            Main.tileBlockLight[Type] = true;
            Main.tileLighted[Type] = true;

            TileID.Sets.CanBeClearedDuringOreRunner[Type] = true;

            MinPick = 225;
            MineResist = 3f;
            AddMapEntry(new Color(180, 180, 180));
            HitSound = SoundID.Dig;
            DustType = ModContent.DustType<RegolithDust>();
        }

        public override bool HasWalkDust() => Main.rand.NextBool(3);

        public override void WalkDust(ref int dustType, ref bool makeDust, ref Color color)
        {
            dustType = ModContent.DustType<RegolithDust>();
        }

        private static readonly bool[] blocksThatMerge = TileID.Sets.Factory.CreateBoolSet(ModContent.TileType<Protolith>(), ModContent.TileType<IrradiatedRock>());
        public void ModifyTileFrame(int i, int j, ref int up, ref int down, ref int left, ref int right, ref int upLeft, ref int upRight, ref int downLeft, ref int downRight)
        {
            WorldGen.TileMergeAttemptFrametest(i, j, Type, blocksThatMerge, ref up, ref down, ref left, ref right, ref upLeft, ref upRight, ref downLeft, ref downRight);
        }
    }
}