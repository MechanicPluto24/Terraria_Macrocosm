using Macrocosm.Common.DataStructures;
using Macrocosm.Common.Utils;
using Macrocosm.Content.Dusts;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Tiles.Blocks.Terrain
{
    public class Cynthalith : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileSolid[Type] = true;
            Main.tileBlockLight[Type] = true;
            Main.tileLighted[Type] = true;

            TileID.Sets.ChecksForMerge[Type] = true;
            Regolith.TileMerge[Type] = true;
            Protolith.TileMerge[Type] = true;

            TileID.Sets.CanBeClearedDuringOreRunner[Type] = true;

            MinPick = 225;
            MineResist = 3f;

            AddMapEntry(new Color(180, 180, 180));

            HitSound = SoundID.Dig;
            DustType = ModContent.DustType<CynthalithDust>();
        }

        public override bool HasWalkDust() => Main.rand.NextBool(3);

        public override void WalkDust(ref int dustType, ref bool makeDust, ref Color color)
        {
            dustType = ModContent.DustType<CynthalithDust>();
        }

        public override void ModifyFrameMerge(int i, int j, ref int up, ref int down, ref int left, ref int right, ref int upLeft, ref int upRight, ref int downLeft, ref int downRight)
        {
            var regolithInfo = new TileNeighbourInfo(i, j).GetPredicateNeighbourInfo((neighbour) => neighbour.TileType == ModContent.TileType<Regolith>());
            var protolithInfo = new TileNeighbourInfo(i, j).GetPredicateNeighbourInfo((neighbour) => neighbour.TileType == ModContent.TileType<Protolith>());

            WorldGen.TileMergeAttempt(-2, ModContent.TileType<Regolith>(), ref up, ref down, ref left, ref right, ref upLeft, ref upRight, ref downLeft, ref downRight);

            bool protolithMerge = (protolithInfo.Count > 0 && regolithInfo.Count == 0);
            WorldGen.TileMergeAttempt(protolithMerge ? -2 : Type, ModContent.TileType<Protolith>(), ref up, ref down, ref left, ref right, ref upLeft, ref upRight, ref downLeft, ref downRight);
        }

        public override void PostTileFrame(int i, int j, int up, int down, int left, int right, int upLeft, int upRight, int downLeft, int downRight)
        {
            Tile tile = Main.tile[i, j];

            var regolithInfo = new TileNeighbourInfo(i, j).GetPredicateNeighbourInfo((neighbour) => neighbour.TileType == ModContent.TileType<Regolith>());
            var protolithInfo = new TileNeighbourInfo(i, j).GetPredicateNeighbourInfo((neighbour) => neighbour.TileType == ModContent.TileType<Protolith>());

            if (protolithInfo.Count > 0 && regolithInfo.Count == 0 && Utility.HasBlendingFrame(i, j))
                tile.TileFrameY += 180;
        }
    }
}