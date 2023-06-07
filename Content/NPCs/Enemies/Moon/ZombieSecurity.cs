using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Terraria.GameContent.Bestiary;
using Macrocosm.Common;
using Macrocosm.Common.Utils;
using Macrocosm.Common.Netcode;
using Macrocosm.Content.Biomes;
using Macrocosm.Common.Drawing.Particles;
using Macrocosm.Content.Particles;
using System.IO;
using Macrocosm.Content.Projectiles.Friendly.Ranged;
using Macrocosm.Content.Projectiles.Hostile;

namespace Macrocosm.Content.NPCs.Enemies.Moon
{
	public class ZombieSecurity : MoonEnemy
	{
		public enum ActionState 
		{ 
			Walk,
			Shoot 
		}

		public ActionState AI_State = ActionState.Walk;

		public enum AimType { Horizontal, Upwards, Downwards }
		public AimType AimAngle;

		public int ShootCooldownCounter = maxCooldown;
		public int ShootSequenceCounter = 0;

		public int ConsecutiveShotsCounter = 1;

		public int MaxConsecutiveShots = 3;

		public Player TargetPlayer => Main.player[NPC.target];

		#region Privates 
		private readonly IntRange shootFramesCommon = new(0, 5);
		private readonly IntRange shootFramesHorizontal = new(6, 10);
		private readonly IntRange shootFramesUpwards = new(11, 15);
		private readonly IntRange shootFramesDownward = new(16, 20);
		private readonly IntRange walkFrames = new(21, 30);
		private const int fallingFrame = 31;

		private static int maxCooldown = 240;
		private static int maxShootSequence = 84;
		private static int sequenceShoot = 72;
		private static int visualShoot = 64;
		private static int visualGunDrawEnd = 24;
		#endregion

		public override void SetStaticDefaults()
		{
			Main.npcFrameCount[Type] = 32;
		}

		public override void SetDefaults()
		{
			NPC.width = 24;
			NPC.height = 44;
			NPC.damage = 60;
			NPC.defense = 60;
			NPC.lifeMax = 2200;
			NPC.HitSound = SoundID.NPCHit1;
			NPC.DeathSound = SoundID.NPCDeath2;
			NPC.knockBackResist = 0.5f;
			NPC.aiStyle = -1;
 			Banner = Item.NPCtoBanner(NPCID.Zombie);
			BannerItem = Item.BannerToItem(Banner);
			SpawnModBiomes = new int[1] { ModContent.GetInstance<MoonBiome>().Type }; // Associates this NPC with the Moon Biome in Bestiary
		}

		public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
		{
			bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[]
			{
				new FlavorTextBestiaryInfoElement(
					"")
			});
		}

		public override float SpawnChance(NPCSpawnInfo spawnInfo)
		{
			return spawnInfo.Player.Macrocosm().ZoneMoon && !Main.dayTime ? .08f : 0f;
		}

		public override void OnSpawn(IEntitySource source)
		{
			NPC.frame.Y = NPC.GetFrameHeight() * walkFrames.Start;

			if(Main.netMode != NetmodeID.MultiplayerClient)
				MaxConsecutiveShots = Main.rand.Next(2, 5);
		}

		#region Netcode

		// Required until we fix the NetSyncAttribute thing :/
		// The NPC.ai[] array is used by BaseMod ZombieAI()
		public override void SendExtraAI(BinaryWriter writer)
		{
			writer.Write((byte)AI_State);
			writer.Write((byte)AimAngle);

			writer.Write((ushort)ShootCooldownCounter);
			writer.Write((ushort)ShootSequenceCounter);

			writer.Write((byte)ConsecutiveShotsCounter);
			writer.Write((byte)MaxConsecutiveShots);
		}

		public override void ReceiveExtraAI(BinaryReader reader)
		{
			AI_State = (ActionState)reader.ReadByte();
			AimAngle = (AimType)reader.ReadByte();

			ShootCooldownCounter = reader.ReadUInt16();
			ShootSequenceCounter = reader.ReadUInt16();

			ShootSequenceCounter = reader.ReadByte();
			ShootSequenceCounter = reader.ReadByte();
		}

		#endregion

		public override void AI()
		{
			//Main.NewText("Shoot sequence: " + ShootSequenceCounter + "/" + maxShootSequence);
			//Main.NewText("Shoot cooldown: " + ShootCooldownCounter + "/" + maxCooldown);
			//Main.NewText("AI State: " + AI_State.ToString());
			//Main.NewText("Frame Index: " + NPC.frame.Y / NPC.GetFrameHeight() + "/" + fallingFrame);
			//Main.NewText("Frame cnt: " + NPC.frameCounter);
			//Main.NewText("Shots: " + ConsecutiveShotsCounter + "/" + MaxConsecutiveShots);
			//Main.NewText("\n\n");

			NPC.spriteDirection = -NPC.direction;

			if (NPC.justHit)
			{
				ShootCooldownCounter = maxCooldown / 4;
				ShootSequenceCounter = 0;
				AI_State = ActionState.Walk;
				ConsecutiveShotsCounter = 1;

				if (AI_State == ActionState.Shoot)
					NPC.frame.Y = NPC.GetFrameHeight() * shootFramesCommon.Start;
			}
 
			if (AI_State == ActionState.Walk)
				AI_Walk();
			else
				AI_Shoot();
		}

		private void AI_Walk()
		{
			// Increase gravity a bit
			if (NPC.velocity.Y < 0f)
				NPC.velocity.Y += 0.1f;

			// Base Fighter AI
			Utility.AIZombie(NPC, ref NPC.ai, false, true, 0);

			ShootCooldownCounter--;

			// Enter shoot state if cooldown passed, clear line of sight, enemy is stationary (vertically?)
			if (ShootCooldownCounter <= 0 && Collision.CanHit(NPC, TargetPlayer) && NPC.velocity.Y == 0f)
			{
				// Reset cooldown
				ShootCooldownCounter = maxCooldown; 

				// Randomize shoot count
				if(Main.netMode != NetmodeID.MultiplayerClient)
					MaxConsecutiveShots = Main.rand.Next(2, 5);

				// Set up the first weapon draw frame (did not find a way to do it right in FindFrame)
				NPC.frame.Y = NPC.GetFrameHeight() * shootFramesCommon.Start;

				AI_State = ActionState.Shoot;
			}
		}

		private void AI_Shoot()
		{
			// Exit state if line of sight is broken or shoot sequence succeeded
			if (!Collision.CanHit(NPC, TargetPlayer) || (ConsecutiveShotsCounter >= MaxConsecutiveShots && ShootSequenceCounter >= maxShootSequence))
			{
				ConsecutiveShotsCounter = 1;
				ShootSequenceCounter = 0;
				AI_State = ActionState.Walk;
				return;
			}

			ShootSequenceCounter++;

			if (ConsecutiveShotsCounter < MaxConsecutiveShots && ShootSequenceCounter >= maxShootSequence)
			{
				ConsecutiveShotsCounter++;
				ShootSequenceCounter = visualGunDrawEnd;
			}

			// Decelerate horizontal movement
			NPC.velocity.X *= 0.5f;

			// Orient the NPC towards the player 
			NPC.direction = Main.player[NPC.target].position.X < NPC.position.X ? -1 : 1;

			int projDamage = 100;
			int projType = ModContent.ProjectileType<ZombieSecurityBullet>();
			float projSpeed = 120;

			// Aim
			Vector2 aimPosition = GetAimPosition(projSpeed);
			Vector2 aimVelocity = GetAimVelocity(aimPosition, projSpeed);

			// Shoot (TODO: align projectile and particle by angle)
			if (Main.netMode != NetmodeID.MultiplayerClient && ShootSequenceCounter == sequenceShoot)
			{
				Projectile.NewProjectile(NPC.GetSource_FromAI(), aimPosition.X, aimPosition.Y, aimVelocity.X, aimVelocity.Y, projType, projDamage, 0f, Main.myPlayer);
				Particle.CreateParticle<DesertEagleFlash>(NPC.Center + aimVelocity * 0.24f, aimVelocity * 0.05f, aimVelocity.ToRotation(), 1f, true);
				//NPC.velocity.X = 4f * -NPC.direction;
			}

			// Set the aim angle 
			SetAimMode(aimVelocity);
		}

		private Vector2 GetAimPosition(float projSpeed)
		{
			Vector2 aimPosition = new(NPC.position.X + NPC.width * 0.5f, NPC.position.Y + NPC.height * 0.5f);

			float aimSpeedX = TargetPlayer.position.X + TargetPlayer.width * 0.5f - aimPosition.X;
			float aimSpeedY = TargetPlayer.position.Y + TargetPlayer.height * 0.5f - aimPosition.Y;

			float aimLength = (float)Math.Sqrt(aimSpeedX * aimSpeedX + aimSpeedY * aimSpeedY);
			aimLength = projSpeed / aimLength;
			aimSpeedX *= aimLength;
			aimSpeedY *= aimLength;

			aimPosition.X += aimSpeedX;
			aimPosition.Y += aimSpeedY;

			return aimPosition;
		}

		private Vector2 GetAimVelocity(Vector2 aimPosition, float projSpeed)
		{
			Vector2 aimVelocity = new(
				TargetPlayer.position.X + TargetPlayer.width * 0.5f - aimPosition.X,
				TargetPlayer.position.Y + TargetPlayer.height * 0.5f - aimPosition.Y
			);

			float aimLength = (float)Math.Sqrt(aimVelocity.X * aimVelocity.X + aimVelocity.Y * aimVelocity.Y);
			aimLength = projSpeed / aimLength;
			aimVelocity *= aimLength;

			return aimVelocity;
		}

		private void SetAimMode(Vector2 aimVelocity)
		{
			if (Math.Abs(aimVelocity.X) > Math.Abs(aimVelocity.Y) * 2f)
				AimAngle = AimType.Horizontal;
			else if (aimVelocity.Y > 0f)
				AimAngle = AimType.Downwards;
			else
				AimAngle = AimType.Upwards;
		}

		// Animation. 
		public override void FindFrame(int frameHeight)
		{
			int frameIndex = NPC.frame.Y / frameHeight;

			// If not airborne
			if (NPC.velocity.Y == 0f)
			{
				// Walking animation 
				if(AI_State == ActionState.Walk)
				{
					// Reset walking 
					if(!walkFrames.Contains(frameIndex))
						NPC.frame.Y = frameHeight * walkFrames.Start;

					// Walking animation frame counter, accounting for walk speed
					NPC.frameCounter += Math.Abs(NPC.velocity.X) * 2f;

					// Update frame
					if (NPC.frameCounter > 6.0)
					{
						NPC.frame.Y += frameHeight;
						NPC.frameCounter = 0.0;
					}

					if (frameIndex >= walkFrames.End)
 						NPC.frame.Y = frameHeight * walkFrames.Start;
 				}
				// Shooting animation
				else if(AI_State == ActionState.Shoot)
				{
					NPC.frameCounter += 1f;

					// Speed up animation for recoil animation  
					if(ShootSequenceCounter > visualShoot)
						NPC.frameCounter += 0.2f;

					// Update frame 
					if (NPC.frameCounter > 6.0)
					{
						// After weapon draw animation, sitck to an aim frame, based on the aim angle
						if (ShootSequenceCounter >= visualGunDrawEnd && ShootSequenceCounter <= visualShoot)
						{
							switch (AimAngle)
							{
								case AimType.Horizontal:
									NPC.frame.Y = frameHeight * shootFramesHorizontal.Start;
									break;

								case AimType.Upwards:
									NPC.frame.Y = frameHeight * shootFramesUpwards.Start;
									break;

								case AimType.Downwards:
									NPC.frame.Y = frameHeight * shootFramesDownward.Start;
									break;
							}
						}
						else // Update the frame
						{
							NPC.frame.Y += frameHeight;
						}

						NPC.frameCounter = 0.0;
					}
 				}
 			}
			// Air-borne frame
			else if(MathF.Abs(NPC.velocity.Y) > 1f)
			{
				NPC.frameCounter = 0.0;
				NPC.frame.Y = frameHeight * fallingFrame;
			}
		}

		public override void ModifyNPCLoot(NPCLoot loot)
		{
		}

		public override void HitEffect(NPC.HitInfo hit)
		{
			if (NPC.life > 0)
			{
				for (int i = 0; i < 30; i++)
				{
					int dustType = Utils.SelectRandom<int>(Main.rand, DustID.TintableDust, DustID.Blood);

 					Dust dust = Dust.NewDustDirect(NPC.position, NPC.width, NPC.height, dustType);
					dust.velocity.X *= (dust.velocity.X + + Main.rand.Next(0, 100) * 0.015f) * hit.HitDirection;
					dust.velocity.Y =  3f + Main.rand.Next(-50, 51) * 0.01f  ;
					dust.scale *= 1f + Main.rand.Next(-30, 31) * 0.01f;
					dust.noGravity = true;
				}
			}

			// Don't spawn gores on dedicated server
			if (Main.dedServ)
 				return; 
 
			if (NPC.life <= 0)
			{
				var entitySource = NPC.GetSource_Death();

				Gore.NewGore(entitySource, NPC.position, NPC.velocity, Mod.Find<ModGore>("ZombieSecurityHead").Type);
				Gore.NewGore(entitySource, NPC.position, NPC.velocity, Mod.Find<ModGore>("ZombieSecurityArm").Type);
				Gore.NewGore(entitySource, NPC.position, NPC.velocity, Mod.Find<ModGore>("ZombieSecurityArm").Type);
				Gore.NewGore(entitySource, NPC.position, NPC.velocity, Mod.Find<ModGore>("ZombieSecurityLeg1").Type);
				Gore.NewGore(entitySource, NPC.position, NPC.velocity, Mod.Find<ModGore>("ZombieSecurityLeg2").Type);
			}
		}
	}
}
