using Macrocosm.Content.Dusts;
using Macrocosm.Content.Projectiles.Environment.Sands;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Tiles.Blocks.Sands
{
    public class SilicaPearlsand : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileSand[Type] = true;

            Main.tileSolid[Type] = true;
            Main.tileBlockLight[Type] = true;

            // Merge with most blocks
            Main.tileBrick[Type] = true;

            // Merge with dirt without blend frames
            Main.tileMergeDirt[Type] = false;
            Main.tileMerge[TileID.Dirt][Type] = true;
            Main.tileMerge[Type][TileID.Dirt] = true;

            TileID.Sets.ForAdvancedCollision.ForSandshark[Type] = true;
            TileID.Sets.CanBeDugByShovel[Type] = true;
            TileID.Sets.Falling[Type] = true;
            TileID.Sets.Suffocate[Type] = true;
            TileID.Sets.FallingBlockProjectile[Type] = new(ModContent.ProjectileType<SilicaPearlsandFalling>(), FallingProjectileDamage: 20);

            TileID.Sets.CanBeClearedDuringOreRunner[Type] = true;
            TileID.Sets.ChecksForMerge[Type] = true;

            MineResist = 0.5f;
            DustType = ModContent.DustType<SilicaPearlsandDust>();

            AddMapEntry(new Color(202, 188, 198));
        }

        public override bool HasWalkDust() => true;

        public override void WalkDust(ref int dustType, ref bool makeDust, ref Color color)
        {
            dustType = ModContent.DustType<SilicaPearlsandDust>();
        }
    }
}