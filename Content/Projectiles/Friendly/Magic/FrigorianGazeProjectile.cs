using Macrocosm.Common.DataStructures;
using Macrocosm.Common.Drawing.Particles;
using Macrocosm.Common.Subworlds;
using Macrocosm.Common.Utils;
using Macrocosm.Content.Dusts;
using Macrocosm.Content.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
namespace Macrocosm.Content.Projectiles.Friendly.Magic
{
    public class FrigorianGazeProjectile : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 3;

            ProjectileID.Sets.TrailCacheLength[Type] = 15;
            ProjectileID.Sets.TrailingMode[Type] = 3;
        }

        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.timeLeft = 1000;
            Projectile.penetrate = -1;
            Projectile.tileCollide = true;
            Projectile.frame = 0;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
        }

        public int IceShardCounter
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
            Broke = true;
            Projectile.oldVelocity = -Projectile.velocity;
            Bounce(Projectile.oldVelocity);

        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Projectile.owner == Main.myPlayer)
            {
                for (int i = 0; i < 2; i++)
                {
                    Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, (-Vector2.UnitY * 8f).RotatedByRandom(Math.PI / 4), ModContent.ProjectileType<FrigorianIceCrystal>(), Projectile.damage / 2, 2, -1);
                }
            }
            CreateALotOfIce();
        }

        public override void AI()
        {
            if (!spawned)
            {
                IceShardCounter = 9;
                Projectile.frame = 0;
                spawned = true;
            }

            Projectile.velocity.Y += 0.6f * (0.3f + 0.7f * MacrocosmSubworld.GetGravityMultiplier()) * (0.6f + ((IceShardCounter + 1) / 9));

            if (Projectile.timeLeft < timeUntilMandatoryBreak)
            {
                Broke = true;
                Bounce(Projectile.oldVelocity);
            }

            if (Broke)
            {
                Projectile.rotation += Projectile.velocity.X * 0.01f;
            }
            else
            {
                Projectile.rotation += Projectile.velocity.X * 0.01f;
            }

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

                if (!Main.dedServ && BounceCounter > 0)
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
                for (int i = 0; i < Main.rand.Next(2, 3); i++)
                {
                    if (Projectile.owner == Main.myPlayer)
                        Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, -Projectile.oldVelocity.SafeNormalize(Vector2.UnitX).RotatedByRandom(Math.PI / 4) * 17f, ModContent.ProjectileType<FrigorianIceShard>(), Projectile.damage / 4, 2, -1);
                    IceShardCounter--;
                }


                Particle.Create<PrettySparkle>((p) =>
                {
                    p.Position = Projectile.Center + Projectile.oldVelocity;
                    p.Velocity = Vector2.Zero;
                    p.DrawHorizontalAxis = true;
                    p.DrawVerticalAxis = true;
                    p.AdditiveAmount = 0.4f;
                    p.Scale = new(2f);
                    p.ScaleVelocity = -new Vector2(Main.rand.NextFloat(0.01f, 0.02f));
                    p.Color = (Main.rand.NextBool() ? new Color(56, 188, 173) : new Color(93, 81, 164));
                    p.Rotation = 0f;
                    p.TimeToLive = 30;
                    p.FadeInNormalizedTime = 0f;
                    p.FadeOutNormalizedTime = 0.5f;
                });

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
            if (Projectile.owner == Main.myPlayer)
            {
                for (int i = 0; i < IceShardCounter; i++)
                {
                    Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, Projectile.oldVelocity.SafeNormalize(Vector2.UnitX).RotatedByRandom(Math.PI / 4) * 17f, ModContent.ProjectileType<FrigorianIceShard>(), Projectile.damage / 4, 2, -1);
                }
            }

            SoundEngine.PlaySound(SoundID.Item107 with
            {
                Volume = 1,
                Pitch = 0f
            },
            Projectile.position);

            for (int i = 0; i < 5; i++)
                Gore.NewGore(Projectile.GetSource_Death(), Projectile.position, Projectile.velocity * 0.1f, Mod.Find<ModGore>("FrigorianGore3").Type);

            for (int i = 0; i < 30; i++)
            {
                Particle.Create<IceMist>((p) =>
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

            Projectile.Kill();
        }

        public override Color? GetAlpha(Color lightColor) => lightColor;

        private SpriteBatchState state;

        public override bool PreDraw(ref Color lightColor)
        {
            int length = Projectile.oldPos.Length;
            Rectangle frame = TextureAssets.Projectile[Type].Frame(verticalFrames: Main.projFrames[Type], frameY: Projectile.frame);

            state.SaveState(Main.spriteBatch);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(BlendState.Additive, state);
            for (int i = 1; i < length; i++)
            {
                Vector2 drawPos = Projectile.oldPos[i] - Main.screenPosition;
                Color trailColor = Color.White * (((float)Projectile.oldPos.Length - i) / Projectile.oldPos.Length) * 0.45f * (1f - Projectile.alpha / 255f);
                Main.EntitySpriteDraw(TextureAssets.Projectile[Type].Value, drawPos, frame, trailColor, Projectile.oldRot[i], frame.Size() / 2f, Projectile.scale, Projectile.oldSpriteDirection[i] == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0f);
            }

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(state);

            Main.EntitySpriteDraw(TextureAssets.Projectile[Type].Value, Projectile.position - Main.screenPosition, frame, GetAlpha(lightColor) ?? Color.White, Projectile.rotation, frame.Size() / 2f, Projectile.scale, Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0f);
            return false;
        }

        public override void PostDraw(Color lightColor)
        {
            base.PostDraw(lightColor);
        }
    }
}
