using Macrocosm.Common.Utility;
using Macrocosm.Content.Dusts;
using Macrocosm.Content.Projectiles.Unfriendly;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.NPCs.Unfriendly.Bosses.Moon{
	[AutoloadBossHead]
	public class CraterDemon : ModNPC{
		private struct AttackInfo{
			public Func<CraterDemon, int> initialProgress;
			public int initialTimer;
			public bool resetAnimationCounter;
		}

		private struct BigPortalInfo{
			public Vector2 center;
			public float scale;
			public float alpha;
			public bool visible;
			public float rotation;
			public bool fast;

			public void Write(BinaryWriter writer){
				writer.WriteVector2(center);
				writer.Write(scale);
				writer.Write(alpha);
				writer.Write(rotation);
				
				BitsByte bb = new BitsByte(visible, fast);
				writer.Write(bb);
			}

			public void Read(BinaryReader reader){
				center = reader.ReadVector2();
				scale = reader.ReadSingle();
				alpha = reader.ReadSingle();
				rotation = reader.ReadSingle();

				BitsByte bb = reader.ReadByte();
				bb.Retrieve(ref visible, ref fast);
			}

			public void Update(){
				if(!visible)
					return;

				float rotationsPerSecond = fast ? 3.1f : 1.8f;
				rotation -= MathHelper.ToRadians(rotationsPerSecond * 6f);
			}
		}

		public ref float AI_Attack => ref npc.ai[0];
		public ref float AI_Timer => ref npc.ai[1];
		public ref float AI_AttackProgress => ref npc.ai[2];
		public ref float AI_AnimationCounter => ref npc.ai[3];

		public const int FadeIn                    = -2;
		public const int FadeAway                  = -1;
		public const int Attack_DoNothing          = 0;
		public const int Attack_SummonMeteors      = 1;
		public const int Attack_SummonLesserDemons = 2;
		public const int Attack_ChargeAtPlayer     = 3;
		public const int Attack_PostCharge         = 4;

		private const int Attack_ChargeAtPlayer_RepeatStart         = 2;
		private const int Attack_ChargeAtPlayer_RepeatSubphaseCount = 5;

		private static readonly AttackInfo[] attacks = new AttackInfo[]{
			new AttackInfo(){
				initialProgress = null,
				initialTimer = 90,
				resetAnimationCounter = false
			},
			new AttackInfo(){
				initialProgress = null,
				initialTimer = PortalTimerMax,
				resetAnimationCounter = true
			},
			new AttackInfo(){
				initialProgress = npc => npc.CountAliveLesserDemons(),
				initialTimer = 8 * 60
			},
			new AttackInfo(){
				initialProgress = null,
				initialTimer = 70  //Wait before spawning first portal
			},
			new AttackInfo(){
				initialProgress = null,
				initialTimer = 45
			}
		};

		private void SetAttack(int attack){
			var info = attacks[attack];

			AI_Attack = attack;

			AI_AttackProgress = info.initialProgress?.Invoke(this) ?? 0;

			AI_Timer = info.initialTimer;

			if(!Main.expertMode)
				AI_Timer *= 1.5f;

			if(info.resetAnimationCounter)
				AI_AnimationCounter = -1;

			if(attack == Attack_DoNothing){
				movementTarget = null;
				zAxisLerpStrength = DefaultZAxisLerpStrength;
			}
		}

		private bool spawned = false;
		private float targetAlpha = 255f;
		private bool ignoreRetargetPlayer = false;
		private float targetZAxisRotation = 0f;
		private float zAxisRotation = 0f;
		private bool hadNoPlayerTargetForLongEnough;
		private bool hideMapIcon;
		private const float DefaultZAxisLerpStrength = 2.3f;
		private float zAxisLerpStrength = DefaultZAxisLerpStrength;
		private const float DefaultInertia = 20f;
		private float inertia = DefaultInertia;
		private int oldAttack;

		private Vector2? movementTarget;

		private BigPortalInfo bigPortal;
		private BigPortalInfo bigPortal2;

		public const int PortalTimerMax = (int)(4f * 60 + 1.5f * 60 + 24);  //Portal spawning leadup + time portals are active before they shrink

		public override void SetStaticDefaults(){
			DisplayName.SetDefault("Crater Demon");
			Main.npcFrameCount[npc.type] = 6;
			NPCID.Sets.TrailCacheLength[npc.type] = 5;
			NPCID.Sets.TrailingMode[npc.type] = 0;
		}

		private int baseWidth, baseHeight;

		public override void SetDefaults(){
			baseWidth = npc.width = 178;
			baseHeight = npc.height = 196;
			npc.knockBackResist = 0f;

			npc.defense = 100;
			npc.damage = 120;
			npc.lifeMax = 110000;  //For comparison, Moon Lord has 145,000 total HP in Normal Mode

			npc.boss = true;
			npc.noGravity = true;
			npc.noTileCollide = true;

			targetAlpha = npc.alpha = 255;
			npc.dontTakeDamage = true;
			npc.aiStyle = -1;

			npc.npcSlots = 40f;

			music = MusicID.Boss1;

			npc.HitSound = SoundID.NPCHit2;
		}

		public override void ScaleExpertStats(int numPlayers, float bossLifeScale){
			npc.ScaleHealthBy(0.35f);  //For comparison, Moon Lord's scale factor is 0.7f

			npc.damage = 180;
			npc.defense = 150;
		}

		public override void BossLoot(ref string name, ref int potionType){
			potionType = ItemID.GreaterHealingPotion;
		}

		public override void OnHitPlayer(Player target, int damage, bool crit){
			target.GetModPlayer<MacrocosmPlayer>().accMoonArmorDebuff = Main.expertMode ? 5 * 60 : 2 * 60;
		}

		private void UpdateScale(float newScale){
			if(npc.scale == newScale)
				return;

			npc.UpdateScaleAndHitbox(baseWidth, baseHeight, newScale);
		}

		const float ZAxisRotationThreshold = 25f;

		public override void SendExtraAI(BinaryWriter writer){
			bool hasCustomTarget = movementTarget != null;
			BitsByte bb = new BitsByte(spawned, ignoreRetargetPlayer, hadNoPlayerTargetForLongEnough, hasCustomTarget);
			writer.Write(bb);

			writer.Write(targetAlpha);
			writer.Write(targetZAxisRotation);
			writer.Write(zAxisRotation);
			
			bigPortal.Write(writer);
			bigPortal2.Write(writer);

			if(hasCustomTarget)
				writer.WriteVector2(movementTarget.Value);

			writer.Write(oldAttack);
		}

		public override void ReceiveExtraAI(BinaryReader reader){
			bool hasCustomTarget = false;
			BitsByte bb = reader.ReadByte();
			bb.Retrieve(ref spawned, ref ignoreRetargetPlayer, ref hadNoPlayerTargetForLongEnough, ref hasCustomTarget);

			targetAlpha = reader.ReadSingle();
			targetZAxisRotation = reader.ReadSingle();
			zAxisRotation = reader.ReadSingle();
			
			bigPortal.Read(reader);
			bigPortal2.Read(reader);

			movementTarget = hasCustomTarget ? (Vector2?)reader.ReadVector2() : null;

			oldAttack = reader.ReadInt32();
		}

		public override void BossHeadSlot(ref int index){
			if(hideMapIcon)
				index = -1;
		}

		public override bool PreDraw(SpriteBatch spriteBatch, Color drawColor){
			//Draw portals
			Texture2D portal = mod.GetTexture("Content/NPCs/Unfriendly/Bosses/Moon/BigPortal");

			DrawBigPortal(spriteBatch, portal, bigPortal);
			DrawBigPortal(spriteBatch, portal, bigPortal2);

			if(AI_Attack == Attack_SummonMeteors && Math.Abs(npc.velocity.X) < 0.1f && Math.Abs(npc.velocity.Y) < 0.1f)
				return true;

			if(AI_Attack == Attack_ChargeAtPlayer && AI_AttackProgress > 2 && npc.alpha >= 160)
				return true;
			
			for(int i = 0; i < npc.oldPos.Length; i++){
				Vector2 drawPos = npc.oldPos[i] - Main.screenPosition + new Vector2(0, 4);

				Color color = npc.GetAlpha(drawColor) * (((float)npc.oldPos.Length - i) / npc.oldPos.Length);

				spriteBatch.Draw(Main.npcTexture[npc.type], drawPos, npc.frame, color * 0.6f, npc.rotation, Vector2.Zero, npc.scale, SpriteEffects.None, 0f);
			}

			return true;
		}

		private void DrawBigPortal(SpriteBatch spriteBatch, Texture2D texture, BigPortalInfo info){
			if(!info.visible)
				return;

			spriteBatch.Draw(texture, info.center - Main.screenPosition, null, Color.White * info.alpha, info.rotation, texture.Size() / 2f, info.scale, SpriteEffects.None, 0);
		}

		private int GetAnimationSetFrame(){
			float rad = MathHelper.ToRadians(ZAxisRotationThreshold);
			int set = zAxisRotation < -rad
				? Animation_LookLeft_JawClosed
				: (zAxisRotation < rad
					? Animation_LookFront_JawClosed / 2
					: Animation_LookRight_JawClosed / 2);

			//Set sets -> frame
			set *= 2;

			switch((int)AI_Attack){
				case FadeIn:
					if(AI_Timer > 0 && AI_Timer <= 100 && AI_AnimationCounter % 16 < 8)
						set++;
					break;
				case Attack_DoNothing:
				case Attack_SummonLesserDemons:
					if(AI_AnimationCounter % 26 < 13)
						set++;
					break;
				case Attack_SummonMeteors:
					//Open mouth
					set++;
					break;
				case Attack_ChargeAtPlayer:
					if(AI_AttackProgress > Attack_ChargeAtPlayer_RepeatStart){
						int repeatRelative = ((int)AI_AttackProgress - Attack_ChargeAtPlayer_RepeatStart) % Attack_ChargeAtPlayer_RepeatSubphaseCount;
						int repeatCount = ((int)AI_AttackProgress - Attack_ChargeAtPlayer_RepeatStart) / Attack_ChargeAtPlayer_RepeatSubphaseCount;

						const float portalTargetDist = 4 * 16;

						if(repeatCount == 0 && repeatRelative == 0){
							if(AI_AnimationCounter % 26 < 13)
								set++;
						}else if(repeatRelative == 4 && npc.DistanceSQ(bigPortal2.center) >= portalTargetDist * portalTargetDist)
							set++;
					}else if(AI_AnimationCounter % 26 < 13)
						set++;
					break;
				case Attack_PostCharge:
					if(npc.velocity != Vector2.Zero)
						set++;
					break;
			}

			return set;
		}

		public const int Animation_LookLeft_JawClosed  = 0;
		public const int Animation_LookLeft_JawOpen    = 1;
		public const int Animation_LookFront_JawClosed = 2;
		public const int Animation_LookFront_JawOpen   = 3;
		public const int Animation_LookRight_JawClosed = 4;
		public const int Animation_LookRight_JawOpen   = 5;

		public override void FindFrame(int frameHeight){
			int set = GetAnimationSetFrame();

			npc.frame.Y = frameHeight * set;
		}

		public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Vector2 position){
			scale = 1.5f;

			return hideMapIcon ? (bool?)false : null;
		}

		public override void AI(){
			npc.defense = npc.defDefense;

			if(!spawned){
				//Laugh after a second
				if(AI_Timer == 0){
					targetZAxisRotation = 0f;
					zAxisRotation = 0f;
					targetAlpha = 255f;

					ignoreRetargetPlayer = true;

					AI_Timer = 60 + 100;
					AI_Attack = FadeIn;
					npc.TargetClosest();

					npc.netUpdate = true;
				}else if(AI_Timer <= 100){
					spawned = true;
					targetAlpha = 0f;

					Main.PlaySound(SoundID.Zombie, npc.Center, 105);  //Cultist laugh sound

					npc.netUpdate = true;
				}else
					targetAlpha -= 255f / 60f;
			}

			//Player is dead/not connected?  Target a new one
			//That player is also dead/not connected?  Begin the "fade away" animation and despawn
			Player player = npc.target >= 0 && npc.target < Main.maxPlayers ? Main.player[npc.target] : null;

			if(!ignoreRetargetPlayer && (npc.target < 0 || npc.target >= Main.maxPlayers || player.dead || !player.active)){
				npc.TargetClosest();

				player = npc.target >= 0 && npc.target < Main.maxPlayers ? Main.player[npc.target] : null;

				if(npc.target < 0 || npc.target >= Main.maxPlayers || player.dead || !player.active){
					//Go away
					AI_Attack = FadeAway;
					AI_Timer = -1;
					AI_AttackProgress = -1;

					hadNoPlayerTargetForLongEnough = true;

					npc.netUpdate = true;
				}else if(hadNoPlayerTargetForLongEnough){
					hadNoPlayerTargetForLongEnough = false;

					//Start back in the idle phase
					SetAttack(Attack_DoNothing);

					npc.netUpdate = true;
				}
			}

			npc.defense = npc.defDefense;

			switch((int)AI_Attack){
				case FadeIn:
					//Do the laughing animation
					if(AI_Timer > 0 && AI_Timer <= 100){
						npc.velocity *= 1f - 3f / 60f;
					}else if(AI_Timer <= 0){
						//Transition to the next subphase
						SetAttack(Attack_DoNothing);

						npc.dontTakeDamage = false;
						ignoreRetargetPlayer = false;
					}else{
						npc.velocity = new Vector2(0, 1.5f);

						SpawnDusts();
					}

					break;
				case FadeAway:
					npc.velocity *= 1f - 3f / 60f;

					//Spawn dusts as the boss fades away, then despawn it once fully invisible
					SpawnDusts();

					targetAlpha += 255f / 180f;

					if(targetAlpha >= 255){
						npc.active = false;
						return;
					}
					break;
				case Attack_DoNothing:
					inertia = DefaultInertia;
					zAxisLerpStrength = DefaultZAxisLerpStrength;

					FloatTowardsTarget(player);

					if(AI_Timer <= 0){
						//Prevent the same attack from being rolled twice in a row
						int attack;
						do{
							attack = Main.rand.Next(Attack_SummonMeteors, Attack_ChargeAtPlayer + 1);
						}while(attack == oldAttack);

						SetAttack(attack);
						npc.netUpdate = true;

						oldAttack = (int)AI_Attack;
					}

					break;
				case Attack_SummonMeteors:
					npc.defense = npc.defDefense + 40;

					//Face forwards
					targetZAxisRotation = 0f;

					npc.velocity *= 1f - 4.67f / 60f;

					if(Math.Abs(npc.velocity.X) < 0.02f)
						npc.velocity.X = 0f;
					if(Math.Abs(npc.velocity.Y) < 0.02f)
						npc.velocity.Y = 0f;

					int max = Main.expertMode ? PortalTimerMax : (int)(PortalTimerMax * 1.5f);

					if((int)AI_Timer == max - 1)
						Main.PlaySound(SoundID.Zombie, npc.Center, 99);
					else if((int)AI_Timer == max - 24){
						//Wait until the animation frame changes to the one facing forwards
						if(GetAnimationSetFrame() == Animation_LookFront_JawOpen){
							Main.PlaySound(SoundID.Zombie, npc.Center, 93);
							AI_AttackProgress++;
						}else
							AI_Timer++;
					}

					if(AI_AttackProgress == 1){
						//Spawn portals near and above the player
						Vector2 orig = player.Center - new Vector2(0, 15 * 16);

						int count = Main.expertMode ? 4 : 2;

						for(int i = 0; i < count; i++){
							Vector2 spawn = orig + new Vector2(Main.rand.NextFloat(-1, 1) * 40 * 16, Main.rand.NextFloat(-1, 1) * 6 * 16);

							Projectile.NewProjectile(spawn, Vector2.Zero, ModContent.ProjectileType<Portal>(), MiscUtils.TrueDamage(Main.expertMode ? 140 : 90), 0f, Main.myPlayer);

							// TODO: netcode
						}

						AI_AttackProgress++;
					}else if(AI_AttackProgress == 2 && AI_Timer == 0)
						SetAttack(Attack_DoNothing);

					break;
				case Attack_SummonLesserDemons:
					FloatTowardsTarget(player);

					if(AI_AttackProgress < 3){
						if(AI_Timer % 75 == 0){
							Vector2 spawn = npc.Center + new Vector2(Main.rand.NextFloat(-1, 1) * 22 * 16, Main.rand.NextFloat(-1, 1) * 10 * 16);

							NPC.NewNPC((int)spawn.X, (int)spawn.Y, ModContent.NPCType<MiniCraterDemon>(), ai3: npc.whoAmI);

							//Exhale sound
							Main.PlaySound(SoundID.Zombie, npc.Center, 93);

							// TODO: netcode

							AI_AttackProgress++;
						}
					}else if(AI_Timer <= 0){
						//Go to next attack immediately
						SetAttack(Attack_DoNothing);
					}else if(CountAliveLesserDemons() == 0){
						//All the minions have been killed.  Transition to the next subphase immediately
						AI_Timer = 1;
					}

					break;
				case Attack_ChargeAtPlayer:
					inertia = DefaultInertia * 0.1f;

					float chargeVelocity = Main.expertMode ? 26f : 17f;
					int repeatRelative = AI_AttackProgress >= Attack_ChargeAtPlayer_RepeatStart
						? ((int)AI_AttackProgress - Attack_ChargeAtPlayer_RepeatStart) % Attack_ChargeAtPlayer_RepeatSubphaseCount
						: -1;
					int repeatCount = AI_AttackProgress >= Attack_ChargeAtPlayer_RepeatStart
						? ((int)AI_AttackProgress - Attack_ChargeAtPlayer_RepeatStart) / Attack_ChargeAtPlayer_RepeatSubphaseCount
						: 0;

					if(AI_AttackProgress == 0){
						if(bigPortal.visible)
							bigPortal = new BigPortalInfo();
						if(bigPortal2.visible)
							bigPortal2 = new BigPortalInfo();

						//Wait until initial wait is done
						if(AI_Timer <= 0){
							AI_AttackProgress++;

							//Spawn portal -- must be close to boss and player
							Vector2 spawn;
							const float playerDistMax = 40 * 16;
							int tries = 0;
							bool success = false;
							do{
								spawn = npc.Center + Main.rand.NextVector2Unit() * 40 * 16;
								tries++;
							}while(tries < 1000 && (success = player.DistanceSQ(spawn) >= playerDistMax * playerDistMax));

							if(!success && tries == 1000){
								//Failsafe: put the portal directly on the target player
								spawn = player.Center;
							}

							SpawnBigPortal(spawn, ref bigPortal);
						}
					}else if(AI_AttackProgress == 1){
						float dist = npc.DistanceSQ(bigPortal.center);
						const float portalDist = 5;
						bool tooFar = dist > portalDist * portalDist;

						//Update the portal
						if(bigPortal.scale > 0.98f){
							bigPortal.scale = 1f;
							bigPortal.alpha = 1f;
						}else{
							bigPortal.scale = MiscUtils.ScaleLogarithmic(bigPortal.scale, 1f, 2.7219f, 1f / 60f);
							bigPortal.alpha = bigPortal.scale;
						}

						if(tooFar){
							//Float toward first portal
							movementTarget = bigPortal.center;

							FloatTowardsTarget(player, minimumDistanceThreshold: 0);
						}else{
							npc.Center = bigPortal.center;
							npc.velocity = Vector2.Zero;
							movementTarget = null;

							//If the portal hasn't gotten to the full size yet, wait for it to do so
							if(bigPortal.scale >= 1f){
								AI_AttackProgress++;

								UpdateScale(1f);

								AI_Timer = 45;
							}
						}
					}else if(repeatRelative == 0){
						//Shrink (factor of 4.3851 makes it reach 0.01 in around 60 ticks, 12.2753 ~= 20 ticks)
						const float epsilon = 0.01f;
						if(npc.scale > epsilon)
							UpdateScale(MiscUtils.ScaleLogarithmic(npc.scale, 0f, repeatCount == 0 ? 4.3851f : 12.2753f, 1f / 60f));
						
						targetAlpha = 255f * (1f - npc.scale);

						if(AI_Timer <= 0){
							//Shrink the portal, but a bit slower
							bigPortal.scale = MiscUtils.ScaleLogarithmic(bigPortal.scale, 0f, repeatCount == 0 ? 2.7219f : 9.2153f, 1f / 60f);
							bigPortal.alpha = bigPortal.scale;
						}

						if(npc.scale < 0.4f){
							hideMapIcon = true;
							npc.dontTakeDamage = true;
						}

						if(npc.scale < epsilon){
							UpdateScale(epsilon);
							
							targetAlpha = 255f;

							//Sanity check
							targetZAxisRotation = 0;
							zAxisRotation = 0;
						}

						if(bigPortal.scale < epsilon){
							bigPortal.scale = 0f;
							bigPortal.visible = false;

							AI_AttackProgress++;

							if(repeatCount == 0)
								Main.PlaySound(SoundID.Zombie, npc.Center, 105);  //Cultist laugh sound

							//Wait for a random amount of time
							AI_Timer = Main.expertMode ? Main.rand.Next(40, 90 + 1) : Main.rand.Next(100, 220 + 1);
							npc.netUpdate = true;

							movementTarget = null;
						}
					}else if(repeatRelative == 1){
						//Wait, then spawn a portal
						if(AI_Timer <= 0){
							Vector2 offset = Main.rand.NextVector2Unit() * 30 * 16;

							//Second portal is where the boss will end up
							SpawnBigPortal(player.Center + offset, ref bigPortal, fast: true);
							SpawnBigPortal(player.Center - offset, ref bigPortal2, fast: true);
							bigPortal2.visible = false;

							npc.Center = bigPortal.center;
							npc.velocity = Vector2.Zero;

							AI_AttackProgress++;
						}
					}else if(repeatRelative == 2){
						//Make the portal grow FAST (9.9583 results in around 26 ticks)
						bigPortal.scale = MiscUtils.ScaleLogarithmic(bigPortal.scale, 1f, 9.9583f, 1f / 60f);
						bigPortal.alpha = bigPortal.scale;

						if(bigPortal.scale >= 0.99f){
							bigPortal.scale = 1f;

							AI_AttackProgress++;
							AI_Timer = 40;
						}

						bigPortal.alpha = bigPortal.scale;
					}else if(repeatRelative == 3){
						//Make the boss charge at the player after fading in
						zAxisLerpStrength = DefaultZAxisLerpStrength * 2.7f;

						//15.7025 ~= 16 ticks to go from 0.01 to 1
						if(npc.scale < 0.99f)
							UpdateScale(MiscUtils.ScaleLogarithmic(npc.scale, 1, 15.7025f, 1f / 60f));
						else
							UpdateScale(1f);

						if(npc.scale > 0.4f){
							npc.dontTakeDamage = false;
							hideMapIcon = false;
						}

						targetAlpha = 255f * (1f - npc.scale);

						SetTargetZAxisRotation(player, out _);

						if(AI_Timer <= 0 && npc.scale == 1f){
							AI_AttackProgress++;
							AI_Timer = Main.expertMode ? 30 : 60;

							npc.Center = bigPortal.center;
							npc.velocity = npc.DirectionTo(player.Center) * chargeVelocity;
							Main.PlaySound(SoundID.ForceRoar, (int)npc.Center.X, (int)npc.Center.Y, -1);

							if(repeatCount >= (Main.expertMode ? Main.rand.Next(5, 8) : Main.rand.Next(2, 5))){
								//Stop the repetition
								bigPortal2.visible = false;
								bigPortal2.scale = 8f / 240f;

								SetAttack(Attack_PostCharge);
							}
						}
					}else if(repeatRelative == 4){
						//Second portal appears once the boss is within 22 update ticks of its center
						float activeDist = chargeVelocity * 22;
						if(AI_Timer < 0 && npc.DistanceSQ(bigPortal2.center) <= activeDist * activeDist)
							bigPortal2.visible = true;

						//First portal disappears once the boss leaves within 22 update ticks of its center
						if(npc.DistanceSQ(bigPortal.center) > activeDist * activeDist){
							bigPortal.scale = MiscUtils.ScaleLogarithmic(bigPortal.scale, 0f, 15.2753f, 1f / 60f);
							bigPortal.alpha = bigPortal.scale;

							if(bigPortal.scale <= 0.01f){
								bigPortal.scale = 0f;
								bigPortal.alpha = 0f;
								bigPortal.visible = false;
							}
						}

						if(bigPortal2.visible){
							bigPortal2.scale = MiscUtils.ScaleLogarithmic(bigPortal2.scale, 1f, 15.2753f, 1f / 60f);
							bigPortal2.alpha = bigPortal2.scale;

							if(bigPortal2.scale >= 0.99f){
								bigPortal2.scale = 1f;
								bigPortal2.alpha = 1f;
							}
						}

						const float portalEnterDist = 5, portalTargetDist = 4 * 16;

						if(AI_Timer >= 0){
							if(AI_Timer == 0)
								bigPortal2.center = npc.Center + Vector2.Normalize(npc.velocity) * (activeDist + 3 * 16);
						}else{
							//Make sure the boss snaps to the center of the portal before repeating the logic
							float dist = npc.DistanceSQ(bigPortal2.center);
							if(dist < portalEnterDist * portalEnterDist){
								UpdateScale(1f);

								npc.Center = bigPortal2.center;
								npc.velocity = Vector2.Zero;

								targetAlpha = 0;

								movementTarget = null;

								if(bigPortal2.scale >= 1f){
									AI_AttackProgress++;

									Utils.Swap(ref bigPortal, ref bigPortal2);
								}
							}else if(dist < portalTargetDist * portalTargetDist){
								//Float to the center
								movementTarget = bigPortal2.center;
								inertia = DefaultInertia * 0.1f;

								float oldZ = targetZAxisRotation;
								FloatTowardsTarget(player, minimumDistanceThreshold: 0);
								targetZAxisRotation = oldZ;
							}
						}
					}
					break;
				case Attack_PostCharge:
					if(npc.velocity == Vector2.Zero && !bigPortal.visible)
						SetAttack(Attack_DoNothing);
					else if(AI_Timer <= 0){
						//Charge has ended.  Make the portal fade away and slow the boss down
						npc.velocity *= 1f - 8.5f / 60f;

						//5.9192 ~= 45 ticks to reach 0.01 scale
						bigPortal.scale = MiscUtils.ScaleLogarithmic(bigPortal.scale, 0f, 5.9192f, 1f / 60f);

						if(Math.Abs(npc.velocity.X) < 0.05f && Math.Abs(npc.velocity.Y) <= 0.05f)
							npc.velocity = Vector2.Zero;

						if(bigPortal.scale <= 0.01f)
							bigPortal = new BigPortalInfo();
					}
					break;
			}

			AI_Timer--;
			AI_AnimationCounter++;

			if(AI_Attack != FadeAway && targetAlpha > 0){
				targetAlpha -= 255f / 60f;

				if(targetAlpha < 0)
					targetAlpha = 0;
			}

			npc.alpha = (int)targetAlpha;

			if(Math.Abs(zAxisRotation - targetZAxisRotation) < 0.02f)
				zAxisRotation = targetZAxisRotation;
			else
				zAxisRotation = MathHelper.Lerp(zAxisRotation, targetZAxisRotation, zAxisLerpStrength / 60f);

			//We don't want sprite flipping
			npc.spriteDirection = -1;

			bigPortal.Update();
			bigPortal2.Update();
		}

		private void SetTargetZAxisRotation(Player player, out Vector2 targetCenter){
			float rad = MathHelper.ToRadians(ZAxisRotationThreshold * 2);
			targetCenter = movementTarget ?? player.Center;

			targetZAxisRotation = targetCenter.X < npc.Left.X
				? -rad
				: (targetCenter.X > npc.Right.X
					? rad
					: 0f);
		}

		private void FloatTowardsTarget(Player player, float minimumDistanceThreshold = 5 * 16){
			//Look at the player and float around
			SetTargetZAxisRotation(player, out Vector2 targetCenter);

			const float speedX = 8f;
			const float speedY = speedX * 0.4f;

			if(npc.DistanceSQ(targetCenter) >= minimumDistanceThreshold * minimumDistanceThreshold){
				Vector2 direction = npc.DirectionTo(targetCenter) * speedX;

				npc.velocity = (npc.velocity * (inertia - 1) + direction) / inertia;

				if(npc.velocity.X < -speedX)
					npc.velocity.X = -speedX;
				else if(npc.velocity.X > speedX)
					npc.velocity.X = speedX;

				if(npc.velocity.Y < -speedY)
					npc.velocity.Y = -speedY;
				else if(npc.velocity.Y > speedY)
					npc.velocity.Y = speedY;
			}

			//Play one of two sounds randomly
			if(Main.rand.NextFloat() < 0.02f / 60f)
				Main.PlaySound(SoundID.Zombie, npc.Center, Style: 96);
			else if(Main.rand.NextFloat() < 0.02f / 60f)
				Main.PlaySound(SoundID.Zombie, npc.Center, Style: 5);
		}

		private int CountAliveLesserDemons(){
			int count = 0;
			
			for(int i = 0; i < Main.maxNPCs; i++){
				NPC other = Main.npc[i];
				if(other.active && other.modNPC is MiniCraterDemon mini && mini.ParentBoss == npc.whoAmI)
					count++;
			}

			return count;
		}

		private void SpawnDusts(){
			GetHitboxRects(out Rectangle head, out Rectangle jaw);

			var type = ModContent.DustType<RegolithDust>();
			var pos = head.Location.ToVector2();
			for(int i = 0; i < 20; i++)
				SpawnDustsInner(pos, head.Width, head.Height, type);

			pos = jaw.Location.ToVector2();
			for(int i = 0; i < 8; i++)
				SpawnDustsInner(pos, jaw.Width, jaw.Height, type);
		}

		internal static void SpawnDustsInner(Vector2 pos, int width, int height, int type){
			Dust dust = Dust.NewDustDirect(pos, width, height, type, Scale: Main.rand.NextFloat(0.85f, 1.2f));
			dust.velocity = new Vector2(0, Main.rand.NextFloat(1.4f, 2.8f));
		}

		private void SpawnBigPortal(Vector2 center, ref BigPortalInfo info, bool fast = false){
			info.center = center;
			info.visible = true;
			info.scale = 8f / 240f;  //Initial size of 8 pxiels
			info.alpha = info.scale;
			info.rotation = 0f;
			info.fast = fast;

			Main.PlaySound(SoundID.Item84.WithVolume(0.9f), info.center);
		}

		public override Color? GetAlpha(Color drawColor)
			=> Color.White * (1f - targetAlpha / 255f);

		public override bool? CanBeHitByItem(Player player, Item item)
			=> CanBeHitByThing(player.GetSwungItemHitbox());

		public override bool? CanBeHitByProjectile(Projectile projectile)
			=> CanBeHitByThing(projectile.Hitbox);

		private bool? CanBeHitByThing(Rectangle hitbox){
			//Make the hit detection dynamic be based on the sprite for extra coolness points
			GetHitboxRects(out Rectangle head, out Rectangle jaw);

			return head.Intersects(hitbox) || jaw.Intersects(hitbox) ? null : (bool?)false;
		}

		public override bool? CanHitNPC(NPC target){
			var canHit = CanBeHitByThing(target.Hitbox);

			if(canHit is null)
				return canHit;

			return hideMapIcon ? (bool?)false : null;
		}

		public override bool CanHitPlayer(Player target, ref int cooldownSlot)
			=> (CanBeHitByThing(target.Hitbox) ?? true) && !hideMapIcon;

		private void GetHitboxRects(out Rectangle head, out Rectangle jaw){
			head = Rectangle.Empty;
			jaw = Rectangle.Empty;
			
			int set = GetAnimationSetFrame();

			switch(set){
				case Animation_LookLeft_JawClosed:
					head = new Rectangle(24, 11, 141, 113);
					jaw = new Rectangle(32, 116, 84, 55);
					break;
				case Animation_LookLeft_JawOpen:
					head = new Rectangle(24, 11, 141, 113);
					jaw = new Rectangle(32, 116, 94, 74);
					break;
				case Animation_LookFront_JawClosed:
					head = new Rectangle(13, 11, 152, 113);
					jaw = new Rectangle(49, 116, 84, 61);
					break;
				case Animation_LookFront_JawOpen:
					head = new Rectangle(13, 11, 152, 113);
					jaw = new Rectangle(49, 116, 84, 76);
					break;
				case Animation_LookRight_JawClosed:
					head = new Rectangle(13, 11, 141, 113);
					jaw = new Rectangle(62, 116, 84, 55);
					break;
				case Animation_LookRight_JawOpen:
					head = new Rectangle(13, 11, 141, 113);
					jaw = new Rectangle(62, 116, 94, 74);
					break;
			}

			head.Location = (head.Location.ToVector2() + npc.position).ToPoint();
			jaw.Location = (jaw.Location.ToVector2() + npc.position).ToPoint();

			head.Width = (int)(head.Width * npc.scale);
			head.Height = (int)(head.Height * npc.scale);
			jaw.Width = (int)(jaw.Width * npc.scale);
			jaw.Height = (int)(jaw.Height * npc.scale);
		}
	}
}
