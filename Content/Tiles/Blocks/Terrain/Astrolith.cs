using Macrocosm.Common.DataStructures;
using Macrocosm.Common.Subworlds;
using Macrocosm.Common.TileFrame;
using Macrocosm.Common.Utils;
using Macrocosm.Content.Dusts;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Tiles.Blocks.Terrain
{
    public class Astrolith : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileSolid[Type] = true;
            Main.tileBlockLight[Type] = true;
            Main.tileLighted[Type] = true;

            Main.tileMerge[Type][TileID.Meteorite] = true;
            Main.tileMerge[TileID.Meteorite][Type] = true;

            // Only to avoid slope slicing, TileFrame code is different
            TileID.Sets.HasSlopeFrames[Type] = true;

            TileID.Sets.ChecksForMerge[Type] = true;
            TileID.Sets.CanBeClearedDuringOreRunner[Type] = true;

            TileID.Sets.AllBlocksWithSmoothBordersToResolveHalfBlockIssue[Type] = true;
            TileID.Sets.IgnoresNearbyHalfbricksWhenDrawn[Type] = true;

            MineResist = 1f;

            AddMapEntry(new Color(70, 65, 65));

            HitSound = SoundID.Dig;
            DustType = ModContent.DustType<AstrolithDust>();
        }

        public override bool TileFrame(int i, int j, ref bool resetFrame, ref bool noBreak)
        {

            if (Main.tile[i, j].IsSloped())
                TileFraming.OutlineSlopeFraming(i, j, resetFrame);
            else
                TileFraming.CommonFraming(i, j, resetFrame);

            return false;
        }

        public override bool HasWalkDust() => Main.rand.NextBool(3) && MacrocosmSubworld.GetGravityMultiplier() != 0f;
        public override void WalkDust(ref int dustType, ref bool makeDust, ref Color color)
        {
            dustType = ModContent.DustType<AstrolithDust>();
        }
    }
}