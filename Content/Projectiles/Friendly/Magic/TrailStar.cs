using Microsoft.Xna.Framework;
using SteelSeries.GameSense;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Projectiles.Friendly.Magic
{
    public class TrailStar : ModProjectile
    {
        private float currentAmplitude; //Current amplitude of the sine wave
        private float frequency = 0.125f;  // The frequency of the sine wave movement
        private float elapsedTime; // Pretty self-explanatory
        private float amplitudeDecay = 0.2f; // Percentage of amplitude retained after each cycle
        private int cycleCount = 0; // Keep track of the cycles so we can dimish the amplitude on a per-cycle basis

        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 16;

            Projectile.aiStyle = 1;
            Projectile.friendly = true;
            Projectile.hostile = false;

            Projectile.timeLeft = 600;

            Projectile.ignoreWater = true;
            Projectile.tileCollide = true;

            AIType = ProjectileID.Bullet;
            Projectile.light = 0.7f;
            Projectile.penetrate = 5;
        }

        public override void OnSpawn(IEntitySource source)
        {
            currentAmplitude = Main.rand.NextFloat(-10f, 10f);
        }


        public override void AI()
        {
            Dust.NewDust(Projectile.Center, 16, 16, DustID.GemDiamond);
            elapsedTime += 1f;

            float phase = frequency * elapsedTime;

            // Check for completed cycles (2π radians is one complete cycle)
            if (phase >= (cycleCount + 1) * MathHelper.TwoPi)
            {
                cycleCount++;
                
                currentAmplitude *= amplitudeDecay;

                // Stop movement if amplitude gets too small
                if (currentAmplitude < 0.1f)
                    currentAmplitude = 0f;
            }

            float sineOffset = currentAmplitude * (float)Math.Sin(phase);
            Vector2 offset = Projectile.velocity.SafeNormalize(Vector2.Zero).RotatedBy(MathHelper.PiOver2) * sineOffset;
            Projectile.position += offset;

        }

        public override void OnKill(int timeLeft)
        {
            Dust.NewDust(Projectile.Center, 16, 16, DustID.GemDiamond, Scale: 2f);
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Projectile.penetrate--;
            if (Projectile.penetrate <= 0)
            {
                Projectile.Kill();
            }
        }
    }
}
