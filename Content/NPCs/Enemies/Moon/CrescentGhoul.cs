using Macrocosm.Common.Utils;
using Macrocosm.Content.Biomes;
using Macrocosm.Content.Dusts;
using Macrocosm.Content.Items.Materials;
using Macrocosm.Content.NPCs.Global;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.NPCs.Enemies.Moon
{
    public class CrescentGhoul : ModNPC, IMoonEnemy
    {
        public enum ActionState
        {
            Chase,
            StartSpin,
            Spin,
            StopSpin
        }

        public ActionState AI_State
        {
            get => (ActionState)NPC.ai[0];
            set => NPC.ai[0] = (float)value;
        }

        public ref float AI_Timer => ref NPC.ai[1];

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();

            Main.npcFrameCount[NPC.type] = 4;
            NPCID.Sets.TrailCacheLength[NPC.type] = 4;
            NPCID.Sets.TrailingMode[NPC.type] = 0;
        }

        public override void SetDefaults()
        {

            base.SetDefaults();

            NPC.width = 72;
            NPC.height = 84;
            NPC.lifeMax = 4000;
            NPC.damage = 200;
            NPC.defense = 60;
            NPC.HitSound = SoundID.NPCHit2;
            NPC.DeathSound = SoundID.NPCDeath2;
            NPC.aiStyle = -1;
            NPC.value = 60f;
            NPC.knockBackResist = 0f;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[]
            {
				//BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Times.NightTime,
			});
        }

        /// <summary> Adapted from Corite AI </summary>
        public override void AI()
        {
            if (NPC.target < 0 || NPC.target == 255 || Main.player[NPC.target].dead || !Main.player[NPC.target].active)
                NPC.TargetClosest(true);

            bool playerActive = Main.player[NPC.target] != null && Main.player[NPC.target].active && !Main.player[NPC.target].dead;

            float kbResist = 0.3f;
            float chaseUpwardsMult = 8f;

            // the radius where the ghoul will slow down and start rising above the player 
            float chaseRadius = 60f;

            float startSpinDeceleration = 0.8f;
            float startSpinDuration = 5f;

            float maxDashDistance = 400f;
            float dashAngleDiv = 8f;
            float dashSpeed = 10f;

            int dashAngleVariation = 0; // some angle variation for the dash (?)

            // breaks the dash if:
            //float dashChaseDuration = 10f; // this many ticks passed 
            //float dashStopDistance = 150f; // distance is greater than this 

            float dashStopSpeed = 8f; // also breaks the dash if the length of velocity vector is below this value 

            // the radius around the player where the ghoul tends to spin 
            float dashRadius = 60f;
            // huh 
            float dashFactorThird = 0.33333334f * dashRadius;

            if (Main.expertMode)
                kbResist *= Main.GameModeInfo.KnockbackToEnemiesMultiplier;

            if (AI_State != ActionState.Spin)
                Utility.LookAt(playerActive ? Main.player[NPC.target].Center : NPC.Center + NPC.velocity, NPC, 0);

            if (AI_State == ActionState.Chase)
            {
                NPC.knockBackResist = kbResist;

                Vector2 positionDiff = Main.player[NPC.target].Center - NPC.Center;
                Vector2 upwards = positionDiff - Vector2.UnitY * 300f;
                float distanceToTarget = positionDiff.Length();
                positionDiff = Vector2.Normalize(positionDiff) * chaseUpwardsMult;
                upwards = Vector2.Normalize(upwards) * chaseUpwardsMult;
                bool canDash = Collision.CanHit(NPC.Center, 1, 1, Main.player[NPC.target].Center, 1, 1);

                if (NPC.ai[3] >= 120f)
                    canDash = true;

                canDash = (canDash && positionDiff.ToRotation() > (float)Math.PI / dashAngleDiv && positionDiff.ToRotation() < (float)Math.PI - (float)Math.PI / dashAngleDiv);

                if (distanceToTarget > maxDashDistance || !canDash)
                {
                    NPC.velocity.X = (NPC.velocity.X * (chaseRadius - 1f) + upwards.X) / chaseRadius;
                    NPC.velocity.Y = (NPC.velocity.Y * (chaseRadius - 1f) + upwards.Y) / chaseRadius;
                    if (!canDash)
                    {
                        NPC.ai[3]++;
                        if (NPC.ai[3] == 120f)
                            NPC.netUpdate = true;
                    }
                    else
                    {
                        NPC.ai[3] = 0f;
                    }
                }
                else
                {
                    AI_State = ActionState.StartSpin;
                    NPC.ai[2] = positionDiff.X;
                    NPC.ai[3] = positionDiff.Y;
                    NPC.netUpdate = true;
                }
            }
            else if (AI_State == ActionState.StartSpin)
            {
                NPC.knockBackResist = 0f;
                NPC.velocity *= startSpinDeceleration;
                AI_Timer++;
                if (AI_Timer >= startSpinDuration)
                {
                    AI_State = ActionState.Spin;
                    AI_Timer = 0f;
                    NPC.netUpdate = true;
                    Vector2 dashVector = new Vector2(NPC.ai[2], NPC.ai[3]) + new Vector2(Main.rand.Next(-dashAngleVariation, dashAngleVariation + 1), Main.rand.Next(-dashAngleVariation, dashAngleVariation + 1)) * 0.04f;
                    dashVector.Normalize();
                    NPC.velocity = dashVector * dashSpeed;
                }

            }
            else if (AI_State == ActionState.Spin)
            {

                NPC.rotation += 0.48f;
                NPC.knockBackResist = 0f;
                AI_Timer++;

                //bool playerClose = Vector2.Distance(NPC.Center, Main.player[NPC.target].Center) > dashStopDistance; /* corites also check for player below : && NPC.Center.Y > Main.player[NPC.target].Center.Y;*/
                //if ((AI_Timer >= dashChaseDuration && playerClose) || NPC.velocity.Length() < dashStopSpeed)

                if (NPC.velocity.Length() < dashStopSpeed)
                {
                    AI_State = ActionState.StopSpin;
                    AI_Timer = 45f;
                    NPC.ai[2] = 0f;
                    NPC.ai[3] = 0f;
                    NPC.velocity /= 2f;
                    NPC.netUpdate = true;
                }
                else
                {
                    Vector2 positionDiff = Main.player[NPC.target].Center - NPC.Center;
                    positionDiff.Normalize();
                    if (positionDiff.HasNaNs())
                        positionDiff = new Vector2(NPC.direction, 0f);

                    NPC.velocity = (NPC.velocity * (dashRadius - 1f) + positionDiff * (NPC.velocity.Length() + dashFactorThird)) / dashRadius;
                }

            }
            else if (AI_State == ActionState.StopSpin)
            {
                AI_Timer -= 3f;
                if (AI_Timer <= 0f)
                {
                    AI_State = ActionState.Chase;
                    AI_Timer = 0f;
                    NPC.netUpdate = true;
                }

                NPC.velocity *= 0.95f;
            }
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            return spawnInfo.Player.InModBiome<MoonBiome>() && !Main.dayTime && spawnInfo.SpawnTileY <= Main.worldSurface + 100 ? .1f : 0f;
        }

        public override void ModifyNPCLoot(NPCLoot loot)
        {
            loot.Add(ItemDropRule.Common(ModContent.ItemType<SpaceDust>()));             // Always drop 1 cosmic dust
            loot.Add(ItemDropRule.Common(ModContent.ItemType<ArtemiteOre>(), 16, 1, 6));  // 1/16 chance to drop 1-6 Artemite Ore
            loot.Add(ItemDropRule.Common(ModContent.ItemType<ChandriumOre>(), 16, 1, 6)); // 1/16 chance to drop 1-6 Chandrium Ore
            loot.Add(ItemDropRule.Common(ModContent.ItemType<SeleniteOre>(), 16, 1, 6));  // 1/16 chance to drop 1-6 Selenite Ore
            loot.Add(ItemDropRule.Common(ModContent.ItemType<DianiteOre>(), 16, 1, 6));   // 1/16 chance to drop 1-6 DianiteOre Ore
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            if (AI_State != ActionState.Spin)
                return true;

            for (int i = 0; i < NPC.oldPos.Length; i++)
            {
                Vector2 drawPos = NPC.oldPos[i] + NPC.Size / 2 - Main.screenPosition;
                Color color = NPC.GetAlpha(drawColor) * (((float)NPC.oldPos.Length - i) / NPC.oldPos.Length);
                SpriteEffects effect = NPC.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
                spriteBatch.Draw(TextureAssets.Npc[NPC.type].Value, drawPos, NPC.frame, color * 0.6f, NPC.rotation - 0.48f * i, NPC.Size / 2, NPC.scale, effect, 0f);
            }

            return true;
        }

        public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D glowmask = ModContent.Request<Texture2D>("Macrocosm/Content/NPCs/Enemies/Moon/CrescentGhoul_Glow").Value;
            SpriteEffects effect = NPC.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            int frameHeight = TextureAssets.Npc[Type].Height() / Main.npcFrameCount[Type];
            spriteBatch.Draw(glowmask, NPC.Center - Main.screenPosition, NPC.frame, Color.White, NPC.rotation, new Vector2(TextureAssets.Npc[Type].Width() / 2f, frameHeight / 2f), NPC.scale, effect, 0f);
        }

        public override void FindFrame(int frameHeight)
        {
            NPC.frame.Y = (int)(NPC.frameCounter / 10) * frameHeight;

            if (NPC.localAI[0] == 0f)
            {
                NPC.frameCounter++;

                if (NPC.frameCounter >= 39)
                    NPC.localAI[0] = 1f;
            }
            else if (NPC.localAI[0] == 1f)
            {
                NPC.frameCounter--;

                if (NPC.frameCounter <= 0)
                    NPC.localAI[0] = 0f;
            }
            else
            {
                NPC.localAI[0] = 0f;
            }
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            for (int i = 0; i < 10; i++)
            {
                int dustIndex = Dust.NewDust(NPC.position, NPC.width, NPC.height, ModContent.DustType<RegolithDust>());
                Dust dust = Main.dust[dustIndex];
                dust.velocity.X *= dust.velocity.X * 1.25f * hit.HitDirection + Main.rand.Next(0, 100) * 0.015f;
                dust.velocity.Y *= dust.velocity.Y * 0.25f + Main.rand.Next(-50, 51) * 0.01f;
                dust.scale *= 1f + Main.rand.Next(-30, 31) * 0.01f;
            }

            if (Main.dedServ)
                return;

            if (NPC.life <= 0)
            {
                var entitySource = NPC.GetSource_Death();

                for (int i = 0; i < 50; i++)
                {
                    int dustIndex = Dust.NewDust(NPC.position, NPC.width, NPC.height, ModContent.DustType<RegolithDust>());
                    Dust dust = Main.dust[dustIndex];
                    dust.velocity.X *= dust.velocity.X * 1.25f * hit.HitDirection + Main.rand.Next(0, 100) * 0.015f;
                    dust.velocity.Y *= dust.velocity.Y * 0.25f + Main.rand.Next(-50, 51) * 0.01f;
                    dust.scale *= 1f + Main.rand.Next(-30, 31) * 0.01f;
                }
            }
        }
    }
}
