using Macrocosm.Common.DataStructures;
using Macrocosm.Common.Subworlds;
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

            TileID.Sets.ChecksForMerge[Type] = true;
            TileID.Sets.CanBeClearedDuringOreRunner[Type] = true;

            MineResist = 1f;

            AddMapEntry(new Color(70, 65, 65));

            HitSound = SoundID.Dig;
            DustType = ModContent.DustType<AstrolithDust>();
        }

        public override bool HasWalkDust() => Main.rand.NextBool(3) && MacrocosmSubworld.GetGravityMultiplier() != 0f;

        public override void WalkDust(ref int dustType, ref bool makeDust, ref Color color)
        {
            dustType = ModContent.DustType<AstrolithDust>();
        }

        public override bool CanExplode(int i, int j) => true;
    }
}