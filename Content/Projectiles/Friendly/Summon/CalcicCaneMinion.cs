using Macrocosm.Common.Utils;
using Macrocosm.Content.Buffs.GoodBuffs.MinionBuffs;
using Macrocosm.Content.Dusts;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Projectiles.Friendly.Summon
{
    /// <summary>
    /// This is based off ExampleMod for now 
    /// </summary>
    public class CalcicCaneMinion : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 10;
            ProjectileID.Sets.MinionTargettingFeature[Type] = true;

            ProjectileID.Sets.TrailCacheLength[Type] = 6;
            ProjectileID.Sets.TrailingMode[Type] = 2;

            Main.projPet[Type] = true;

            ProjectileID.Sets.MinionSacrificable[Type] = true;
            ProjectileID.Sets.CultistIsResistantTo[Type] = true;
        }

        public sealed override void SetDefaults()
        {
            Projectile.width = 42;
            Projectile.height = 48;
            Projectile.tileCollide = false;

            Projectile.friendly = true;
            Projectile.minion = true;
            Projectile.DamageType = DamageClass.Summon;
            Projectile.minionSlots = 1f;
            Projectile.penetrate = -1;

            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 20;

            Projectile.alpha = 255;
        }

        public bool HasTarget
        {
            get => Projectile.ai[0] != 0f;
            set => Projectile.ai[0] = value ? 1f : 0f;
        }

        public ref float AttackTimer => ref Projectile.ai[1];

        public override bool? CanCutTiles() => false;

        public override bool MinionContactDamage() => true;

        public override Color? GetAlpha(Color lightColor)
            => lightColor * (1f - Projectile.alpha / 255f);

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = TextureAssets.Projectile[Type].Value;
            Texture2D glow = ModContent.Request<Texture2D>("Macrocosm/Content/Projectiles/Friendly/Summon/CalcicCaneMinion_Glow").Value;

            for (int i = 0; i < Projectile.oldPos.Length; i++)
            {
                Vector2 drawPos = Projectile.oldPos[i] + Projectile.Size / 2 - Main.screenPosition;
                float dashFactor = MathHelper.Clamp(Projectile.velocity.Length(), 0, 20) / 20f;
                float trailFactor = (((float)Projectile.oldPos.Length - i) / Projectile.oldPos.Length);
                Color color = Projectile.GetAlpha(lightColor) * trailFactor * dashFactor;
                SpriteEffects effect = Projectile.oldSpriteDirection[i] == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
                Main.EntitySpriteDraw(tex, drawPos, tex.Frame(1, Main.projFrames[Type], frameY: Projectile.frame), color * 0.6f, Projectile.oldRot[i], Projectile.Size / 2, Projectile.scale, effect, 0f);
                Main.EntitySpriteDraw(glow, drawPos, tex.Frame(1, Main.projFrames[Type], frameY: Projectile.frame), Color.White * trailFactor * dashFactor, Projectile.oldRot[i], Projectile.Size / 2, Projectile.scale, effect, 0f);
            }

            return true;
        }

        public override void PostDraw(Color lightColor)
        {
            Texture2D glow = ModContent.Request<Texture2D>("Macrocosm/Content/Projectiles/Friendly/Summon/CalcicCaneMinion_Glow").Value;
            Projectile.DrawAnimatedExtra(glow, Color.White, Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, new Vector2(0, Projectile.spriteDirection == 1 ? 5 : -1));
        }

        bool spawned = false;
        public override void OnSpawn(IEntitySource source)
        {
        }

        public override void AI()
        {
            if (!spawned)
            {
                spawned = true;
            }

            Player owner = Main.player[Projectile.owner];

            if (Projectile.alpha >= 0)
            {
                Projectile.alpha -= 5;
                SpawnDusts();
            }

            if (!CheckActive(owner))
                return;

            GeneralBehavior(owner, out Vector2 vectorToIdlePosition, out float distanceToIdlePosition);
            SearchForTargets(owner, out bool foundTarget, out float distanceFromTarget, out Vector2 targetCenter);
            Movement(foundTarget, distanceFromTarget, targetCenter, distanceToIdlePosition, vectorToIdlePosition);
            Visuals(foundTarget);

            HasTarget = foundTarget;
        }

        private void SpawnDusts()
        {
            for (int i = 0; i < (int)(5 * (Projectile.alpha / 255f)); i++)
            {
                Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<RegolithDust>(), Scale: Main.rand.NextFloat(0.4f, 0.6f));
                dust.velocity = new Vector2(0, Main.rand.NextFloat(1.2f, 2.2f));
            }
        }


        private bool CheckActive(Player owner)
        {
            if (owner.dead || !owner.active)
            {
                owner.ClearBuff(ModContent.BuffType<CalcicCaneMinionBuff>());

                return false;
            }

            if (owner.HasBuff(ModContent.BuffType<CalcicCaneMinionBuff>()))
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

            if (!foundTarget && Projectile.alpha < 1)
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

            // friendly needs to be set to true so the minion can deal contact damage
            // friendly needs to be set to false so it doesn't damage things like target dummies while idling
            // Both things depend on if it has a target or not, so it's just one assignment here
            // You don't need this assignment if your minion is shooting things instead of dealing contact damage
            Projectile.friendly = foundTarget;
        }

        private void Movement(bool foundTarget, float distanceFromTarget, Vector2 targetCenter, float distanceToIdlePosition, Vector2 vectorToIdlePosition)
        {
            // Default movement parameters (here for attacking)
            float speed = 8f;
            float inertia = 20f;
            bool dash = false;

            if (foundTarget)
            {

                if (AttackTimer < 120)
                {
                    AttackTimer++;
                }
                else if (AttackTimer < 140)
                {
                    speed = 0f;
                    AttackTimer++;
                }
                else
                {
                    AttackTimer = 0;
                    speed *= 20f * MathHelper.Clamp(distanceFromTarget, 0, 100) / 100;
                    inertia = 10f;
                    dash = true;
                }

                // Minion has a target: attack (here, fly towards the enemy)
                if (distanceFromTarget > 60f || dash)
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

        private void Visuals(bool hasTarget)
        {
            // So it will lean slightly towards the direction it's moving
            Projectile.rotation = Projectile.velocity.X * 0.05f;
            Projectile.spriteDirection = Projectile.direction;

            int frameSpeed = 15;

            Projectile.frameCounter++;

            // frames 0 - 3: attack frames (+0 idle)
            if (hasTarget)
            {
                if (Projectile.frameCounter >= frameSpeed)
                {
                    Projectile.frameCounter = 0;
                    Projectile.frame++;

                    if (Projectile.frame >= 4)
                    {
                        Projectile.frame = 0;
                    }
                }
            }
            // frame 0 + frames 4-9: idle frames 
            else
            {
                // idle animation is played randomly or every ten seconds 
                if ((Main.rand.NextBool(300) || Main.timeForVisualEffects % 600 == 0) && Projectile.frame == 0)
                {
                    Projectile.frame = 4;
                }

                // if animation is ongoing
                if (Projectile.frame >= 4)
                {
                    if (Projectile.frameCounter >= frameSpeed)
                    {
                        Projectile.frameCounter = 0;
                        Projectile.frame++;

                        if (Projectile.frame >= Main.projFrames[Type])
                        {
                            Projectile.frame = 0;
                        }
                    }
                }
                // else default frame 
                else
                {
                    Projectile.frame = 0;
                }
            }
        }

    }
}
