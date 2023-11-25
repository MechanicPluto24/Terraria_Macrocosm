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

        public override void SetDefaults()
        {
            Projectile.width = 8;
            Projectile.height = 8;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.tileCollide = true;
            Projectile.ignoreWater = true;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 45;

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
                targetAngle = (OrbitAngle * MathHelper.Pi / 180) - MathHelper.Pi / 2;
                spawned = true;

                targetPosition = Projectile.Center;
                movementVector = Projectile.velocity;
            }

            AI_Timer++;

            // Keep alive unless metting specific conditions
            Projectile.timeLeft++;

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

                        if (Projectile.owner == Main.myPlayer)
                        {
                            floatDuration = (ushort)Main.rand.Next(1, 12);
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
                            turnSpeed = Main.rand.NextFloat(0.01f, 0.1f);
                            Projectile.netUpdate = true;
                        }

                        for (int i = 0; i < 15; i++)
                        {
                            Vector2 velocity = Main.rand.NextVector2Circular(16, 2f).RotatedBy(movementVector.ToRotation());
                            Dust dust = Dust.NewDustPerfect(targetPosition, ModContent.DustType<DianiteBrightDust>(), velocity, Scale: Main.rand.NextFloat(1f, 1.4f));
                            dust.noGravity = true;
                        }
                    }

                    break;

                case ActionState.Home:

                    float homingDistance = 800f;
                    float closestDistance = homingDistance;

                    for (int i = 0; i < Main.maxNPCs; i++)
                    {
                        NPC npc = Main.npc[i];
                        if (npc.active && !npc.friendly && npc.lifeMax > 5 && !npc.dontTakeDamage)
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
                    }
                    else
                    {
                        Projectile.timeLeft--;
                    }

                    break;
            }

            Lighting.AddLight(Projectile.Center, new Color(255, 146, 0).ToVector3());

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
                if (AI_Timer % 2 == 0)
                {
                    Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<DianiteBrightDust>(), -Projectile.velocity.X * 0.4f, -Projectile.velocity.Y * 0.4f);
                    dust.noGravity = true;
                }
            }
        }


        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 20; i++)
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
            Texture2D glow = ModContent.Request<Texture2D>(Macrocosm.TextureAssetsPath + "Circle6").Value;
            Vector2 origin = Projectile.Size / 2f;
            ;
            state.SaveState(Main.spriteBatch);
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(BlendState.Additive, state);

            Projectile.GetTrail().Draw(Projectile.Size / 2f);
            Main.EntitySpriteDraw(glow, Projectile.Center - Main.screenPosition, null, new Color(215, 101, 0), 0f, glow.Size() / 2, 0.05f * Projectile.scale, SpriteEffects.None, 0f);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(state);

            Main.EntitySpriteDraw(tex, Projectile.Center - Main.screenPosition,
                null, Color.White, Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(state);

            return false;
        }
    }
}

