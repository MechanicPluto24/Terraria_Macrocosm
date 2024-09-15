using Macrocosm.Common.Drawing.Particles;
using Macrocosm.Common.Subworlds;
using Macrocosm.Common.Utils;
using Macrocosm.Content.Dusts;
using Macrocosm.Content.Particles;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
namespace Macrocosm.Content.Projectiles.Friendly.Magic
{
    public class FrigorianGazeProjectile : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 3;
        }

        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.friendly = false;
            Projectile.hostile = false;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.timeLeft = 1000;
            Projectile.penetrate = -1;
            Projectile.tileCollide = true;
            Projectile.frame = 0;
        }

        public int AI_Timer
        {
            get => (int)Projectile.ai[0];
            set => Projectile.ai[0] = value;
        }

        public bool Broke
        {
            get => Projectile.ai[1] > 0;
            set => Projectile.ai[1] = value ? 1f : 0f;
        }

        public int BounceCounter
        {
            get => (int)Projectile.ai[2];
            set => Projectile.ai[2] = value;
        }

        public bool HitSomething => BounceCounter == 1;

        private bool exploded;

        private int timeUntilMandatoryBreak = 500;
        private int numBounces = 3;
        private bool spawned = false;

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Broke = true;
            Projectile.oldVelocity = oldVelocity;
            Bounce(oldVelocity);
            return false;
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
        }

        public override void AI()
        {
            if (exploded)
                return;

            if (!spawned)
            {
                Projectile.frame = 0;
                spawned = true;
            }

            Projectile.velocity.Y += 0.9f * MacrocosmSubworld.CurrentGravityMultiplier;

            if (Projectile.timeLeft < timeUntilMandatoryBreak)
            {
                Broke = true;
                Bounce(Projectile.oldVelocity);
            }

            if (Broke)
            {
                Projectile.rotation += Projectile.velocity.X * 0.01f;
                AI_Timer++;
            }
            else
            {
                Projectile.rotation += Projectile.velocity.X * 0.01f;
            }

            if (AI_Timer % 6 == 1)
                if (Main.netMode != NetmodeID.MultiplayerClient)
                    Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, Projectile.oldVelocity.SafeNormalize(Vector2.UnitX).RotatedByRandom(Math.PI / 4) * 17f, ModContent.ProjectileType<FrigorianIceShard>(), Projectile.damage / 4, 2, -1);

            if (BounceCounter > numBounces)
                CreateALotOfIce();

            Projectile.oldVelocity = Projectile.velocity * -1f;
        }

        private void Bounce(Vector2 oldVelocity)
        {
            BounceCounter++;

            if (BounceCounter < 3)
            {
                Projectile.frame = BounceCounter;

                if(!Main.dedServ && BounceCounter > 0)
                    Gore.NewGore(Projectile.GetSource_Death(), Projectile.position, Vector2.Zero, Mod.Find<ModGore>($"FrigorianGore{BounceCounter}").Type);
            }

            if (BounceCounter < numBounces)
            {
                float bounceFactor = 0.5f + 0.5f * BounceCounter / (float)numBounces;

                for (int i = 0; i < (int)10; i++)
                {
                    Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<FrigorianDust>());
                    dust.velocity.X = Main.rand.Next(-30, 31) * 0.02f;
                    dust.velocity.Y = Main.rand.Next(-30, 30) * 0.02f;
                    dust.scale *= 1f + Main.rand.Next(-12, 13) * 0.01f;
                    dust.noGravity = true;
                }

                 SoundEngine.PlaySound(SoundID.Item107 with
                {
                    Volume = 0.5f * bounceFactor,
                    Pitch = 0.25f + 0.35f * bounceFactor
                },
                Projectile.position);

                if (Projectile.velocity.X != oldVelocity.X)
                    Projectile.velocity.X = oldVelocity.X * -bounceFactor;

                if (Projectile.velocity.Y != oldVelocity.Y && oldVelocity.Y > 0.7f)
                    Projectile.velocity.Y = oldVelocity.Y * -bounceFactor;

                Projectile.rotation = 0f;
            }
        }

        private void CreateALotOfIce()
        {
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, (-Vector2.UnitY * 8f).RotatedByRandom(Math.PI / 4), ModContent.ProjectileType<FrigorianIceCrystal>(), Projectile.damage / 2, 2, -1);
                Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, (-Vector2.UnitY * 8f).RotatedByRandom(Math.PI / 4), ModContent.ProjectileType<FrigorianIceCrystal>(), Projectile.damage / 2, 2, -1);
                Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, (-Vector2.UnitY * 8f).RotatedByRandom(Math.PI / 4), ModContent.ProjectileType<FrigorianIceCrystal>(), Projectile.damage / 2, 2, -1);
            }

            for (int i = 0; i < 5; i++)
                Gore.NewGore(Projectile.GetSource_Death(), Projectile.position, Projectile.velocity * 0.1f, Mod.Find<ModGore>("FrigorianGore3").Type);

            for (int i = 0; i < 30; i++)
            {
                Particle.CreateParticle<IceMist>((p) =>
                {
                    p.Position = Projectile.Center;
                    p.Velocity = -Vector2.UnitY.RotatedByRandom(MathHelper.TwoPi) * Main.rand.NextFloat(0.2f, 4f);
                    p.Scale = new(Main.rand.NextFloat(0.2f, 0.5f));
                }, shouldSync: true
                );
            }

            for (int i = 0; i < 40; i++)
            {
                Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<FrigorianDust>());
                dust.velocity.X = Main.rand.Next(-70, 71) * 0.04f;
                dust.velocity.Y = Main.rand.Next(-70, 70) * 0.04f;
                dust.scale *= 1f + Main.rand.Next(-30, 31) * 0.02f;
                dust.noGravity = true;
            }

            Projectile.friendly = true;
            Projectile.Resize(300, 300);
            Projectile.timeLeft = 3;
            Projectile.alpha = 255;
            Projectile.velocity *= 0;
            exploded = true;
        }

        public override Color? GetAlpha(Color lightColor) => lightColor;

        public override bool PreDraw(ref Color lightColor)
        {
            return base.PreDraw(ref lightColor);
        }

        public override void PostDraw(Color lightColor)
        {
            base.PostDraw(lightColor);
        }
    }
}
