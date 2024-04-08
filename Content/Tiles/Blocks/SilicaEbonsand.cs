using Macrocosm.Common.Utils;
using Macrocosm.Content.Dusts;
using Macrocosm.Content.Projectiles.Environment.Sands;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Tiles.Blocks
{
    public class SilicaEbonsand : ModTile
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
            TileID.Sets.FallingBlockProjectile[Type] = new(ModContent.ProjectileType<SilicaEbonsandFalling>(), FallingProjectileDamage: 15);

            TileID.Sets.CanBeClearedDuringOreRunner[Type] = true;
            TileID.Sets.GeneralPlacementTiles[Type] = false;
            TileID.Sets.ChecksForMerge[Type] = true;

            MineResist = 0.5f;
            DustType = ModContent.DustType<SilicaEbonsandDust>();
            AddMapEntry(new Color(195, 197, 196));
        }

        public override bool HasWalkDust() => Main.rand.NextBool(3);

        public override void WalkDust(ref int dustType, ref bool makeDust, ref Color color)
        {
            dustType = ModContent.DustType<SilicaEbonsandDust>();
        }
    }
}