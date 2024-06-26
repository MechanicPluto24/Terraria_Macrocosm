using Macrocosm.Common.Bases.Tiles;
using Macrocosm.Common.DataStructures;
using Macrocosm.Common.Utils;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Tiles.Blocks.Terrain
{
    public class IrradiatedRock : ModTile, IModifyTileFrame
    {
        public override void SetStaticDefaults()
        {
            Main.tileSolid[Type] = true;
            Main.tileBlockLight[Type] = true;
            Main.tileLighted[Type] = true;

            MinPick = 275;
            MineResist = 3f;
            AddMapEntry(new Color(199, 199, 184));
            HitSound = SoundID.Tink;
        }

        public void ModifyTileFrame(int i, int j, ref int up, ref int down, ref int left, ref int right, ref int upLeft, ref int upRight, ref int downLeft, ref int downRight)
        {
            var regolithInfo = new TileNeighbourInfo(i, j).GetPredicateNeighbourInfo((neighbour) => neighbour.TileType == ModContent.TileType<Regolith>());
            var protolithInfo = new TileNeighbourInfo(i, j).GetPredicateNeighbourInfo((neighbour) => neighbour.TileType == ModContent.TileType<Protolith>());

            WorldGen.TileMergeAttempt(-2, ModContent.TileType<Regolith>(), ref up, ref down, ref left, ref right, ref upLeft, ref upRight, ref downLeft, ref downRight);

            if (protolithInfo.Count > 0 && regolithInfo.Count == 0)
                WorldGen.TileMergeAttempt(-2, ModContent.TileType<Protolith>(), ref up, ref down, ref left, ref right, ref upLeft, ref upRight, ref downLeft, ref downRight);
        }

        public override void SetDrawPositions(int i, int j, ref int width, ref int offsetY, ref int height, ref short tileFrameX, ref short tileFrameY)
        {
            var regolithInfo = new TileNeighbourInfo(i, j).GetPredicateNeighbourInfo((neighbour) => neighbour.TileType == ModContent.TileType<Regolith>());
            var protolithInfo = new TileNeighbourInfo(i, j).GetPredicateNeighbourInfo((neighbour) => neighbour.TileType == ModContent.TileType<Protolith>());

            if (protolithInfo.Count > 0 && regolithInfo.Count == 0 && Utility.HasBlendingFrame(i, j))
                 tileFrameY += 180;
        }
    }
}