using Macrocosm.Content.Dusts;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Projectiles.Environment.Sands
{
    public class SilicaSandFalling : ModProjectile
    {
        public override string Texture => base.Texture.Replace("Falling", "");

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.ForcePlateDetection[Type] = true;
            ProjectileID.Sets.FallingBlockDoesNotFallThroughPlatforms[Type] = true;
            ProjectileID.Sets.FallingBlockTileItem[Type] = new(ModContent.TileType<Tiles.Blocks.Sands.SilicaSand>(), ModContent.ItemType<Items.Blocks.Sands.SilicaSand>());
        }

        public override void SetDefaults()
        {
            Projectile.CloneDefaults(ProjectileID.SandBallFalling);
        }

        public override void AI()
        {
            if (Main.rand.NextBool(5))
            {
                int dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<SilicaSandDust>());
                Main.dust[dust].velocity.X *= 0.4f;
            }
        }
    }
}