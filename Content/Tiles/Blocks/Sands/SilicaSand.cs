using Macrocosm.Content.Dusts;
using Macrocosm.Content.Projectiles.Environment.Sands;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Tiles.Blocks.Sands
{
    public class SilicaSand : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileSolid[Type] = true;
            Main.tileBrick[Type] = true;
            Main.tileMergeDirt[Type] = true;
            Main.tileBlockLight[Type] = true;

            Main.tileSand[Type] = true;
            TileID.Sets.ForAdvancedCollision.ForSandshark[Type] = true;
            TileID.Sets.CanBeDugByShovel[Type] = true;
            TileID.Sets.Falling[Type] = true;
            TileID.Sets.Suffocate[Type] = true;
            TileID.Sets.FallingBlockProjectile[Type] = new(ModContent.ProjectileType<SilicaSandFalling>(), FallingProjectileDamage: 20);

            TileID.Sets.CanBeClearedDuringOreRunner[Type] = true;
            TileID.Sets.GeneralPlacementTiles[Type] = false;
            TileID.Sets.ChecksForMerge[Type] = true;

            MineResist = 0.5f;
            DustType = ModContent.DustType<SilicaSandDust>();
            AddMapEntry(new Color(199, 197, 171));
        }

        public override bool HasWalkDust() => Main.rand.NextBool(3);

        public override void WalkDust(ref int dustType, ref bool makeDust, ref Color color)
        {
            dustType = ModContent.DustType<SilicaSandDust>();
        }
    }
}