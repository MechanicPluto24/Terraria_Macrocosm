using Macrocosm.Common.DataStructures;
using Macrocosm.Common.Utils;
using Macrocosm.Content.Dusts;
using Macrocosm.Content.Trails;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Projectiles.Friendly.Magic
{
    // Parent projectile class below
    public class DianiteForkProjectile : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 15;
            ProjectileID.Sets.TrailingMode[Type] = 3;
        }

        private const int spawnTimeLeft = 45; //90

        public override void SetDefaults()
        {
            Projectile.width = 8;
            Projectile.height = 8;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.tileCollide = true;
            Projectile.ignoreWater = true;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 90;

            Projectile.SetTrail<DianiteForkTrail>();
        }

        public enum ActionState { Orbit, Float, Home }
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

        public float TimeLeftProgress => (float)Projectile.timeLeft / 90;

        private int spawnDamage;

        // The orbit center's position
        private Vector2 targetPosition;

        // The orbit center's movement vector (i.e. its velocity)
        private Vector2 movementVector;

        // The target angle when breaking apart the orbit
        private float targetAngle;

        // The free float duration, randomized and netsynced
        private int floatDuration;

        // The speed at the start of the homing
        private float originalSpeed;

        // The turn speed of the homing, randomized and netsynced
        private float turnSpeed;

        // The targeted NPC whoAmI
        private int targetNPC;

        // The targeted NPC 
        private NPC TargetNPC => Main.npc[targetNPC];

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

        private bool spawned = false;
        public override void AI()
        {
            if (!spawned)
            {
                targetAngle = MathHelper.Pi / 8 - MathHelper.Pi / 4 * (OrbitAngle / 180);
                spawned = true;

                targetPosition = Projectile.Center;
                movementVector = Projectile.velocity;

                spawnDamage = Projectile.damage;
            }

            AI_Timer++;

            // Keep alive unless metting specific conditions
            Projectile.timeLeft++;

            Projectile.Opacity = TimeLeftProgress;

            Projectile.damage = (int)(spawnDamage * TimeLeftProgress);
            if (TimeLeftProgress < 0.2f)
                Projectile.damage = 0;

            switch (AI_State)
            {
                case ActionState.Orbit:

                    Vector2 vector = (targetPosition - Projectile.Center).SafeNormalize(Vector2.UnitY);
                    Projectile.rotation = vector.ToRotation() - 1.57f;
                    OrbitAngle += 12 * Math.Sign(movementVector.X);
                    float orbitDistance = 10;

                    float vX = orbitDistance * MathF.Cos(OrbitAngle / 180 * MathHelper.Pi);
                    float vY = orbitDistance * MathF.Sin(OrbitAngle / 180 * MathHelper.Pi);

                    Projectile.position = targetPosition - Projectile.Size / 2f;

                    Projectile.velocity.X = vX;
                    Projectile.velocity.Y = vY;

                    targetPosition += movementVector;

                    if (AI_Timer >= 60)
                    {
                        AI_State = ActionState.Float;
                        AI_Timer = 0;

                        Projectile.velocity = movementVector.RotatedBy(targetAngle) * 0.8f;

                        for (int i = 0; i < 35; i++)
                        {
                            Vector2 velocity = Main.rand.NextVector2Circular(16, 1f).RotatedBy(Projectile.velocity.ToRotation());
                            Dust dust = Dust.NewDustPerfect(targetPosition, ModContent.DustType<DianiteBrightDust>(), velocity, Scale: Main.rand.NextFloat(1f, 1.4f));
                            dust.noGravity = true;
                        }

                        if (Projectile.owner == Main.myPlayer)
                        {
                            floatDuration = (ushort)Main.rand.Next(20, 40);
                            Projectile.netUpdate = true;
                        }
                    }

                    break;

                case ActionState.Float:

                    if (AI_Timer >= floatDuration)
                    {
                        AI_State = ActionState.Home;
                        AI_Timer = 0;

                        Projectile.penetrate = 1;
                        originalSpeed = Projectile.velocity.Length();

                        if (Projectile.owner == Main.myPlayer)
                        {
                            turnSpeed = Main.rand.NextFloat(0.01f, 0.07f);
                            Projectile.netUpdate = true;
                        }
                    }

                    break;

                case ActionState.Home:

                    float homingDistance = 500f;
                    float closestDistance = homingDistance;

                    for (int i = 0; i < Main.maxNPCs; i++)
                    {
                        NPC npc = Main.npc[i];
                        if (npc.active && !npc.friendly && npc.lifeMax > 5 && !npc.dontTakeDamage && npc.type != NPCID.TargetDummy)
                        {
                            float distance = Vector2.Distance(Projectile.Center, npc.Center);
                            if (distance < closestDistance)
                            {
                                closestDistance = distance;
                                targetNPC = npc.whoAmI;
                            }
                        }
                    }

                    // Adjust the projectile's velocity towards the target over time
                    if (TargetNPC is not null && Vector2.Distance(Projectile.Center, TargetNPC.Center) < homingDistance && TargetNPC.active && !TargetNPC.friendly && TargetNPC.lifeMax > 5 && !TargetNPC.dontTakeDamage)
                    {
                        Vector2 direction = (TargetNPC.Center - Projectile.Center).SafeNormalize(Vector2.Zero);
                        Projectile.velocity = Vector2.Lerp(Projectile.velocity, direction * originalSpeed, turnSpeed);
                        Projectile.timeLeft--;
                    }
                    else
                    {
                        Projectile.timeLeft -= 2;
                    }

                    break;
            }

            Lighting.AddLight(Projectile.Center, new Color(255, 146, 0).ToVector3() * TimeLeftProgress);

            if (AI_State is ActionState.Orbit)
            {
                if (AI_Timer % 3 == 0)
                {
                    Dust dust = Dust.NewDustDirect(Projectile.Center, 1, 1, ModContent.DustType<DianiteBrightDust>(), -Projectile.velocity.X * 0.4f, -Projectile.velocity.Y * 0.4f);
                    dust.noGravity = true;
                }
            }
            else
            {
                if (AI_Timer % (2 + (int)(3 * (1f - TimeLeftProgress))) == 0)
                {
                    Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<DianiteBrightDust>(), -Projectile.velocity.X * 0.4f, -Projectile.velocity.Y * 0.4f);
                    dust.noGravity = true;
                }
            }
        }


        public override void OnKill(int timeLeft)
        {
            int count = (int)(20f * Projectile.Opacity);
            for (int i = 0; i < count; i++)
            {
                Vector2 velocity = Main.rand.NextVector2Circular(4, 4);
                Dust dust = Dust.NewDustPerfect(Projectile.position, ModContent.DustType<DianiteBrightDust>(), velocity, Scale: Main.rand.NextFloat(1f, 1.6f));
                dust.noGravity = true;
            }
        }

        private SpriteBatchState state;
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = TextureAssets.Projectile[Type].Value;
            Texture2D glow = ModContent.Request<Texture2D>(Macrocosm.TextureEffectsPath + "Circle6").Value;
            Vector2 origin = Projectile.Size / 2f;

            state.SaveState(Main.spriteBatch);
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(BlendState.Additive, state);

            Projectile.GetTrail().Opacity = TimeLeftProgress;
            Projectile.GetTrail().Draw(Projectile.Size / 2f);

            Color glowColor = new Color(248, 137, 0).WithOpacity(Projectile.Opacity);
            int glowTrailCount = (int)(ProjectileID.Sets.TrailCacheLength[Type] * 0.5f * TimeLeftProgress);
            for (int i = 0; i < glowTrailCount - 1; i++)
            {
                float trailMultCurrent = 1f - ((float)i / glowTrailCount);
                float trailMultNext = 1f - ((float)(i + 1) / glowTrailCount);

                Vector2 currentPosition = Projectile.oldPos[i] + Projectile.Size / 2f - Main.screenPosition;
                Vector2 nextPosition = Projectile.oldPos[i + 1] + Projectile.Size / 2f - Main.screenPosition;

                Vector2 firstLerpPosition = Vector2.Lerp(currentPosition, nextPosition, 1f / 3f);
                Vector2 secondLerpPosition = Vector2.Lerp(currentPosition, nextPosition, 2f / 3f);

                Main.EntitySpriteDraw(glow, currentPosition, null, glowColor.WithOpacity(0.33f), Projectile.rotation, glow.Size() / 2, 0.035f * Projectile.scale * trailMultCurrent, SpriteEffects.None, 0f);

                float avgTrailMultFirst = (trailMultCurrent * 2 + trailMultNext) / 3;
                float avgTrailMultSecond = (trailMultCurrent + trailMultNext * 2) / 3;

                Main.EntitySpriteDraw(glow, firstLerpPosition, null, glowColor.WithOpacity(0.33f), Projectile.rotation, glow.Size() / 2, 0.035f * Projectile.scale * avgTrailMultFirst, SpriteEffects.None, 0f);
                Main.EntitySpriteDraw(glow, secondLerpPosition, null, glowColor.WithOpacity(0.33f), Projectile.rotation, glow.Size() / 2, 0.035f * Projectile.scale * avgTrailMultSecond, SpriteEffects.None, 0f);
            }

            Main.EntitySpriteDraw(glow, Projectile.Center - Main.screenPosition, null, glowColor, Projectile.rotation, glow.Size() / 2, 0.042f * Projectile.scale, SpriteEffects.None, 0f);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(state);

            Main.EntitySpriteDraw(tex, Projectile.Center - Main.screenPosition,
                null, Color.White * TimeLeftProgress, Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(state);

            return false;
        }
    }
}

