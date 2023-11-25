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
    public class MoonBasePlating : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileSolid[Type] = true;
            Main.tileBlockLight[Type] = true;
            Main.tileLighted[Type] = true;
            Main.tileBlendAll[Type] = true;

            //Main.tileMerge[ModContent.TileType<Regolith>()][Type] = true;
            //Main.tileMerge[ModContent.TileType<RegolithBrick>()][Type] = true;
            //Main.tileMerge[ModContent.TileType<Protolith>()][Type] = true;
            //Main.tileMerge[ModContent.TileType<IrradiatedRock>()][Type] = true;
            //Main.tileMerge[ModContent.TileType<IrradiatedBrick>()][Type] = true;
            //
            //Main.tileMerge[Type][ModContent.TileType<Regolith>()] = true;
            //Main.tileMerge[Type][ModContent.TileType<RegolithBrick>()] = true;
            //Main.tileMerge[Type][ModContent.TileType<Protolith>()] = true;
            //Main.tileMerge[Type][ModContent.TileType<IrradiatedRock>()] = true;
            //Main.tileMerge[Type][ModContent.TileType<IrradiatedBrick>()] = true;

            TileID.Sets.IgnoresNearbyHalfbricksWhenDrawn[Type] = true;
            //TileID.Sets.GemsparkFramingTypes[Type] = Type;

            DustType = ModContent.DustType<MoonBasePlatingDust>();

            MinPick = 225;
            MineResist = 4f;

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
            Tile tile = Main.tile[i, j];

            var info = new TileNeighbourInfo(i, j).GetPredicateNeighbourInfo((neighbour)
                => WorldGen.SolidTile(neighbour) && neighbour.TileType != Type);

            TileFraming.PlatingStyle(i, j);

            if (tile.IsSloped() || info.Count4Way > 0)
                tile.TileFrameY += 90;

            return false;
        }


    }
}