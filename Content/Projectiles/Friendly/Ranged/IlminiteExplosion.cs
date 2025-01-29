using Macrocosm.Common.DataStructures;
using Macrocosm.Common.Drawing.Particles;
using Macrocosm.Common.Sets;
using Macrocosm.Common.Utils;
using Macrocosm.Content.Particles;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace Macrocosm.Content.Projectiles.Friendly.Ranged
{
    public class IlmeniteExplosion : ModProjectile
    {
        public override string Texture => Macrocosm.EmptyTexPath;

        public float Strength
        {
            get => MathHelper.Clamp(Projectile.ai[0], 0f, 1f);
            set => Projectile.ai[0] = MathHelper.Clamp(value, 0f, 1f);
        }

        public override void SetStaticDefaults()
        {
            ProjectileSets.HitsTiles[Type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.Size = new Vector2(50, 50);
            Projectile.aiStyle = -1;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 120;

            Projectile.CritChance = 16;

            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 60;
        }

        public override void OnSpawn(IEntitySource source)
        {
            Projectile.Size *= 1f + (0.5f * Projectile.ai[0]);
        }

        private bool spawned;
        SpriteBatchState state;
        public override void AI()
        {
            if (!spawned)
            {
                Particle.Create<TintableFlash>(p =>
                {
                    p.Position = Projectile.Center;
                    p.Scale = new(0.2f);
                    p.ScaleVelocity = new(0.2f);
                    p.Color = Main.rand.NextBool() ? new(188, 89, 134) : new(33, 188, 190);
                });

                Lighting.AddLight(Projectile.Center, new Color(188, 89, 134).ToVector3() * 4f);
                Lighting.AddLight(Projectile.Center, new Color(33, 188, 190).ToVector3() * 4f);

                spawned = true;
            }

            Lighting.AddLight(Projectile.Center, new Color(188, 89, 134).ToVector3());
            Lighting.AddLight(Projectile.Center, new Color(33, 188, 190).ToVector3());

            for (int i = 0; i < 20 * Strength; i++)
            {
                Vector2 position = Projectile.Center + Main.rand.NextVector2Circular(Projectile.width, Projectile.height);
                float distanceFromCenter = Vector2.Distance(position, Projectile.Center);
                float maxDistance = Math.Max(Projectile.width, Projectile.height) / 2f;
                float fallOffFactor = 1f - (distanceFromCenter / maxDistance);
                fallOffFactor = MathF.Pow(fallOffFactor, 2f);

                if (Main.rand.NextFloat() <= fallOffFactor)
                {
                    Particle.Create<LunarRustStar>((p) =>
                    {
                        p.Position = position;
                        p.Velocity = Vector2.Zero;
                        p.Rotation = Utility.RandomRotation();
                        p.Scale = new Vector2(1f * (0.6f + Strength * 0.15f)) * Main.rand.NextFloat(0.5f, 1.2f);
                    });
                }
            }
        }

        public void OnHit()
        {
            Projectile.timeLeft += 2;

            for(int i = 0; i < (int)(Strength * 6); i++)
            {
                Vector2 targetCenter = new(-1000f);
                float distanceFromTarget = 1200f;
                bool foundTarget = false;
                foreach (var npc in Main.ActiveNPCs)
                {
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

                Projectile.NewProjectileDirect(
                    Projectile.GetSource_FromAI(),
                    Projectile.Center,
                    ((Projectile.Center - targetCenter).SafeNormalize(default) * 100f).RotatedByRandom(MathHelper.Pi / 7),
                    ModContent.ProjectileType<IlmeniteDeflectedProjectile>(),
                    Projectile.damage / 3,
                    knockback: 2f,
                    owner: Main.myPlayer,
                    ai0: i % 2
                );
            }
           
            Particle.Create<TintableFlash>(p =>
            {
                p.Position = Projectile.Center;
                p.Scale = new(0.2f);
                p.ScaleVelocity = new(0.2f);
                p.Color = Main.rand.NextBool() ? new(188, 89, 134) : new(33, 188, 190);
            });

            for (int i = 0; i < 10 * Strength; i++)
            {
                Vector2 position = Projectile.Center + Main.rand.NextVector2Circular(Projectile.width, Projectile.height);
                Particle.Create<LunarRustStar>((p) =>
                {
                    p.Position = position;
                    p.Velocity = Vector2.Zero;
                    p.Rotation = Utility.RandomRotation();
                    p.Scale = new Vector2(1f * (0.6f + Strength * 0.15f)) * Main.rand.NextFloat(0.5f, 1.2f);
                    p.Opacity = Main.rand.NextFloat();
                });
            }
        }


        public override bool PreDraw(ref Color lightColor)
        {
            return false;
        }
    }
}
