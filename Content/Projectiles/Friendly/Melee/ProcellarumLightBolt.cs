using Macrocosm.Common.Drawing.Particles;
using Macrocosm.Common.Utils;
using Macrocosm.Content.Particles;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace Macrocosm.Content.Projectiles.Friendly.Melee
{
    public class ProcellarumLightBolt : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.width = 45;
            Projectile.height = 45;
            DrawOffsetX = -30;

            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = 1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 4;
            Projectile.timeLeft = 90;
        }

        public override void AI()
        {
            if (Projectile.ai[0] != 0)
                HomeIn(Projectile.ai[0] - 1, Projectile.ai[1]);
            else
                Projectile.rotation = Projectile.velocity.ToRotation();
        }

        public void HomeIn(float targetIndex, float firedIndex)
        {
            NPC target = Main.npc[(int)targetIndex];
            float TargetRotation = (target.Center - Projectile.Center).ToRotation();
            float currentRotation = Projectile.velocity.ToRotation();
            if (TargetRotation - currentRotation <= MathHelper.PiOver4 && TargetRotation - currentRotation >= -MathHelper.PiOver4)
            {
                Projectile.rotation = 0.5f * (TargetRotation + currentRotation);
                Projectile.velocity = Utility.PolarVector(40 + firedIndex * 4, Projectile.rotation);
            }
            else
            {
                Projectile.rotation = currentRotation;
            }
        }

        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 40; i++)
            {
                Particle.Create<TintableSpark>((p) =>
                {
                    p.Position = Projectile.Center;
                    p.Velocity = Projectile.velocity.RotatedByRandom(MathHelper.TwoPi) * Main.rand.NextFloat(0.05f, 0.2f);
                    p.Scale = new(2f);
                    p.Rotation = 0f;
                    p.Color = new List<Color>() {
                        new(77, 99, 124),
                        new(90, 83, 92),
                        new(232, 239, 255)
                        }.GetRandom();
                });
            }
            Particle.Create<TintableFlash>((p) =>
            {
                p.Position = Projectile.Center;
                p.Scale = new(0.01f);
                p.ScaleVelocity = new(0.2f);
                p.Color = new Color(232, 239, 255);
            });
        }

        public override bool PreDraw(ref Color lightColor)
        {
            return true;
        }

        public override Color? GetAlpha(Color lightColor) => Color.White;
    }
}
