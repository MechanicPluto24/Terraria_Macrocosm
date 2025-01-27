﻿using Macrocosm.Common.DataStructures;
using Macrocosm.Common.Drawing.Particles;
using Macrocosm.Common.Sets;
using Macrocosm.Common.Utils;
using Macrocosm.Content.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Projectiles.Friendly.Ranged
{
    public class IlmeniteRegularProj : ModProjectile
    {
        public override string Texture => Macrocosm.EmptyTexPath;

        float trailMultiplier = 0f;
        int colourLerpProg = 0;
        public Color colour1 = new Color(188, 89, 134);
        public Color colour2 = new Color(33, 188, 190);

        public override void SetStaticDefaults()
        {
            ProjectileSets.HitsTiles[Type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.ignoreWater = true;
            Projectile.tileCollide = true;
            Projectile.width = 10;
            Projectile.height = 10;
            Projectile.aiStyle = -1;
            Projectile.friendly = true;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 360;
            Projectile.extraUpdates = 1;

            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 20;
        }

        public override void OnSpawn(IEntitySource source)
        {
            Projectile.extraUpdates += (int)Projectile.ai[1];
            Projectile.penetrate = (int)Projectile.ai[2];
        }
        public bool hitExplosion;
        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;

            if (trailMultiplier < 1f + (0.15f * Projectile.extraUpdates))
                trailMultiplier += 0.03f * (0.2f + Projectile.ai[0] * 0.9f);

            if (Projectile.ai[0] == 0)
            {
                Lighting.AddLight(Projectile.Center, colour1.ToVector3());
            }
            else if (Projectile.ai[0] == 1)
            {
                Lighting.AddLight(Projectile.Center, colour2.ToVector3() * 1.25f);
            }
            else
            {
                Lighting.AddLight(Projectile.Center, Color.Lerp(colour1, colour2, MathF.Pow(MathF.Cos(colourLerpProg / 10f), 3)).ToVector3() * 1.5f);
                colourLerpProg++;
            }

            if (!hitExplosion)
            {
                Projectile explosion = Utility.FindClosestProjectileOfType(Projectile.Center, ModContent.ProjectileType<IlmeniteExplosion>());
                if (explosion is not null)
                {
                    if (Vector2.Distance(Projectile.Center, explosion.Center) < 50f)
                    {
                        Vector2 targetCenter = new Vector2(-1000000, -1000000);
                        float distanceFromTarget = 1200f;
                        bool foundTarget = false;
                        for (int i = 0; i < Main.maxNPCs; i++)
                        {
                            NPC npc = Main.npc[i];

                            if (npc.CanBeChasedBy())
                            {
                                float between = Vector2.Distance(npc.Center, Projectile.Center);
                                bool closest = Vector2.Distance(Projectile.Center, targetCenter) > between;
                                bool inRange = between < distanceFromTarget;
                                bool lineOfSight = Collision.CanHitLine(Projectile.position, Projectile.width, Projectile.height, npc.position, npc.width, npc.height);

                                bool closeThroughWall = between < 100f;

                                if (((closest && inRange) || !foundTarget) && (lineOfSight || closeThroughWall))
                                {
                                    targetCenter = npc.Center;
                                    foundTarget = true;
                                    distanceFromTarget = between;
                                }
                            }
                        }

                        IlmeniteRegularProj regular = (Projectile.NewProjectileDirect(
                            Projectile.GetSource_FromAI(),
                            Projectile.Center,
                            ((targetCenter - Projectile.Center).SafeNormalize(Vector2.UnitX) * Projectile.velocity.Length()).RotatedByRandom(MathHelper.Pi / 7),
                            ModContent.ProjectileType<IlmeniteRegularProj>(),
                            Projectile.damage / 2,
                            2,
                            -1,
                            Projectile.ai[0],
                            Projectile.ai[1],
                            Projectile.ai[2]
                        ).ModProjectile as IlmeniteRegularProj);
                        regular.Projectile.scale *= 0.7f;
                        regular.hitExplosion = true;
                        hitExplosion = true;
                        (explosion.ModProjectile as IlmeniteExplosion).OnHit();
                    }

                }
            }
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item10);

            for (int i = 0; i < 25; i++)
            {
                Particle.Create<TintableSpark>((p) =>
                {
                    p.Position = Projectile.Center + Projectile.oldVelocity;
                    p.Velocity = Main.rand.NextVector2Circular(4, 4) * Main.rand.NextFloat();
                    p.Scale = new(6f, Main.rand.NextFloat(2.5f, 3.5f));
                    p.Color = Main.rand.NextBool() ? colour1 : colour2;
                });
            }

            float count = Projectile.oldVelocity.Length() * trailMultiplier;
            for (int i = 1; i < count; i++)
            {
                Vector2 trailPosition = Projectile.Center - Projectile.oldVelocity * i * 0.4f;
                for (int j = 0; j < 2; j++)
                {
                    Particle.Create<TintableSpark>((p) =>
                    {
                        p.Position = trailPosition;
                        p.Velocity = Vector2.Zero;
                        p.Scale = new(Main.rand.NextFloat(1f, 2f) * (1f - i / count));
                        p.Color = Main.rand.NextBool() ? colour1 : colour2;
                    });
                }
            }
        }

        SpriteBatchState state;
        public override bool PreDraw(ref Color lightColor)
        {
            var spriteBatch = Main.spriteBatch;

            state.SaveState(spriteBatch);
            spriteBatch.End();
            spriteBatch.Begin(BlendState.Additive, state);

            float count = Projectile.velocity.LengthSquared() * trailMultiplier;
            Color color;
            if (Projectile.ai[0] == 0)
            {
                color = colour1;
            }
            else if (Projectile.ai[0] == 1)
            {
                color = colour2;
            }
            else
            {
                color = Color.Lerp(colour1, colour2, MathF.Pow(MathF.Cos(colourLerpProg / 10f), 3)) * 1.5f;
            }

            for (int n = 1; n < count; n++)
            {
                Vector2 trailPosition = Projectile.Center - Projectile.oldVelocity * n * 0.4f;
                color *= (1f - (float)n / count);
                Utility.DrawStar(trailPosition - Main.screenPosition, 1, color, Projectile.scale * (0.6f + Projectile.ai[0] * 0.15f), Projectile.rotation, entity: true);
            }

            spriteBatch.End();
            spriteBatch.Begin(state);

            return false;
        }
    }
}
