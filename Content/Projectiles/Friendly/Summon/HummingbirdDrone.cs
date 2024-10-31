using Macrocosm.Content.Buffs.Minions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Projectiles.Friendly.Summon
{
    public class HummingbirdDrone : ModProjectile
    {
        private static Asset<Texture2D> beam;
        private static Asset<Texture2D> glowmask;
        private static Asset<Texture2D> flash;
        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 2;
            ProjectileID.Sets.MinionTargettingFeature[Type] = true;

            ProjectileID.Sets.TrailCacheLength[Type] = 6;
            ProjectileID.Sets.TrailingMode[Type] = 2;

            Main.projPet[Type] = true;

            ProjectileID.Sets.MinionSacrificable[Type] = true;
        }

        public sealed override void SetDefaults()
        {
            Projectile.width = 50;
            Projectile.height = (int)(78 / 2);
            Projectile.tileCollide = false;

            Projectile.friendly = true;
            Projectile.minion = true;
            Projectile.DamageType = DamageClass.Summon;
            Projectile.minionSlots = 1f;
            Projectile.penetrate = -1;

            Projectile.alpha = 255;

            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 20;
        }

        public bool HasTarget
        {
            get => Projectile.ai[0] != 0f;
            set => Projectile.ai[0] = value ? 1f : 0f;
        }

        public ref float OrbitAngle => ref Projectile.ai[1];
        public ref float AnimationState => ref Projectile.localAI[0];

        public override bool? CanCutTiles() => false;
        public override bool MinionContactDamage() => true;

        public bool ShouldProtect(Player owner) => owner.active && !owner.dead && owner.statLife < (owner.statLifeMax2 / 2) && owner.ownedProjectileCounts[Projectile.type] > 2;

        private bool spawned = false;
        private Vector2 orbitPos;
        private int shootTimer = 0;
        private Vector2 flyTo;

        private int flashFrame = -1;
        private float flashRotation = 0;

        public override void AI()
        {
            if (!spawned)
            {
                spawned = true;
            }

            OrbitAngle = 0f;
            if (Projectile.owner == Main.myPlayer)
            {
                for (int i = 0; i < Projectile.whoAmI; i++)
                {
                    Projectile proj = Main.projectile[i];
                    if (proj.active && proj.type == Type && proj.owner == Projectile.owner && Projectile.whoAmI != proj.whoAmI)
                    {
                        OrbitAngle += 1f;
                    }
                }
                Projectile.netUpdate = true;
            }

            Player owner = Main.player[Projectile.owner];

            if (!CheckActive(owner))
                return;

            GeneralBehavior(owner, out Vector2 vectorToIdlePosition, out float distanceToIdlePosition);
            SearchForTargets(owner, out bool foundTarget, out float distanceFromTarget, out Vector2 targetCenter);

            if (!ShouldProtect(owner))
                Attack(foundTarget, distanceFromTarget, targetCenter);
            else
                Protect(foundTarget, owner, targetCenter);

            Visuals(foundTarget);

            HasTarget = foundTarget;

            if (owner.statLife > (owner.statLifeMax2 / 2) && owner.ownedProjectileCounts[Projectile.type] > 2)
            {
                Projectile.friendly = false;
            }
            else
            {
                Projectile.friendly = true;
                owner.statDefense += 3;
            }

            Lighting.AddLight(Projectile.position, new Color(225, 100, 100).ToVector3() * 0.4f);
        }

        public override void OnSpawn(IEntitySource source)
        {
        }

        private bool CheckActive(Player owner)
        {
            if (owner.dead || !owner.active)
            {
                owner.ClearBuff(ModContent.BuffType<HummingbirdDroneSummonBuff>());

                return false;
            }

            if (owner.HasBuff(ModContent.BuffType<HummingbirdDroneSummonBuff>()))
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
            bool shouldTeleportToOwner = HasTarget ? distanceToIdlePosition > 4000f : distanceToIdlePosition > 2000f;

            if (Main.myPlayer == owner.whoAmI && shouldTeleportToOwner)
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
            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                Projectile other = Main.projectile[i];

                if (i != Projectile.whoAmI && other.active && other.owner == Projectile.owner && Math.Abs(Projectile.position.X - other.position.X) + Math.Abs(Projectile.position.Y - other.position.Y) < Projectile.width)
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
            Projectile.friendly = true;
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
                for (int i = 0; i < Main.maxNPCs; i++)
                {
                    NPC npc = Main.npc[i];

                    if (npc.CanBeChasedBy())
                    {
                        float between = Vector2.Distance(npc.Center, Projectile.Center);
                        bool closest = Vector2.Distance(Projectile.Center, targetCenter) > between;
                        bool inRange = between < distanceFromTarget;
                        bool lineOfSight = Collision.CanHitLine(Projectile.position, Projectile.width, Projectile.height, npc.position, npc.width, npc.height);
                        // Additional check for this specific minion behavior, otherwise it will stop attacking once it dashed through an enemy while flying though tiles afterwards
                        // The number depends on various parameters seen in the movement code below. Test different ones out until it works alright
                        bool closeThroughWall = between < 100f;

                        if (((closest && inRange) || !foundTarget) && (lineOfSight || closeThroughWall))
                        {
                            distanceFromTarget = between;
                            targetCenter = npc.Center;
                            foundTarget = true;
                        }
                    }
                }
            }
        }

        public void BurstFire(Vector2 target)
        {
            Projectile.direction = Math.Sign(target.X - Projectile.Center.X);
            Projectile.rotation = (target - Projectile.Center).ToRotation();

            if (shootTimer > 100 && shootTimer % 4 == 0 && Projectile.owner == Main.myPlayer)
            {
                SoundEngine.PlaySound(SoundID.Item11, Projectile.Center);
                Vector2 position = Projectile.Center + (Projectile.spriteDirection == 1 ? new Vector2(30, 6) : new Vector2(-30, 6));
                Vector2 velocity = (target - position).RotatedByRandom(MathHelper.Pi / 10).SafeNormalize(Vector2.UnitX) * 28f;
                Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), position, velocity, ModContent.ProjectileType<HummingbirdBullet>(), (int)(Projectile.damage), 1f, Main.myPlayer, 1f);
                flashFrame = 0;
            }
        }
        private void Attack(bool foundTarget, float distanceFromTarget, Vector2 targetCenter)
        {
            Player owner = Main.player[Projectile.owner];

            // Default movement parameters (here for attacking)
            float speed = 45f;
            float inertia = 60f;
            Vector2 vectorToIdlePosition = owner.Center - Projectile.Center;
            float distanceToIdlePosition = Vector2.Distance(Projectile.Center, owner.Center);
            if (foundTarget)
            {
                // Minion has a target: attack (here, fly towards the enemy)

                shootTimer--;

                if (shootTimer < 1)
                {
                    shootTimer = 120;
                    flyTo = (new Vector2(200, 0)).RotatedByRandom(MathHelper.TwoPi);
                }

                BurstFire(targetCenter);

                // The immediate range around the target (so it doesn't latch onto it when close)
                if (Vector2.Distance(Projectile.Center, targetCenter + flyTo) > 80f)
                {
                    Vector2 direction = (targetCenter + flyTo) - Projectile.Center;
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
                    speed = 12f;
                    inertia = 60f;
                }
                else
                {
                    // Slow down the minion if closer to the player
                    speed = 4f;
                    inertia = 80f;
                }

                if (distanceToIdlePosition > 20f)
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

        private void Protect(bool foundTarget, Player owner, Vector2 targetCenter)
        {
            // Default movement parameters (here for attacking)
            float speed = 20f;
            float inertia = 60f;

            if (foundTarget)
            {
                // Minion has a target: attack (here, fly towards the enemy)  
                shootTimer--;

                if (shootTimer < 1)
                {
                    shootTimer = 120;
                }

                BurstFire(targetCenter);
            }

            orbitPos = owner.Center + ((MathHelper.TwoPi * OrbitAngle / owner.ownedProjectileCounts[Projectile.type]).ToRotationVector2() * 250f);
            // Minion doesn't have a target: return to player and idle
            if (Vector2.Distance(orbitPos, Projectile.Center) > 30f)
            {
                Vector2 direction = orbitPos - Projectile.Center;
                direction.Normalize();
                direction *= speed;

                Projectile.velocity = (Projectile.velocity * (inertia - 1) + direction) / inertia;
            }
        }

        private void Visuals(bool hasTarget)
        {
            // So it will lean slightly towards the direction it's moving
            Projectile.rotation = Projectile.velocity.X * 0.05f;

            Projectile.spriteDirection = Projectile.direction;
            int frameSpeed = 4;
            Projectile.frameCounter++;

            if (Projectile.frameCounter >= frameSpeed)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;

                if (Projectile.frame >= 2)
                    Projectile.frame = 0;
            }

            if (flashFrame >= 0 && flashFrame++ > 2)
                flashFrame = -1;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            Player owner = Main.player[Projectile.owner];
            float _ = float.NaN;
            if (owner.statLife <= (owner.statLifeMax2 / 2) && owner.ownedProjectileCounts[Projectile.type] > 2)
            {
                // If the target is touching the beam's hitbox (which is a small rectangle vaguely overlapping the host Prism), that's good enough.
                if (projHitbox.Intersects(targetHitbox))
                {
                    return true;
                }

                Projectile proj;
                Vector2 beamStart = Projectile.Center;

                if (OrbitAngle != 0f)
                    proj = GetConnectedDrone(OrbitAngle - 1f);
                else
                    proj = GetConnectedDrone((float)(owner.ownedProjectileCounts[Projectile.type] - 1));

                if (proj is null)
                    return false;

                Vector2 beamEnd = proj.Center;

                if (Vector2.Distance(beamStart, beamEnd) > 700f)
                    return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.Center, Projectile.Center, 10f * Projectile.scale, ref _);
                if (beamStart == beamEnd)
                    return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.Center, Projectile.Center, 10f * Projectile.scale, ref _);

                // Otherwise, perform an AABB line collision check to check the whole beam.
                return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.Center, beamEnd, 10f * Projectile.scale, ref _);
            }
            else
            {
                return false;
            }
        }

        private Projectile GetConnectedDrone(float droneOffset)
        {
            Projectile proj = null;
            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                Projectile projcheck = Main.projectile[i];
                if (projcheck.type == ModContent.ProjectileType<HummingbirdDrone>() && projcheck.ai[1] == droneOffset && projcheck.active)
                    proj = Main.projectile[i];
            }
            return proj;
        }

        private void DrawBeam(SpriteBatch spriteBatch)
        {
            Player owner = Main.player[Projectile.owner];
            Projectile proj = (OrbitAngle != 0f) ? GetConnectedDrone(OrbitAngle - 1f) : GetConnectedDrone((float)owner.ownedProjectileCounts[Projectile.type] - 1f);
            Vector2 beamStart = Projectile.Center;
            if (proj is null)
                return;
            Vector2 beamEnd = proj.Center;

            if (beamStart == beamEnd)
                return;

            if (Vector2.Distance(beamStart, beamEnd) > 700f)
                return;

            beamStart -= Main.screenPosition;
            beamEnd -= Main.screenPosition;

            float rotation = (beamEnd - beamStart).ToRotation() + MathHelper.PiOver2;
            beam ??= ModContent.Request<Texture2D>(Macrocosm.TextureEffectsPath + "Beam4");
            Vector2 scale = new Vector2(45f, Vector2.Distance(beamStart, beamEnd)) / beam.Size();
            Vector2 origin = new(beam.Width() * 0.5f, beam.Height());

            spriteBatch.Draw(beam.Value, beamStart, null, new Color(255, 0, 0, 0), rotation, origin, scale, SpriteEffects.None, 0f);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = TextureAssets.Projectile[Type].Value;
            Vector2 pos = Projectile.position + Projectile.Size / 2 - Main.screenPosition;
            SpriteEffects effects = Projectile.spriteDirection == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

            if (Projectile.alpha > 0)
                Main.EntitySpriteDraw(tex, pos, tex.Frame(1, Main.projFrames[Type], frameY: Projectile.frame), (lightColor * (Projectile.alpha / 255f)), Projectile.rotation, Projectile.Size / 2, Projectile.scale, effects, 0f);

            return false;
        }

        public override void PostDraw(Color lightColor)
        {
            glowmask ??= ModContent.Request<Texture2D>(Texture + "_Glow");
            Vector2 pos = Projectile.position + Projectile.Size / 2 - Main.screenPosition;
            SpriteEffects effects = Projectile.spriteDirection == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

            if (Projectile.alpha > 0)
                Main.EntitySpriteDraw(glowmask.Value, pos, glowmask.Frame(1, Main.projFrames[Type], frameY: Projectile.frame), (Color.White * (Projectile.alpha / 255f)), Projectile.rotation, Projectile.Size / 2, Projectile.scale, effects, 0f);

            if (flashFrame >= 0)
            {
                flash ??= ModContent.Request<Texture2D>(Texture + "_Flash");
                Vector2 flashPos = Projectile.Center + (Projectile.spriteDirection == 1 ? new Vector2(24, 3) : new Vector2(-24, 3)).RotatedBy(Projectile.rotation) - Main.screenPosition;
                Rectangle frame = flash.Frame(1, 3, frameY: flashFrame);
                Main.EntitySpriteDraw(flash.Value, flashPos, frame, (Color.White * (Projectile.alpha / 255f)), Projectile.rotation, frame.Size() / 2, Projectile.scale, effects, 0f);
            }

            if (ShouldProtect(Main.player[Projectile.owner]))
                DrawBeam(Main.spriteBatch);
        }
    }
}
