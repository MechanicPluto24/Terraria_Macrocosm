using Macrocosm.Common.DataStructures;
using Macrocosm.Common.Drawing.Particles;
using Macrocosm.Common.Utils;
using Macrocosm.Content.Debuffs.Weapons;
using Macrocosm.Content.Dusts;
using Macrocosm.Content.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Projectiles.Friendly.Summon
{
    public class TotalityProjectile : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.IsAWhip[Type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.DefaultToWhip();

            Projectile.WhipSettings.Segments = 24;
            Projectile.WhipSettings.RangeMultiplier = 1.6f;
        }

        // AI timer for whip swing 
        private float Timer
        {
            get => Projectile.ai[0];
            set => Projectile.ai[0] = value;
        }

        public Vector2 WhipTipPosition;

        private bool spawned;

        public override void PostAI()
        {
            if (!spawned)
            {
                Player player = Main.player[Projectile.owner];
                if (player.whoAmI == Main.myPlayer)
                {
                    Vector2 position = Projectile.Center;
                    Vector2 velocity = (Projectile.velocity * 2.4f).RotatedBy(MathHelper.Pi / 3 * -player.direction);
                    float rotation = velocity.ToRotation();
                    Projectile.NewProjectile(Projectile.GetSource_FromAI(), position, velocity, ModContent.ProjectileType<TotalitySlashProjectile>(), (int)((float)Projectile.damage), 0f, Main.myPlayer, rotation);
                }

                spawned = true;
            }

            List<Vector2> list = new();
            Projectile.FillWhipControlPoints(Projectile, list);

            foreach (Vector2 position in list)
            {
                Lighting.AddLight(position, new Color(253, 174, 248).ToVector3() * 0.2f);
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            // minions will attack the npcs hit with this whip 
            Player player = Main.player[Projectile.owner];
            player.MinionAttackTargetNPC = target.whoAmI;

            target.AddBuff(ModContent.BuffType<TotalitySlashed>(), 2 * 60);

            Color color = new List<Color>() {
                    new(44, 210, 91),
                    new(201, 125, 205),
                    new(114, 111, 207)
            }.GetRandom();
            color.A = (byte)Main.rand.Next(120, 200);

            float rotation = (target.Center - player.Center).ToRotation() + Main.rand.NextFloat(-MathHelper.Pi / 8, MathHelper.Pi / 8);
            for (float f = 0f; f < 1f; f += 1f / 12f)
            {
                rotation = MathHelper.TwoPi * f + Main.rand.NextFloat() * MathHelper.TwoPi + Main.rand.NextFloatDirection() * 0.25f;

                Particle.Create<LightningParticle>((p) =>
                {
                    p.Position = target.Center;
                    p.Velocity = rotation.ToRotationVector2() * (Main.rand.NextFloat() * 6f) * new Vector2(0.6f, 1f);
                    p.Rotation = rotation;
                    p.Color = (color * 0.3f).WithAlpha(127);
                    p.OutlineColor = (color * 0.9f).WithAlpha(127);
                    p.Scale = new(Main.rand.NextFloat(0.8f, 1.4f));
                    p.ScaleVelocity = new Vector2(-0.05f);
                    p.FadeInNormalizedTime = 0.01f;
                    p.FadeOutNormalizedTime = 0.7f;
                });
            }

            for (float f = 0f; f < 1f; f += 1f / 24f)
            {
                Vector2 velocity = new Vector2(3f).RotatedByRandom(MathHelper.TwoPi) * Main.rand.NextFloat();
                Dust dust = Dust.NewDustPerfect(target.Center, ModContent.DustType<ElectricSparkDust>(), velocity, Scale: Main.rand.NextFloat(0.4f, 0.6f));
                dust.noGravity = true;
                dust.color = color.WithLuminance(0.1f);
                dust.alpha = 15;
            }
        }

        private readonly int frameWidth = 18;
        private readonly int frameHeight = 26;

        private int[] animFrames;

        private SpriteBatchState state;
        public override bool PreDraw(ref Color lightColor)
        {
            List<Vector2> list = new();
            Projectile.FillWhipControlPoints(Projectile, list);

            //Utility.DrawWhipLine(list, new Color(60, 27, 120));

            SpriteEffects flip = Projectile.spriteDirection < 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            Texture2D texture = TextureAssets.Projectile[Type].Value;

            Vector2 pos = list[0];

            for (int i = 0; i < list.Count - 1; i++)
            {
                Rectangle frame = new(0, 0, frameWidth, frameHeight);
                Vector2 origin = new(frameWidth / 2, frameHeight / 2);
                float scale = 1;

                Projectile.GetWhipSettings(Projectile, out float timeToFlyOut, out int _, out float _);
                float progress = Timer / timeToFlyOut;

                Vector2 element = list[i];
                Vector2 diff = list[i + 1] - element;

                float rotation = diff.ToRotation() - MathHelper.PiOver2; // This projectile's sprite faces down, so PiOver2 is used to correct rotation.
                Color color = Lighting.GetColor(element.ToTileCoordinates());

                bool tip = i == list.Count - 2;
                bool handle = i == 0;

                if (tip)
                {
                    frame.Y = 6 * frameHeight;

                    // For a more impactful look, this scales the tip of the whip up when fully extended, and down when curled up.
                    scale = MathHelper.Lerp(0.4f, 1.3f, Utils.GetLerpValue(0.1f, 0.7f, progress, true) * Utils.GetLerpValue(0.9f, 0.7f, progress, true));
                    WhipTipPosition = pos;

                    /*
                    // Depends on whip extenstion
                    float dustChance = Utils.GetLerpValue(0.1f, 0.7f, t, clamped: true) * Utils.GetLerpValue(0.9f, 0.7f, t, clamped: true);

                    // Spawn dust
                    if (dustChance > 0.5f && Main.rand.NextFloat() < dustChance * 0.7f)
                    {
                        Vector2 outwardsVector = list[^2].DirectionTo(list[^1]).SafeNormalize(Vector2.Zero);
                        Dust dust = Dust.NewDustDirect(list[^1] - texture.Size() / 2, texture.Width, texture.Height, ModContent.DustType<ChandriumBrightDust>(), 0f, 0f, 100, default, Main.rand.NextFloat(1f, 1.5f));

                        dust.noGravity = true;
                        dust.velocity *= Main.rand.NextFloat() * 0.2f;
                        dust.velocity += outwardsVector * 0.2f;
                    }
                    */
                }
                else if (handle)
                {
                    frame.Y = 0;
                }
                else
                {
                    animFrames ??= new int[list.Count];
                    if (Timer % 8 == 0)
                        animFrames[i] = Main.rand.Next(1, 5 + 1);

                    frame.Y = animFrames[i] * frameHeight;
                }

                Main.EntitySpriteDraw(texture, pos - Main.screenPosition, frame, color, rotation, origin, scale, flip, 0);

                //if (!handle && !tip)
                //    for (float f = 0f; f < 1f; f += 0.5f)
                //        Utility.DrawStar(Vector2.Lerp(list[i], list[i + 1], f) + Main.rand.NextVector2Circular(6, 6) - Main.screenPosition, 1, new Color(253, 174, 248, 125), scale * 0.4f, rotation, flip, entity: true);

                state.SaveState(Main.spriteBatch);
                Main.spriteBatch.End();
                Main.spriteBatch.Begin(BlendState.Additive, state);

                if (!tip && !handle && Timer > 10)
                {
                    for (int j = 0; j < 15; j++)
                    {
                        Main.EntitySpriteDraw(
                            texture,
                            pos - Main.screenPosition,
                            frame,
                            (new Color(255, 163, 217) * 0.6f * (1f - (j / 15f))),
                            rotation,
                            origin,
                            scale + (0.2f * j),
                            flip,
                            0
                        );
                    }
                }

                if (tip && Timer > 10)
                {
                    for (int j = 0; j < 15; j++)
                    {
                        Main.EntitySpriteDraw(
                            texture,
                            Vector2.Lerp(list[^2], list[^3], 0.3f) - Main.screenPosition,
                            frame,
                            (new Color(255, 163, 217, 5) * 0.1f * (1f - (j / 15f))).WithAlpha(255),
                            rotation,
                            origin,
                            scale + (0.12f * j),
                            flip,
                            0
                        );
                    }
                }


                Main.spriteBatch.End();
                Main.spriteBatch.Begin(state);


                pos += diff;
            }

            return false;
        }
    }
}
