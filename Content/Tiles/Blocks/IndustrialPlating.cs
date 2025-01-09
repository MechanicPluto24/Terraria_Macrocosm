using Macrocosm.Common.DataStructures;
using Macrocosm.Common.TileFrame;
using Macrocosm.Common.Utils;
using Macrocosm.Content.Dusts;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Tiles.Blocks
{
    [LegacyName("MoonBasePlating")]
    public class IndustrialPlating : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileSolid[Type] = true;
            Main.tileBlockLight[Type] = true;
            Main.tileLighted[Type] = true;
            Main.tileBlendAll[Type] = true;

            // To avoid things overlapping with outposts or the Moon Base
            TileID.Sets.GeneralPlacementTiles[Type] = false;

            TileID.Sets.IgnoresNearbyHalfbricksWhenDrawn[Type] = true;

            DustType = ModContent.DustType<IndustrialPlatingDust>();

            MinPick = 100;
            MineResist = 2f;

            AddMapEntry(new Color(180, 180, 180));
        }

        public override bool Slope(int i, int j)
        {
            WorldGen.TileFrame(i + 1, j + 1);
            WorldGen.TileFrame(i + 1, j - 1);
            WorldGen.TileFrame(i - 1, j + 1);
            WorldGen.TileFrame(i - 1, j - 1);
            return true;
        }

        public override void KillTile(int i, int j, ref bool fail, ref bool effectOnly, ref bool noItem)
        {
        }

        public override bool TileFrame(int i, int j, ref bool resetFrame, ref bool noBreak)
        {
            TileFraming.PlatingStyle(i, j);
            return false;
        }

        public override void SetDrawPositions(int i, int j, ref int width, ref int offsetY, ref int height, ref short tileFrameX, ref short tileFrameY)
        {
            Tile tile = Main.tile[i, j];

            var info = new TileNeighbourInfo(i, j).GetPredicateNeighbourInfo
            (
                (neighbour) =>
                    WorldGen.SolidTile(neighbour) && neighbour.TileType != Type ||
                    Utility.IsPlatform(neighbour.TileType) ||
                    TileID.Sets.CloseDoorID[neighbour.TileType] > 0 ||
                    neighbour.TileType == TileID.TallGateOpen

            );

            if (tile.IsSloped() || info.Count4Way > 0)
                tileFrameY += 90;
        }
    }
}