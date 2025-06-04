using Macrocosm.Common.DataStructures;
using Macrocosm.Common.Global.Projectiles;
using Macrocosm.Common.Utils;
using Macrocosm.Content.Buffs.Minions;
using Macrocosm.Content.Rockets;
using Macrocosm.Content.Trails;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Projectiles.Friendly.Summon
{
    public class GreatstaffOfHorusMinion : ModProjectile
    {
        private static Asset<Texture2D> glow;
        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 4;
            ProjectileID.Sets.MinionTargettingFeature[Type] = true;

            Main.projPet[Projectile.type] = true; 

            ProjectileID.Sets.TrailCacheLength[Type] = 25;
            ProjectileID.Sets.TrailingMode[Type] = -1; // custom trail updates

            ProjectileID.Sets.MinionSacrificable[Type] = true; 
            ProjectileID.Sets.CultistIsResistantTo[Type] = true; 
        }
        private HorusTrail trail;
        public sealed override void SetDefaults()
        {
            Projectile.width = 42;
            Projectile.height = 42;
            Projectile.tileCollide = false; 
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 20;

            Projectile.friendly = true;

            Projectile.minion = true; 
            Projectile.DamageType = DamageClass.Summon; 
            Projectile.minionSlots = 1f;
            Projectile.penetrate = -1;
            trail = new();
        }

        public override bool MinionContactDamage() => true;
        public override bool? CanCutTiles() => false;

        private float glowTimer = 0;
        private int shineOrbitTimer = 0;
        private Vector2 shineOrbit = new(32f, 12f);

        public override void AI()
        {
            Player owner = Main.player[Projectile.owner];

            if (!CheckActive(owner))
                return;

            GeneralBehavior(owner, out Vector2 vectorToIdlePosition, out float distanceToIdlePosition);
            SearchForTargets(owner, out bool foundTarget, out float distanceFromTarget, out Vector2 targetCenter);
            Movement(foundTarget, distanceFromTarget, targetCenter, distanceToIdlePosition, vectorToIdlePosition);
            Visuals(foundTarget);
        }

        private bool CheckActive(Player owner)
        {
            if (owner.dead || !owner.active)
            {
                owner.ClearBuff(ModContent.BuffType<GreatstaffOfHorusMinionBuff>());
                return false;
            }

            if (owner.HasBuff(ModContent.BuffType<GreatstaffOfHorusMinionBuff>()))
            {
                Projectile.timeLeft = 2;
            }

            return true;
        }

        private void GeneralBehavior(Player owner, out Vector2 vectorToIdlePosition, out float distanceToIdlePosition)
        {
            Vector2 idlePosition = owner.Center;
            idlePosition.Y -= 48f; // Go up 48 coordinates (three tiles from the center of the player)

            // If your minion doesn't aimlessly move around when it's idle, you need to "put" it into the line of other summoned minions
            // The index is projectile.minionPos
            float minionPositionOffsetX = (10 + Projectile.minionPos * 40) * -owner.direction;
            idlePosition.X += minionPositionOffsetX; // Go behind the player

            // Teleport to player if distance is too big
            vectorToIdlePosition = idlePosition - Projectile.Center;
            distanceToIdlePosition = vectorToIdlePosition.Length();

            if (Main.myPlayer == owner.whoAmI && distanceToIdlePosition > 2000f)
            {
                // Whenever you deal with non-regular events that change the behavior or position drastically, make sure to only run the code on the owner of the projectile, and then set netUpdate to true
                Projectile.position = idlePosition;
                Projectile.velocity *= 0.1f;
                Projectile.netUpdate = true;
            }

            // Fix overlap with other minions
            float overlapVelocity = 0.04f;
            foreach (var other in Main.ActiveProjectiles)
            {
                if (other.whoAmI != Projectile.whoAmI && other.owner == Projectile.owner && Math.Abs(Projectile.position.X - other.position.X) + Math.Abs(Projectile.position.Y - other.position.Y) < Projectile.width)
                {
                    if (Projectile.position.X < other.position.X)
                        Projectile.velocity.X -= overlapVelocity;
                    else
                        Projectile.velocity.X += overlapVelocity;

                    if (Projectile.position.Y < other.position.Y)
                        Projectile.velocity.Y -= overlapVelocity;
                    else
                        Projectile.velocity.Y += overlapVelocity;
                }
            }
        }

        private void SearchForTargets(Player owner, out bool foundTarget, out float distanceFromTarget, out Vector2 targetCenter)
        {
            distanceFromTarget = 700f;
            targetCenter = Projectile.position;
            foundTarget = false;

            // This code is required if your minion weapon has the targeting feature
            if (owner.HasMinionAttackTargetNPC)
            {
                NPC npc = Main.npc[owner.MinionAttackTargetNPC];
                float between = Vector2.Distance(npc.Center, Projectile.Center);

                // Reasonable distance away so it doesn't target across multiple screens
                if (between < 2000f)
                {
                    distanceFromTarget = between;
                    targetCenter = npc.Center;
                    foundTarget = true;
                }
            }

            if (!foundTarget)
            {
                // This code is required either way, used for finding a target
                foreach (var npc in Main.ActiveNPCs)
                {
                    if (npc.CanBeChasedBy())
                    {
                        float between = Vector2.Distance(npc.Center, Projectile.Center);
                        bool closest = Vector2.Distance(Projectile.Center, targetCenter) > between;
                        bool inRange = between < distanceFromTarget;
                        bool lineOfSight = Collision.CanHitLine(Projectile.position, Projectile.width, Projectile.height, npc.position, npc.width, npc.height);
                        // Additional check for this specific minion behavior, otherwise it will stop attacking once it dashed through an enemy while flying though tiles afterwards
                        // The number depends on various parameters seen in the movement code below. Test different ones out until it works alright
                        bool closeThroughWall = between < 100f;

                        if ((closest && inRange || !foundTarget) && (lineOfSight || closeThroughWall))
                        {
                            distanceFromTarget = between;
                            targetCenter = npc.Center;
                            foundTarget = true;
                        }
                    }
                }
            }

            // friendly needs to be set to true so the minion can deal contact damage
            // friendly needs to be set to false so it doesn't damage things like target dummies while idling
            // Both things depend on if it has a target or not, so it's just one assignment here
            // You don't need this assignment if your minion is shooting things instead of dealing contact damage
            Projectile.friendly = foundTarget;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<HorusExplosion>(), (int)(Projectile.damage * 0.3f), 0f, Main.myPlayer);
        }

        private void Movement(bool foundTarget, float distanceFromTarget, Vector2 targetCenter, float distanceToIdlePosition, Vector2 vectorToIdlePosition)
        {
            float speed = 15f;
            float inertia = 16f;
            if (foundTarget)
            {
                // Minion has a target: attack (here, fly towards the enemy)
                if (distanceFromTarget > 250f)
                {
                    // The immediate range around the target (so it doesn't latch onto it when close)
                    Vector2 direction = targetCenter - Projectile.Center;
                    direction.Normalize();
                    direction *= speed;

                    Projectile.velocity = (Projectile.velocity * (inertia - 1) + direction) / inertia;
                }
            }
            else
            {
                // Minion doesn't have a target: return to player and idle
                if (distanceToIdlePosition > 600f)
                {
                    // Speed up the minion if it's away from the player
                    speed = 20f;
                    inertia = 60f;
                }
                else
                {
                    // Slow down the minion if closer to the player
                    speed = 8f;
                    inertia = 80f;
                }

                if (distanceToIdlePosition > 100f)
                {
                    // The immediate range around the player (when it passively floats about)
                    // This is a simple movement formula using the two parameters and its desired direction to create a "homing" movement
                    vectorToIdlePosition.Normalize();
                    vectorToIdlePosition *= speed;
                    Projectile.velocity = (Projectile.velocity * (inertia - 1) + vectorToIdlePosition) / inertia;
                }
                else if (Projectile.velocity == Vector2.Zero)
                {
                    // If there is a case where it's not moving at all, give it a little "poke"
                    Projectile.velocity.X = -0.15f;
                    Projectile.velocity.Y = -0.05f;
                }
            }
        }

        private void Visuals(bool foundTarget)
        {
            // Rotation
            float targetRotation;
            if (foundTarget)
                targetRotation = Projectile.velocity.ToRotation() - MathHelper.PiOver2;
            else
                targetRotation = Projectile.velocity.X * 0.05f;
            Projectile.rotation = MathHelper.Lerp(Projectile.rotation, MathHelper.WrapAngle(targetRotation), 0.2f);

            // Frame
            int frameSpeed = 5;
            Projectile.frameCounter++;
            if (Projectile.frameCounter >= frameSpeed)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;

                if (Projectile.frame >= Main.projFrames[Projectile.type])
                    Projectile.frame = 0;
            }

            // Shine
            shineOrbitTimer += 8;
            if (shineOrbitTimer >= 180)
                shineOrbitTimer = 0;

            // Glow
            glowTimer++;

            // Light
            Lighting.AddLight(Projectile.Center, new Color(255, 141, 114).ToVector3() * 0.48f);

            // Trail
            if (foundTarget)
            {
                Utility.UpdateTrail(Projectile, updatePos: true, updateRot: true, smoothAmount: 0.65f);
            }
            else
            {
                for (int i = 0; i < ProjectileID.Sets.TrailCacheLength[Type]; i++)
                {
                    Projectile.oldPos[i] = Projectile.position + new Vector2(0, Projectile.height / 2) - new Vector2(0, 4 * i).RotatedBy(Projectile.rotation);
                    Projectile.oldRot[i] = Projectile.rotation + MathHelper.PiOver2;
                }
            }
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White.WithAlpha(220);
        }

        private SpriteBatchState state;
        public override bool PreDraw(ref Color lightColor)
        {
            Vector2 orbit = new((float)Math.Cos(MathHelper.ToRadians(shineOrbitTimer * 2)) * shineOrbit.X, -(float)Math.Sin(MathHelper.ToRadians(shineOrbitTimer * 2)) * shineOrbit.Y);

            state.SaveState(Main.spriteBatch);
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(BlendState.Additive, state);

            if ((-MathF.Sin(MathHelper.ToRadians(shineOrbitTimer * 2)) * shineOrbit.Y) < 0f)
                DrawShine(Main.spriteBatch, new Color(127, 200, 155), orbit);

            trail?.Draw(Projectile, Projectile.Size / 2f);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(state);
            return true;
        }

        public override void PostDraw(Color lightColor)
        {
            state.SaveState(Main.spriteBatch);
            Vector2 orbit = new((float)Math.Cos(MathHelper.ToRadians(shineOrbitTimer * 2)) * shineOrbit.X, -(float)Math.Sin(MathHelper.ToRadians(shineOrbitTimer * 2)) * shineOrbit.Y);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(BlendState.Additive, state);

            glow ??= ModContent.Request<Texture2D>(Macrocosm.FancyTexturesPath + "Star5");
            float glowOpacity = 0.25f + (MathF.Sin(MathHelper.TwoPi * glowTimer / 90f) + 1f) * 0.5f * 0.75f; 
            Main.EntitySpriteDraw(glow.Value, Projectile.Center - Main.screenPosition + new Vector2(0, 8), null, new Color(255, 170, 142) * glowOpacity, Projectile.rotation, glow.Size() / 2, Projectile.scale * Main.rand.NextFloat(0.19f, 0.21f), SpriteEffects.None, 0f);
            
            if ((-MathF.Sin(MathHelper.ToRadians(shineOrbitTimer * 2)) * shineOrbit.Y) >= 0f)
                DrawShine(Main.spriteBatch, new Color(127, 200, 155), orbit);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(state);
        }

        private void DrawShine(SpriteBatch spriteBatch, Color drawColor, Vector2 orbit)
        {
            Texture2D shineTexture = TextureAssets.Extra[ExtrasID.SharpTears].Value;
            int trailLength = 80;
            float opacityFactor = 0.75f;
            float rotation = orbit.ToRotation() + MathHelper.Pi / 4f;
            Vector2 position = Projectile.Center + new Vector2(0, 8) - Main.screenPosition;
            for (int i = 0; i < trailLength; i++)
            {
                float lerpFactor = i / (float)trailLength;
                Vector2 previousOrbit = new
                (
                    (float)Math.Cos(MathHelper.ToRadians((shineOrbitTimer - i) * 2)) * shineOrbit.X,
                    -(float)Math.Sin(MathHelper.ToRadians((shineOrbitTimer - i) * 2)) * shineOrbit.Y
                );

                Color trailColor = drawColor * (1f - lerpFactor) * opacityFactor;
                spriteBatch.Draw(shineTexture, position + previousOrbit.RotatedBy(MathHelper.ToRadians(-70)) , null, trailColor, rotation, shineTexture.Size() / 2, Projectile.scale * (1f - lerpFactor) * 0.4f, SpriteEffects.None, 0f);
            }

            spriteBatch.Draw(shineTexture, position + orbit.RotatedBy(MathHelper.ToRadians(-70)), null, drawColor, rotation, shineTexture.Size() / 2, Projectile.scale * 0.4f, SpriteEffects.None, 0f);
        }
    }
}
