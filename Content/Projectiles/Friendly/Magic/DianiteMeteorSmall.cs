using Macrocosm.Common.Drawing.Particles;
using Macrocosm.Common.Utils;
using Macrocosm.Content.Dusts;
using Macrocosm.Content.Particles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace Macrocosm.Content.Projectiles.Friendly.Magic;

public class DianiteMeteorSmall : DianiteMeteor
{
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
    }

    public override void SetDefaults()
    {
        base.SetDefaults();
        Projectile.width = 16;
        Projectile.height = 16;
    }

    public override bool PreDraw(ref Color lightColor)
    {
        trailOffset = 4;
        return base.PreDraw(ref lightColor);
    }

    public override void OnKill(int timeLeft)
    {
        for (int i = 0; i < 45; i++)
        {
            Vector2 velocity = Main.rand.NextVector2Circular(5f, 5f);
            Dust dust = Dust.NewDustDirect(Projectile.position + Projectile.oldVelocity, (int)(Projectile.width), Projectile.height, ModContent.DustType<DianiteBrightDust>(), velocity.X, velocity.Y, Scale: 1f);
            dust.noGravity = true;
        }

        //var smoke = Particle.CreateParticle<Smoke>(Projectile.Center + Projectile.oldVelocity, Vector2.Zero, scale: 0.6f);
        //smoke.Velocity *= 0.2f;
        //smoke.Velocity.X += Main.rand.Next(-10, 11) * 0.15f;
        //smoke.Velocity.Y += Main.rand.Next(-10, 11) * 0.15f;

        var explosion = Particle.Create<TintableExplosion>(p =>
        {
            p.Position = Projectile.Center + Projectile.oldVelocity + Main.rand.NextVector2Circular(10f, 10f);
            p.Color = new Color(195, 115, 62).WithOpacity(0.6f);
            p.Scale = new(0.6f);
            p.NumberOfInnerReplicas = 4;
            p.ReplicaScalingFactor = 0.3f;
        });
    }
}