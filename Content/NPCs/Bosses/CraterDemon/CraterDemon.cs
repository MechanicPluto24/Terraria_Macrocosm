using Macrocosm.Common.DataStructures;
using Macrocosm.Common.Systems;
using Macrocosm.Common.Utils;
using Macrocosm.Content.Buffs.Debuffs;
using Macrocosm.Content.Dusts;
using Macrocosm.Content.Items.Consumables.BossBags;
using Macrocosm.Content.Items.Currency;
using Macrocosm.Content.Items.Materials;
using Macrocosm.Content.Items.Placeable.Trophies;
using Macrocosm.Content.Items.Vanity.BossMasks;
using Macrocosm.Content.Items.Weapons.Magic;
using Macrocosm.Content.Items.Weapons.Ranged;
using Macrocosm.Content.Items.Weapons.Summon;
using Macrocosm.Content.NPCs.Friendly.TownNPCs;
using Macrocosm.Content.NPCs.Global;
using Macrocosm.Content.Projectiles.Hostile;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
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
            public int initialTimer;
            public bool resetAnimationCounter;
        }

        private struct BigPortalInfo
        {
            public Vector2 center;
            public float scale;
            public float alpha;
            public bool visible;
            public float rotation;
            public bool fast;

            public void Write(BinaryWriter writer)
            {
                writer.WriteVector2(center);
                writer.Write(scale);
                writer.Write(alpha);
                writer.Write(rotation);

                BitsByte bb = new(visible, fast);
                writer.Write(bb);
            }

            public void Read(BinaryReader reader)
            {
                center = reader.ReadVector2();
                scale = reader.ReadSingle();
                alpha = reader.ReadSingle();
                rotation = reader.ReadSingle();

                BitsByte bb = reader.ReadByte();
                bb.Retrieve(ref visible, ref fast);
            }

            public void Update()
            {
                if (!visible)
                    return;

                float rotationsPerSecond = fast ? 1.5f : 0.9f;
                rotation -= MathHelper.ToRadians(rotationsPerSecond * 6f);
            }
        }

        public ref float AI_Attack => ref NPC.ai[0];
        public ref float AI_Timer => ref NPC.ai[1];
        public ref float AI_AttackProgress => ref NPC.ai[2];
        public ref float AI_AnimationCounter => ref NPC.ai[3];

        public const int Phase2Transition = -3;
        public const int FadeIn = -2;
        public const int FadeAway = -1;
        public const int Attack_DoNothing = 0;
        public const int Attack_SummonMeteors = 1;
        public const int Attack_SummonCraterImps = 2;
        public const int Attack_ChargeAtPlayer = 3;
        public const int Attack_SummonPhantasmals = 4;
        public const int Attack_PostCharge = 5;

        private const int Attack_ChargeAtPlayer_RepeatStart = 2;
        private const int Attack_ChargeAtPlayer_RepeatSubphaseCount = 5;

        private static readonly AttackInfo[] attacks = new AttackInfo[]{
			// DoNothing
			new AttackInfo(){
                initialProgress = null,
                initialTimer = 90,
                resetAnimationCounter = false
            },
			// SummonMeteors
			new AttackInfo(){
                initialProgress = null,
                initialTimer = PortalTimerMax,
                resetAnimationCounter = true
            },
			// SummonCraterImps
			new AttackInfo(){
                initialProgress = NPC => NPC.CountAliveCraterImps(),
                initialTimer = 8 * 60
            },
			// ChargeAtPlayer
			new AttackInfo(){
                initialProgress = null,
                initialTimer = 70  //Wait before spawning first portal
			},
			// SummonPhantasmals
			new AttackInfo(){
                initialProgress = null,
                initialTimer = 3 * 60
            },
			// PostCharge
			new AttackInfo(){
                initialProgress = null,
                initialTimer = 45
            }
        };

        private enum DifficultyScale
        {
            AttackDurationScaling,     // The initial AI timer scaling, based on AttackInfo, when switching attacks
            MeteorPortalIdleTime,      // The idle time scaling on meteor portal attack
            PortalChargeVelocity,      // Charge attack velocity value 
            FloatTowardsTargetVelocity // Velocity towards target when not doing a special attack
        }

        private enum DifficultyInfo
        {
            SuitBreachTime,           // Suit breach debuff time 

            Phase2AnimDefenseBoost,   // Extra defense during phase2 animation sequence

            MeteorPortalCount,        // Number of meteor portals 
            MeteorPortalDefenseBoost, // Extra defense during meteor portal attack

            CraterImpMaxCount,        // Max number of Crater Imp minions that can exist

            PortalWaitTimeMin,        // Min wait time between portal attacks 
            PortalWaitTimeMax,        // Max wait time between portal attacks 
            PortalSecondTime,         // Time to spawn the second portal 
            PortalAttackCountMin,     // Min number of consecutive portal attacks 
            PortalAttackCountMax,     // Max number of consecutive portal attacks 

            PhantasmalRepeatCountMin, // Min number of phantasmal attacks in a row
            PhantasmalRepeatCountMax, // Max number of phantasmal attacks in a row
            PhantasmalCountSmallMin,  // Min number of small phantasmal skulls in an attack 
            PhantasmalCountSmallMax,  // Max number of small phantasmal skulls in an attack 
            PhantasmalCountLargeMin,  // Min number of large phantasmal skulls in an attack 
            PhantasmalCountLargeMax,  // Max number of large phantasmal skulls in an attack 
        }

        private static readonly float[,] scaleInfo = new float[,]
        {	
			 // Scaled element on :             NM1, NM2, EM1, EM2, MM1, MM2, FTW1, FTW2
			 /*AttackDurationScaling,      */ { 1.25f, 1.1f, 1f, 0.8f, 0.75f, 0.65f, 0.6f, 0.55f }, 
			 /*MeteorPortalIdleTime,       */ { 1f, 0.9f, 0.75f, 0.65f, 0.5f, 0.45f, 0.4f, 0.3f }, 
			 /*PortalChargeVelocity,       */ { 17f, 19f, 24f, 26f, 30f, 32f, 36f, 40f },
			 /*FloatTowardsTargetVelocity, */ { 8f, 8.5f, 9f, 9.5f, 10f, 10.5f, 11f, 11.5f }
        };

        private static readonly int[,] difficultyInfo = new int[,]
        {
		     // Difficulty value on :         NM1, NM2, EM1, EM2, MM1, MM2, FTW1, FTW2
			 /*SuitBreachTime,           */ { 120, 120, 240, 240, 260, 260, 300, 300 },  

			 /*Phase2AnimDefenseBoost,   */ { 40, 40, 50, 50, 50, 50, 60, 60 },
			 
			 /*MeteorPortalCount,        */ { 2, 2, 3, 3, 4, 4, 5, 5 },                           
			 /*MeteorPortalDefenseBoost, */ { 40, 40, 50, 50, 50, 50, 60, 60 },

			 /*CraterImpMaxCount,        */ { 3, 3, 4, 4, 4, 4, 5, 5 },      

			 /*PortalWaitTimeMin,        */ { 80, 70, 40, 30, 20, 10, 5, 0 },                   
			 /*PortalWaitTimeMax,        */ { 160, 150, 80, 70, 40, 30, 10, 5 },                
			 /*PortalSecondTime,         */ { 60, 55, 35, 30, 25, 20, 15, 10 },                 
			 /*PortalAttackCountMin,     */ { 2, 3, 4, 5, 4, 5, 6, 7 },                         
			 /*PortalAttackCountMax,     */ { 4, 5, 8, 9, 8, 9, 10, 11 },

			 /*PhantasmalRepeatCountMin  */ { 1, 1, 1, 1, 1, 1, 1, 1},
			 /*PhantasmalRepeatCountMax	 */ { 3, 3, 3, 3, 3, 3, 3, 3},
			 /*PhantasmalCountSmallMin,  */ { 2, 2, 2, 2, 2, 2, 2, 2},
			 /*PhantasmalCountSmallMax,  */	{ 3, 3, 3, 3, 3, 3, 3 ,3},
			 /*PhantasmalCountLargeMin,  */	{ 1, 1, 1, 1, 1, 1, 1, 1},
			 /*PhantasmalCountLargeMax,  */ { 2, 2, 2, 2, 2, 2, 2, 2}
        };

        /// <summary> Return the difficulty index (gamemode, phase): NM1, NM2, EM1, EM2, MM1, MM2, FTW1, FTW2  </summary>
        private int GetDifficultyIndex()
        {
            int difficultyIndex = Main.getGoodWorld ? 6 : // FTW
                                  Main.masterMode ? 4 : // MM
                                  Main.expertMode ? 2 : // EM
                                                      0;  // NM
            if (phase2)
                difficultyIndex += 1;

            return difficultyIndex;
        }

        private float GetDifficultyScaling(DifficultyScale scaleType)
            => scaleInfo[(int)scaleType, GetDifficultyIndex()];

        private int GetDifficultyInfo(DifficultyInfo infoType)
            => difficultyInfo[(int)infoType, GetDifficultyIndex()];

        private void SetAttack(int attack)
        {
            var info = attacks[attack];

            AI_Attack = attack;

            AI_AttackProgress = info.initialProgress?.Invoke(this) ?? 0;

            AI_Timer = info.initialTimer;
            AI_Timer *= GetDifficultyScaling(DifficultyScale.AttackDurationScaling);

            if (info.resetAnimationCounter)
                AI_AnimationCounter = -1;

            if (attack == Attack_DoNothing)
            {
                movementTarget = null;
                zAxisLerpStrength = DefaultZAxisLerpStrength;
            }
        }

        public float GetScaledInitialTimer()
        {
            if (AI_Attack < 0)
                return 1f;

            return attacks[(int)AI_Attack].initialTimer * GetDifficultyScaling(DifficultyScale.AttackDurationScaling);
        }

        public float GetAttackProgress()
        {
            if (AI_Attack < 0)
                return 1f;

            return AI_Timer / GetScaledInitialTimer();
        }

        private bool phase2 = false;

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
        private int oldAttack = 1;

        private Vector2? movementTarget;

        private BigPortalInfo bigPortal;
        private BigPortalInfo bigPortal2;

        private Vector2 portalTarget;
        private Vector2 portalOffset;
        private int portalAttackCount;

        private int phantasmalRepeatCount;

        public const int PortalTimerMax = (int)(4f * 60 + 1.5f * 60 + 24);  //Portal spawning leadup + time portals are active before they shrink

        public const int Phase2TimerMax = 150;

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 6;
            NPCID.Sets.TrailCacheLength[NPC.type] = 5;
            NPCID.Sets.TrailingMode[NPC.type] = 0;
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
            //npcLoot.Add(ItemDropRule.MasterModeCommonDrop(ModContent.ItemType<CraterDemonRelic>()));
            //npcLoot.Add(ItemDropRule.MasterModeDropOnAllPlayers(ModContent.ItemType<CraterDemonMMPet>(), 4));

            // BELOW: for normal mode, same as boss bag (excluding Broken Hero Shield)
            LeadingConditionRule notExpertRule = new(new Conditions.NotExpert());
            notExpertRule.OnSuccess(ItemDropRule.Common(ModContent.ItemType<CraterDemonMask>(), 7));

            notExpertRule.OnSuccess(ItemDropRule.Common(ModContent.ItemType<Moonstone>(), 1, 30, 60));
            notExpertRule.OnSuccess(ItemDropRule.Common(ModContent.ItemType<DeliriumPlating>(), 1, 30, 90));

            notExpertRule.OnSuccess(ItemDropRule.OneFromOptions(1,
                ModContent.ItemType<CalcicCane>(),
                ModContent.ItemType<Cruithne>(),
                ModContent.ItemType<ImbriumJewel>()
                /*, ModContent.ItemType<ChampionsBlade>() */
                ));

            npcLoot.Add(notExpertRule);
        }

        public override void OnKill()
        {
            if (!WorldDataSystem.Instance.DownedCraterDemon)
                NPC.NewNPC(NPC.GetSource_NaturalSpawn(), (int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<MoonChampion>());

            // This is only used currently for lantern night, which we don't want in our subworlds
            //NPC.SetEventFlagCleared(ref DownedBossSystem.downedCraterDemon, -1);

            // Similar events can still happen with our event listener system
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

        const float ZAxisRotationThreshold = 25f;

        public override void SendExtraAI(BinaryWriter writer)
        {
            bool hasCustomTarget = movementTarget != null;
            BitsByte bb = new(phase2, spawned, ignoreRetargetPlayer, hadNoPlayerTargetForLongEnough, hasCustomTarget);
            writer.Write(bb);

            writer.Write(targetAlpha);
            writer.Write(targetZAxisRotation);
            writer.Write(zAxisRotation);

            bigPortal.Write(writer);
            bigPortal2.Write(writer);

            if (hasCustomTarget)
                writer.WriteVector2(movementTarget.Value);

            writer.Write(oldAttack);
            writer.WriteVector2(portalTarget);
            writer.WriteVector2(portalOffset);

            writer.Write((byte)portalAttackCount);

            writer.Write((byte)phantasmalRepeatCount);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            bool hasCustomTarget = false;
            BitsByte bb = reader.ReadByte();
            bb.Retrieve(ref phase2, ref spawned, ref ignoreRetargetPlayer, ref hadNoPlayerTargetForLongEnough, ref hasCustomTarget);

            targetAlpha = reader.ReadSingle();
            targetZAxisRotation = reader.ReadSingle();
            zAxisRotation = reader.ReadSingle();

            bigPortal.Read(reader);
            bigPortal2.Read(reader);

            movementTarget = hasCustomTarget ? (Vector2?)reader.ReadVector2() : null;

            oldAttack = reader.ReadInt32();
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

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            //Draw portals
            Texture2D portal = ModContent.Request<Texture2D>("Macrocosm/Content/NPCs/Bosses/CraterDemon/BigPortal").Value;

            DrawBigPortal(spriteBatch, portal, bigPortal);
            DrawBigPortal(spriteBatch, portal, bigPortal2);

            if (AI_Attack == Attack_SummonMeteors && Math.Abs(NPC.velocity.X) < 0.1f && Math.Abs(NPC.velocity.Y) < 0.1f)
                return true;

            if (AI_Attack == Attack_ChargeAtPlayer && AI_AttackProgress > 2 && NPC.alpha >= 160)
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

        private SpriteBatchState state1, state2;
        public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D glowmask = ModContent.Request<Texture2D>("Macrocosm/Content/NPCs/Bosses/CraterDemon/CraterDemon_Glow").Value;
            Texture2D star = ModContent.Request<Texture2D>(Macrocosm.TextureAssetsPath + "Star1").Value;

            spriteBatch.Draw(glowmask, NPC.position - screenPos + new Vector2(0, 4), NPC.frame, (Color)GetAlpha(Color.White), NPC.rotation, Vector2.Zero, NPC.scale, SpriteEffects.None, 0f);

            if (AI_Attack == Phase2Transition && AI_AttackProgress >= 1)
            {
                float progress = (1f * (1f - (AI_Timer / Phase2TimerMax)));

                float starScale = NPC.scale * 0.3f * (progress < 0.5f ? progress : 1f - progress);
                float rotation = NPC.rotation + progress * 0.4f;

                state1.SaveState(spriteBatch);

                spriteBatch.End();
                spriteBatch.Begin(BlendState.Additive, state1);

                spriteBatch.Draw(star, NPC.Center - screenPos + new Vector2(30, -8), null, new Color(157, 255, 156), rotation, star.Size() / 2, starScale, SpriteEffects.None, 0f);

                spriteBatch.End();
                spriteBatch.Begin(state1);
            }

            if (phase2)
            {
                //var state = spriteBatch.SaveState();
                //
                //spriteBatch.End();
                //spriteBatch.Begin(BlendState.NonPremultiplied, state);
                //
                //spriteBatch.Draw(glowmaskPhase2, NPC.position - screenPos + new Vector2(0, 4), NPC.frame, Color.White, NPC.rotation, Vector2.Zero, NPC.scale, SpriteEffects.None, 0f);
                //
                //spriteBatch.End();
                //spriteBatch.Begin(state);
            }
        }

        private void DrawBigPortal(SpriteBatch spriteBatch, Texture2D texture, BigPortalInfo info)
        {
            if (!info.visible)
                return;

            spriteBatch.Draw(texture, info.center - Main.screenPosition, null, Color.White * info.alpha * 0.4f, (-info.rotation) * 0.65f, texture.Size() / 2f, info.scale * 1.2f, SpriteEffects.FlipHorizontally, 0);
            spriteBatch.Draw(texture, info.center - Main.screenPosition, null, Color.White * info.alpha * 0.8f, info.rotation, texture.Size() / 2f, info.scale, SpriteEffects.None, 0);

            state2.SaveState(spriteBatch);
            spriteBatch.End();
            spriteBatch.Begin(BlendState.Additive, state2);

            spriteBatch.Draw(texture, info.center - Main.screenPosition, null, Color.White.WithOpacity(0.6f), info.rotation * 4f, texture.Size() / 2f, info.scale * 0.85f, SpriteEffects.None, 0);

            spriteBatch.End();
            spriteBatch.Begin(state2);

            if (info.scale > 0.9f && Vector2.Distance(info.center, NPC.Center) > 60f)
                SpawnPortalDusts(info);

            Lighting.AddLight(info.center, new Vector3(30, 255, 105) / 255 * info.scale * 3f);
        }

        private void SpawnPortalDusts(BigPortalInfo info)
        {
            for (int i = 0; i < 30; i++)
            {
                int type = ModContent.DustType<PortalLightGreenDust>();
                Vector2 rotVector1 = Vector2.UnitY.RotatedByRandom(MathHelper.TwoPi);
                Dust lightDust = Main.dust[Dust.NewDust(info.center - rotVector1 * 30f, 0, 0, type)];
                lightDust.noGravity = true;
                lightDust.position = info.center - rotVector1 * Main.rand.Next(40, 80) * info.scale;
                lightDust.velocity = rotVector1.RotatedBy(MathHelper.PiOver2) * 6f;
                lightDust.scale = 0.5f + Main.rand.NextFloat();
                lightDust.fadeIn = 0.5f;
                lightDust.customData = info.center;
            }
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

            switch ((int)AI_Attack)
            {
                case FadeIn:
                    if (AI_Timer > 0 && AI_Timer <= 100 && AI_AnimationCounter % 16 < 8)
                        set++;
                    break;

                case Phase2Transition:
                    if (AI_Timer >= Math.Floor(Phase2TimerMax * 0.5f) && AI_Timer <= Math.Floor(Phase2TimerMax * 0.9f))
                        set++;
                    else if (AI_AnimationCounter % 26 < 13)
                        set++;
                    break;

                case Attack_DoNothing:
                case Attack_SummonCraterImps:
                    if (AI_AnimationCounter % 26 < 13)
                        set++;
                    break;

                case Attack_SummonMeteors:
                case Attack_SummonPhantasmals:
                    //Open mouth
                    set++;
                    break;

                case Attack_ChargeAtPlayer:
                    if (AI_AttackProgress > Attack_ChargeAtPlayer_RepeatStart)
                    {
                        int repeatRelative = ((int)AI_AttackProgress - Attack_ChargeAtPlayer_RepeatStart) % Attack_ChargeAtPlayer_RepeatSubphaseCount;
                        int repeatCount = ((int)AI_AttackProgress - Attack_ChargeAtPlayer_RepeatStart) / Attack_ChargeAtPlayer_RepeatSubphaseCount;

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

                case Attack_PostCharge:
                    if (NPC.velocity != Vector2.Zero)
                        set++;
                    break;
            }

            return set;
        }

        public const int Animation_LookLeft_JawClosed = 0;
        public const int Animation_LookLeft_JawOpen = 1;
        public const int Animation_LookFront_JawClosed = 2;
        public const int Animation_LookFront_JawOpen = 3;
        public const int Animation_LookRight_JawClosed = 4;
        public const int Animation_LookRight_JawOpen = 5;

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

            if (!phase2 && NPC.life < NPC.lifeMax / 2)
            {
                AI_Attack = Phase2Transition;
                AI_Timer = Phase2TimerMax;
                AI_AttackProgress = 0;
                targetZAxisRotation = 0f;
                phase2 = true;
            }

            if (!spawned)
            {
                //Laugh after a second
                if (AI_Timer == 0)
                {
                    targetZAxisRotation = 0f;
                    zAxisRotation = 0f;
                    targetAlpha = 255f;

                    ignoreRetargetPlayer = true;

                    AI_Timer = 60 + 100;
                    AI_Attack = FadeIn;
                    NPC.TargetClosest();

                    NPC.netUpdate = true;
                }
                else if (AI_Timer <= 100)
                {
                    spawned = true;
                    targetAlpha = 0f;

                    //Cultist laugh sound
                    if (!Main.dedServ)
                        SoundEngine.PlaySound(SoundID.Zombie105 with { Pitch = -0.2f }, NPC.Center);

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
                    AI_Attack = FadeAway;
                    AI_Timer = -1;
                    AI_AttackProgress = -1;

                    hadNoPlayerTargetForLongEnough = true;

                    NPC.netUpdate = true;
                }
                else if (hadNoPlayerTargetForLongEnough)
                {
                    hadNoPlayerTargetForLongEnough = false;

                    //Start back in the idle phase
                    SetAttack(Attack_DoNothing);

                    NPC.netUpdate = true;
                }
            }

            NPC.defense = NPC.defDefense;

            switch ((int)AI_Attack)
            {
                case FadeIn:
                    //Do the laughing animation
                    if (AI_Timer > 0 && AI_Timer <= 100)
                    {
                        NPC.velocity *= 1f - 3f / 60f;
                    }
                    else if (AI_Timer <= 0)
                    {
                        //Transition to the next subphase
                        SetAttack(Attack_DoNothing);

                        NPC.dontTakeDamage = false;
                        ignoreRetargetPlayer = false;
                    }
                    else
                    {
                        NPC.velocity = new Vector2(0, 1.5f);

                        SpawnDusts();
                    }

                    break;

                case FadeAway:
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
                    break;

                case Attack_DoNothing:
                    inertia = DefaultInertia;
                    zAxisLerpStrength = DefaultZAxisLerpStrength;

                    FloatTowardsTarget(player);

                    if (AI_Timer <= 0 && Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        int attack;
                        int watchdog = 10;
                        int maxAttack = phase2 ? Attack_SummonPhantasmals : Attack_ChargeAtPlayer;

                        //Prevent the same attack from being rolled twice in a row...
                        do
                        {
                            attack = Main.rand.Next(Attack_SummonMeteors, maxAttack + 1);

                            //.. except when debugging one attack
                            if (watchdog-- <= 0) break;

                        } while (attack == oldAttack);

                        SetAttack(attack);
                        NPC.netUpdate = true;
                    }
                    break;

                case Attack_SummonMeteors:
                    NPC.defense = NPC.defDefense + GetDifficultyInfo(DifficultyInfo.MeteorPortalDefenseBoost);

                    //Face forwards
                    targetZAxisRotation = 0f;

                    NPC.velocity *= 1f - 4.67f / 60f;

                    if (Math.Abs(NPC.velocity.X) < 0.02f)
                        NPC.velocity.X = 0f;
                    if (Math.Abs(NPC.velocity.Y) < 0.02f)
                        NPC.velocity.Y = 0f;

                    int max = (int)(PortalTimerMax * GetDifficultyScaling(DifficultyScale.MeteorPortalIdleTime));

                    if ((int)AI_Timer == max - 1)
                    {
                        if (!Main.dedServ)
                            SoundEngine.PlaySound(SoundID.Zombie99, NPC.Center);
                    }
                    else if ((int)AI_Timer == max - 24)
                    {
                        //Wait until the animation frame changes to the one facing forwards
                        if (GetAnimationSetFrame() == Animation_LookFront_JawOpen)
                        {
                            if (!Main.dedServ)
                                SoundEngine.PlaySound(SoundID.Zombie93, NPC.Center);

                            AI_AttackProgress++;
                        }
                        else
                            AI_Timer++;
                    }

                    if (AI_AttackProgress == 1)
                    {
                        //Spawn portals near and above the player
                        Vector2 orig = player.Center - new Vector2(0, 15 * 16);

                        int count = GetDifficultyInfo(DifficultyInfo.MeteorPortalCount);

                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            for (int i = 0; i < count; i++)
                            {
                                Vector2 spawn = orig + new Vector2(Main.rand.NextFloat(-1, 1) * 40 * 16, Main.rand.NextFloat(-1, 1) * 6 * 16);

                                int portalID = Projectile.NewProjectile(NPC.GetSource_FromAI(), spawn, Vector2.Zero, ModContent.ProjectileType<MeteorPortal>(), Utility.TrueDamage(Main.expertMode ? 140 : 90), 0f, Main.myPlayer, ai1: phase2 ? 1f : 0f);
                                Main.projectile[portalID].netUpdate = true;
                            }
                        }

                        AI_AttackProgress++;
                    }
                    else if (AI_AttackProgress == 2 && AI_Timer <= 0)
                        SetAttack(Attack_DoNothing);

                    break;

                case Attack_SummonCraterImps:
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

                            //Exhale sound
                            if (!Main.dedServ)
                                SoundEngine.PlaySound(SoundID.Zombie93, NPC.Center);

                            AI_AttackProgress++;
                        }
                    }
                    else if (AI_Timer <= 0)
                    {
                        //Go to next attack immediately
                        SetAttack(Attack_DoNothing);
                    }
                    else if (CountAliveCraterImps() == 0)
                    {
                        //All the minions have been killed.  Transition to the next subphase immediately
                        AI_Timer = 1;
                    }

                    break;

                case Attack_ChargeAtPlayer:

                    inertia = DefaultInertia * 0.1f;

                    float chargeVelocity = GetDifficultyScaling(DifficultyScale.PortalChargeVelocity);

                    int repeatRelative = AI_AttackProgress >= Attack_ChargeAtPlayer_RepeatStart
                        ? ((int)AI_AttackProgress - Attack_ChargeAtPlayer_RepeatStart) % Attack_ChargeAtPlayer_RepeatSubphaseCount
                        : -1;
                    int repeatCount = AI_AttackProgress >= Attack_ChargeAtPlayer_RepeatStart
                        ? ((int)AI_AttackProgress - Attack_ChargeAtPlayer_RepeatStart) / Attack_ChargeAtPlayer_RepeatSubphaseCount
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

                            if (repeatCount == 0 && !Main.dedServ)
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
                            SpawnBigPortal(player.Center + portalOffset, ref bigPortal, fast: true);
                            SpawnBigPortal(player.Center - portalOffset, ref bigPortal2, fast: true);
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

                        if (AI_Timer <= 0 && NPC.scale == 1f)
                        {
                            AI_AttackProgress++;
                            AI_Timer = GetDifficultyInfo(DifficultyInfo.PortalSecondTime);

                            NPC.Center = bigPortal.center;
                            NPC.velocity = NPC.DirectionTo(player.Center) * chargeVelocity;

                            if (!Main.dedServ)
                                SoundEngine.PlaySound(SoundID.ForceRoar with { Pitch = -0.2f }, NPC.position);

                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                portalAttackCount = Main.rand.Next(GetDifficultyInfo(DifficultyInfo.PortalAttackCountMin), GetDifficultyInfo(DifficultyInfo.PortalAttackCountMax) + 1);
                                NPC.netUpdate = true;
                            }

                            if (repeatCount >= portalAttackCount)
                            {
                                //Stop the repetition
                                bigPortal2.visible = false;
                                bigPortal2.scale = 8f / 240f;

                                SetAttack(Attack_PostCharge);
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
                    break;

                case Attack_PostCharge:
                    if (NPC.velocity == Vector2.Zero && !bigPortal.visible)
                        SetAttack(Attack_DoNothing);
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
                    break;

                case Attack_SummonPhantasmals:

                    FloatTowardsTarget(Main.player[NPC.target]);

                    Vector2 targetVelocity = (Main.player[NPC.target].Center - NPC.Center).SafeNormalize(Vector2.Zero) * 1.8f;
                    targetZAxisRotation = Math.Sign(targetVelocity.X);

                    if (AI_AttackProgress == 0 && (AI_Timer >= (int)GetScaledInitialTimer() - 1) && Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        phantasmalRepeatCount = Main.rand.Next(GetDifficultyInfo(DifficultyInfo.PhantasmalRepeatCountMin), GetDifficultyInfo(DifficultyInfo.PhantasmalRepeatCountMax) + 1);
                        NPC.netUpdate = true;
                    }

                    if (AI_AttackProgress % 2 == 0)
                    {
                        if (GetAnimationSetFrame() == Animation_LookLeft_JawOpen || GetAnimationSetFrame() == Animation_LookRight_JawOpen)
                            AI_AttackProgress++;
                        else
                            AI_Timer++;
                    }

                    if (AI_AttackProgress % 2 == 1)
                    {
                        if (AI_Timer > 1 && GetAttackProgress() < 0.5f)
                        {
                            NPC.velocity *= 1f - 3f / 60f;

                            float progress = 0.25f + 0.75f * GetAttackProgress();

                            for (int i = 0; i < 14; i++)
                            {
                                Dust dust = Dust.NewDustPerfect(NPC.Center, ModContent.DustType<PortalLightGreenDust>(), Scale: Main.rand.NextFloat(0.8f, 1.2f));

                                dust.position = NPC.Center + new Vector2(Main.rand.Next(120, 200) * Math.Sign(zAxisRotation), 65 + MathHelper.Lerp(Main.rand.NextFloat(-65f, 65f), 0f, 1f - progress));
                                dust.velocity = ((NPC.Center + new Vector2(20, 65)) - dust.position).SafeNormalize(Vector2.One) * Main.rand.Next(8, 12);
                                dust.noGravity = true;
                                dust.rotation = Utility.RandomRotation();

                                Vector2 rotVector1 = Vector2.UnitY.RotatedByRandom(MathHelper.TwoPi) * 3f;
                                Dust lightDust = Dust.NewDustPerfect((NPC.Center - rotVector1 * 30), ModContent.DustType<PortalLightGreenDust>(), Scale: Main.rand.NextFloat(0.8f, 1.2f));
                                lightDust.noGravity = true;
                                lightDust.position = NPC.Center + new Vector2(200 * Math.Sign(zAxisRotation), 65) - rotVector1 * Main.rand.Next(10, 25) * progress;
                                lightDust.velocity = rotVector1.RotatedBy(MathHelper.PiOver2) * 2.4f;
                                lightDust.scale = (0.8f + Main.rand.NextFloat());
                                lightDust.fadeIn = 0.5f;
                                lightDust.customData = NPC.Center + new Vector2(280 * Math.Sign(zAxisRotation), 65);
                            }
                        }
                        else if (AI_Timer <= 0)
                        {

                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                int numLarge = Main.rand.Next(GetDifficultyInfo(DifficultyInfo.PhantasmalCountLargeMin), GetDifficultyInfo(DifficultyInfo.PhantasmalCountLargeMax) + 1);
                                int numSmall = Main.rand.Next(GetDifficultyInfo(DifficultyInfo.PhantasmalCountSmallMin), GetDifficultyInfo(DifficultyInfo.PhantasmalCountSmallMax) + 1);

                                for (int i = 0; i < numLarge; i++)
                                {
                                    Vector2 position = NPC.Center + new Vector2(50 * Math.Sign(zAxisRotation), 50) + new Vector2(Main.rand.Next(10) * Math.Sign(zAxisRotation), Main.rand.Next(2));
                                    Vector2 velocity = targetVelocity.RotatedByRandom(MathHelper.PiOver4 / 2) * 7f;
                                    int damage = (int)((float)NPC.damage * 0.7f);
                                    int projId = Projectile.NewProjectile(NPC.GetSource_FromAI(), position, velocity, ModContent.ProjectileType<PhantasmalImpLarge>(), damage, 1f, ai2: player.whoAmI);
                                    Main.projectile[projId].netUpdate = true;
                                }

                                for (int i = 0; i < numSmall; i++)
                                {
                                    Vector2 position = NPC.Center + new Vector2(50 * Math.Sign(zAxisRotation), 50) + new Vector2(Main.rand.Next(10) * Math.Sign(zAxisRotation), Main.rand.Next(2));
                                    Vector2 velocity = targetVelocity.RotatedByRandom(MathHelper.PiOver4 / 2) * 8f;
                                    int damage = (int)((float)NPC.damage * 0.6f);
                                    int projId = Projectile.NewProjectile(NPC.GetSource_FromAI(), position, velocity, ModContent.ProjectileType<PhantasmalImpSmall>(), damage, 1f, ai2: player.whoAmI);
                                    Main.projectile[projId].netUpdate = true;
                                }
                            }

                            for (int i = 0; i < 80; i++)
                            {
                                Dust dust = Dust.NewDustDirect(NPC.Center + new Vector2(20 * Math.Sign(zAxisRotation), 50), 1, 1, ModContent.DustType<PortalLightGreenDust>(), Scale: 2.2f);
                                dust.velocity = (targetVelocity.SafeNormalize(Vector2.UnitX) * Main.rand.NextFloat(15f, 20f)).RotatedByRandom(MathHelper.PiOver4 * 0.4);
                                dust.noLight = false;
                                dust.alpha = 200;
                                dust.noGravity = true;
                            }

                            if (AI_AttackProgress / 2 < phantasmalRepeatCount)
                            {
                                AI_AttackProgress++;
                                AI_Timer = GetScaledInitialTimer();
                            }
                            else
                                SetAttack(Attack_DoNothing);
                        }
                    }

                    break;

                case Phase2Transition:

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
                        if (!Main.dedServ)
                        {
                            if (AI_Timer == Math.Floor(Phase2TimerMax * 0.8f))
                                SoundEngine.PlaySound(SoundID.ForceRoar with { Pitch = -0.6f }, NPC.position);

                            if (AI_Timer == Math.Floor(Phase2TimerMax * 0.75f))
                                SoundEngine.PlaySound(SoundID.ForceRoar with { Pitch = -0.8f }, NPC.position);

                            if (AI_Timer == Math.Floor(Phase2TimerMax * 0.7f))
                                SoundEngine.PlaySound(SoundID.ForceRoar with { Pitch = -0.4f }, NPC.position);

                            if (AI_Timer == Math.Floor(Phase2TimerMax * 0.6f))
                                SoundEngine.PlaySound(SoundID.Zombie105 with { Pitch = -0.2f }, NPC.position);
                        }

                        if (AI_Timer <= 0)
                        {
                            SetAttack(Attack_DoNothing);
                        }
                    }

                    break;
            }

            if (AI_Attack != Attack_DoNothing)
                oldAttack = (int)AI_Attack;

            AI_Timer--;
            AI_AnimationCounter++;

            if (AI_Attack != FadeAway && targetAlpha > 0)
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

            //We don't want sprite flipping
            NPC.spriteDirection = -1;

            bigPortal.Update();
            bigPortal2.Update();
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

            float speedX = GetDifficultyScaling(DifficultyScale.FloatTowardsTargetVelocity);
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

            if (!Main.dedServ)
            {
                //Play one of two sounds randomly
                if (Main.rand.NextFloat() < 0.02f / 60f)
                    SoundEngine.PlaySound(SoundID.Zombie96, NPC.Center);
                else if (Main.rand.NextFloat() < 0.02f / 60f)
                    SoundEngine.PlaySound(SoundID.Zombie5, NPC.Center);
            }
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

        private void SpawnBigPortal(Vector2 center, ref BigPortalInfo info, bool fast = false)
        {
            info.center = center;
            info.visible = true;
            info.scale = 8f / 240f;  //Initial size of 8 pxiels
            info.alpha = info.scale;
            info.rotation = 0f;
            info.fast = fast;

            if (!Main.dedServ)
            {
                SoundStyle sound = SoundID.Item84 with { Volume = 0.9f };
                SoundEngine.PlaySound(sound, info.center);
            }
        }

        public override Color? GetAlpha(Color drawColor)
        {
            if (NPC.IsABestiaryIconDummy)
                return NPC.GetBestiaryEntryColor(); // This is required because initially we have NPC.alpha = 255, in the bestiary it would look transparent

            return drawColor * (1f - targetAlpha / 255f);
        }

        public override bool? CanBeHitByItem(Player player, Item item)
            => CanBeHitByThing(player.GetSwungItemHitbox());

        public override bool? CanBeHitByProjectile(Projectile projectile)
        {
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