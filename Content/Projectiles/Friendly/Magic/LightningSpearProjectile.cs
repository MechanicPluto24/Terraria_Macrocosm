using Macrocosm.Common.CrossMod;
using Macrocosm.Common.Drawing.Particles;
using Macrocosm.Common.Utils;
using Macrocosm.Content.Dusts;
using Macrocosm.Content.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Linq;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using System.Collections.Generic;
using Terraria.Graphics;
using Terraria.Graphics.Shaders;
using ReLogic.Content;
using ReLogic.Utilities;
using Macrocosm.Common.DataStructures;
using Terraria.ModLoader;
using Terraria.GameContent;
using System;

namespace Macrocosm.Content.Projectiles.Friendly.Magic
{
    public class LightningSpearProjectile : ModProjectile
    {

        public override void SetDefaults()
        {
            Projectile.width = 20;
            Projectile.height = 10;

            AIType = ProjectileID.WoodenArrowFriendly;

            Projectile.friendly = true; 
            Projectile.DamageType = DamageClass.Magic;
            Projectile.ignoreWater = true; 
            Projectile.tileCollide = true;

            ProjectileID.Sets.TrailCacheLength[Type] = 3;
            ProjectileID.Sets.TrailingMode[Type] = 2;

            Redemption.SetSpearBonus(Projectile);
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.Electrified, 300);
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            float _ = float.NaN;
            Vector2 beamEndPos = Projectile.Center + (Projectile.velocity.SafeNormalize(Vector2.UnitX)) * (4*28);
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.Center - (Projectile.velocity.SafeNormalize(Vector2.UnitX)) * (4*28), beamEndPos, 10 * Projectile.scale, ref _);
        }

        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation();

            Projectile.direction = Math.Sign(Projectile.velocity.X);
            Projectile.tileCollide = true;
            Projectile.velocity.Y += 0.12f;

            if (!Main.dedServ)
            {
                int type =226;
                Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, type, Projectile.velocity.X / -64f, Projectile.velocity.Y / -16f, Scale: 0.8f);
                dust.noGravity = true;
            }
            
        }

        public override void OnKill(int timeLeft)
        {
                for (int i = 0; i < Main.rand.Next(30, 40); i++)
                {
                    int type = 226;
                    Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, type, Main.rand.NextFloat(-1.6f, 1.6f), Main.rand.NextFloat(-1.6f, 1.6f), Scale: Main.rand.NextFloat(0.7f, 1f));
                    dust.noGravity = true;
                    dust = Dust.NewDustDirect(Projectile.oldPos.ToList().GetRandom(), Projectile.width, Projectile.height, type, Main.rand.NextFloat(-1.6f, 1.6f), Main.rand.NextFloat(-1.6f, 1.6f), Scale: Main.rand.NextFloat(0.7f, 1f));
                    dust.noGravity = true;
                }
            Particle.Create<LightningParticle>((p) =>
                {
                    p.Position = Projectile.Center;
                    p.Velocity = new Vector2(12).RotatedByRandom(MathHelper.TwoPi) * Main.rand.NextFloat(0.2f, 0.8f);
                    p.Scale = new(Main.rand.NextFloat(1f));
                    p.Rotation = p.Velocity.ToRotation();
                    p.Color = new List<Color>() {
                        new(200, 200, 255, 0)
                    }.GetRandom();
                    p.OutlineColor = p.Color * 0.2f;
                    p.FadeOutNormalizedTime = 0.5f;
                });
            
        }
        private SpriteBatchState state1;
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Type].Value;
            state1.SaveState(Main.spriteBatch);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, state1);

            
                VertexStrip strip = new();
                int stripDataCount = 36;
                Vector2[] positions = new Vector2[stripDataCount];
                float[] rotations = new float[stripDataCount];
                Array.Fill(positions, Projectile.Center + (new Vector2(0, Projectile.direction==1 ? 3:-3).RotatedBy(Projectile.rotation + (MathHelper.Pi)) * Projectile.direction) - Main.screenPosition);
                Array.Fill(rotations, Projectile.rotation + (MathHelper.Pi) + (MathHelper.Pi));

                for (int i = 0; i < stripDataCount; i++)
                    positions[i] += Utility.PolarVector(4f * i, Projectile.rotation + (MathHelper.Pi));

                var shader = new MiscShaderData(Utility.VanillaVertexShader, "MagicMissile")
                    .UseProjectionMatrix(doUse: true)
                    .UseSaturation(-2.4f)
                    .UseImage0(ModContent.Request<Texture2D>(Macrocosm.FancyTexturesPath + "FadeOutMask"))
                    .UseImage1(ModContent.Request<Texture2D>(Macrocosm.FancyTexturesPath + "Spark5"))
                    .UseImage2(ModContent.Request<Texture2D>(Macrocosm.FancyTexturesPath + "Spark6"));

                shader.Apply();

                strip.PrepareStrip(positions, rotations,
                    (float progress) => new Color(98, 204, 255, 0),
                    (float progress) => MathHelper.Lerp(25, 55 + 30 * Utility.PositiveSineWave(70), progress));
                strip.DrawTrail();

                VertexStrip strip2 = new();
                Vector2[] positions2 = new Vector2[stripDataCount];
                float[] rotations2 = new float[stripDataCount];
                Array.Fill(positions2, Projectile.Center + (new Vector2(0, Projectile.direction==1 ? 3:-3).RotatedBy(Projectile.rotation ) * Projectile.direction) - Main.screenPosition);
                Array.Fill(rotations2, Projectile.rotation + (MathHelper.Pi) );

                for (int i = 0; i < stripDataCount; i++)
                    positions2[i] += Utility.PolarVector(4f * i, Projectile.rotation );

                var shader2 = new MiscShaderData(Utility.VanillaVertexShader, "MagicMissile")
                    .UseProjectionMatrix(doUse: true)
                    .UseSaturation(-2.4f)
                    .UseImage0(ModContent.Request<Texture2D>(Macrocosm.FancyTexturesPath + "FadeOutMask"))
                    .UseImage1(ModContent.Request<Texture2D>(Macrocosm.FancyTexturesPath + "Spark5"))
                    .UseImage2(ModContent.Request<Texture2D>(Macrocosm.FancyTexturesPath + "Spark6"));

                shader2.Apply();

                strip2.PrepareStrip(positions2, rotations2,
                    (float progress) => new Color(98, 204, 255, 0),
                    (float progress) => MathHelper.Lerp(25, 55 + 30 * Utility.PositiveSineWave(70), progress));
                strip2.DrawTrail();

                Main.spriteBatch.End();
                Main.spriteBatch.Begin(state1);
            
            
            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, null, lightColor, Projectile.rotation, Projectile.Size / 2f, Projectile.scale, SpriteEffects.None, 0);

            return false;
        }
    }
}
