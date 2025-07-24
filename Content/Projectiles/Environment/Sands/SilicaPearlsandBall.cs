using Macrocosm.Content.Dusts;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Projectiles.Environment.Sands;

public class SilicaPearlsandBall : ModProjectile
{
    public override string Texture => base.Texture.Replace("Ball", "");

    public override void SetStaticDefaults()
    {
        ProjectileID.Sets.ForcePlateDetection[Type] = true;
        ProjectileID.Sets.FallingBlockDoesNotFallThroughPlatforms[Type] = true;
        ProjectileID.Sets.FallingBlockTileItem[Type] = new(ModContent.TileType<Tiles.Blocks.Sands.SilicaPearlsand>());
    }

    public override void SetDefaults()
    {
        Projectile.CloneDefaults(ProjectileID.PearlSandBallGun);
        AIType = ProjectileID.PearlSandBallGun;
    }

    public override void AI()
    {
        if (Main.rand.NextBool(5))
        {
            int dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<SilicaPearlsandDust>());
            Main.dust[dust].velocity.X *= 0.4f;
        }
    }
}