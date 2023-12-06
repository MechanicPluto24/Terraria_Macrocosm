using Macrocosm.Common.DataStructures;
using Macrocosm.Common.Drawing.Particles;
using Macrocosm.Common.Systems;
using Macrocosm.Common.Utils;
using Macrocosm.Content.Buffs.Debuffs;
using Macrocosm.Content.Dusts;
using Macrocosm.Content.Items.Consumables.BossBags;
using Macrocosm.Content.Items.Currency;
using Macrocosm.Content.Items.Materials;
using Macrocosm.Content.Items.Placeable.Relics;
using Macrocosm.Content.Items.Placeable.Trophies;
using Macrocosm.Content.Items.Vanity.BossMasks;
using Macrocosm.Content.Items.Weapons.Magic;
using Macrocosm.Content.Items.Weapons.Melee;
using Macrocosm.Content.Items.Weapons.Ranged;
using Macrocosm.Content.Items.Weapons.Summon;
using Macrocosm.Content.NPCs.Friendly.TownNPCs;
using Macrocosm.Content.NPCs.Global;
using Macrocosm.Content.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.NPCs.Bosses.CraterDemon
{
	[AutoloadBossHead]
	public class CraterDemon : ModNPC, IMoonEnemy
	{
		private struct AttackInfo
		{
			public Func<CraterDemon, int> initialProgress;
			public Func<CraterDemon, int> initialTimer;
			public bool resetAnimationCounter;
		}

		private struct BigPortalInfo
		{
			public Vector2 center;
			public float scale;
			public float alpha;
			public bool visible;
			public float rotation;
			public float rotationsPerSecond;

			public void Write(BinaryWriter writer)
			{
				writer.WriteVector2(center);
				writer.Write(scale);
				writer.Write(alpha);
				writer.Write(rotation);
				writer.Write(rotationsPerSecond);

				BitsByte bb = new(visible);
				writer.Write(bb);
			}

			public void Read(BinaryReader reader)
			{
				center = reader.ReadVector2();
				scale = reader.ReadSingle();
				alpha = reader.ReadSingle();
				rotation = reader.ReadSingle();
				rotationsPerSecond = reader.ReadSingle();

				BitsByte bb = reader.ReadByte();
				bb.Retrieve(ref visible);
			}

			public void Update()
			{
				if (!visible)
					return;

				rotation -= MathHelper.ToRadians(rotationsPerSecond * 6f);

				Lighting.AddLight(center, new Vector3(30, 255, 105) / 255 * scale * 3f);
			}

			public void SpawnParticles(BigPortalInfo info, NPC owner)
			{
				for (int i = 0; i < 20; i++)
				{
					Particle.CreateParticle<PortalSwirl>(p =>
					{
						p.Position = info.center + Main.rand.NextVector2Circular(180, 180) * 0.95f * info.scale;
						p.Velocity = Vector2.One * 22;
						p.Scale = (0.1f + Main.rand.NextFloat(0.1f)) * info.scale;
						p.Color = new Color(92, 206, 130);
						p.TargetCenter = info.center;
						p.CustomDrawer = owner;
					});
				}
			}

			SpriteBatchState state;
			public void Draw(SpriteBatch spriteBatch, Vector2 screenPos)
			{
				if (!visible)
					return;

				Texture2D portal = ModContent.Request<Texture2D>("Macrocosm/Content/NPCs/Bosses/CraterDemon/BigPortal").Value;

				spriteBatch.Draw(portal, center - screenPos, null, Color.White * alpha * 0.4f, (-rotation) * 0.65f, portal.Size() / 2f, scale * 1.2f, SpriteEffects.FlipHorizontally, 0);
				spriteBatch.Draw(portal, center - screenPos, null, Color.White * alpha * 0.8f, rotation, portal.Size() / 2f, scale, SpriteEffects.None, 0);

				state.SaveState(spriteBatch);
				spriteBatch.End();
				spriteBatch.Begin(BlendState.Additive, state);

				spriteBatch.Draw(portal, center - screenPos, null, Color.White.WithOpacity(0.6f), rotation * 4f, portal.Size() / 2f, scale * 0.85f, SpriteEffects.None, 0);

				spriteBatch.End();
				spriteBatch.Begin(state);
			}
		}

		public enum AttackState
		{
			FadeIn = -3,
			FadeAway = -2,
			Phase2Transition = -1,
			DoNothing = 0,
			SummonMeteors = 1,
			SummonCraterImps = 2,
			SummonPhantasmals = 3,
			SummonPhantasmalPortals = 4,
			Charge = 5,
			PortalCharge = 6,
			PostCharge = 7
		}

		public AttackState AI_Attack
		{
			get => (AttackState)NPC.ai[0];
			set => NPC.ai[0] = (float)value;
		}
		public int AI_Timer
		{
			get => (int)NPC.ai[1];
			set => NPC.ai[1] = value;
		}
		public int AI_AttackProgress
		{
			get => (int)NPC.ai[2];
			set => NPC.ai[2] = value;
		}
		public int AI_AnimationCounter
		{
			get => (int)NPC.ai[3];
			set => NPC.ai[3] = value;
		}

		private readonly AttackInfo[] attacks = new AttackInfo[]{
			// AttackState.DoNothing
			new AttackInfo(){
				initialProgress = null,
				initialTimer = (CD) => 90,
				resetAnimationCounter = false
			},
			// AttackState.SummonMeteors
			new AttackInfo(){
				initialProgress = null,
				initialTimer = (CD) => (int)(5 * 60 * CD.GetDifficultyScaling(DifficultyScale.AttackDurationScaling)),
				resetAnimationCounter = true
			},
			// AttackState.SummonCraterImps
			new AttackInfo(){
				initialProgress = CD => CD.CountAliveCraterImps(),
				initialTimer = (CD) => (int)(8 * 60 * CD.GetDifficultyScaling(DifficultyScale.AttackDurationScaling))
			},
			// AttackState.SummonPhantasmals
			new AttackInfo(){
				initialProgress = null,
				initialTimer = (CD) => (int)(8 * 60 * CD.GetDifficultyScaling(DifficultyScale.AttackDurationScaling))
			},
            // AttackState.SummonPhantasmalPortals
			new AttackInfo(){
				initialProgress = null,
				initialTimer = (CD) => (int)(5 * 60 * CD.GetDifficultyScaling(DifficultyScale.AttackDurationScaling)),
				resetAnimationCounter = true
			},
            // AttackState.Charge
			new AttackInfo(){
				initialProgress = null,
				initialTimer = (CD) => (int)(35 * CD.GetDifficultyScaling(DifficultyScale.AttackDurationScaling))
			},
            // AttackState.PortalCharge
			new AttackInfo(){
				initialProgress = null,
				initialTimer = (CD) => (int)(70 * CD.GetDifficultyScaling(DifficultyScale.AttackDurationScaling))  //Wait before spawning first portal
			},
			// AttackState.PostCharge
			new AttackInfo(){
				initialProgress = null,
				initialTimer = (CD) => (int)(45 * CD.GetDifficultyScaling(DifficultyScale.AttackDurationScaling))
			}
		};

		private enum DifficultyScale
		{
			AttackDurationScaling,      // The initial AI timer scaling, based on AttackInfo, when switching attacks                                        
			ChargeSpeed,                // Regular charge attack speed
			PortalChargeSpeed,          // Portal charge attack speed
			FloatTowardsTargetSpeed,    // Speed towards target when not doing a special attack
			CirclePlayerSpeed            // Orbit player speed
		}

		private enum DifficultyInfo
		{
			Phase2AnimDefenseBoost,       // Extra defense during phase2 animation sequence

			SuitBreachTime,               // Suit breach debuff time 

			MeteorPortalCount,            // Number of meteor portals 

			CraterImpMaxCount,            // Max number of Crater Imp minions that can exist

			ChargeAttackCountMin,         // Min number of consecutive charge attacks 
			ChargeAttackCountMax,         // Max number of consecutive charge attacks 
			ChargeAttackRepeatTime,       // Time in between charges

			PortalWaitTimeMin,            // Min wait time between portal attacks 
			PortalWaitTimeMax,            // Max wait time between portal attacks 
			PortalSecondTime,             // Time to spawn the second portal 
			PortalChargeAttackCountMin,   // Min number of consecutive portal charge attacks 
			PortalChargeAttackCountMax,   // Max number of consecutive portal charge attacks 

			PhantasmalImpMaxCount,        // Max number of phantasmal skulls spawned in a sequence

			PhantasmalPortalCount,        // Number of phantasmal portals 
		}

		private readonly float[,] scaleInfo = new float[,]
		{	
			 // Scaled element on :             NM1,  NM2,  EM1,   EM2,   MM1,   MM2,   FTW1,  FTW2
			 /*AttackDurationScaling      */ { 1.25f, 1.1f, 1f,    0.8f,  0.75f, 0.65f, 0.6f,  0.55f }, 
			 /*ChargeSpeed                */ { 20f,   22f , 27f,   29f,   33f,   35f,   39f,   43f   },
			 /*PortalChargeSpeed          */ { 17f,   19f , 24f,   26f,   30f,   32f,   36f,   40f   },
			 /*FloatTowardsTargetSpeed    */ { 8f,    8.5f, 9f,    9.5f,  10f,   10.5f, 11f,   11.5f },
			 /*CirclePlayerSpeed          */ { 12f,   13f,  14f,   15f,   16f,   17f,   18f,   19f   }
		};

		private readonly int[,] difficultyInfo = new int[,]
		{
		     // Difficulty value on :           NM1, NM2, EM1, EM2, MM1, MM2, FTW1, FTW2
			 /*Phase2AnimDefenseBoost      */ { 40, 40, 50, 50, 50, 50, 60, 60 },

			 /*SuitBreachTime              */ { 120, 120, 240, 240, 260, 260, 300, 300 },  
                  
			 /*MeteorPortalCount           */ { 3, 3, 4, 4, 6, 6, 7, 8 },                           
                                           
			 /*CraterImpMaxCount           */ { 3, 3, 4, 4, 4, 4, 5, 5 },      

             /*PortalChargeAttackCountMin  */ { 2, 3, 4, 5, 4, 5, 6, 7 },                         
			 /*PortalChargeAttackCountMax  */ { 4, 5, 8, 9, 8, 9, 10, 11 },
             /*ChargeAttackRepeatTime      */ { 50, 40, 40, 30, 30, 10, 20, 5 },                 
                       
			 /*PortalWaitTimeMin           */ { 80, 70, 40, 30, 20, 10, 5, 0 },                   
			 /*PortalWaitTimeMax           */ { 160, 150, 80, 70, 40, 30, 10, 5 },                
			 /*PortalSecondTime            */ { 60, 55, 35, 30, 25, 20, 15, 10 },                 
			 /*PortalChargeAttackCountMin  */ { 2, 3, 4, 5, 4, 5, 6, 7 },                         
			 /*PortalChargeAttackCountMax  */ { 4, 5, 8, 9, 8, 9, 10, 11 },

			 /*PhantasmalImpMaxCount       */ { 5, 5, 6, 6, 8, 8, 9, 9 },

			 /*PhantasmalPortalCount       */ { 1, 1, 1, 1, 1, 1, 1, 1 },
		};

		/// <summary> Return the difficulty index (gamemode, phase): NM1, NM2, EM1, EM2, MM1, MM2, FTW1, FTW2  </summary>
		private int GetDifficultyIndex()
		{
			int difficultyIndex = Main.getGoodWorld ? 6 : // FTW
								  Main.masterMode ? 4 :   // MM
								  Main.expertMode ? 2 :   // EM
													  0;  // NM
			if (phase2)
				difficultyIndex += 1;

			return difficultyIndex;
		}

		private float GetDifficultyScaling(DifficultyScale scaleType)
			=> scaleInfo[(int)scaleType, GetDifficultyIndex()];

		private int GetDifficultyInfo(DifficultyInfo infoType)
			=> difficultyInfo[(int)infoType, GetDifficultyIndex()];

		private void SetAttack(AttackState attack)
		{
			AttackInfo info = attacks[(int)attack];
			AI_Attack = attack;

			AI_AttackProgress = info.initialProgress?.Invoke(this) ?? 0;

			AI_Timer = GetInitialTimer(attack);

			if (info.resetAnimationCounter)
				AI_AnimationCounter = -1;

			if (attack == AttackState.DoNothing)
			{
				movementTarget = null;
				zAxisLerpStrength = DefaultZAxisLerpStrength;
			}
		}

		private int GetInitialTimer(AttackState attack)
		{
			AttackInfo info = attacks[(int)attack];
			return info.initialTimer.Invoke(this);
		}

		private bool phase2 = false;

		private bool spawned = false;
		private int noMovementCounter;
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
		private AttackState oldAttack = AttackState.SummonMeteors;
		private Vector2? movementTarget;
		private int chargeAttackCount;
		private BigPortalInfo bigPortal;
		private BigPortalInfo bigPortal2;
		private Vector2 portalTarget;
		private Vector2 portalOffset;
		private int portalAttackCount;
		private int phantasmalRepeatCount;

		public const float ZAxisRotationThreshold = 25f;

		public int OnSpawn_InitialTimer = 320;
		public int OnSpawn_FadeInTime = 280;
		public int OnSpawn_LaughTime = 80;
		public int OnSpawn_LaughTimeEnd = 0;

		public const int PortalTimerMax = (int)(4f * 60 + 1.5f * 60 + 24);  //Portal spawning leadup + time portals are active before they shrink
		public const int Phase2TimerMax = 150;

		public const int Animation_LookLeft_JawClosed = 0;
		public const int Animation_LookLeft_JawOpen = 1;
		public const int Animation_LookFront_JawClosed = 2;
		public const int Animation_LookFront_JawOpen = 3;
		public const int Animation_LookRight_JawClosed = 4;
		public const int Animation_LookRight_JawOpen = 5;


		private const int AIAttack_Charge_RepeatStart = 1;
		private const int AIAttack_Charge_RepeatSubphaseCount = 3;

		private const int AIAttack_PortalCharge_RepeatStart = 2;
		private const int AIAttack_PortalCharge_RepeatSubphaseCount = 5;

		public override void SetStaticDefaults()
		{
			Main.npcFrameCount[NPC.type] = 6;
			NPCID.Sets.TrailCacheLength[NPC.type] = 5;
			NPCID.Sets.TrailingMode[NPC.type] = 3;
			NPCID.Sets.MPAllowedEnemies[Type] = true;
			NPCID.Sets.BossBestiaryPriority.Add(Type);

			NPCID.Sets.ImmuneToRegularBuffs[Type] = true;

			NPCID.Sets.NPCBestiaryDrawModifiers bestiaryData = new()
			{
				Frame = 1,
				Position = new Vector2(0f, 8f)
			};

			NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, bestiaryData);
		}

		private int baseWidth, baseHeight;

		public override void SetDefaults()
		{
			baseWidth = NPC.width = 178;
			baseHeight = NPC.height = 196;
			NPC.knockBackResist = 0f;

			NPC.defense = 100;
			NPC.damage = 80;
			NPC.lifeMax = 110000;  //For comparison, Moon Lord has 145,000 total HP in Normal Mode

			NPC.boss = true;
			NPC.noGravity = true;
			NPC.noTileCollide = true;

			targetAlpha = NPC.alpha = 255;
			NPC.dontTakeDamage = true;
			NPC.aiStyle = -1;

			NPC.npcSlots = 40f;

			NPC.HitSound = SoundID.NPCHit2;

			if (!Main.dedServ)
				Music = MusicLoader.GetMusicSlot(Mod, "Assets/Music/SpaceInvader");
		}

		public override void ApplyDifficultyAndPlayerScaling(int numPlayers, float balance, float bossAdjustment)/* tModPorter Note: bossLifeScale -> balance (bossAdjustment is different, see the docs for details) */
		{
			//For comparison, Moon Lord's scale factor is 0.7f
			NPC.ScaleHealthBy(0.35f, balance, bossAdjustment);
			NPC.damage = (int)(NPC.damage * 0.75f * bossAdjustment);
		}

		public override void BossLoot(ref string name, ref int potionType)
		{
			potionType = ItemID.SuperHealingPotion;
		}

		public override void ModifyNPCLoot(NPCLoot npcLoot)
		{
			// common drops (non-boss bag)
			npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<CraterDemonTrophy>(), 10));

			// EM drop
			npcLoot.Add(ItemDropRule.BossBag(ModContent.ItemType<CraterDemonBossBag>()));

			// MM drops
			npcLoot.Add(ItemDropRule.MasterModeCommonDrop(ModContent.ItemType<CraterDemonRelic>()));
			//npcLoot.Add(ItemDropRule.MasterModeDropOnAllPlayers(ModContent.ItemType<CraterDemonMMPet>(), 4));

			// BELOW: for normal mode, same as boss bag (excluding Broken Hero Shield)
			LeadingConditionRule notExpertRule = new(new Conditions.NotExpert());
			notExpertRule.OnSuccess(ItemDropRule.Common(ModContent.ItemType<CraterDemonMask>(), 7));

			notExpertRule.OnSuccess(ItemDropRule.Common(ModContent.ItemType<Moonstone>(), 1, 30, 60));
			notExpertRule.OnSuccess(ItemDropRule.Common(ModContent.ItemType<DeliriumPlating>(), 1, 30, 90));

			notExpertRule.OnSuccess(ItemDropRule.OneFromOptions(1,
				ModContent.ItemType<CalcicCane>(),
				ModContent.ItemType<Cruithne>(),
				ModContent.ItemType<ImbriumJewel>(),
				ModContent.ItemType<ChampionsBladeBroken>()
				));

			npcLoot.Add(notExpertRule);
		}

		public override void OnKill()
		{
			if (!WorldDataSystem.Instance.DownedCraterDemon)
				NPC.NewNPC(NPC.GetSource_Death(), (int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<MoonChampion>());

			WorldDataSystem.Instance.DownedCraterDemon = true;
		}

		public override void OnHitPlayer(Player target, Player.HurtInfo hurtInfo)
		{
			int breachTime = GetDifficultyInfo(DifficultyInfo.SuitBreachTime);
			target.AddBuff(ModContent.BuffType<SuitBreach>(), breachTime);
		}

		private void UpdateScale(float newScale)
		{
			if (NPC.scale == newScale)
				return;

			NPC.UpdateScaleAndHitbox(baseWidth, baseHeight, newScale);
		}


		public override void SendExtraAI(BinaryWriter writer)
		{
			bool hasCustomTarget = movementTarget != null;
			BitsByte bb = new(phase2, spawned, ignoreRetargetPlayer, hadNoPlayerTargetForLongEnough, hasCustomTarget, NPC.dontTakeDamage);
			writer.Write(bb);

			writer.Write(targetAlpha);
			writer.Write(targetZAxisRotation);
			writer.Write(zAxisRotation);

			bigPortal.Write(writer);
			bigPortal2.Write(writer);

			if (hasCustomTarget)
				writer.WriteVector2(movementTarget.Value);

			writer.Write((byte)chargeAttackCount);

			writer.Write((byte)oldAttack);
			writer.WriteVector2(portalTarget);
			writer.WriteVector2(portalOffset);

			writer.Write((byte)portalAttackCount);

			writer.Write((byte)phantasmalRepeatCount);
		}

		public override void ReceiveExtraAI(BinaryReader reader)
		{
			bool hasCustomTarget = false;
			BitsByte bb = reader.ReadByte();
			bb.Retrieve(ref phase2, ref spawned, ref ignoreRetargetPlayer, ref hadNoPlayerTargetForLongEnough, ref hasCustomTarget, ref NPC.dontTakeDamage);

			targetAlpha = reader.ReadSingle();
			targetZAxisRotation = reader.ReadSingle();
			zAxisRotation = reader.ReadSingle();

			bigPortal.Read(reader);
			bigPortal2.Read(reader);

			movementTarget = hasCustomTarget ? (Vector2?)reader.ReadVector2() : null;

			chargeAttackCount = reader.ReadByte();

			oldAttack = (AttackState)reader.ReadByte();
			portalTarget = reader.ReadVector2();
			portalOffset = reader.ReadVector2();

			portalAttackCount = reader.ReadByte();

			phantasmalRepeatCount = reader.ReadByte();
		}

		public override void BossHeadSlot(ref int index)
		{
			if (hideMapIcon)
				index = -1;
		}

		private SpriteBatchState state1, state2;
		public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
		{
			bigPortal.Draw(Main.spriteBatch, Main.screenPosition);
			bigPortal2.Draw(Main.spriteBatch, Main.screenPosition);

			state1.SaveState(spriteBatch);
			spriteBatch.End();
			spriteBatch.Begin(BlendState.Additive, state1);

			foreach (var particle in ParticleManager.GetParticlesDrawnBy(NPC))
			{
				particle.PreDrawAdditive(spriteBatch, screenPos, Color.White);
			}

			spriteBatch.End();
			spriteBatch.Begin(state1);

			if (AI_Attack == AttackState.SummonMeteors && Math.Abs(NPC.velocity.X) < 0.1f && Math.Abs(NPC.velocity.Y) < 0.1f)
				return true;

			if (AI_Attack == AttackState.PortalCharge && AI_AttackProgress > 2 && NPC.alpha >= 160)
				return true;

			Texture2D glowmask = ModContent.Request<Texture2D>("Macrocosm/Content/NPCs/Bosses/CraterDemon/CraterDemon_Glow").Value;

			for (int i = 0; i < NPC.oldPos.Length; i++)
			{
				Vector2 drawPos = NPC.oldPos[i] - Main.screenPosition + new Vector2(0, 4);
				Color trailColor = NPC.GetAlpha(drawColor) * (((float)NPC.oldPos.Length - i) / NPC.oldPos.Length);
				spriteBatch.Draw(TextureAssets.Npc[NPC.type].Value, drawPos, NPC.frame, trailColor * 0.6f, NPC.rotation, Vector2.Zero, NPC.scale, SpriteEffects.None, 0f);

				Color glowColor = (Color)GetAlpha(Color.White) * (((float)NPC.oldPos.Length - i) / NPC.oldPos.Length);
				spriteBatch.Draw(glowmask, drawPos, NPC.frame, glowColor * 0.6f, NPC.rotation, Vector2.Zero, NPC.scale, SpriteEffects.None, 0f);
			}

			return true;
		}

		public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
		{
			Texture2D glowmask = ModContent.Request<Texture2D>("Macrocosm/Content/NPCs/Bosses/CraterDemon/CraterDemon_Glow").Value;
			spriteBatch.Draw(glowmask, NPC.position - screenPos + new Vector2(0, 4), NPC.frame, (Color)GetAlpha(Color.White), NPC.rotation, Vector2.Zero, NPC.scale, SpriteEffects.None, 0f);

			DrawLensFlares(spriteBatch, screenPos);
		}

		private void DrawLensFlares(SpriteBatch spriteBatch, Vector2 screenPos)
		{
			state2.SaveState(spriteBatch);
			spriteBatch.End();
			spriteBatch.Begin(BlendState.Additive, state2);

			Texture2D flare = ModContent.Request<Texture2D>(Macrocosm.TextureAssetsPath + "Flare2").Value;

			if (AI_Attack == AttackState.Phase2Transition && AI_AttackProgress >= 1)
			{
				float progress = (1f - ((float)AI_Timer / Phase2TimerMax));
				float scale = NPC.scale * 0.3f * (progress < 0.5f ? progress : 1f - progress);
				spriteBatch.Draw(flare, NPC.Center - screenPos + GetEyeOffset(), null, new Color(157, 255, 156), NPC.rotation, flare.Size() / 2, scale, SpriteEffects.None, 0f);
			}

			if (AI_Attack == AttackState.SummonMeteors)
			{
				int maxTimer = GetInitialTimer(AttackState.SummonMeteors);
				float progress = MathHelper.Clamp((float)(AI_Timer - 320) / (maxTimer - 320), 0f, 1f);
				float scale = NPC.scale * 0.5f * (progress < 0.5f ? progress : 1f - progress);
				spriteBatch.Draw(flare, NPC.Center - screenPos + GetEyeOffset(), null, new Color(255, 141, 36), NPC.rotation, flare.Size() / 2, scale, SpriteEffects.None, 0f);
			}

			if (AI_Attack == AttackState.FadeIn)
			{
				float progress = MathHelper.Clamp((float)(AI_Timer - OnSpawn_FadeInTime) / (OnSpawn_LaughTime - OnSpawn_FadeInTime), 0f, 1f);

				if (AI_Timer < OnSpawn_LaughTime)
					progress *= (float)AI_Timer / OnSpawn_LaughTime;

				spriteBatch.Draw(flare, NPC.Center - screenPos + GetEyeOffset(), null, new Color(157, 255, 156), NPC.rotation, flare.Size() / 2, 1.1f * Utility.QuadraticEaseIn(progress) * Main.rand.NextFloat(0.8f, 1f), SpriteEffects.None, 0f);
			}

			spriteBatch.End();
			spriteBatch.Begin(state2);
		}

		private Vector2 GetEyeOffset()
		{
			return GetAnimationSetFrame() switch
			{
				Animation_LookLeft_JawClosed => new Vector2(-4, -16),
				Animation_LookLeft_JawOpen => new Vector2(-4, -16),
				Animation_LookFront_JawClosed => new Vector2(30, -8),
				Animation_LookFront_JawOpen => new Vector2(30, -8),
				Animation_LookRight_JawClosed => new Vector2(60, -16),
				Animation_LookRight_JawOpen => new Vector2(60, -16),
				_ => new Vector2(30, -8)
			};
		}

		private int GetAnimationSetFrame()
		{
			float rad = MathHelper.ToRadians(ZAxisRotationThreshold);
			int set = zAxisRotation < -rad
				? Animation_LookLeft_JawClosed
				: (zAxisRotation < rad
					? Animation_LookFront_JawClosed / 2
					: Animation_LookRight_JawClosed / 2);

			//Set sets -> frame
			set *= 2;

			switch (AI_Attack)
			{
				case AttackState.FadeIn:
					if (AI_Timer > OnSpawn_LaughTimeEnd && AI_Timer <= OnSpawn_LaughTime && AI_AnimationCounter % 16 < 8)
						set++;
					break;

				case AttackState.Phase2Transition:
					if (AI_Timer >= Math.Floor(Phase2TimerMax * 0.5f) && AI_Timer <= Math.Floor(Phase2TimerMax * 0.9f))
						set++;
					else if (AI_AnimationCounter % 26 < 13)
						set++;
					break;

				case AttackState.DoNothing:
				case AttackState.SummonCraterImps:
				case AttackState.SummonPhantasmals:
				case AttackState.SummonMeteors:
				case AttackState.SummonPhantasmalPortals:
					if (AI_AnimationCounter % 26 < 13)
						set++;
					break;

				case AttackState.Charge:

					int repeat = (AI_AttackProgress - AIAttack_Charge_RepeatStart) % AIAttack_Charge_RepeatSubphaseCount;

					if (AI_AttackProgress > AIAttack_Charge_RepeatStart && repeat == 1)
					{
						set++;
					}
					else if (AI_AnimationCounter % 26 < 13)
						set++;
					break;

				case AttackState.PortalCharge:
					if (AI_AttackProgress > AIAttack_PortalCharge_RepeatStart)
					{
						int repeatRelative = (AI_AttackProgress - AIAttack_PortalCharge_RepeatStart) % AIAttack_PortalCharge_RepeatSubphaseCount;
						int repeatCount = (AI_AttackProgress - AIAttack_PortalCharge_RepeatStart) / AIAttack_PortalCharge_RepeatSubphaseCount;

						const float portalTargetDist = 4 * 16;

						if (repeatCount == 0 && repeatRelative == 0)
						{
							if (AI_AnimationCounter % 26 < 13)
								set++;
						}
						else if (repeatRelative == 4 && NPC.DistanceSQ(bigPortal2.center) >= portalTargetDist * portalTargetDist)
							set++;
					}
					else if (AI_AnimationCounter % 26 < 13)
						set++;
					break;

				case AttackState.PostCharge:
					if (NPC.velocity != Vector2.Zero)
						set++;
					break;
			}

			return set;
		}

		public override void FindFrame(int frameHeight)
		{
			int set = GetAnimationSetFrame();

			NPC.frame.Y = frameHeight * set;
		}

		public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Vector2 position)
		{
			scale = 1.5f;

			return hideMapIcon ? (bool?)false : null;
		}

		public override void AI()
		{
			NPC.defense = NPC.defDefense;

			if (!spawned)
			{
				if (AI_Timer == 0)
				{
					targetZAxisRotation = 0f;
					zAxisRotation = 0f;
					targetAlpha = 255f;

					ignoreRetargetPlayer = true;

					UpdateScale(0.01f);
					AI_Timer = OnSpawn_InitialTimer;
					AI_Attack = AttackState.FadeIn;
					NPC.TargetClosest();

					spawned = true;
					NPC.netUpdate = true;
				}
				else
					targetAlpha -= 255f / 60f;
			}

			//Player is dead/not connected?  Target a new one
			//That player is also dead/not connected?  Begin the "fade away" animation and despawn
			Player player = NPC.target >= 0 && NPC.target < Main.maxPlayers ? Main.player[NPC.target] : null;

			if (!ignoreRetargetPlayer && (NPC.target < 0 || NPC.target >= Main.maxPlayers || player.dead || !player.active))
			{
				NPC.TargetClosest();

				player = NPC.target >= 0 && NPC.target < Main.maxPlayers ? Main.player[NPC.target] : null;

				if (NPC.target < 0 || NPC.target >= Main.maxPlayers || player.dead || !player.active)
				{
					//Go away
					AI_Attack = AttackState.FadeAway;
					AI_Timer = -1;
					AI_AttackProgress = -1;

					hadNoPlayerTargetForLongEnough = true;

					NPC.netUpdate = true;
				}
				else if (hadNoPlayerTargetForLongEnough)
				{
					hadNoPlayerTargetForLongEnough = false;

					//Start back in the idle phase
					SetAttack(AttackState.DoNothing);

					NPC.netUpdate = true;
				}
			}

			NPC.defense = NPC.defDefense;

			switch (AI_Attack)
			{
				case AttackState.FadeIn:
					AI_FadeIn();
					break;

				case AttackState.FadeAway:
					AI_FadeAway();
					break;

				case AttackState.Phase2Transition:
					AI_Phase2Transition();
					break;

				case AttackState.DoNothing:
					AI_DoNothing(player);
					break;

				case AttackState.SummonMeteors:
					AI_SummonMeteors(player);
					break;

				case AttackState.SummonCraterImps:
					AI_SummonCraterImps(player);
					break;

				case AttackState.SummonPhantasmals:
					AI_SummonPhantasmals(player);
					break;

				case AttackState.SummonPhantasmalPortals:
					AI_SummonPhantasmalPortals(player);
					break;

				case AttackState.Charge:
					AI_Charge(player);
					break;

				case AttackState.PortalCharge:
					AI_PortalCharge(player);
					break;

				case AttackState.PostCharge:
					AI_PostCharge();
					break;

			}

			if (AI_Attack != AttackState.DoNothing)
				oldAttack = AI_Attack;

			AI_Timer--;
			AI_AnimationCounter++;

			if (AI_Attack != AttackState.FadeAway && targetAlpha > 0)
			{
				targetAlpha -= 255f / 60f;

				if (targetAlpha < 0)
					targetAlpha = 0;
			}

			NPC.alpha = (int)targetAlpha;

			if (Math.Abs(zAxisRotation - targetZAxisRotation) < 0.02f)
				zAxisRotation = targetZAxisRotation;
			else
				zAxisRotation = MathHelper.Lerp(zAxisRotation, targetZAxisRotation, zAxisLerpStrength / 60f);

			if (NPC.velocity == NPC.oldVelocity && AI_Attack is AttackState.SummonMeteors or AttackState.SummonPhantasmalPortals)
				noMovementCounter++;
			else
				noMovementCounter = 0;

			if (noMovementCounter >= 240)
				SetAttack(AttackState.DoNothing);

			//We don't want sprite flipping
			NPC.spriteDirection = -1;

			bigPortal.Update();
			bigPortal2.Update();

			bigPortal.SpawnParticles(bigPortal, NPC);
			bigPortal2.SpawnParticles(bigPortal2, NPC);
		}

		private void AI_FadeIn()
		{
			if (AI_Timer == OnSpawn_InitialTimer)
			{
				bigPortal = new BigPortalInfo();
				SpawnBigPortal(NPC.Center, ref bigPortal, 0.9f);
			}
			else if (AI_Timer > OnSpawn_FadeInTime && AI_Timer < OnSpawn_InitialTimer - 1)
			{
				NPC.Center = bigPortal.center;

				float targetScale = 1.3f;
				if (bigPortal.scale > targetScale - 0.02f)
				{
					bigPortal.scale = targetScale;
					bigPortal.alpha = 1f;
				}
				else
				{
					bigPortal.scale = Utility.ScaleLogarithmic(bigPortal.scale, targetScale, 4.6f, 1f / 60f);
					bigPortal.alpha = bigPortal.scale;
				}
			}
			else if (AI_Timer > 0 && AI_Timer <= OnSpawn_FadeInTime)
			{
				UpdateScale(Utility.ScaleLogarithmic(NPC.scale, 1f, 2.3851f, 1f / 60f));

				if (AI_Timer == OnSpawn_LaughTime)
					SoundEngine.PlaySound(SoundID.Zombie105 with { Pitch = -0.2f }, NPC.Center);

				if (AI_Timer < 20)
				{
					bigPortal.scale = Utility.ScaleLogarithmic(bigPortal.scale, 0f, 4f, 1f / 60f);
					bigPortal.alpha = bigPortal.scale;
				}
			}
			else if (AI_Timer <= 0)
			{
				//Transition to the next phase
				SetAttack(AttackState.DoNothing);
				targetAlpha = 0f;
				bigPortal.visible = false;
				NPC.dontTakeDamage = false;
				ignoreRetargetPlayer = false;
			}
		}
		private void AI_FadeAway()
		{
			NPC.velocity *= 1f - 3f / 60f;

			//Spawn dusts as the boss fades away, then despawn it once fully invisible
			SpawnDusts();

			targetAlpha += 255f / 180f;

			if (targetAlpha >= 255)
			{
				for (int i = 0; i < Main.maxNPCs; i++)
				{
					NPC other = Main.npc[i];
					if (other.active && other.ModNPC is CraterImp mini && mini.ParentBoss == NPC.whoAmI)
						mini.AI_Attack = CraterImp.Despawning;
				}

				NPC.active = false;
				return;
			}
		}
		private void AI_Phase2Transition()
		{
			NPC.defense = NPC.defDefense + GetDifficultyInfo(DifficultyInfo.Phase2AnimDefenseBoost);

			// decelerate
			NPC.velocity *= 1f - 3f / 60f;

			if (Math.Abs(NPC.velocity.X) < 0.05f && Math.Abs(NPC.velocity.Y) <= 0.05f)
				NPC.velocity = Vector2.Zero;

			// close down any existing portals
			if (bigPortal.visible)
				bigPortal.scale = Utility.ScaleLogarithmic(bigPortal.scale, 0f, 9.2153f, 1f / 60f);
			else
				bigPortal = new BigPortalInfo();

			if (bigPortal2.visible)
				bigPortal2.scale = Utility.ScaleLogarithmic(bigPortal2.scale, 0f, 9.2153f, 1f / 60f);
			else
				bigPortal = new BigPortalInfo();

			// Wait to face forward, mouth open
			if (AI_AttackProgress == 0)
			{
				if (NPC.velocity == Vector2.Zero && GetAnimationSetFrame() == Animation_LookFront_JawOpen)
					AI_AttackProgress++;
				else
					AI_Timer++;
			}

			if (AI_AttackProgress == 1)
			{

				if (AI_Timer == Math.Floor(Phase2TimerMax * 0.8f))
					SoundEngine.PlaySound(SoundID.ForceRoar with { Pitch = -0.6f }, NPC.position);

				if (AI_Timer == Math.Floor(Phase2TimerMax * 0.75f))
					SoundEngine.PlaySound(SoundID.ForceRoar with { Pitch = -0.8f }, NPC.position);

				if (AI_Timer == Math.Floor(Phase2TimerMax * 0.7f))
					SoundEngine.PlaySound(SoundID.ForceRoar with { Pitch = -0.4f }, NPC.position);

				if (AI_Timer == Math.Floor(Phase2TimerMax * 0.6f))
					SoundEngine.PlaySound(SoundID.Zombie105 with { Pitch = -0.2f }, NPC.position);

				if (AI_Timer <= 0)
				{
					phase2 = true;
					SetAttack(AttackState.DoNothing);
				}
			}
		}

		private void AI_DoNothing(Player player)
		{
			inertia = DefaultInertia;
			zAxisLerpStrength = DefaultZAxisLerpStrength;

			FloatTowardsTarget(player);


			int phase2LifeMax = (int)(NPC.lifeMax * (Main.getGoodWorld ? 0.75f :
													   Main.masterMode ? 0.6f :
													   Main.expertMode ? 0.5f :
																		 0.4f));

			if (!phase2 && AI_Attack != AttackState.Phase2Transition && NPC.life < phase2LifeMax)
			{
				AI_Attack = AttackState.Phase2Transition;
				AI_Timer = Phase2TimerMax;
				AI_AttackProgress = 0;
				targetZAxisRotation = 0f;
			}

			if (AI_Timer <= 0 && Main.netMode != NetmodeID.MultiplayerClient)
			{
				if (NPC.DistanceSQ(player.Center) > (120 * 16f) * (120 * 16f))
				{
					SetAttack(AttackState.PortalCharge);
				}
				else
				{
					List<AttackState> list = new();

					if (phase2 || Main.expertMode)
						list.Add(AttackState.SummonPhantasmalPortals);

					list.Add(AttackState.SummonMeteors);
					list.Add(AttackState.SummonCraterImps);
					list.Add(AttackState.SummonPhantasmals);

					float portalAttackChance = Main.getGoodWorld ? 0.1f :
												Main.masterMode ? 0.25f :
												Main.expertMode ? 0.35f :
																  0.50f;

					if (CountAliveCraterImps() > 0)
						portalAttackChance *= 2f;

					if (Main.rand.NextFloat() < portalAttackChance)
						list.Add(AttackState.PortalCharge);
					else
						list.Add(AttackState.Charge);

					list.Remove(oldAttack);

					SetAttack(list.GetRandom());
				}


				NPC.netUpdate = true;
			}
		}
		private void AI_SummonMeteors(Player player)
		{
			int maxTimer = GetInitialTimer(AttackState.SummonMeteors);

			if (AI_Timer == maxTimer - 1)
			{
				SoundEngine.PlaySound(SoundID.Zombie99, NPC.Center);
			}
			else if (AI_Timer == maxTimer - 24)
			{
				Vector2 orig = player.Center + player.velocity;

				int count = GetDifficultyInfo(DifficultyInfo.MeteorPortalCount);

				if (Main.netMode != NetmodeID.MultiplayerClient)
				{
					for (int i = 0; i < count; i++)
					{
						Vector2 spawn = orig + new Vector2(Main.rand.NextFloat(player.direction * 0.5f, player.direction * 1.5f) * 60 * 16, Main.rand.NextFloat(-1.5f, -0.5f) * 60 * 16);
						Projectile.NewProjectile(NPC.GetSource_FromAI(), spawn, Vector2.Zero, ModContent.ProjectileType<MeteorPortal>(), Utility.TrueDamage(Main.expertMode ? 140 : 90), 0f, Main.myPlayer, 0f, phase2 ? 1f : 0f);
					}
				}

				SoundEngine.PlaySound(SoundID.Zombie93, NPC.Center);

				AI_AttackProgress++;
			}

			if (AI_AttackProgress == 1)
			{
				FloatTowardsTarget(player);

				if (AI_Timer <= 0)
					SetAttack(AttackState.DoNothing);
			}
		}
		private void AI_SummonCraterImps(Player player)
		{
			FloatTowardsTarget(player);

			if (AI_AttackProgress < GetDifficultyInfo(DifficultyInfo.CraterImpMaxCount))
			{
				if (AI_Timer % (int)(75 * GetDifficultyScaling(DifficultyScale.AttackDurationScaling)) == 0)
				{
					if (Main.netMode != NetmodeID.MultiplayerClient)
					{
						Vector2 spawn = NPC.Center + new Vector2(Main.rand.NextFloat(-1, 1) * 22 * 16, Main.rand.NextFloat(-1, 1) * 10 * 16);
						int craterImpID = NPC.NewNPC(NPC.GetSource_FromAI(), (int)spawn.X, (int)spawn.Y, ModContent.NPCType<CraterImp>(), ai3: NPC.whoAmI);
						Main.npc[craterImpID].netUpdate = true;
					}

					SoundEngine.PlaySound(SoundID.Zombie93, NPC.Center);

					AI_AttackProgress++;
				}
			}
			else if (AI_Timer <= 0)
			{
				//Go to next attack immediately
				SetAttack(AttackState.DoNothing);
			}
			else if (CountAliveCraterImps() == 0)
			{
				//All the minions have been killed.  Transition to the next subphase immediately
				AI_Timer = 1;
			}
		}
		private void AI_SummonPhantasmals(Player player)
		{
			FloatTowardsTarget(player);

			if (AI_AttackProgress < GetDifficultyInfo(DifficultyInfo.PhantasmalImpMaxCount))
			{
				if (AI_Timer % (int)(25 * GetDifficultyScaling(DifficultyScale.AttackDurationScaling)) == 0)
				{
					if (Main.netMode != NetmodeID.MultiplayerClient)
					{
						int type = ModContent.ProjectileType<PhantasmalImpSmall>();
						int damage = Utility.TrueDamage(Main.masterMode ? 72 : Main.expertMode ? 56 : 36);
						Vector2 spawn = NPC.Center + new Vector2(Main.rand.NextFloat(0, 1) * -NPC.direction * 22 * 16, Main.rand.NextFloat(-1, 1) * 10 * 16);
						Vector2 velocity = (player.Center - NPC.Center).SafeNormalize(Vector2.One).RotatedByRandom(MathHelper.PiOver4) * Main.rand.NextFloat(16, 22);

						if (Main.rand.NextBool(3))
						{
							type = ModContent.ProjectileType<PhantasmalImp>();
							velocity *= 0.8f;
							damage = (int)(damage * 1.4f);
						}

						Projectile.NewProjectile(NPC.GetSource_FromAI(), spawn, velocity, type, damage, 1f, Main.myPlayer, ai0: player.whoAmI);
					}

					SoundEngine.PlaySound(SoundID.Zombie93, NPC.Center);

					AI_AttackProgress++;
				}
			}
			else if (AI_Timer <= 0)
			{
				//Go to next attack immediately
				SetAttack(AttackState.DoNothing);
			}
		}

		private void AI_SummonPhantasmalPortals(Player player)
		{
			int maxTimer = GetInitialTimer(AttackState.SummonPhantasmalPortals);

			if (AI_Timer == maxTimer - 1)
			{
				SoundEngine.PlaySound(SoundID.Zombie99, NPC.Center);
			}
			else if (AI_Timer == maxTimer - 24)
			{
				if (Main.netMode != NetmodeID.MultiplayerClient)
				{
					//Spawn portals near and above the player
					Vector2 orig = player.Center - new Vector2(0, 15 * 16);
					int count = GetDifficultyInfo(DifficultyInfo.PhantasmalPortalCount);
					int damage = Utility.TrueDamage(Main.masterMode ? 72 : Main.expertMode ? 56 : 36);

					for (int i = 0; i < count; i++)
					{
						Vector2 spawn = orig + new Vector2(Main.rand.NextFloat(-1, 1) * 40 * 16, Main.rand.NextFloat(-1, 1) * 40 * 16);
						Projectile.NewProjectile(NPC.GetSource_FromAI(), spawn, Vector2.Zero, ModContent.ProjectileType<PhantasmalPortal>(), damage, 0f, Main.myPlayer, ai1: phase2 ? 1f : 0f, ai2: NPC.target);
					}
				}

				SoundEngine.PlaySound(SoundID.Zombie93, NPC.Center);

				AI_AttackProgress++;
			}

			if (AI_AttackProgress == 1)
			{
				FloatTowardsTarget(player);

				if (AI_Timer <= 0)
					SetAttack(AttackState.DoNothing);
			}
		}

		private void AI_Charge(Player player)
		{
			inertia = DefaultInertia * 0.1f;

			int repeatRelative = AI_AttackProgress >= AIAttack_Charge_RepeatStart
								? (AI_AttackProgress - AIAttack_Charge_RepeatStart) % AIAttack_Charge_RepeatSubphaseCount
								: -1;
			int repeatCount = AI_AttackProgress >= AIAttack_Charge_RepeatStart
							  ? (AI_AttackProgress - AIAttack_Charge_RepeatStart) / AIAttack_Charge_RepeatSubphaseCount
							  : 0;

			float chargeVelocity = GetDifficultyScaling(DifficultyScale.ChargeSpeed);

			if (AI_AttackProgress == 0)
			{
				//Wait until initial wait is done
				if (AI_Timer <= 0)
				{
					if (Main.netMode != NetmodeID.MultiplayerClient)
					{
						chargeAttackCount = Main.rand.Next(GetDifficultyInfo(DifficultyInfo.ChargeAttackCountMin), GetDifficultyInfo(DifficultyInfo.ChargeAttackCountMax) + 1);
						NPC.netUpdate = true;
					}

					AI_AttackProgress++;
					AI_Timer = GetDifficultyInfo(DifficultyInfo.ChargeAttackRepeatTime);
				}
				else
				{
					//CircleAroundTarget(player, 80 * 16, 0.1f);
					FloatTowardsTarget(player);
				}
			}
			else if (repeatRelative == 0)
			{
				if (AI_Timer <= 0)
				{
					AI_AttackProgress++;
					AI_Timer = GetDifficultyInfo(DifficultyInfo.ChargeAttackRepeatTime);

					NPC.velocity = NPC.DirectionTo(player.Center + player.velocity) * chargeVelocity;

					SoundEngine.PlaySound(SoundID.ForceRoar with { Pitch = -0.2f }, NPC.position);

					if (repeatCount >= portalAttackCount)
					{
						SetAttack(AttackState.PostCharge);
					}
				}
				else
				{
					FloatTowardsTarget(player);
				}
			}
			else if (repeatRelative == 1)
			{
				if (AI_Timer <= 0)
				{
					NPC.velocity *= 1f - 2f / 60f;

					if (NPC.velocity.LengthSquared() < 4f * 4f)
						AI_AttackProgress++;
				}
			}
			else if (repeatRelative == 2)
			{
				FloatTowardsTarget(player);
				AI_Timer = GetDifficultyInfo(DifficultyInfo.ChargeAttackRepeatTime);
				AI_AttackProgress++;
			}

			if (repeatRelative != 1)
			{
				zAxisLerpStrength = DefaultZAxisLerpStrength * 6f;
				SetTargetZAxisRotation(player, out _);
			}
		}
		private void AI_PortalCharge(Player player)
		{
			NPC.netOffset *= 0f;

			inertia = DefaultInertia * 0.1f;

			float chargeVelocity = GetDifficultyScaling(DifficultyScale.PortalChargeSpeed);

			int repeatRelative = AI_AttackProgress >= AIAttack_PortalCharge_RepeatStart
				? (AI_AttackProgress - AIAttack_PortalCharge_RepeatStart) % AIAttack_PortalCharge_RepeatSubphaseCount
				: -1;
			int repeatCount = AI_AttackProgress >= AIAttack_PortalCharge_RepeatStart
				? (AI_AttackProgress - AIAttack_PortalCharge_RepeatStart) / AIAttack_PortalCharge_RepeatSubphaseCount
				: 0;

			if (AI_AttackProgress == 0)
			{
				if (bigPortal.visible)
					bigPortal = new BigPortalInfo();
				if (bigPortal2.visible)
					bigPortal2 = new BigPortalInfo();

				//Wait until initial wait is done
				if (AI_Timer <= 0)
				{
					AI_AttackProgress++;

					if (Main.netMode != NetmodeID.MultiplayerClient)
					{
						//Spawn portal -- must be close to boss and player
						Vector2 spawn;
						const float playerDistMax = 40 * 16;
						int tries = 0;
						bool success = false;
						do
						{
							spawn = NPC.Center + Main.rand.NextVector2Unit() * 40 * 16;
							tries++;
						} while (tries < 1000 && (success = player.DistanceSQ(spawn) >= playerDistMax * playerDistMax));

						if (!success && tries == 1000)
						{
							//Failsafe: put the portal directly on the target player
							spawn = player.Center;
						}

						portalTarget = spawn;
						NPC.netUpdate = true;
					}

					SpawnBigPortal(portalTarget, ref bigPortal);
				}
			}
			else if (AI_AttackProgress == 1)
			{
				float dist = NPC.DistanceSQ(bigPortal.center);
				const float portalDist = 5;
				bool tooFar = dist > portalDist * portalDist;

				//Update the portal
				if (bigPortal.scale > 0.98f)
				{
					bigPortal.scale = 1f;
					bigPortal.alpha = 1f;
				}
				else
				{
					bigPortal.scale = Utility.ScaleLogarithmic(bigPortal.scale, 1f, 2.7219f, 1f / 60f);
					bigPortal.alpha = bigPortal.scale;
				}

				if (tooFar)
				{
					//Float toward first portal
					movementTarget = bigPortal.center;

					FloatTowardsTarget(player, minimumDistanceThreshold: 0);
				}
				else
				{
					NPC.Center = bigPortal.center;
					NPC.velocity = Vector2.Zero;
					movementTarget = null;

					//If the portal hasn't gotten to the full size yet, wait for it to do so
					if (bigPortal.scale >= 1f)
					{
						AI_AttackProgress++;

						UpdateScale(1f);

						AI_Timer = 45;
					}
				}
			}
			else if (repeatRelative == 0)
			{
				//Shrink (factor of 4.3851 makes it reach 0.01 in around 60 ticks, 12.2753 ~= 20 ticks)
				const float epsilon = 0.01f;
				if (NPC.scale > epsilon)
					UpdateScale(Utility.ScaleLogarithmic(NPC.scale, 0f, repeatCount == 0 ? 4.3851f : 12.2753f, 1f / 60f));

				targetAlpha = 255f * (1f - NPC.scale);

				if (AI_Timer <= 0)
				{
					//Shrink the portal, but a bit slower
					bigPortal.scale = Utility.ScaleLogarithmic(bigPortal.scale, 0f, repeatCount == 0 ? 2.7219f : 9.2153f, 1f / 60f);
					bigPortal.alpha = bigPortal.scale;
				}

				if (NPC.scale < 0.4f)
				{
					hideMapIcon = true;
					NPC.dontTakeDamage = true;
				}

				if (NPC.scale < epsilon)
				{
					UpdateScale(epsilon);

					targetAlpha = 255f;

					//Sanity check
					targetZAxisRotation = 0;
					zAxisRotation = 0;
				}

				if (bigPortal.scale < epsilon)
				{
					bigPortal.scale = 0f;
					bigPortal.visible = false;

					AI_AttackProgress++;

					if (repeatCount == 0)
						SoundEngine.PlaySound(SoundID.Zombie105 with { Pitch = -0.2f }, NPC.Center); //Cultist laugh sound

					//Wait for a random amount of time
					if (Main.netMode != NetmodeID.MultiplayerClient)
					{
						AI_Timer = Main.rand.Next(GetDifficultyInfo(DifficultyInfo.PortalWaitTimeMin), GetDifficultyInfo(DifficultyInfo.PortalWaitTimeMax) + 1);
						NPC.netUpdate = true;
					}

					movementTarget = null;
				}
			}
			else if (repeatRelative == 1)
			{
				// pick a random position a tick before spawning portal (to allow for netsync)
				if (AI_Timer > 1 * GetDifficultyScaling(DifficultyScale.AttackDurationScaling) && AI_Timer <= 2 * GetDifficultyScaling(DifficultyScale.AttackDurationScaling))
				{
					if (Main.netMode != NetmodeID.MultiplayerClient)
					{
						portalOffset = Main.rand.NextVector2Unit() * 30 * 16;
						NPC.netUpdate = true;
					}
				}

				//Wait, then spawn a portal
				if (AI_Timer <= 0)
				{
					//Second portal is where the boss will end up
					SpawnBigPortal(player.Center + portalOffset, ref bigPortal, 1.6f);
					SpawnBigPortal(player.Center - portalOffset, ref bigPortal2, 1.6f);
					bigPortal2.visible = false;

					NPC.Center = bigPortal.center;
					NPC.velocity = Vector2.Zero;

					AI_AttackProgress++;
				}
			}
			else if (repeatRelative == 2)
			{
				//Make the portal grow FAST (9.9583 results in around 26 ticks)
				bigPortal.scale = Utility.ScaleLogarithmic(bigPortal.scale, 1f, 9.9583f, 1f / 60f);
				bigPortal.alpha = bigPortal.scale;

				if (bigPortal.scale >= 0.99f)
				{
					bigPortal.scale = 1f;

					AI_AttackProgress++;
					AI_Timer = 40;
				}

				bigPortal.alpha = bigPortal.scale;
			}
			else if (repeatRelative == 3)
			{
				//Make the boss charge at the player after fading in
				zAxisLerpStrength = DefaultZAxisLerpStrength * 2.7f;

				if (NPC.scale < 0.99f)
					UpdateScale(Utility.ScaleLogarithmic(NPC.scale, 1, 9.2153f, 1f / 60f));
				else
					UpdateScale(1f);

				if (NPC.scale > 0.4f)
				{
					NPC.dontTakeDamage = false;
					hideMapIcon = false;
				}

				targetAlpha = 255f * (1f - NPC.scale);

				SetTargetZAxisRotation(player, out _);

				if (AI_Timer == 1 && Main.netMode != NetmodeID.MultiplayerClient)
				{
					portalAttackCount = Main.rand.Next(GetDifficultyInfo(DifficultyInfo.PortalChargeAttackCountMin), GetDifficultyInfo(DifficultyInfo.PortalChargeAttackCountMax) + 1);
					NPC.netUpdate = true;
				}

				if (AI_Timer <= 0 && NPC.scale == 1f)
				{
					AI_AttackProgress++;
					AI_Timer = GetDifficultyInfo(DifficultyInfo.PortalSecondTime);

					NPC.Center = bigPortal.center;
					NPC.velocity = NPC.DirectionTo(player.Center) * chargeVelocity;

					SoundEngine.PlaySound(SoundID.ForceRoar with { Pitch = -0.2f }, NPC.position);

					if (repeatCount >= portalAttackCount)
					{
						//Stop the repetition
						bigPortal2.visible = false;
						bigPortal2.scale = 8f / 240f;

						SetAttack(AttackState.PostCharge);
					}
				}
			}
			else if (repeatRelative == 4)
			{
				//Second portal appears once the boss is within 22 update ticks of its center
				float activeDist = chargeVelocity * 22;
				if (AI_Timer < 0 && NPC.DistanceSQ(bigPortal2.center) <= activeDist * activeDist)
					bigPortal2.visible = true;

				//First portal disappears once the boss leaves within 22 update ticks of its center
				if (NPC.DistanceSQ(bigPortal.center) > activeDist * activeDist)
				{
					bigPortal.scale = Utility.ScaleLogarithmic(bigPortal.scale, 0f, 9.2153f, 1f / 60f);
					bigPortal.alpha = bigPortal.scale;

					if (bigPortal.scale <= 0.01f)
					{
						bigPortal.scale = 0f;
						bigPortal.alpha = 0f;
						bigPortal.visible = false;
					}
				}

				if (bigPortal2.visible)
				{
					bigPortal2.scale = Utility.ScaleLogarithmic(bigPortal2.scale, 1f, 9.2153f, 1f / 60f);
					bigPortal2.alpha = bigPortal2.scale;

					if (bigPortal2.scale >= 0.99f)
					{
						bigPortal2.scale = 1f;
						bigPortal2.alpha = 1f;
					}
				}

				const float portalEnterDist = 5, portalTargetDist = 4 * 16;

				if (AI_Timer >= 0)
				{
					if (AI_Timer == 0)
						bigPortal2.center = NPC.Center + Vector2.Normalize(NPC.velocity) * (activeDist + 3 * 16);
				}
				else
				{
					//Make sure the boss snaps to the center of the portal before repeating the logic
					float dist = NPC.DistanceSQ(bigPortal2.center);
					if (dist < portalEnterDist * portalEnterDist)
					{
						UpdateScale(1f);

						NPC.Center = bigPortal2.center;
						NPC.velocity = Vector2.Zero;

						targetAlpha = 0;

						movementTarget = null;

						if (bigPortal2.scale >= 1f)
						{
							AI_AttackProgress++;

							Utils.Swap(ref bigPortal, ref bigPortal2);
						}
					}
					else if (dist < portalTargetDist * portalTargetDist)
					{
						//Float to the center
						movementTarget = bigPortal2.center;
						inertia = DefaultInertia * 0.1f;

						float oldZ = targetZAxisRotation;
						FloatTowardsTarget(player, minimumDistanceThreshold: 0);
						targetZAxisRotation = oldZ;
					}
				}
			}
		}
		private void AI_PostCharge()
		{
			if (NPC.velocity == Vector2.Zero && !bigPortal.visible)
				SetAttack(AttackState.DoNothing);
			else if (AI_Timer <= 0)
			{
				//Charge has ended.  Make the portal fade away and slow the boss down
				NPC.velocity *= 1f - 8.5f / 60f;

				//5.9192 ~= 45 ticks to reach 0.01 scale
				bigPortal.scale = Utility.ScaleLogarithmic(bigPortal.scale, 0f, 5.9192f, 1f / 60f);

				if (Math.Abs(NPC.velocity.X) < 0.05f && Math.Abs(NPC.velocity.Y) <= 0.05f)
					NPC.velocity = Vector2.Zero;

				if (bigPortal.scale <= 0.01f)
					bigPortal = new BigPortalInfo();
			}
		}

		public override void HitEffect(NPC.HitInfo hit)
		{
			SpawnDusts(10);

			if (Main.dedServ)
				return;

			var entitySource = NPC.GetSource_Death();

			if (NPC.life <= 0)
			{
				SpawnDusts(30);

				Gore.NewGore(entitySource, NPC.position, NPC.velocity, Mod.Find<ModGore>("CraterDemonGoreFace").Type);
				Gore.NewGore(entitySource, NPC.position, NPC.velocity, Mod.Find<ModGore>("CraterDemonGoreHead").Type);
				Gore.NewGore(entitySource, NPC.position, NPC.velocity, Mod.Find<ModGore>("CraterDemonGoreJaw").Type);
			}
		}

		private void SetTargetZAxisRotation(Player player, out Vector2 targetCenter)
		{
			float rad = MathHelper.ToRadians(ZAxisRotationThreshold * 2);
			targetCenter = movementTarget ?? player.Center;

			targetZAxisRotation = targetCenter.X < NPC.Left.X
				? -rad
				: (targetCenter.X > NPC.Right.X
					? rad
					: 0f);
		}

		private void FloatTowardsTarget(Player player, float minimumDistanceThreshold = 5 * 16)
		{
			//Look at the player and float around
			SetTargetZAxisRotation(player, out Vector2 targetCenter);

			float speedX = GetDifficultyScaling(DifficultyScale.FloatTowardsTargetSpeed);

			float speedup = Utility.InverseLerp((30 * 16f) * (30 * 16f), (60 * 16f) * (60 * 16f), NPC.DistanceSQ(targetCenter), true);
			speedX += speedX * speedup;

			float speedY = speedX * 0.4f;

			if (NPC.DistanceSQ(targetCenter) >= minimumDistanceThreshold * minimumDistanceThreshold)
			{
				Vector2 direction = NPC.DirectionTo(targetCenter) * speedX;

				NPC.velocity = (NPC.velocity * (inertia - 1) + direction) / inertia;

				if (NPC.velocity.X < -speedX)
					NPC.velocity.X = -speedX;
				else if (NPC.velocity.X > speedX)
					NPC.velocity.X = speedX;

				if (NPC.velocity.Y < -speedY)
					NPC.velocity.Y = -speedY;
				else if (NPC.velocity.Y > speedY)
					NPC.velocity.Y = speedY;
			}

			//Play one of two sounds randomly
			if (Main.rand.NextFloat() < 0.02f / 60f)
				SoundEngine.PlaySound(SoundID.Zombie96, NPC.Center);
			else if (Main.rand.NextFloat() < 0.02f / 60f)
				SoundEngine.PlaySound(SoundID.Zombie5, NPC.Center);
		}

		private int CountAliveCraterImps()
		{
			int count = 0;

			for (int i = 0; i < Main.maxNPCs; i++)
			{
				NPC other = Main.npc[i];
				if (other.active && other.ModNPC is CraterImp mini && mini.ParentBoss == NPC.whoAmI)
					count++;
			}

			return count;
		}

		private void SpawnDusts(int number = 20)
		{
			GetHitboxRects(out Rectangle head, out Rectangle jaw);

			var type = ModContent.DustType<RegolithDust>();
			var pos = head.Location.ToVector2();
			for (int i = 0; i < number; i++)
				SpawnDustsInner(pos, head.Width, head.Height, type);

			pos = jaw.Location.ToVector2();
			for (int i = 0; i < number / 2; i++)
				SpawnDustsInner(pos, jaw.Width, jaw.Height, type);
		}

		public static void SpawnDustsInner(Vector2 pos, int width, int height, int type)
		{
			Dust dust = Dust.NewDustDirect(pos, width, height, type, Scale: Main.rand.NextFloat(0.85f, 1.2f));
			dust.velocity = new Vector2(0, Main.rand.NextFloat(1.4f, 2.8f));
		}

		private void SpawnBigPortal(Vector2 center, ref BigPortalInfo info, float rotationsPerSecond = 0.9f)
		{
			info.center = center;
			info.visible = true;
			info.scale = 8f / 240f;  //Initial size of 8 pxiels
			info.alpha = info.scale;
			info.rotation = 0f;
			info.rotationsPerSecond = rotationsPerSecond;

			SoundStyle sound = SoundID.Item84 with { Volume = 0.9f };
			SoundEngine.PlaySound(sound, info.center);
		}

		public override Color? GetAlpha(Color drawColor)
		{
			if (NPC.IsABestiaryIconDummy)
				return NPC.GetBestiaryEntryColor(); // This is required because initially we have NPC.alpha = 255, in the bestiary it would look transparent

			if (AI_Attack == AttackState.FadeIn)
			{
				float progress = MathHelper.Clamp((float)(AI_Timer - OnSpawn_FadeInTime) / (OnSpawn_LaughTime - OnSpawn_FadeInTime), 0f, 1f);
				return Color.Lerp(drawColor, new Color(157, 255, 156).WithOpacity(0.1f), 1f - progress) * progress;
			}

			return drawColor * (1f - targetAlpha / 255f);
		}

		public override bool? CanBeHitByItem(Player player, Item item)
			=> CanBeHitByThing(player.GetSwungItemHitbox());

		public override bool? CanBeHitByProjectile(Projectile projectile)
		{
			if (projectile.hostile && !projectile.friendly)
				return null;

			GetHitboxRects(out Rectangle head, out Rectangle jaw);
			return projectile.Colliding(projectile.Hitbox, head) || projectile.Colliding(projectile.Hitbox, jaw);
		}

		private bool? CanBeHitByThing(Rectangle hitbox)
		{
			//Make the hit detection dynamic be based on the sprite for extra coolness points
			GetHitboxRects(out Rectangle head, out Rectangle jaw);

			return head.Intersects(hitbox) || jaw.Intersects(hitbox) ? null : (bool?)false;
		}

		public override bool CanHitNPC(NPC target)
		{
			var canHit = CanBeHitByThing(target.Hitbox);

			if (canHit is null)
				return true;

			return !hideMapIcon;
		}

		public override bool CanHitPlayer(Player target, ref int cooldownSlot)
			=> (CanBeHitByThing(target.Hitbox) ?? true) && !hideMapIcon;

		private void GetHitboxRects(out Rectangle head, out Rectangle jaw)
		{
			head = Rectangle.Empty;
			jaw = Rectangle.Empty;

			int set = GetAnimationSetFrame();

			switch (set)
			{
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

			head.Location = (head.Location.ToVector2() + NPC.position).ToPoint();
			jaw.Location = (jaw.Location.ToVector2() + NPC.position).ToPoint();

			head.Width = (int)(head.Width * NPC.scale);
			head.Height = (int)(head.Height * NPC.scale);
			jaw.Width = (int)(jaw.Width * NPC.scale);
			jaw.Height = (int)(jaw.Height * NPC.scale);
		}
	}
}