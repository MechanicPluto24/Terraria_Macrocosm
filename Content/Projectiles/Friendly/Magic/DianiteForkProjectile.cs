using Macrocosm.Common.Utils;
using Macrocosm.Content.Trails;
using Microsoft.CodeAnalysis;
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
            ProjectileID.Sets.TrailCacheLength[Type] = 10;
            ProjectileID.Sets.TrailingMode[Type] = 0;
        }

        public override void SetDefaults()
        {
            Projectile.width = 8;
            Projectile.height = 8;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.penetrate = -1;

            Projectile.SetTrail<DianiteForkTrail>();
        }

        public Projectile Parent => Main.projectile[(int)Projectile.ai[0]];

        public ref float OrbitAngle => ref Projectile.ai[1];

        public int AI_Timer
        {
            get => (int)Projectile.ai[2];
            set => Projectile.ai[2] = value;
        }

        public enum ActionState { Orbit, Float, Home }
        public ActionState AI_State
        {
            get => (ActionState)Projectile.localAI[0];
            set => Projectile.localAI[0] = (float)value;
        }

        bool isTheFirstSpawned; // whether this projectile has control over the parent projectile

        float targetAngle; // the target angle when breaking apart the orbit
        float originalSpeed; // the speed at the start of the homing
        float turnSpeed; // the turn speed of the homing

        int floatDuration; // the free float duration
        int targetNPC; // the targeted NPC

        private NPC TargetNPC => Main.npc[targetNPC];

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(isTheFirstSpawned);

            writer.Write(targetAngle);
            writer.Write(originalSpeed);
            writer.Write(turnSpeed);

            writer.Write((byte)floatDuration);
            writer.Write((byte)targetNPC);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            isTheFirstSpawned = reader.ReadBoolean();
            targetAngle = reader.ReadSingle();
            originalSpeed = reader.ReadSingle();
            turnSpeed = reader.ReadSingle();

			floatDuration = reader.ReadByte();
            targetNPC = reader.ReadByte();
		}

        bool spawned = false;

        public override void AI()
        {
            if (!spawned)
            {
                targetAngle = (OrbitAngle * MathHelper.Pi / 180) - MathHelper.Pi / 2;
                isTheFirstSpawned = OrbitAngle == 0;
                spawned = true;
            }

            Lighting.AddLight(Projectile.Center, new Color(255, 146, 0).ToVector3());

            AI_Timer++;

            switch (AI_State)
            {
                case ActionState.Orbit:

                    if (!Parent.active)
                    {
                        Projectile.Kill();
                        return;
                    }

                    Vector2 vector = (Parent.Center - Projectile.Center).SafeNormalize(Vector2.UnitY);

                    Projectile.rotation = vector.ToRotation() - 1.57f;
                    OrbitAngle += 12f * Math.Sign(Parent.velocity.X);

                    float vX = 12 * MathF.Cos(OrbitAngle / 180 * MathHelper.Pi);
                    float vY = 12 * MathF.Sin(OrbitAngle / 180 * MathHelper.Pi);

                    Projectile.position = Parent.Center - Projectile.Size / 2f;

                    Projectile.velocity.X = vX;
                    Projectile.velocity.Y = vY;

                    if (AI_Timer >= 60)
                    {
                        if(Projectile.owner == Main.myPlayer)
                        {
                            AI_State = ActionState.Float;
                            AI_Timer = 0;

                            Projectile.velocity = Parent.velocity.RotatedBy(targetAngle) * 0.8f;
        
                            floatDuration = (ushort)Main.rand.Next(1, 12);
                            Projectile.netUpdate = true;

							// Schedule despawn of the parent projectile
							if (isTheFirstSpawned)
                            {
								Parent.timeLeft = 3;
                                Parent.netUpdate = true;
                            }
						}                   
                    }

                    break;

				case ActionState.Float:

					if (AI_Timer >= floatDuration)
                    {
                        if(Projectile.owner == Main.myPlayer)
                        {
                            AI_State = ActionState.Home;
							AI_Timer = 0;

							Projectile.penetrate = 1;
                            originalSpeed = Projectile.velocity.Length();

                            turnSpeed = Main.rand.NextFloat(0.01f, 0.1f);
                            Projectile.netUpdate = true;
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

                                if (targetNPC != npc.whoAmI)
                                    Projectile.netUpdate = true; // hmm, to test

                                targetNPC = npc.whoAmI;

                            }
                        }
                    }

                    // Adjust the projectile's velocity towards the target over time
                    if (TargetNPC is not null)
                    {
                        Vector2 direction = (TargetNPC.Center - Projectile.Center).SafeNormalize(Vector2.Zero);
                        Projectile.velocity = Vector2.Lerp(Projectile.velocity, direction * originalSpeed, turnSpeed);
                    }
                    break;
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = TextureAssets.Projectile[Type].Value;
			Texture2D glow = ModContent.Request<Texture2D>("Macrocosm/Assets/Textures/SimpleGlow").Value;
			Vector2 origin = Projectile.Size / 2f;

            ProjectileID.Sets.TrailCacheLength[Type] = 15;
            ProjectileID.Sets.TrailingMode[Type] = 3;

			//Projectile.DrawMagicPixelTrail(Vector2.Zero, 8, 1, Color.Orange, Color.Black.NewAlpha(0));

			var state = Main.spriteBatch.SaveState();
			Main.spriteBatch.End();
			Main.spriteBatch.Begin(BlendState.Additive, state);

			Main.EntitySpriteDraw(glow, Projectile.Center - Main.screenPosition, null, new Color(255, 101, 0), 0f, glow.Size() / 2, 0.05f * Projectile.scale, SpriteEffects.None, 0f);

			Main.spriteBatch.End();
			Main.spriteBatch.Begin(state);
			Projectile.GetTrail().Draw();

			Main.EntitySpriteDraw(tex, Projectile.Center - Main.screenPosition,
                null, Color.White, Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0);

            return false;
        }
    }

    public class DianiteForkCoreProjectile : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_0";

        public bool DoOnce
        {
            get => Projectile.ai[0] == 1f;
            set => Projectile.ai[0] = value ? 1f : 0f;
        }

        public override void SetStaticDefaults()
        {
        }

        public override void SetDefaults()
        {
            Projectile.width = 8;
            Projectile.height = 8;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
        }

        public override bool? CanCutTiles() => false;

        public override bool? CanHitNPC(NPC npc) => false;

        public override void AI()
        {
            if (Projectile.owner == Main.myPlayer && !DoOnce)
            {
                float centerX = Projectile.position.X;
                float centerY = Projectile.position.Y;
                int projCount = 2;
                for (int i = 0; i < projCount; i++)
                {
                    int proj = Projectile.NewProjectile(Projectile.GetSource_FromThis(), (int)centerX, (int)centerY, 0, 0, ModContent.ProjectileType<DianiteForkProjectile>(), (int)(Projectile.damage), Projectile.knockBack, Main.player[Projectile.owner].whoAmI, Projectile.whoAmI, (360f / (float)projCount) * i);
                    NetMessage.SendData(MessageID.SyncProjectile, -1, -1, null, proj);
                }

                DoOnce = true;
            }
        }

    }
}

