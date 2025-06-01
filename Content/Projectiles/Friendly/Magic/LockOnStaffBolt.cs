using Macrocosm.Common.Drawing.Particles;
using Macrocosm.Common.Global.NPCs;
using Macrocosm.Common.Utils;
using Macrocosm.Content.Particles;
using Macrocosm.Content.Projectiles.Friendly.Ranged;
using Macrocosm.Content.Trails;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Projectiles.Friendly.Magic
{
    public class LockOnStaffBolt : ModProjectile
    {
        public override string Texture => ModContent.GetInstance<NWAMissile>().Texture;

        private MissileTrail trail;

        public ref float AI_HomingTimer => ref Projectile.ai[0];
        public ref float AI_AccelerationTimer => ref Projectile.ai[1];
        public ref float AI_InitialDecelerationTimer => ref Projectile.ai[2];

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 30;
            ProjectileID.Sets.TrailingMode[Type] = 3;
        }

        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.aiStyle = -1;
            Projectile.penetrate = 1;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 300;
            Projectile.light = .5f;
            trail = new();
        }

        public override bool? CanHitNPC(NPC npc)
        {
            if (npc.GetGlobalNPC<MacrocosmNPC>().TargetedByHomingProjectile == false)
                return false;
            else
                return true;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (target.TryGetGlobalNPC(out MacrocosmNPC macNpc))
                macNpc.TargetedByHomingProjectile = false;
        }

        /// <summary> Adapted from Projectile.AI_016() for homing snowman rockets </summary>
        public override void AI()
        {
            if (Projectile.owner == Main.myPlayer && Projectile.timeLeft <= 3)
                Projectile.PrepareBombToBlow();

            #region Acceleration

            float timeToReachTopSpeed = 10;
            if (AI_AccelerationTimer < timeToReachTopSpeed)
                AI_AccelerationTimer++;

            float timerForInitialDeceleration = 5;
            if (AI_InitialDecelerationTimer < timerForInitialDeceleration)
                AI_InitialDecelerationTimer++;

            #endregion

            #region Homing
            float x = Projectile.position.X;
            float y = Projectile.position.Y;

            float maxDistance = 1200f;
            float minHomingTime = 40f;

            bool hasTarget = false;
            AI_HomingTimer++;
            if (AI_HomingTimer > minHomingTime)
            {
                AI_HomingTimer = minHomingTime;
                // only run locally
                NPC npc = Main.npc[(int)Projectile.localAI[0]];
                if (Projectile.owner == Main.myPlayer && npc.CanBeChasedBy(Projectile) && npc.GetGlobalNPC<MacrocosmNPC>().TargetedByHomingProjectile)
                {
                    float targetCenterX = npc.position.X + npc.width / 2;
                    float targetCenterY = npc.position.Y + npc.height / 2;
                    float distanceL1 = Math.Abs(Projectile.position.X + Projectile.width / 2 - targetCenterX) + Math.Abs(Projectile.position.Y + Projectile.height / 2 - targetCenterY);
                    if (distanceL1 < maxDistance)
                    {
                        maxDistance = distanceL1;
                        x = targetCenterX;
                        y = targetCenterY;
                        hasTarget = true;
                    }
                }
            }

            if (!hasTarget)
            {
                x = Projectile.position.X + Projectile.width / 2 + Projectile.velocity.X * 20f;
                y = Projectile.position.Y + Projectile.height / 2 + Projectile.velocity.Y * 20f;
            }
            else
            {
                // targeting was done locally, sync
                Projectile.netUpdate = true;
            }

            Vector2 maxVelocity = (new Vector2(x, y) - Projectile.Center).SafeNormalize(-Vector2.UnitY) * (10f + 6f * (AI_AccelerationTimer / timeToReachTopSpeed));
            maxVelocity *= (AI_InitialDecelerationTimer / timerForInitialDeceleration);
            Projectile.velocity = Vector2.Lerp(Projectile.velocity, maxVelocity, 0.1666666666666667f);

            #endregion

            #region Rotation

            if (Projectile.velocity.X < 0f)
            {
                Projectile.spriteDirection = -1;
                Projectile.rotation = (float)Math.Atan2(0f - Projectile.velocity.Y, 0f - Projectile.velocity.X) - 1.57f;
            }
            else
            {
                Projectile.spriteDirection = 1;
                Projectile.rotation = (float)Math.Atan2(Projectile.velocity.Y, Projectile.velocity.X) + 1.57f;
            }

            #endregion

            #region Visual Effects

            // alpha fade-in
            Projectile.localAI[1] += 0.6f;
            if (Projectile.localAI[1] > 6f)
            {
                Projectile.localAI[1] = 6f;
                Projectile.alpha = 0;
            }
            else
            {
                Projectile.alpha = (int)(255f - 42f * Projectile.localAI[1]) + 100;
                if (Projectile.alpha > 255)
                    Projectile.alpha = 255;
            }

            if (Projectile.localAI[1] < 1.2f)
            {
                // spawn some dusts as barrel flash
                for (int i = 0; i < 30; i++)
                {
                    Dust dust = Dust.NewDustDirect(Projectile.position + new Vector2(0, 0), Projectile.width, Projectile.height, DustID.Flare, Scale: 3);
                    dust.velocity = (Projectile.velocity.SafeNormalize(Vector2.UnitX) * Main.rand.NextFloat(8f, 12f)).RotatedByRandom(MathHelper.PiOver4 * 0.2f) + Main.player[Projectile.owner].velocity;
                    dust.noLight = false;
                    dust.alpha = 200;
                    dust.noGravity = true;
                }
            }


            // spawn dust trail
            for (int i = 0; i < 2; i++)
            {
                Vector2 dustVelocity = Vector2.Zero;
                if (i == 1)
                    dustVelocity = Projectile.velocity * 0.5f;

                if (!(Projectile.localAI[1] > 9f))
                    continue;

                if (Main.rand.NextBool(2))
                {
                    int flameIdx = Dust.NewDust(new Vector2(Projectile.position.X + 3f + dustVelocity.X, Projectile.position.Y + 3f + dustVelocity.Y) - Projectile.velocity * 0.5f, Projectile.width - 8, Projectile.height - 8, DustID.Torch, 0f, 0f, 100);
                    Main.dust[flameIdx].scale *= 1.4f + Main.rand.Next(10) * 0.1f;
                    Main.dust[flameIdx].velocity *= 0.2f;
                    Main.dust[flameIdx].noGravity = true;
                    if (Main.dust[flameIdx].type == 152)
                    {
                        Main.dust[flameIdx].scale *= 0.5f;
                        Main.dust[flameIdx].velocity += Projectile.velocity * 0.1f;
                    }
                    else if (Main.dust[flameIdx].type == 35)
                    {
                        Main.dust[flameIdx].scale *= 0.5f;
                        Main.dust[flameIdx].velocity += Projectile.velocity * 0.1f;
                    }
                    else if (Main.dust[flameIdx].type == Dust.dustWater())
                    {
                        Main.dust[flameIdx].scale *= 0.65f;
                        Main.dust[flameIdx].velocity += Projectile.velocity * 0.1f;
                    }
                }

                if (Main.rand.NextBool(2))
                {
                    int smokeIdx = Dust.NewDust(new Vector2(Projectile.position.X + 3f + dustVelocity.X, Projectile.position.Y + 3f + dustVelocity.Y) - Projectile.velocity * 0.5f, Projectile.width - 8, Projectile.height - 8, DustID.Smoke, 0f, 0f, 100, default, 0.5f);
                    Main.dust[smokeIdx].fadeIn = 0.5f + Main.rand.Next(5) * 0.1f;
                    Main.dust[smokeIdx].velocity *= 0.05f;
                }
            }

            #endregion
        }

        public override void PrepareBombToBlow()
        {
            Projectile.tileCollide = false;
            Projectile.alpha = 255;
            Projectile.Resize(128, 128);
            Projectile.knockBack = 8f;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Projectile.timeLeft = 2;
            return false;
        }

        public virtual Color GetColor()
        {
            return new Color(0, 127, 255);
        }

        public override void OnKill(int timeLeft)
        {
            Player player = Main.player[Projectile.owner];
            if (Main.npc[(int)Projectile.localAI[0]].TryGetGlobalNPC(out MacrocosmNPC macNpc)
            && player.ownedProjectileCounts[player.HeldItem.shoot] <= 0)
            {
                macNpc.TargetedByHomingProjectile = false;
            }

            if (Main.dedServ)
                return;

            SoundEngine.PlaySound(SoundID.Item14, Projectile.position);

            //Spawn trail dust
            for (int i = 3; i < Projectile.oldPos.Length; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    Vector2 pos = Projectile.oldPos[i];
                    Dust dust = Dust.NewDustDirect(pos, 20, 20, DustID.Torch, Projectile.oldVelocity.X * 0.5f, Projectile.oldVelocity.Y * 0.5f, Scale: 0.12f * (Projectile.oldPos.Length - i));
                    dust.noGravity = true;
                }
            }

            var explosion = Particle.Create<TintableExplosion>(p =>
            {
                p.Position = Projectile.Center;
                p.Color = (new Color(195, 115, 62)).WithOpacity(0.6f);
                p.Scale = new(1.2f);
                p.NumberOfInnerReplicas = 9;
                p.ReplicaScalingFactor = 0.5f;
            });

        }

        public override bool PreDraw(ref Color lightColor)
        {
            trail.Opacity = Projectile.localAI[1];

            if (Projectile.alpha < 1 && Projectile.timeLeft > 3)
                trail?.Draw(Projectile, Projectile.Size / 2f);

            return true;
        }
    }
}