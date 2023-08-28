using Macrocosm.Common.Utils;
using Macrocosm.Content.Dusts;
using Macrocosm.Content.Trails;
using Microsoft.CodeAnalysis;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Projectiles.Friendly.Magic
{
    // Parent projectile class below
    internal class DianiteForkProjectile : ModProjectile
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

            AI_Timer++;

            // Keep alive unless metting specific conditions
			Projectile.timeLeft++;

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
                    OrbitAngle += 12 * Math.Sign(Parent.velocity.X);

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
								Parent.timeLeft = 1;
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
					Dust dust = Dust.NewDustDirect(Projectile.Center, 1, 1, ModContent.DustType<DianiteBrightDust>(), -Parent.velocity.X * 0.4f, -Parent.velocity.Y * 0.4f);
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


		public override void Kill(int timeLeft)
		{
            if(AI_State is ActionState.Orbit && Parent is not null && Parent.ModProjectile is DianiteForkCoreProjectile)
                (Parent.ModProjectile as DianiteForkCoreProjectile).BrokenApartEarly = true;

			for (int i = 0; i < 20; i++)
            {
                Vector2 velocity = Main.rand.NextVector2Circular(4, 4);
                Vector2 extraPosition = AI_State is ActionState.Orbit ? Parent.oldVelocity : Projectile.oldVelocity;
				Dust dust = Dust.NewDustPerfect(Projectile.position + extraPosition * 2f, ModContent.DustType<DianiteBrightDust>(), velocity, Scale: Main.rand.NextFloat(1f, 1.6f));
			    dust.noGravity = true;
            }
		}

		public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = TextureAssets.Projectile[Type].Value;
			Texture2D glow = ModContent.Request<Texture2D>(Macrocosm.TextureAssetsPath + "SimpleGlow").Value;
			Vector2 origin = Projectile.Size / 2f;
;
			var state = Main.spriteBatch.SaveState();
			Main.spriteBatch.End();
			Main.spriteBatch.Begin(BlendState.Additive, state);

			Projectile.GetTrail().Draw(tex.Size()/2);
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

    internal class DianiteForkCoreProjectile : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_0";

        public bool DoOnce
        {
            get => Projectile.ai[0] == 1f;
            set => Projectile.ai[0] = value ? 1f : 0f;
        }

		public bool BrokenApartEarly
		{
			get => Projectile.ai[1] == 1f;
			set => Projectile.ai[1] = value ? 1f : 0f;
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
            Projectile.tileCollide = true;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 61;
        }

        public override bool? CanCutTiles() => false;

        public override bool? CanHitNPC(NPC npc) => false;

        public override void AI()
        {
            if (Projectile.owner == Main.myPlayer && !DoOnce)
            {
                int projCount = 2;

				for (int i = 0; i < projCount; i++)
                {
                    Projectile proj = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<DianiteForkProjectile>(), (int)(Projectile.damage), Projectile.knockBack, Main.player[Projectile.owner].whoAmI, Projectile.whoAmI, (360f / (float)projCount) * i);
					NetMessage.SendData(MessageID.SyncProjectile, -1, -1, null, proj.whoAmI);
                }

                DoOnce = true;
            }
        }

		public override void Kill(int timeLeft)
		{
            if (BrokenApartEarly)
                return;

			for (int i = 0; i < 35; i++)
			{
				Vector2 velocity = Main.rand.NextVector2Circular(16, 2f).RotatedBy(Projectile.velocity.ToRotation());
				Dust dust = Dust.NewDustPerfect(Projectile.position, ModContent.DustType<DianiteBrightDust>(), velocity, Scale: Main.rand.NextFloat(1f, 1.4f));
				dust.noGravity = true;
			}
		}

	}
}

