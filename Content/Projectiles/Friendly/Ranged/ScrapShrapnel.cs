using Macrocosm.Common.Utils;
using Macrocosm.Content.Dusts;
using Macrocosm.Content.Subworlds;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Projectiles.Friendly.Ranged;

public class ScrapShrapnel : ModProjectile
{
    public override void SetStaticDefaults()
    {
        Main.projFrames[Type] = 5;
    }

    public override void SetDefaults()
    {
        Projectile.width = 12;
        Projectile.height = 12;
        Projectile.friendly = true;
        Projectile.DamageType = DamageClass.Ranged;
        Projectile.knockBack = 6;
        Projectile.penetrate = 3;
    }

    public override void AI()
    {
        // Animation
        if (++Projectile.frameCounter >= 10)
        {
            Projectile.frameCounter = 0;

            if (++Projectile.frame >= 5)
            {
                Projectile.frame = Main.rand.Next(5);
            }
        }

        // Rotation
        Projectile.rotation += Main.rand.NextFloat(0.05f, 0.1f) * (float)Projectile.direction;

        // Drag
        Projectile.velocity.X = Projectile.velocity.X * 0.99f;

        // Gravity
        Projectile.velocity.Y = Projectile.velocity.Y + 0.2f * Moon.GetGravityMultiplier();

        if (Projectile.velocity.Y > 16f)
        {
            Projectile.velocity.Y = 16f;
        }
    }

    public override void OnKill(int timeLeft)
    {
        // SFX
        SoundEngine.PlaySound(SoundID.Dig with
        {
            Volume = 0.20f,
            Pitch = 0.8f,
            PitchVariance = 0.8f
        });

        // Dust
        for (int d = 0; d < 10; d++)
        {
            d = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<ArtemiteDust>());
            Main.dust[d].velocity *= 0.5f;
            Main.dust[d].scale *= 0.9f;
        }
    }
}
