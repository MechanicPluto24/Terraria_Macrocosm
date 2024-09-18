using Macrocosm.Common.DataStructures;
using Macrocosm.Common.Drawing.Particles;
using Macrocosm.Common.Sets;
using Macrocosm.Common.Systems;
using Macrocosm.Common.Utils;
using Macrocosm.Content.Dusts;
using Macrocosm.Content.Items.Armor.Vanity.BossMasks;
using Macrocosm.Content.Items.Consumables.BossBags;
using Macrocosm.Content.Items.Currency;
using Macrocosm.Content.Items.Drops;
using Macrocosm.Content.Items.Furniture;
using Macrocosm.Content.Items.Pets;
using Macrocosm.Content.Items.Relics;
using Macrocosm.Content.Items.Trophies;
using Macrocosm.Content.Items.Weapons.Magic;
using Macrocosm.Content.Items.Weapons.Melee;
using Macrocosm.Content.Items.Weapons.Ranged;
using Macrocosm.Content.Items.Weapons.Summon;
using Macrocosm.Content.NPCs.TownNPCs;
using Macrocosm.Content.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
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
    public class CraterDemon : ModNPC
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
                for (int i = 0; i < 6; i++)
                {
                    Particle.Create<PortalSwirl>(p =>
                    {
                        p.Position = info.center + Main.rand.NextVector2CircularEdge(140, 140) * info.scale * Main.rand.NextFloat(0.5f, 1f);
                        p.Velocity = Vector2.One * 24;
                        p.Scale = new((0.14f + Main.rand.NextFloat(0.1f)) * info.scale);
                        p.Color = new Color(92, 206, 130);
                        p.TargetCenter = info.center;
                        p.CustomDrawer = owner;
                    });
                }
            }

            SpriteBatchState state;
            public void Draw(SpriteBatch spriteBatch, Vector2 screenPos, float lensFlareIntensity)
            {
                if (!visible)
                    return;

                Texture2D portal = ModContent.Request<Texture2D>("Macrocosm/Content/NPCs/Bosses/CraterDemon/BigPortal").Value;
                Texture2D circle = ModContent.Request<Texture2D>(Macrocosm.TextureEffectsPath + "Circle7").Value;
                Texture2D flare = ModContent.Request<Texture2D>(Macrocosm.TextureEffectsPath + "Flare1").Value;

                spriteBatch.Draw(portal, center - screenPos, null, Color.White * alpha * 0.4f, (-rotation) * 0.65f, portal.Size() / 2f, scale * 1.2f, SpriteEffects.FlipHorizontally, 0);
                spriteBatch.Draw(portal, center - screenPos, null, Color.White * alpha * 0.8f, rotation, portal.Size() / 2f, scale, SpriteEffects.None, 0);

                state.SaveState(spriteBatch);
                spriteBatch.End();
                spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, state);

                spriteBatch.Draw(circle, center - screenPos, null, new Color(30, 255, 105).WithOpacity(0.5f * (1f - lensFlareIntensity)), 0f, circle.Size() / 2f, scale * 0.8f * Main.rand.NextFloat(0.9f, 1.1f) * (1f - lensFlareIntensity), SpriteEffects.None, 0f);
                spriteBatch.Draw(flare, center - screenPos, null, new Color(30, 255, 105).WithOpacity(0.5f * lensFlareIntensity), 0f, flare.Size() / 2f, new Vector2(scale * 0.65f, scale * 0.85f) * Main.rand.NextFloat(0.9f, 1.1f) * lensFlareIntensity, SpriteEffects.None, 0f);

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
            Charge = 5
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

        public enum ChargeSubphase
        {
            Initial,
            BeginRegularCharge,
            SlowDownAfterCharge,
            FindInitialPortalPosition,
            SpawnInitialPortal,
            ShrinkSelfAndPortal,
            FindPortalPositions,
            GrowSourcePortal,
            DashFromPortal,
            DashIntoPortal
        }

        private readonly AttackInfo[] attacks = [
			// AttackState.DoNothing
			new AttackInfo(){
                initialProgress = null,
                initialTimer = (CD) => 90,
                resetAnimationCounter = false
            },
			// AttackState.SummonMeteors
			new AttackInfo(){
                initialProgress = null,
                initialTimer = (CD) => (int)(CD.GetDifficultyInfo(DifficultyInfo.MeteorShootCount) * CD.GetDifficultyInfo(DifficultyInfo.MeteorShootPeriod) * CD.GetDifficultyScaling(DifficultyScale.AttackDurationScaling))
            },
			// AttackState.SummonCraterImps
			new AttackInfo(){
                initialProgress = CD => CD.CountAliveCraterImps(),
                initialTimer = (CD) => (int)(8 * 60 * CD.GetDifficultyScaling(DifficultyScale.AttackDurationScaling))
            },
			// AttackState.SummonPhantasmals
			new AttackInfo(){
                initialProgress = null,
                initialTimer = (CD) => (int)(CD.GetDifficultyInfo(DifficultyInfo.PhantasmalImpMaxCount) * CD.GetDifficultyInfo(DifficultyInfo.PhantasmalImpShootPeriod) * CD.GetDifficultyScaling(DifficultyScale.AttackDurationScaling))
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
                initialTimer = (CD) => (int)(50 * CD.GetDifficultyScaling(DifficultyScale.AttackDurationScaling))
            }
        ];

        private enum DifficultyScale
        {
            AttackDurationScaling,      // The initial AI timer scaling, based on AttackInfo, when switching attacks
                                        // 
            ChargeSpeed,                // Regular charge speed
            ChargeSlowdownFactor,       // Regular charge slowdown factor per tick
            ChargeTargetSpeed,          // Regular charge slowdown target speed

            PortalChargeSpeed,          // Portal charge speed
            PortalChargeSlowdownFactor, // Portal charge slowdown factor per tick
            PortalChargeTargetSpeed,    // Portal charge slowdown target speed

            FloatTowardsTargetSpeed     // Speed towards target when not doing a special attack
        }

        private enum DifficultyInfo
        {
            Phase2AnimDefenseBoost,       // Extra defense during phase2 animation sequence

            MeteorShootBurst,             // Number of flaming meteors to spawn at the same time 
            MeteorShootPeriod,            // How frequently flaming meteors are being shot
            MeteorShootCount,             // Number of sequential flaming meteor attack

            CraterImpMaxCount,            // Max number of Crater Imp minions that can exist
            CraterImpSpawnPeriod,         // How often crater imps are spawned

            PhantasmalImpMaxCount,        // Max number of phantasmal skulls spawned in a sequence
            PhantasmalImpShootPeriod,     // How frequently phantasmal skulls are being shot 

            PhantasmalPortalCount,        // Number of phantasmal portals spawned 

            ChargeAttackCountMin,         // Min number of consecutive charge attacks 
            ChargeAttackCountMax,         // Max number of consecutive charge attacks 
            ChargeWaitTime,               // Time in between charges
            ChargeSlowdownDelay,          // Charge slowdown delay           

            PortalWaitTimeMin,            // Min wait time between portal attacks 
            PortalWaitTimeMax,            // Max wait time between portal attacks 
            PortalIdleTime,               // Time to spawn the second portal 
            PortalChargeAttackCountMin,   // Min number of consecutive portal charge attacks 
            PortalChargeAttackCountMax,   // Max number of consecutive portal charge attacks 
            PortalChargeSlowdownDelay     // Portal charge slowdown delay 
        }

        private readonly float[,] scaleInfo = new float[,]
        {	
			 // Scaled element on :             NM1,  NM2,  EM1,   EM2,   MM1,   MM2,   FTW1,  FTW2
			 /*AttackDurationScaling      */ { 1.25f, 1.1f, 1f, 0.8f, 0.75f, 0.65f, 0.6f, 0.55f },
															    
			 /*ChargeSpeed                */ { 20f, 30f, 24f, 34f, 26f, 36f, 30f, 40f },
			 /*ChargeSlowdownFactor       */ { 1.8f, 2.2f, 1.9f, 2.3f, 2f, 2.4f, 2.1f, 2.5f },
			 /*ChargeTargetSpeed          */ { 4.5f, 6.5f, 4.5f, 6.5f, 4.5f, 6.5f, 4.5f, 6.5f },
																									   
			 /*PortalChargeSpeed          */ { 17f, 32f, 20f, 34f, 26f, 36f, 30f, 38f },
			 /*PortalSlowdownFactor       */ { 1.7f, 2.1f, 1.8f, 2.2f, 1.9f, 2.3f, 2f, 2.4f },
			 /*PortalTargetSpeed          */ { 1.5f, 3f, 1.5f, 3f, 1.5f, 3f, 1.5f, 3f },

			 /*FloatTowardsTargetSpeed    */ { 8f, 8.5f, 9f, 9.5f, 10f, 10.5f, 11f, 11.5f }
        };

        private readonly int[,] difficultyInfo = new int[,]
        {
		     // Difficulty value on :           NM1, NM2, EM1, EM2, MM1, MM2, FTW1, FTW2
			 /*Phase2AnimDefenseBoost      */ { 40, 40, 50, 50, 50, 50, 60, 60 },
                  
			 /*MeteorShootBurst            */ { 8, 8, 8, 8, 8, 8, 8, 8 },                           
			 /*MeteorShootPeriod           */ { 60, 60, 60, 60, 60, 60, 60, 60 },  
			 /*MeteorShootCount            */ { 5, 5, 5, 5, 5, 5, 5, 5 },  
			 
			 /*CraterImpMaxCount           */ { 3, 3, 4, 4, 4, 4, 5, 5 },   
			 /*CraterImpSpawnPeriod        */ { 75, 75, 75, 75, 75, 75, 75, 75 },   
			 
			 /*PhantasmalImpMaxCount       */ { 5, 5, 6, 6, 8, 8, 9, 9 },
			 /*PhantasmalImpShootPeriod    */ { 25, 25, 25, 25, 25, 25, 25, 25 },

			 /*PhantasmalPortalCount       */ { 0, 1, 1, 2, 1, 2, 2, 3 },

             /*ChargeAttackCountMin        */ { 2, 3, 2, 4, 2, 4, 2, 4 },                       
			 /*ChargeAttackCountMax        */ { 3, 4, 3, 5, 3, 5, 3, 5 },
			 /*ChargeWaitTime              */ { 60, 20, 50, 15, 45, 10, 45, 10 },    
			 /*ChargeSlowdownDelay         */ { 30, 16, 26, 14, 45, 22, 12, 18 },    
			 
			 /*PortalWaitTimeMin           */ { 80, 70, 40, 30, 20, 10, 5, 0 },                   
			 /*PortalWaitTimeMax           */ { 160, 150, 80, 70, 40, 30, 10, 5 },                
			 /*PortalIdleTime              */ { 60, 55, 35, 30, 25, 20, 15, 10 },                 
			 /*PortalChargeAttackCountMin  */ { 1, 2, 1, 2, 1, 2, 1, 2 },                       
			 /*PortalChargeAttackCountMax  */ { 2, 3, 2, 3, 2, 3, 2, 3 },
			 /*PortalChargeSlowdownDelay   */ { 30, 16, 26, 14, 45, 22, 12, 18 }
        };

        /// <summary> Return the difficulty index (gamemode, phase): NM1, NM2, EM1, EM2, MM1, MM2, FTW1, FTW2  </summary>
        private int GetDifficultyIndex()
        {
            int difficultyIndex = Main.getGoodWorld ? 6 : // FTW
                                    Main.masterMode ? 4 : // MM
                                    Main.expertMode ? 2 : // EM
                                                      0; // NM
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
        private bool hadNoPlayerTargetForLongEnough;
        private bool hideMapIcon;
        private int baseWidth, baseHeight;
        private AttackState oldAttack;
        private Vector2? movementTarget;

        private const float ZAxisRotationThreshold = 25f;
        private const float DefaultZAxisLerpStrength = 2.3f;
        private float zAxisLerpStrength = DefaultZAxisLerpStrength;
        private float targetZAxisRotation = 0f;
        private float zAxisRotation = 0f;

        private const float DefaultInertia = 20f;
        private float inertia = DefaultInertia;

        private bool portalCharge;
        private int chargeAttackCount;
        private int portalAttackCount;
        private int maxChargeAttackCount;
        private int maxPortalAttackCount;
        private BigPortalInfo bigPortal;
        private BigPortalInfo bigPortal2;
        private Vector2 portalTarget;
        private Vector2 portalOffset;

        private const int FadeIn_InitialTimer = 320;
        private const int FadeIn_FadeInTime = 280;
        private const int FadeIn_LaughTime = 80;
        private const int FadeIn_LaughTimeEnd = 0;

        private const int FadeAway_InitialTimer = 160;
        private const int FadeAway_FadeOutTime = 130;

        private const int Phase2Transition_InitialTimer = 150;

        private const int Animation_LookLeft_JawClosed = 0;
        private const int Animation_LookLeft_JawOpen = 1;
        private const int Animation_LookFront_JawClosed = 2;
        private const int Animation_LookFront_JawOpen = 3;
        private const int Animation_LookRight_JawClosed = 4;
        private const int Animation_LookRight_JawOpen = 5;

        private static Asset<Texture2D> glowmask;

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[Type] = 6;
            NPCID.Sets.TrailCacheLength[Type] = 5;
            NPCID.Sets.TrailingMode[Type] = 3;
            NPCID.Sets.MPAllowedEnemies[Type] = true;
            NPCID.Sets.BossBestiaryPriority.Add(Type);

            // For the meteors
            NPCID.Sets.TakesDamageFromHostilesWithoutBeingFriendly[Type] = true;

            NPCID.Sets.ImmuneToRegularBuffs[Type] = true;

            NPCID.Sets.NPCBestiaryDrawModifiers bestiaryData = new()
            {
                Frame = 1,
                Position = new Vector2(0f, 8f)
            };

            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, bestiaryData);

            NPCSets.MoonNPC[Type] = true;
            NPCSets.DropsMoonstone[Type] = false;
        }

        public override void SetDefaults()
        {
            baseWidth = NPC.width = 178;
            baseHeight = NPC.height = 196;
            NPC.knockBackResist = 0f;

            NPC.defense = 150;
            NPC.damage = 80;
            NPC.lifeMax = 80000;

            NPC.boss = true;
            NPC.noGravity = true;
            NPC.noTileCollide = true;

            targetAlpha = NPC.alpha = 255;
            NPC.dontTakeDamage = true;
            NPC.aiStyle = -1;

            NPC.npcSlots = 40f;
            NPC.HitSound = SoundID.NPCHit2;

            NPC.BossBar = ModContent.GetInstance<CraterDemonBossBar>();

            if (!Main.dedServ)
                Music = MusicLoader.GetMusicSlot(Mod, "Assets/Music/SpaceInvader");
        }

        public override bool CanBeHitByNPC(NPC attacker)
        {
            return attacker.type == ModContent.NPCType<FlamingMeteor>() && attacker.dontTakeDamage;
        }

        public override void ApplyDifficultyAndPlayerScaling(int numPlayers, float balance, float bossAdjustment)
        {
            //For comparison, Moon Lord's scale factor is 0.7f
            NPC.ScaleHealthBy(0.55f, balance, bossAdjustment);
            NPC.damage = (int)(NPC.damage * 0.6f * bossAdjustment);
        }

        private void ScaleDamage()
        {
            NPC.damage = NPC.defDamage;

            if (AI_Attack is AttackState.Charge)
            {
                if (phase2)
                    NPC.damage = (int)(NPC.defDamage * 1.55f);
                else
                    NPC.damage = (int)(NPC.defDamage * 1.25f);
            }
            else if (phase2)
            {
                NPC.damage = (int)(NPC.defDamage * 1.25f);
            }
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
            npcLoot.Add(ItemDropRule.MasterModeDropOnAllPlayers(ModContent.ItemType<CraterDemonPet>(), 4));

            // BELOW: for normal mode, same as boss bag (excluding Broken Hero Shield)
            LeadingConditionRule notExpertRule = new(new Conditions.NotExpert());
            notExpertRule.OnSuccess(ItemDropRule.Common(ModContent.ItemType<CraterDemonMask>(), 7));

            notExpertRule.OnSuccess(ItemDropRule.Common(ModContent.ItemType<Moonstone>(), 1, 30, 60));
            notExpertRule.OnSuccess(ItemDropRule.Common(ModContent.ItemType<DeliriumPlating>(), 1, 30, 90));

            notExpertRule.OnSuccess(ItemDropRule.Common(ModContent.ItemType<SpookyDookie>(), 14));

            notExpertRule.OnSuccess(ItemDropRule.OneFromOptions(1,
                ModContent.ItemType<CalcicCane>(),
                ModContent.ItemType<Cruithne>(),
                ModContent.ItemType<ImbriumJewel>(),
                ModContent.ItemType<ChampionsBlade>()
                ));

            npcLoot.Add(notExpertRule);
        }

        public override void OnKill()
        {
            if (!WorldFlags.DownedCraterDemon)
                NPC.NewNPC(NPC.GetSource_Death(), (int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<MoonChampion>());

            WorldFlags.SetFlag(ref WorldFlags.DownedCraterDemon);
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
            BitsByte bb = new(phase2, spawned, ignoreRetargetPlayer, hadNoPlayerTargetForLongEnough, hasCustomTarget, NPC.dontTakeDamage, portalCharge);
            writer.Write(bb);

            writer.Write(targetAlpha);
            writer.Write(targetZAxisRotation);
            writer.Write(zAxisRotation);

            bigPortal.Write(writer);
            bigPortal2.Write(writer);

            if (hasCustomTarget)
                writer.WriteVector2(movementTarget.Value);

            writer.Write((byte)oldAttack);

            writer.Write((byte)chargeAttackCount);
            writer.Write((byte)portalAttackCount);
            writer.Write((byte)maxChargeAttackCount);
            writer.Write((byte)maxPortalAttackCount);

            writer.WriteVector2(portalTarget);
            writer.WriteVector2(portalOffset);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            bool hasCustomTarget = false;
            BitsByte bb = reader.ReadByte();
            bb.Retrieve(ref phase2, ref spawned, ref ignoreRetargetPlayer, ref hadNoPlayerTargetForLongEnough, ref hasCustomTarget, ref NPC.dontTakeDamage, ref portalCharge);

            targetAlpha = reader.ReadSingle();
            targetZAxisRotation = reader.ReadSingle();
            zAxisRotation = reader.ReadSingle();

            bigPortal.Read(reader);
            bigPortal2.Read(reader);

            movementTarget = hasCustomTarget ? (Vector2?)reader.ReadVector2() : null;

            oldAttack = (AttackState)reader.ReadByte();

            chargeAttackCount = reader.ReadByte();
            portalAttackCount = reader.ReadByte();
            maxChargeAttackCount = reader.ReadByte();
            maxPortalAttackCount = reader.ReadByte();

            portalTarget = reader.ReadVector2();
            portalOffset = reader.ReadVector2();
        }

        public override void BossHeadSlot(ref int index)
        {
            if (hideMapIcon)
                index = -1;
        }

        private SpriteBatchState state1, state2;
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            float lensFlareIntensity = NPC.DistanceSQ(bigPortal.center) <= 90f * 90f ? (Math.Max(1f - NPC.scale, 1f - NPC.Opacity)) : Utility.InverseLerp(0f, 90f * 90f, NPC.DistanceSQ(bigPortal.center), true);
            bigPortal.Draw(Main.spriteBatch, Main.screenPosition, lensFlareIntensity);
            bigPortal2.Draw(Main.spriteBatch, Main.screenPosition, lensFlareIntensity);

            state1.SaveState(spriteBatch);
            spriteBatch.End();
            spriteBatch.Begin(BlendState.Additive, state1);

            foreach (var particle in ParticleManager.GetParticlesDrawnBy(NPC))
            {
                particle.PreDrawAdditive(spriteBatch, screenPos, Color.White);
            }

            spriteBatch.End();
            spriteBatch.Begin(state1);

            if (AI_Attack == AttackState.Charge && AI_AttackProgress > 2 && NPC.alpha >= 160)
                return true;

            for (int i = 0; i < NPC.oldPos.Length; i++)
            {
                Vector2 drawPos = NPC.oldPos[i] - Main.screenPosition + new Vector2(0, 4);
                Color trailColor = NPC.GetAlpha(drawColor) * (((float)NPC.oldPos.Length - i) / NPC.oldPos.Length);
                spriteBatch.Draw(TextureAssets.Npc[Type].Value, drawPos, NPC.frame, trailColor * 0.6f, NPC.rotation, Vector2.Zero, NPC.scale, SpriteEffects.None, 0f);

                Color glowColor = (Color)GetAlpha(Color.White) * (((float)NPC.oldPos.Length - i) / NPC.oldPos.Length);

                glowmask ??= ModContent.Request<Texture2D>(Texture + "_Glow");
                spriteBatch.Draw(glowmask.Value, drawPos, NPC.frame, glowColor * 0.6f, NPC.rotation, Vector2.Zero, NPC.scale, SpriteEffects.None, 0f);
            }

            return true;
        }

        public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            glowmask ??= ModContent.Request<Texture2D>(Texture + "_Glow");
            spriteBatch.Draw(glowmask.Value, NPC.position - screenPos + new Vector2(0, 4), NPC.frame, (Color)GetAlpha(Color.White), NPC.rotation, Vector2.Zero, NPC.scale, SpriteEffects.None, 0f);
            DrawLensFlares(spriteBatch, screenPos);
        }

        private void DrawLensFlares(SpriteBatch spriteBatch, Vector2 screenPos)
        {
            state2.SaveState(spriteBatch);
            spriteBatch.End();
            spriteBatch.Begin(BlendState.Additive, state2);

            Texture2D flare = ModContent.Request<Texture2D>(Macrocosm.TextureEffectsPath + "Flare2").Value;

            if (AI_Attack == AttackState.Phase2Transition && AI_AttackProgress >= 1)
            {
                float progress = (1f - ((float)AI_Timer / Phase2Transition_InitialTimer));
                float scale = NPC.scale * 0.3f * (progress < 0.5f ? progress : 1f - progress);
                spriteBatch.Draw(flare, NPC.Center - screenPos + GetEyeOffset(), null, new Color(157, 255, 156), NPC.rotation, flare.Size() / 2, scale, SpriteEffects.None, 0f);
            }

            if (AI_Attack == AttackState.SummonMeteors)
            {
                int maxTimer = GetInitialTimer(AttackState.SummonMeteors);
                float progress = MathHelper.Clamp((float)(AI_Timer - maxTimer * 0.5f) / (maxTimer - maxTimer * 0.5f), 0f, 1f);
                float scale = NPC.scale * 0.5f * (progress < 0.5f ? progress : 1f - progress);
                spriteBatch.Draw(flare, NPC.Center - screenPos + GetEyeOffset(), null, new Color(255, 141, 36), NPC.rotation, flare.Size() / 2, scale, SpriteEffects.None, 0f);
            }

            if (AI_Attack == AttackState.SummonPhantasmals)
            {
                int maxTimer = GetInitialTimer(AttackState.SummonPhantasmals);
                float progress = MathHelper.Clamp((float)(AI_Timer - maxTimer * 0.9f) / (maxTimer - maxTimer * 0.9f), 0f, 1f);
                float scale = NPC.scale * 0.5f * (progress < 0.5f ? progress : 1f - progress);
                spriteBatch.Draw(flare, NPC.Center - screenPos + GetEyeOffset(), null, new Color(157, 255, 156), NPC.rotation, flare.Size() / 2, scale, SpriteEffects.None, 0f);
            }

            if (AI_Attack == AttackState.SummonPhantasmalPortals)
            {
                int maxTimer = GetInitialTimer(AttackState.SummonPhantasmalPortals);
                float progress = MathHelper.Clamp((float)(AI_Timer - maxTimer * 0.7f) / (maxTimer - maxTimer * 0.7f), 0f, 1f);
                float scale = NPC.scale * 0.5f * (progress < 0.5f ? progress : 1f - progress);
                spriteBatch.Draw(flare, NPC.Center - screenPos + GetEyeOffset(), null, new Color(157, 255, 156), NPC.rotation, flare.Size() / 2, scale, SpriteEffects.None, 0f);
            }

            if (AI_Attack == AttackState.FadeIn)
            {
                float progress = MathHelper.Clamp((float)(AI_Timer - FadeIn_FadeInTime) / (FadeIn_LaughTime - FadeIn_FadeInTime), 0f, 1f);

                if (AI_Timer < FadeIn_LaughTime)
                    progress *= (float)AI_Timer / FadeIn_LaughTime;

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
                    if (AI_Timer > FadeIn_LaughTimeEnd && AI_Timer <= FadeIn_LaughTime && AI_AnimationCounter % 16 < 8)
                        set++;
                    break;

                case AttackState.Phase2Transition:
                    if (AI_Timer >= Math.Floor(Phase2Transition_InitialTimer * 0.5f) && AI_Timer <= Math.Floor(Phase2Transition_InitialTimer * 0.9f))
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

                    ChargeSubphase subphase = (ChargeSubphase)AI_AttackProgress;
                    switch (subphase)
                    {
                        case ChargeSubphase.Initial:
                        case ChargeSubphase.SlowDownAfterCharge:
                            if (AI_Timer > 0)
                                set++;
                            else if (AI_AnimationCounter % 26 < 13)
                                set++;
                            break;

                        case ChargeSubphase.BeginRegularCharge:
                            if (AI_AnimationCounter % 26 < 13)
                                set++;
                            break;

                        case ChargeSubphase.ShrinkSelfAndPortal:
                            if (portalAttackCount == 0 && AI_AnimationCounter % 26 < 13)
                                set++;
                            break;

                        case ChargeSubphase.DashIntoPortal:
                            const float portalTargetDist = 4 * 16;
                            if (NPC.DistanceSQ(bigPortal2.center) >= portalTargetDist * portalTargetDist)
                                set++;
                            break;

                        case ChargeSubphase.FindInitialPortalPosition:
                        case ChargeSubphase.SpawnInitialPortal:
                        case ChargeSubphase.FindPortalPositions:
                        case ChargeSubphase.GrowSourcePortal:
                        case ChargeSubphase.DashFromPortal:
                            break;
                    }
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
                    AI_Timer = FadeIn_InitialTimer;
                    AI_Attack = AttackState.FadeIn;
                    NPC.TargetClosest();

                    spawned = true;
                    NPC.netUpdate = true;
                }
            }

            //Player is dead/not connected?  Target a new one
            //That player is also dead/not connected?  Begin the "fade away" animation and despawn
            Player player = NPC.target >= 0 && NPC.target < Main.maxPlayers ? Main.player[NPC.target] : null;

            int phase2Life = (int)(NPC.lifeMax * (Main.getGoodWorld ? 0.75f :
                                                    Main.masterMode ? 0.6f :
                                                    Main.expertMode ? 0.5f :
                                                                      0.4f));

            if (!phase2 && (AI_Attack is not AttackState.Phase2Transition and not AttackState.FadeAway) && NPC.scale >= 1f && NPC.life < phase2Life)
            {
                AI_Attack = AttackState.Phase2Transition;
                AI_Timer = Phase2Transition_InitialTimer;
                AI_AttackProgress = 0;
                targetZAxisRotation = 0f;
            }

            if (!ignoreRetargetPlayer && (NPC.target < 0 || NPC.target >= Main.maxPlayers || player.dead || !player.active))
            {
                NPC.TargetClosest();

                player = NPC.target >= 0 && NPC.target < Main.maxPlayers ? Main.player[NPC.target] : null;

                if (NPC.target < 0 || NPC.target >= Main.maxPlayers || player.dead || !player.active)
                {
                    if (AI_Attack != AttackState.FadeAway)
                        AI_Timer = FadeAway_InitialTimer;

                    //Go away
                    AI_Attack = AttackState.FadeAway;
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
            ScaleDamage();

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
            }

            if (AI_Attack != AttackState.DoNothing)
                oldAttack = AI_Attack;

            AI_Timer--;
            AI_AnimationCounter++;

            if (AI_Attack != AttackState.FadeIn && AI_Attack != AttackState.FadeAway && targetAlpha > 0)
            {
                targetAlpha -= 255f / 60f;

                if (targetAlpha < 0)
                    targetAlpha = 0;
            }

            NPC.alpha = NPC.scale > 0.01f ? (int)targetAlpha : 255;

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

            // scale up and close down any existing portals if no longer used
            if (!(AI_Attack is AttackState.FadeIn or AttackState.Charge or AttackState.FadeAway))
            {
                if (NPC.scale < 0.99f)
                    UpdateScale(Utility.ScaleLogarithmic(NPC.scale, 1, 9.2153f, 1f / 60f));
                else
                    UpdateScale(1f);

                if (bigPortal.visible)
                    bigPortal.scale = Utility.ScaleLogarithmic(bigPortal.scale, 0f, 9.2153f, 1f / 60f);
                else
                    bigPortal = new BigPortalInfo();

                if (bigPortal2.visible)
                    bigPortal2.scale = Utility.ScaleLogarithmic(bigPortal2.scale, 0f, 9.2153f, 1f / 60f);
                else
                    bigPortal = new BigPortalInfo();
            }

            bigPortal.Update();
            bigPortal2.Update();

            bigPortal.SpawnParticles(bigPortal, NPC);
            bigPortal2.SpawnParticles(bigPortal2, NPC);

            //DisplayDebugInfo();
        }

        private void AI_FadeIn()
        {
            if (AI_Timer == FadeIn_InitialTimer)
            {
                bigPortal = new BigPortalInfo();
                SpawnBigPortal(NPC.Center, ref bigPortal, 0.9f);
            }
            else if (AI_Timer > FadeIn_FadeInTime && AI_Timer < FadeIn_InitialTimer - 1)
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
            else if (AI_Timer > 0 && AI_Timer <= FadeIn_FadeInTime)
            {
                UpdateScale(Utility.ScaleLogarithmic(NPC.scale, 1f, 2.3851f, 1f / 60f));

                targetAlpha -= 255f / FadeIn_FadeInTime;

                if (AI_Timer == FadeIn_LaughTime)
                    SoundEngine.PlaySound(SoundID.Zombie105 with { Pitch = -0.2f }, NPC.Center);
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
            if (AI_Timer == FadeAway_InitialTimer)
            {
                if (!bigPortal.visible)
                {
                    bigPortal = new BigPortalInfo();
                    movementTarget = NPC.Center;

                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        Vector2 position = NPC.Center + Main.rand.NextVector2Unit() * 10 * 16;
                        movementTarget = position;
                        SpawnBigPortal(position, ref bigPortal, 0.9f);
                        NPC.netUpdate = true;
                    }
                }
                else
                {
                    movementTarget = bigPortal.center;
                }
            }
            else if (AI_Timer > FadeAway_FadeOutTime && AI_Timer < FadeAway_InitialTimer - 1)
            {
                if (NPC.DistanceSQ(movementTarget.Value) > 10f * 10f)
                {
                    AI_Timer++;
                }
                else
                {
                    inertia = 30f;
                    NPC.velocity *= 1f - 10f / 60f;
                }

                float targetScale = 1.3f;
                if (bigPortal.scale > targetScale - 0.02f)
                {
                    bigPortal.scale = targetScale;
                    bigPortal.alpha = 1f;
                    FloatTowardsTarget(null, 0);
                }
                else
                {
                    bigPortal.scale = Utility.ScaleLogarithmic(bigPortal.scale, targetScale, 4.6f, 1f / 60f);
                    bigPortal.alpha = bigPortal.scale;
                }
            }
            else if (AI_Timer > 0 && AI_Timer <= FadeAway_FadeOutTime)
            {
                UpdateScale(Utility.ScaleLogarithmic(NPC.scale, 0f, 0.15f, 1f / 60f));
                targetAlpha += 255f / 80f;
                NPC.velocity = Vector2.Zero;

                if (AI_Timer < 40)
                {
                    bigPortal.scale = Utility.ScaleLogarithmic(bigPortal.scale, 0f, 5f, 1f / 60f);
                    bigPortal.alpha = bigPortal.scale;
                }

                for (int i = 0; i < Main.maxNPCs; i++)
                {
                    NPC other = Main.npc[i];
                    if (other.active && other.ModNPC is CraterImp mini && mini.ParentBoss == NPC.whoAmI)
                        mini.AI_Attack = CraterImp.AttackType.Despawning;
                }
            }
            else if (AI_Timer <= 0)
            {
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
                if (AI_Timer == Math.Floor(Phase2Transition_InitialTimer * 0.8f))
                    SoundEngine.PlaySound(SoundID.ForceRoar with { Pitch = -0.6f }, NPC.position);

                if (AI_Timer == Math.Floor(Phase2Transition_InitialTimer * 0.75f))
                    SoundEngine.PlaySound(SoundID.ForceRoar with { Pitch = -0.8f }, NPC.position);

                if (AI_Timer == Math.Floor(Phase2Transition_InitialTimer * 0.7f))
                    SoundEngine.PlaySound(SoundID.ForceRoar with { Pitch = -0.4f }, NPC.position);

                if (AI_Timer == Math.Floor(Phase2Transition_InitialTimer * 0.6f))
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

            if (AI_Timer <= 0 && Main.netMode != NetmodeID.MultiplayerClient)
            {
                if (NPC.DistanceSQ(player.Center) > (120 * 16f) * (120 * 16f))
                {
                    SetAttack(AttackState.Charge);
                    portalCharge = true;
                }
                else
                {
                    List<AttackState> list = new();

                    if (phase2 || Main.expertMode)
                        list.Add(AttackState.SummonPhantasmalPortals);

                    list.Add(AttackState.SummonMeteors);
                    list.Add(AttackState.SummonCraterImps);
                    list.Add(AttackState.SummonPhantasmals);
                    list.Add(AttackState.Charge);

                    list.Remove(oldAttack);
                    AttackState setAttack = list.GetRandom();

                    if (setAttack is AttackState.Charge)
                    {
                        float portalAttackChance = Main.getGoodWorld ? 0.1f :
                                                    Main.masterMode ? 0.25f :
                                                    Main.expertMode ? 0.35f :
                                                                      0.50f;

                        if (CountAliveCraterImps() > 0)
                            portalAttackChance *= 2f;

                        if (Main.rand.NextFloat() < portalAttackChance)
                            portalCharge = true;
                    }

                    SetAttack(setAttack);

                }

                NPC.netUpdate = true;
            }
        }

        private void AI_SummonMeteors(Player player)
        {
            FloatTowardsTarget(player);

            if (AI_Timer % (int)(GetDifficultyInfo(DifficultyInfo.MeteorShootPeriod) * GetDifficultyScaling(DifficultyScale.AttackDurationScaling)) == 0)
            {
                Vector2 orig = player.Center + player.velocity;

                int count = GetDifficultyInfo(DifficultyInfo.MeteorShootBurst);

                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    for (int i = 0; i < count; i++)
                    {
                        float posX = Main.rand.NormalFloat(orig.X, 300);
                        float posY = orig.Y + Main.rand.NextFloat(-3f, -1f) * 30 * 16;
                        Vector2 velocity = new Vector2(MathF.Abs((new Vector2(posX, posY) - player.Center).ToRotation()) > MathHelper.PiOver2 ? 1 : -1, 1).RotatedByRandom(MathHelper.ToRadians(30)) * Main.rand.NextFloat(8f, 16f);
                        int damage = Utility.TrueDamage(Main.masterMode ? 240 : Main.expertMode ? 120 : 60);
                        NPC meteor = NPC.NewNPCDirect(NPC.GetSource_FromAI(), (int)posX, (int)posY, ModContent.NPCType<FlamingMeteor>());

                        meteor.velocity = velocity;
                        meteor.netUpdate = true;
                    }
                }

                AI_AttackProgress++;

                if (AI_AttackProgress == 1)
                    SoundEngine.PlaySound(SoundID.Zombie93, NPC.Center);
            }

            if (AI_Timer <= 0)
                SetAttack(AttackState.DoNothing);
        }
        private void AI_SummonCraterImps(Player player)
        {
            FloatTowardsTarget(player);

            if (AI_AttackProgress < GetDifficultyInfo(DifficultyInfo.CraterImpMaxCount))
            {
                if (AI_Timer % (int)(GetDifficultyInfo(DifficultyInfo.CraterImpSpawnPeriod) * GetDifficultyScaling(DifficultyScale.AttackDurationScaling)) == 0)
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
                if (AI_Timer % (int)(GetDifficultyInfo(DifficultyInfo.PhantasmalImpShootPeriod) * GetDifficultyScaling(DifficultyScale.AttackDurationScaling)) == 0)
                {
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        int type = ModContent.ProjectileType<PhantasmalImpSmall>();
                        int damage = Utility.TrueDamage(Main.masterMode ? 165 : Main.expertMode ? 110 : 55);
                        Vector2 spawn = NPC.Center + new Vector2(Main.rand.NextFloat(0, 1) * -NPC.direction * 22 * 16, Main.rand.NextFloat(-1, 1) * 10 * 16);
                        Vector2 velocity = (player.Center - NPC.Center).SafeNormalize(Vector2.One).RotatedByRandom(MathHelper.PiOver4) * Main.rand.NextFloat(16, 22);

                        if (Main.rand.NextBool(3))
                        {
                            type = ModContent.ProjectileType<PhantasmalImp>();
                            velocity *= 0.8f;
                            damage = (int)(damage * 1.2f);
                        }

                        Projectile.NewProjectile(NPC.GetSource_FromAI(), spawn, velocity, type, damage, 1f, Main.myPlayer, ai0: player.whoAmI, ai2: NPC.whoAmI);
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
                    int damage = Utility.TrueDamage(Main.masterMode ? 165 : Main.expertMode ? 110 : 55);

                    for (int i = 0; i < count; i++)
                    {
                        Vector2 spawn = orig + new Vector2(Main.rand.NextFloat(-1, 1) * 40 * 16, Main.rand.NextFloat(-1, 1) * 40 * 16);
                        Projectile.NewProjectile(NPC.GetSource_FromAI(), spawn, Vector2.Zero, ModContent.ProjectileType<PhantasmalPortal>(), damage, 0f, Main.myPlayer, ai1: phase2 ? 1f : 0f, ai2: NPC.target);
                    }
                }

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
            NPC.netOffset *= 0f;
            inertia = DefaultInertia * 0.1f;

            ChargeSubphase subphase = (ChargeSubphase)AI_AttackProgress;
            switch (subphase)
            {
                case ChargeSubphase.Initial:

                    if (AI_Timer <= 0)
                    {
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            maxChargeAttackCount = Main.rand.Next(GetDifficultyInfo(DifficultyInfo.ChargeAttackCountMin), GetDifficultyInfo(DifficultyInfo.ChargeAttackCountMax) + 1);

                            if (portalCharge)
                            {
                                maxChargeAttackCount -= 2;
                                maxPortalAttackCount = Main.rand.Next(GetDifficultyInfo(DifficultyInfo.PortalChargeAttackCountMin), GetDifficultyInfo(DifficultyInfo.PortalChargeAttackCountMax) + 1);
                            }

                            NPC.netUpdate = true;
                        }

                        AI_AttackProgress = portalCharge ? (int)ChargeSubphase.FindInitialPortalPosition : (int)ChargeSubphase.BeginRegularCharge;
                        AI_Timer = 60;
                    }
                    else
                    {
                        FloatTowardsTarget(player);
                    }
                    break;
                case ChargeSubphase.FindInitialPortalPosition:

                    if (bigPortal.visible)
                        bigPortal = new BigPortalInfo();
                    if (bigPortal2.visible)
                        bigPortal2 = new BigPortalInfo();

                    //Wait until initial wait is done
                    if (AI_Timer <= 0)
                    {
                        AI_AttackProgress++;

                        if (portalCharge)
                        {
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
                    break;
                case ChargeSubphase.SpawnInitialPortal:

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

                    break;
                case ChargeSubphase.ShrinkSelfAndPortal:

                    //Shrink (factor of 4.3851 makes it reach 0.01 in around 60 ticks, 12.2753 ~= 20 ticks)
                    const float epsilon = 0.01f;
                    if (NPC.scale > epsilon)
                        UpdateScale(Utility.ScaleLogarithmic(NPC.scale, 0f, portalAttackCount == 0 ? 4.3851f : 12.2753f, 1f / 60f));

                    targetAlpha = 255f * (1f - NPC.scale);

                    if (AI_Timer <= 0)
                    {
                        //Shrink the portal, but a bit slower
                        bigPortal.scale = Utility.ScaleLogarithmic(bigPortal.scale, 0f, portalAttackCount == 0 ? 2.7219f : 9.2153f, 1f / 60f);
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

                        if (portalAttackCount == 0)
                            SoundEngine.PlaySound(SoundID.Zombie105 with { Pitch = -0.2f }, NPC.Center); //Cultist laugh sound

                        //Wait for a random amount of time
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            AI_Timer = Main.rand.Next(GetDifficultyInfo(DifficultyInfo.PortalWaitTimeMin), GetDifficultyInfo(DifficultyInfo.PortalWaitTimeMax) + 1);
                            NPC.netUpdate = true;
                        }

                        movementTarget = null;
                    }

                    break;
                case ChargeSubphase.FindPortalPositions:

                    if (AI_Timer == 1 && Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        portalOffset = Main.rand.NextVector2Unit() * 30 * 16;
                        NPC.netUpdate = true;
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

                    break;
                case ChargeSubphase.GrowSourcePortal:

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

                    break;
                case ChargeSubphase.DashFromPortal:
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

                    if (AI_Timer <= 0 && NPC.scale == 1f)
                    {
                        AI_AttackProgress++;

                        float chargeSpeed = GetDifficultyScaling(DifficultyScale.PortalChargeSpeed);
                        AI_Timer = GetDifficultyInfo(DifficultyInfo.PortalChargeSlowdownDelay);

                        NPC.Center = bigPortal.center;
                        NPC.velocity = NPC.DirectionTo(player.Center) * chargeSpeed;

                        SoundEngine.PlaySound(SoundID.ForceRoar with { Pitch = -0.2f }, NPC.position);

                        bigPortal2.visible = false;
                        bigPortal2.scale = 8f / 240f;

                        AI_AttackProgress = (int)ChargeSubphase.SlowDownAfterCharge;
                    }

                    break;
                case ChargeSubphase.BeginRegularCharge:

                    if (AI_Timer <= 0)
                    {
                        float chargeSpeed = GetDifficultyScaling(DifficultyScale.ChargeSpeed);
                        NPC.velocity = NPC.DirectionTo(player.Center + player.velocity) * chargeSpeed;
                        zAxisLerpStrength = DefaultZAxisLerpStrength * 6f;
                        SetTargetZAxisRotation(player, out _);

                        SoundEngine.PlaySound(SoundID.ForceRoar with { Pitch = -0.2f }, NPC.position);

                        AI_Timer = GetDifficultyInfo(DifficultyInfo.ChargeSlowdownDelay);

                        if (chargeAttackCount >= maxChargeAttackCount && portalCharge)
                        {
                            chargeAttackCount = 0;
                            portalAttackCount++;
                            AI_AttackProgress = (int)ChargeSubphase.DashIntoPortal;

                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                maxChargeAttackCount = Main.rand.Next(GetDifficultyInfo(DifficultyInfo.ChargeAttackCountMin), GetDifficultyInfo(DifficultyInfo.ChargeAttackCountMax) + 1) - 2;
                                NPC.netUpdate = true;
                            }
                        }
                        else
                        {
                            chargeAttackCount++;
                            AI_AttackProgress = (int)ChargeSubphase.SlowDownAfterCharge;
                        }
                    }
                    else
                    {
                        float distanceSQ = NPC.DistanceSQ(player.Center);
                        if (AI_Timer <= 1 && (distanceSQ > 60 * 16 * 60 * 16 || distanceSQ < 4 * 16 * 4 * 16))
                            AI_Timer++;

                        zAxisLerpStrength = DefaultZAxisLerpStrength * 2.8f;
                        float speed = GetDifficultyScaling(DifficultyScale.ChargeTargetSpeed) + (1f - AI_Timer / (float)GetDifficultyInfo(DifficultyInfo.ChargeWaitTime)) * GetDifficultyScaling(DifficultyScale.FloatTowardsTargetSpeed);

                        if (chargeAttackCount == 0)
                            FloatTowardsTarget(player);
                        else
                            FloatTowardsTarget(player, targetSpeed: speed);
                    }

                    break;
                case ChargeSubphase.SlowDownAfterCharge:

                    if (AI_Timer <= 0)
                    {
                        //Charge has ended, slow the boss down

                        if (portalAttackCount >= maxPortalAttackCount)
                        {
                            NPC.velocity *= 1f - (GetDifficultyScaling(DifficultyScale.PortalChargeSlowdownFactor) / 60f);

                            if (Math.Abs(NPC.velocity.X) < GetDifficultyScaling(DifficultyScale.PortalChargeTargetSpeed) && Math.Abs(NPC.velocity.Y) <= GetDifficultyScaling(DifficultyScale.PortalChargeTargetSpeed))
                                NPC.velocity = Vector2.Zero;

                            if (NPC.velocity == Vector2.Zero && !bigPortal.visible)
                            {
                                SetAttack(AttackState.DoNothing);
                                portalAttackCount = 0;
                                portalCharge = false;
                            }
                        }
                        else
                        {
                            NPC.velocity *= 1f - (GetDifficultyScaling(DifficultyScale.ChargeSlowdownFactor) / 60f);

                            if (Math.Abs(NPC.velocity.X) < GetDifficultyScaling(DifficultyScale.ChargeTargetSpeed) && Math.Abs(NPC.velocity.Y) < GetDifficultyScaling(DifficultyScale.ChargeTargetSpeed))
                            {
                                if (chargeAttackCount >= maxChargeAttackCount && !portalCharge)
                                {
                                    chargeAttackCount = 0;
                                    SetAttack(AttackState.DoNothing);
                                }
                                else
                                {
                                    AI_Timer = GetDifficultyInfo(DifficultyInfo.ChargeWaitTime);
                                    AI_AttackProgress = (int)ChargeSubphase.BeginRegularCharge;
                                }
                            }
                        }
                    }

                    //First portal disappears once the boss leaves within 22 update ticks of its center
                    float portalActiveDist = GetDifficultyScaling(DifficultyScale.PortalChargeSpeed) * 22;
                    if (NPC.DistanceSQ(bigPortal.center) > portalActiveDist * portalActiveDist || AI_Timer <= 0)
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

                    break;

                case ChargeSubphase.DashIntoPortal:

                    //Second portal appears once the boss is within 22 update ticks of its center
                    float portal2ActiveDist = GetDifficultyScaling(DifficultyScale.PortalChargeSpeed) * 22;
                    if (AI_Timer < 0 && NPC.DistanceSQ(bigPortal2.center) <= portal2ActiveDist * portal2ActiveDist)
                        bigPortal2.visible = true;

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
                            bigPortal2.center = NPC.Center + Vector2.Normalize(NPC.velocity) * (portal2ActiveDist + 3 * 16);
                    }
                    else
                    {
                        //Make sure the boss snaps to the center of the portal before repeating the logic
                        float distance = NPC.DistanceSQ(bigPortal2.center);
                        if (distance < portalEnterDist * portalEnterDist)
                        {
                            UpdateScale(1f);

                            NPC.Center = bigPortal2.center;
                            NPC.velocity = Vector2.Zero;

                            targetAlpha = 0;

                            movementTarget = null;

                            if (bigPortal2.scale >= 1f)
                            {
                                AI_AttackProgress = (int)ChargeSubphase.ShrinkSelfAndPortal;
                                Utils.Swap(ref bigPortal, ref bigPortal2);
                            }
                        }
                        else if (distance < portalTargetDist * portalTargetDist)
                        {
                            //Float to the center
                            movementTarget = bigPortal2.center;
                            inertia = DefaultInertia * 0.1f;

                            float oldZ = targetZAxisRotation;
                            FloatTowardsTarget(player, minimumDistanceThreshold: 0);
                            targetZAxisRotation = oldZ;
                        }
                    }

                    break;
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

        private void FloatTowardsTarget(Player player, float minimumDistanceThreshold = 5 * 16, float? targetSpeed = null)
        {
            //Look at the player and float around
            SetTargetZAxisRotation(player, out Vector2 targetCenter);

            float speedX = targetSpeed.HasValue ? targetSpeed.Value : GetDifficultyScaling(DifficultyScale.FloatTowardsTargetSpeed);
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
                float progress = MathHelper.Clamp((float)(AI_Timer - FadeIn_FadeInTime) / (FadeIn_LaughTime - FadeIn_FadeInTime), 0f, 1f);
                return Color.Lerp(drawColor, new Color(157, 255, 156).WithOpacity(progress), 1f - progress) * (1f - targetAlpha / 255f);
            }

            if (AI_Attack == AttackState.FadeAway)
            {
                float progress = MathHelper.Clamp((float)AI_Timer / FadeAway_FadeOutTime, 0f, 1f);
                return Color.Lerp(drawColor, new Color(157, 255, 156).WithOpacity(progress), 1f - progress) * (1f - targetAlpha / 255f);
            }

            return drawColor * (1f - targetAlpha / 255f);
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            if (Main.rand.NextBool(20))
            {
                // Can't use info.DamageSource = PlayerDeathReason.ByCustomReason(...) here:
                // HurtInfo is a value type and a DamageSource reassignment won't be reflected outside this method
                // Called on the hit player client, will be synced and message will be broadcasted by the server 
                info.DamageSource.SourceCustomReason = this.GetLocalization("FunnyDeathMessage").Format(target.name);
            }
        }

        public override bool? CanBeHitByItem(Player player, Item item)
            => CanBeHitByThing(player.GetSwungItemHitbox());

        public override bool? CanBeHitByProjectile(Projectile projectile)
        {
            if (projectile.hostile && !projectile.friendly)
                return null;

            GetHitboxRects(out Rectangle head, out Rectangle jaw);
            return projectile.Colliding(projectile.Hitbox, head) || projectile.Colliding(projectile.Hitbox, jaw) ? null : (bool?)false;
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

        private void DisplayDebugInfo()
        {
            Main.NewText("AttackState: " + AI_Attack.ToString());
            Main.NewText("AttackProgress: " + (AI_Attack == AttackState.Charge ? ((ChargeSubphase)AI_AttackProgress).ToString() : AI_AttackProgress.ToString()));
            Main.NewText("AI_Timer: " + AI_Timer);

            if (AI_Attack == AttackState.Charge)
            {
                Main.NewText($"Charge: {chargeAttackCount}/{maxChargeAttackCount}");
                Main.NewText($"PortalCharge: {portalAttackCount}/{maxPortalAttackCount}");
                Main.NewText("\n\n");
            }
            else
            {
                Main.NewText("\n\n\n");
            }
        }
    }
}