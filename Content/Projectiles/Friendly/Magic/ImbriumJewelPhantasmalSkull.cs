using Macrocosm.Common.DataStructures;
using Macrocosm.Common.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Projectiles.Friendly.Magic
{
    public class ImbriumJewelPhantasmalSkull : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 15;
            ProjectileID.Sets.TrailingMode[Type] = 3;
            ProjectileID.Sets.CultistIsResistantTo[Type] = true;
        }

        private static int spawnTimeLeft = 3 * 60;
        public override void SetDefaults()
        {
            Projectile.width = 66;
            Projectile.height = 74;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.timeLeft = spawnTimeLeft;
            Projectile.alpha = 0;
            Projectile.tileCollide = false;
            Projectile.light = 1f;
            CooldownSlot = 1;
        }

        private bool spawned;

        public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac)
            => false;

        //bool spawned = false;
        public enum ActionState { Move, Home }
        public ActionState AI_State
        {
            get => (ActionState)Projectile.ai[0];
            set => Projectile.ai[0] = (float)value;
        }

        public ref float OrbitAngle => ref Projectile.ai[1];

        public int AI_Timer
        {
            get => (int)Projectile.ai[2];
            set => Projectile.ai[2] = value;
        }

        // The orbit center's position
        private Vector2 targetPosition;

        // The orbit center's movement vector (i.e. its velocity)
        private Vector2 movementVector;

        // The target angle when breaking apart the orbit
        private float targetAngle;

        // The free float duration, randomized and netsynced
        private int floatDuration;

        // The speed at the start of the homing

        // The turn speed of the homing, randomized and netsynced
        private float turnSpeed;

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(turnSpeed);
            writer.Write((byte)floatDuration);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            turnSpeed = reader.ReadSingle();
            floatDuration = reader.ReadByte();
        }

        private void AdjustMagnitude(ref Vector2 vector)
        {
            float magnitude = (float)Math.Sqrt(vector.X * vector.X + vector.Y * vector.Y);
            if (magnitude > 12f)
            {
                vector *= 12f / magnitude;
            }
        }

        public override void AI()
        {
            if (!spawned)
            {
                targetAngle = MathHelper.Pi / 8 - MathHelper.Pi / 4 * (OrbitAngle / 180);
                SoundEngine.PlaySound(SoundID.NPCDeath6 with { PitchRange = (-0.5f, 0.5f) }, Projectile.Center);
                spawned = true;

                targetPosition = Projectile.Center;
                movementVector = Projectile.velocity;
            }

            float homingDistance = 500f;
            float closestDistance = homingDistance;

            if (Projectile.localAI[0] == 0f)
            {
                AdjustMagnitude(ref Projectile.velocity);
                Projectile.localAI[0] = 1f;
            }
            Vector2 move = Vector2.Zero;
            float distance = 600f;
            bool target = false;
            Projectile.ai[1]++;
            if (Projectile.ai[1] <= 30)
            {
                for (int k = 0; k < 200; k++)
                {
                    if (Main.npc[k].active && Main.npc[k].CanBeChasedBy(this, true))
                    {
                        Vector2 newMove = Main.npc[k].Center - Projectile.Center;
                        float distanceTo = (float)Math.Sqrt(newMove.X * newMove.X + newMove.Y * newMove.Y);
                        if (distanceTo < distance && Collision.CanHitLine(Projectile.Center, 0, 0, Main.npc[k].Center, 0, 0))
                        {
                            move = newMove;
                            distance = distanceTo;
                            target = true;
                        }
                    }
                }
                if (target)
                {
                    AdjustMagnitude(ref move);
                    Projectile.velocity = (10 * Projectile.velocity + move) / 11f;
                    AdjustMagnitude(ref Projectile.velocity);
                }
            }

            /*if (TargetNPC != null && Vector2.Distance(Projectile.Center, TargetNPC.Center) < homingDistance && TargetNPC.CanBeChasedBy())
            {
                Vector2 direction = (TargetNPC.Center - Projectile.Center).SafeNormalize(Vector2.Zero);
                Projectile.velocity = Vector2.Lerp(Projectile.velocity, direction * originalSpeed, turnSpeed);
            }
            else
            {
                //Projectile.rotation = Projectile.velocity.X < 0 ? MathHelper.Pi + Projectile.velocity.ToRotation() : Projectile.velocity.ToRotation();
            }*/

            /*Vector2 direction = TargetPlayer.Center - Projectile.Center;
            float distance = direction.Length();
            direction.Normalize();

            float deviation = Main.rand.NextFloat(-0.1f, 0.1f);
            direction = direction.RotatedBy(deviation);

            Vector2 adjustedDirection = Vector2.Lerp(Projectile.velocity, direction, 0.2f);
            adjustedDirection.Normalize();

            float accelerateDistance = 30f * 16;
            float speed = Projectile.velocity.Length() + (distance > accelerateDistance ? distance / accelerateDistance * 0.1f : 0f);
            Projectile.velocity = adjustedDirection * speed;*/

            Projectile.direction = Math.Sign(Projectile.velocity.X);
            Projectile.spriteDirection = Projectile.direction;
            Projectile.rotation = Projectile.velocity.X < 0 ? MathHelper.Pi + Projectile.velocity.ToRotation() : Projectile.velocity.ToRotation();

            if (Projectile.timeLeft < (int)(0.33f * spawnTimeLeft) && Projectile.alpha < 255)
                Projectile.alpha += 6;

            if (Projectile.alpha >= 255)
                Projectile.active = false;
        }
        public NPC FindClosestNPC(float maxDetectDistance)
        {
            NPC closestNPC = null;

            float sqrMaxDetectDistance = maxDetectDistance * maxDetectDistance;

            for (int k = 0; k < Main.maxNPCs; k++)
            {
                NPC target = Main.npc[k];
                if (target.CanBeChasedBy())
                {
                    float sqrDistanceToTarget = Vector2.DistanceSquared(target.Center, Projectile.Center);

                    if (sqrDistanceToTarget < sqrMaxDetectDistance)
                    {
                        sqrMaxDetectDistance = sqrDistanceToTarget;
                        closestNPC = target;
                    }
                }
            }

            return closestNPC;
        }

        public override Color? GetAlpha(Color lightColor) => Color.White.WithOpacity(1f);

        private SpriteBatchState state;

        public override bool PreDraw(ref Color lightColor)
        {
            int length = Projectile.oldPos.Length;

            state.SaveState(Main.spriteBatch);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(BlendState.Additive, state);

            for (int i = 1; i < length; i++)
            {
                Vector2 drawPos = Projectile.oldPos[i] - Main.screenPosition + Projectile.Size / 2f;

                Color trailColor = Color.White * (((float)Projectile.oldPos.Length - i) / Projectile.oldPos.Length) * 0.45f * (1f - Projectile.alpha / 255f);
                Main.spriteBatch.Draw(TextureAssets.Projectile[Type].Value, drawPos, null, trailColor, Projectile.velocity.X < 0 ? MathHelper.Pi + Projectile.oldRot[i] : Projectile.oldRot[i], Projectile.Size / 2f, Projectile.scale, Projectile.oldSpriteDirection[i] == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0f);
            }

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(BlendState.NonPremultiplied, state);

            Main.EntitySpriteDraw(TextureAssets.Projectile[Type].Value, Projectile.position - Main.screenPosition + Projectile.Size / 2f, null, Color.White.WithOpacity(0.7f * (1f - Projectile.alpha / 255f)), Projectile.rotation, Projectile.Size / 2f, Projectile.scale, Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0f);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(state);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(BlendState.Additive, state);

            // Strange
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(BlendState.AlphaBlend, state);
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(state);

            return false;
        }
    }
}
