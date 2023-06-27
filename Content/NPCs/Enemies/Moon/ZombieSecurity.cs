using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Terraria.GameContent.Bestiary;
using Macrocosm.Common.Utils;
using Macrocosm.Content.Biomes;
using Macrocosm.Common.Drawing.Particles;
using Macrocosm.Content.Particles;
using System.IO;
using Macrocosm.Content.Projectiles.Hostile;
using Macrocosm.Common.DataStructures;
using Macrocosm.Content.NPCs.Global;
using Terraria.GameContent.ItemDropRules;
using Macrocosm.Content.Items.Weapons.Ranged;

namespace Macrocosm.Content.NPCs.Enemies.Moon
{
    public class ZombieSecurity : ModNPC, IMoonEnemy
	{
		public enum ActionState 
		{ 
			Walk,
			Shoot 
		}

		public ActionState AI_State = ActionState.Walk;

		public enum AimType { Horizontal, Upwards, Downwards }
		public AimType AimAngle;

		public int ShootCooldown = maxCooldown;
		public int ShootSequence = 0;

		public int AttackCooldown = maxCooldown;

		public int ShotsCounter = 1;
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

		public override void ModifyNPCLoot(NPCLoot loot)
		{
			// drop gun, 1/12 normal mode, twice in expert
			loot.Add(ItemDropRule.ExpertGetsRerolls(ModContent.ItemType<Tycho50>(), 12, 1)); 
		}

		public override void OnSpawn(IEntitySource source)
		{
			NPC.frame.Y = NPC.GetFrameHeight() * walkFrames.Start;

			if(Main.netMode != NetmodeID.MultiplayerClient)
			{
				MaxConsecutiveShots = Main.rand.Next(2, 5);
				NPC.netUpdate = true;
			}
		}

		#region Netcode

		// Required until we fix the NetSyncAttribute thing :/
		// The NPC.ai[] array is used by BaseMod ZombieAI()
		public override void SendExtraAI(BinaryWriter writer)
		{
			writer.Write((byte)AI_State);
			writer.Write((byte)AimAngle);

			writer.Write((ushort)ShootCooldown);
			writer.Write((ushort)ShootSequence);

			writer.Write((byte)ShotsCounter);
			writer.Write((byte)MaxConsecutiveShots);
		}

		public override void ReceiveExtraAI(BinaryReader reader)
		{
			AI_State = (ActionState)reader.ReadByte();
			AimAngle = (AimType)reader.ReadByte();

			ShootCooldown = reader.ReadUInt16();
			ShootSequence = reader.ReadUInt16();

			ShotsCounter = reader.ReadByte();
			MaxConsecutiveShots = reader.ReadByte();
		}

		#endregion

		public override void AI()
		{
			//Main.NewText("Shoot sequence: " + ShootSequence + "/" + maxShootSequence);
			//Main.NewText("Shoot cooldown: " + ShootCooldown + "/" + maxCooldown);
			//Main.NewText("Attack cooldown: " + AttackCooldown + "/" + maxCooldown);
			//Main.NewText("AI State: " + AI_State.ToString());
			//Main.NewText("Frame Index: " + NPC.frame.Y / NPC.GetFrameHeight() + "/" + fallingFrame);
			//Main.NewText("Frame cnt: " + NPC.frameCounter);
			//Main.NewText("Shots: " + ShotsCounter + "/" + MaxConsecutiveShots);
			//Main.NewText("\n");

			NPC.spriteDirection = -NPC.direction;

			if (NPC.justHit)
			{
				ShootCooldown = maxCooldown / 4;
				ShootSequence = 0;
				ShotsCounter = 1;

				if (AI_State == ActionState.Shoot)
					NPC.frame.Y = NPC.GetFrameHeight() * shootFramesCommon.Start;

				AI_State = ActionState.Walk;
			}
 
			if (AI_State == ActionState.Walk)
				AI_Walk();
			else
				AI_Shoot();
		}

		bool CanHit => Collision.CanHit(NPC, TargetPlayer) && Vector2.Distance(NPC.Center, TargetPlayer.Center) > 120f;

		private void AI_Walk()
		{
			// Increase gravity a bit
			if (NPC.velocity.Y < 0f)
				NPC.velocity.Y += 0.1f;

			// Base Fighter AI
			Utility.AIZombie(NPC, ref NPC.ai, false, true, 0);

			ShootCooldown--;
			AttackCooldown--;

			if (AttackCooldown <= 0 && Vector2.Distance(NPC.Center, TargetPlayer.Center) < 120f)
			{
				NPC.velocity.X += (TargetPlayer.Center.X - NPC.Center.X) * 0.04f;
				NPC.velocity.Y -= 2f;
				AttackCooldown = maxCooldown;
			}

			if (NPC.velocity.Y != 0f && Math.Abs(NPC.velocity.X) > 2f)
			{
				NPC.direction = Math.Sign(NPC.velocity.X);
				NPC.rotation = NPC.velocity.X * 0.08f;
			}
			else
				NPC.rotation = 0f;
 			
			// Enter shoot state if cooldown passed, clear line of sight, enemy is stationary (vertically?)
			if (ShootCooldown <= 0 && CanHit && NPC.velocity.Y == 0f)
			{
				// Reset cooldown
				ShootCooldown = maxCooldown; 

				// Randomize shoot count
				if(Main.netMode != NetmodeID.MultiplayerClient)
				{
					MaxConsecutiveShots = Main.rand.Next(2, 5);
					NPC.netUpdate = true;
				}

				// Set up the first weapon draw frame (did not find a way to do it right in FindFrame)
				NPC.frame.Y = NPC.GetFrameHeight() * shootFramesCommon.Start;

				AI_State = ActionState.Shoot;
			}
		}

		private void AI_Shoot()
		{
			if (!CanHit)
				NPC.TargetClosest();

			// Exit state if line of sight is broken or shoot sequence succeeded
			if (!(NPC.HasPlayerTarget && CanHit) || (ShotsCounter >= MaxConsecutiveShots && ShootSequence >= maxShootSequence))
			{
				if ((ShotsCounter >= MaxConsecutiveShots && ShootSequence >= maxShootSequence))
					ShootCooldown = maxCooldown;
				else
					ShootCooldown = maxCooldown / 4;

				ShotsCounter = 1;
				ShootSequence = 0;

				AI_State = ActionState.Walk;
				return;
			}

			ShootSequence++;

			if (ShotsCounter < MaxConsecutiveShots && ShootSequence >= maxShootSequence)
			{
				ShotsCounter++;
				ShootSequence = visualGunDrawEnd;
			}

			// Decelerate horizontal movement
			NPC.velocity.X *= 0.5f;

			NPC.rotation = 0f;

			// Orient the NPC towards the player 
			NPC.direction = Main.player[NPC.target].position.X < NPC.position.X ? -1 : 1;

			int projDamage = 100;
			int projType = ModContent.ProjectileType<ZombieSecurityBullet>();
			float projSpeed = 120;

			// Aim
			Vector2 aimPosition = GetAimPosition(projSpeed);
			Vector2 aimVelocity = GetAimVelocity(aimPosition, projSpeed);

			// Set the aim angle 
			SetAimMode(aimVelocity);

			// Shoot (TODO: align projectile and particle by angle)
			if (Main.netMode != NetmodeID.MultiplayerClient && ShootSequence == sequenceShoot)
			{
				Vector2 particleAim = aimVelocity;
				if(AimAngle == AimType.Upwards)
 					particleAim = aimVelocity.RotatedBy(-0.4f * Math.Sign(aimVelocity.X));
 				else if(AimAngle == AimType.Downwards)
 					particleAim = aimVelocity.RotatedBy(-0.2f * Math.Sign(aimVelocity.X));

				Projectile bullet = Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), aimPosition, aimVelocity, projType, projDamage, 0f, Main.myPlayer);
				var flash = Particle.CreateParticle<GunFireRing>(NPC.Center + particleAim * 0.24f, aimVelocity * 0.05f, 1f, aimVelocity.ToRotation(), true);
				flash.Owner = bullet;
			}

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
					if(ShootSequence > visualShoot)
						NPC.frameCounter += 0.2f;

					// Update frame 
					if (NPC.frameCounter > 6.0)
					{
						// After weapon draw animation, sitck to an aim frame, based on the aim angle
						if (ShootSequence >= visualGunDrawEnd && ShootSequence <= visualShoot)
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
