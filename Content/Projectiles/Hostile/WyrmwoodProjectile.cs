using Macrocosm.Common.Subworlds;
using Macrocosm.Common.Utils;
using Macrocosm.Content.Dusts;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
namespace Macrocosm.Content.Projectiles.Hostile;

public class WyrmwoodProjectile : ModProjectile
{
    private const int TicksPerFrame = 6;

    public override void SetStaticDefaults()
    {
        Main.projFrames[Type] = 4;
    }

    public override void SetDefaults()
    {
        Projectile.width = 8;
        Projectile.height = 8;
        Projectile.hostile = true;
        Projectile.friendly = false;
        Projectile.timeLeft = 1200;
        Projectile.penetrate = -1;

        Projectile.tileCollide = true;
    }

    public override void AI()
    {
        Projectile.velocity.Y += MacrocosmSubworld.GetGravityMultiplier(Projectile.Center) / 4;
        Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;

        if (Projectile.frameCounter++ >= TicksPerFrame)
        {
            Projectile.frameCounter = 0;
            Projectile.frame++;

            if (Projectile.frame >= Main.projFrames[Type])
                Projectile.frame = 0;
        }

    }

    public override void OnKill(int timeLeft)
    {
        for (int i = 0; i < 15; i++)
        {
            Vector2 dustVelocity = Utility.PolarVector(0.01f, Utility.RandomRotation());
            Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<TarDust>(), dustVelocity.X, dustVelocity.Y, newColor: Color.White * 0.1f, Scale: Main.rand.NextFloat(0.5f, 0.8f));
        }
    }
}
