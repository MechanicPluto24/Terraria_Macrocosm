using Macrocosm.Common.DataStructures;
using Macrocosm.Common.Utils;
using Macrocosm.Content.Trails;
using Macrocosm.Content.Buffs.Minions;
using Microsoft.Xna.Framework;
using ReLogic.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Projectiles.Friendly.Summon
{
    
    // This minion shows a few mandatory things that make it behave properly.
    // Its attack pattern is simple: If an enemy is in range of 43 tiles, it will fly to it and deal contact damage
    // If the player targets a certain NPC with right-click, it will fly through tiles to it
    // If it isn't attacking, it will float near the player with minimal movement
    public class GreatstaffOfHorusMinion : ModProjectile
    {
        private static Asset<Texture2D> shineTexture;
        public override void SetStaticDefaults()
        {
            // Sets the amount of frames this minion has on its spritesheet
            Main.projFrames[Projectile.type] = 4;
            // This is necessary for right-click targeting
            ProjectileID.Sets.MinionTargettingFeature[Projectile.type] = true;

            Main.projPet[Projectile.type] = true; // Denotes that this projectile is a pet or minion
            ProjectileID.Sets.TrailCacheLength[Type] = 25;
            ProjectileID.Sets.TrailingMode[Type] = 3;
            ProjectileID.Sets.MinionSacrificable[Projectile.type] = true; // This is needed so your minion can properly spawn when summoned and replaced when other minions are summoned
            ProjectileID.Sets.CultistIsResistantTo[Projectile.type] = true; // Make the cultist resistant to this projectile, as it's resistant to all homing projectiles.
        }
        private HorusTrail trail;
        public sealed override void SetDefaults()
        {
            Projectile.width = 78;
            Projectile.height = 60;
            Projectile.tileCollide = false; // Makes the minion go through tiles freely
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 20;
            // These below are needed for a minion weapon
            Projectile.friendly = true; // Only controls if it deals damage to enemies on contact (more on that later)
            Projectile.minion = true; // Declares this as a minion (has many effects)
            Projectile.DamageType = DamageClass.Summon; // Declares the damage type (needed for it to deal damage)
            Projectile.minionSlots = 1f; // Amount of slots this minion occupies from the total minion slots available to the player (more on that later)
            Projectile.penetrate = -1; // Needed so the minion doesn't despawn on collision with enemies or tiles
            trail = new();
        }

        // Here you can decide if your minion breaks things like grass or pots
        public override bool? CanCutTiles()
        {
            return false;
        }

        // This is mandatory if your minion deals contact damage (further related stuff in AI() in the Movement region)
        public override bool MinionContactDamage()
        {
            return true;
        }

        // The AI of this minion is split into multiple methods to avoid bloat. This method just passes values between calls actual parts of the AI.
        int shineOrbitTimer=0;

        public override void AI()
        {
            Player owner = Main.player[Projectile.owner];
            shineOrbitTimer += 2;
            if (shineOrbitTimer >= 180)
                shineOrbitTimer = 0;
            if (!CheckActive(owner))
            {
                return;
            }

            GeneralBehavior(owner, out Vector2 vectorToIdlePosition, out float distanceToIdlePosition);
            SearchForTargets(owner, out bool foundTarget, out float distanceFromTarget, out Vector2 targetCenter);
            Movement(foundTarget, distanceFromTarget, targetCenter, distanceToIdlePosition, vectorToIdlePosition);
            Visuals();
        }

        // This is the "active check", makes sure the minion is alive while the player is alive, and despawns if not
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

            // All of this code below this line is adapted from Spazmamini code (ID 388, aiStyle 66)

            // Teleport to player if distance is too big
            vectorToIdlePosition = idlePosition - Projectile.Center;
            distanceToIdlePosition = vectorToIdlePosition.Length();

            if (Main.myPlayer == owner.whoAmI && distanceToIdlePosition > 2000f)
            {
                // Whenever you deal with non-regular events that change the behavior or position drastically, make sure to only run the code on the owner of the projectile,
                // and then set netUpdate to true
                Projectile.position = idlePosition;
                Projectile.velocity *= 0.1f;
                Projectile.netUpdate = true;
            }

            // If your minion is flying, you want to do this independently of any conditions
            float overlapVelocity = 0.04f;

            // Fix overlap with other minions
            foreach (var other in Main.ActiveProjectiles)
            {
                if (other.whoAmI != Projectile.whoAmI && other.owner == Projectile.owner && Math.Abs(Projectile.position.X - other.position.X) + Math.Abs(Projectile.position.Y - other.position.Y) < Projectile.width)
                {
                    if (Projectile.position.X < other.position.X)
                    {
                        Projectile.velocity.X -= overlapVelocity;
                    }
                    else
                    {
                        Projectile.velocity.X += overlapVelocity;
                    }

                    if (Projectile.position.Y < other.position.Y)
                    {
                        Projectile.velocity.Y -= overlapVelocity;
                    }
                    else
                    {
                        Projectile.velocity.Y += overlapVelocity;
                    }
                }
            }
        }

        private void SearchForTargets(Player owner, out bool foundTarget, out float distanceFromTarget, out Vector2 targetCenter)
        {
            // Starting search distance
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
            Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<HorusExplosion>(), Utility.TrueDamage((int)(Projectile.damage * 0.3f)),0f, Main.myPlayer);
        }

        private void Movement(bool foundTarget, float distanceFromTarget, Vector2 targetCenter, float distanceToIdlePosition, Vector2 vectorToIdlePosition)
        {
            // Default movement parameters (here for attacking)
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

        private void Visuals()
        {
            // So it will lean slightly towards the direction it's moving
            
            Projectile.rotation = Projectile.velocity.ToRotation()-MathHelper.PiOver2;

            // This is a simple "loop through all frames from top to bottom" animation
            int frameSpeed = 5;

            Projectile.frameCounter++;

            if (Projectile.frameCounter >= frameSpeed)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;

                if (Projectile.frame >= Main.projFrames[Projectile.type])
                {
                    Projectile.frame = 0;
                }
            }

            // Some visuals here
            Lighting.AddLight(Projectile.Center, new Color(255, 141, 114).ToVector3() * 0.48f);
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White.WithAlpha(220);
        }

        private SpriteBatchState state;
        public override bool PreDraw(ref Color lightColor)
        {
            shineTexture ??= TextureAssets.Extra[ExtrasID.SharpTears]; 
            Vector2 orbit = new((float)Math.Cos(MathHelper.ToRadians(shineOrbitTimer * 2)) * 50f, -(float)Math.Sin(MathHelper.ToRadians(shineOrbitTimer * 2)) * 20f);

            state.SaveState(Main.spriteBatch);
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(BlendState.Additive, state);
            if ((-(float)Math.Sin(MathHelper.ToRadians(shineOrbitTimer * 2)) * 20f) < 0f)
                DrawShine(Main.spriteBatch, new Color(127,200,155), orbit);
            trail?.Draw(Projectile, Projectile.Size / 2f);
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(state);
            return true;
        }

        public override void PostDraw(Color lightColor)
        {
            state.SaveState(Main.spriteBatch);
            Vector2 orbit = new((float)Math.Cos(MathHelper.ToRadians(shineOrbitTimer * 2)) * 50f, -(float)Math.Sin(MathHelper.ToRadians(shineOrbitTimer * 2)) * 20f);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(BlendState.Additive, state);

            Texture2D glow = ModContent.Request<Texture2D>(Macrocosm.TextureEffectsPath + "Star5").Value;
            float opacity = 1f;
            Main.EntitySpriteDraw(glow, Projectile.Center - Main.screenPosition, null, new Color(255, 170, 142, 255) * opacity, Projectile.rotation, glow.Size() / 2, Projectile.scale * Main.rand.NextFloat(0.19f, 0.21f), SpriteEffects.None, 0f);
            if ((-(float)Math.Sin(MathHelper.ToRadians(shineOrbitTimer * 2)) * 20f) >= 0f)
                DrawShine(Main.spriteBatch, new Color(127,200,155), orbit);
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(state);
        }
        private void DrawShine(SpriteBatch spriteBatch, Color drawColor, Vector2 orbit)
        {
            int trailLength = 50;
            float opacityFactor = 0.4f;
            for (int i = 0; i < trailLength; i++)
            {
                float lerpFactor = i / (float)trailLength;
                Vector2 previousOrbit = new
                (
                    (float)Math.Cos(MathHelper.ToRadians((shineOrbitTimer - i) * 2)) * 50f,
                    -(float)Math.Sin(MathHelper.ToRadians((shineOrbitTimer - i) * 2)) * 20f
                );

                Color trailColor = drawColor * (1f - lerpFactor) * opacityFactor;

                spriteBatch.Draw(shineTexture.Value, Projectile.Center + previousOrbit.RotatedBy(MathHelper.ToRadians(-70)) - Main.screenPosition, null, trailColor, MathHelper.ToRadians(shineOrbitTimer)+MathHelper.ToRadians(-70), shineTexture.Size() / 2, Projectile.scale*(1f - lerpFactor)*0.7f, SpriteEffects.None, 0f);
            }

            spriteBatch.Draw(shineTexture.Value, Projectile.Center + orbit.RotatedBy(MathHelper.ToRadians(-70)) - Main.screenPosition, null, drawColor, MathHelper.ToRadians(shineOrbitTimer)+MathHelper.ToRadians(-70), shineTexture.Size() / 2, Projectile.scale*0.7f, SpriteEffects.None, 0f);
        }
    }
}
