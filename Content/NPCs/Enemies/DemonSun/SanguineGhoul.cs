using Macrocosm.Common.CrossMod;
using Macrocosm.Common.Enums;
using Macrocosm.Common.Sets;
using Macrocosm.Common.Utils;
using Macrocosm.Content.Biomes;
using Macrocosm.Content.Dusts;
using Macrocosm.Content.Items.Drops;
using Macrocosm.Content.Items.Food;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.NPCs.Enemies.DemonSun
{
    public class SanguineGhoul : ModNPC
    {
        public enum ActionState
        {
            Chase,
            Accelerate,
            Spin,
            Decelerate
        }

        public ActionState AI_State
        {
            get => (ActionState)NPC.ai[0];
            set => NPC.ai[0] = (float)value;
        }

        public ref float AI_Timer => ref NPC.ai[1];

        private static Asset<Texture2D> glowmask;

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 1;
            NPCID.Sets.TrailCacheLength[NPC.type] = 4;
            NPCID.Sets.TrailingMode[NPC.type] = 0;

            NPCSets.MoonNPC[Type] = true;
            NPCSets.DemonSunNPC[Type] = true;

            NPCSets.Material[Type] = NPCMaterial.Organic;
            Redemption.AddElementToNPC(Type, Redemption.ElementID.Blood);
            Redemption.AddNPCToElementList(Type, Redemption.NPCType.Blood);
            Redemption.AddNPCToElementList(Type, Redemption.NPCType.Inorganic);
        }

        public override void SetDefaults()
        {
            NPC.width = 72;
            NPC.height = 84;
            NPC.lifeMax = 3000;
            NPC.damage = 55;
            NPC.defense = 80;
            NPC.HitSound = SoundID.NPCHit2;
            NPC.DeathSound = SoundID.NPCDeath2;
            NPC.aiStyle = -1;
            NPC.value = 60f;
            NPC.knockBackResist = 0f;
            NPC.noGravity = true;
            NPC.noTileCollide = false;
            SpawnModBiomes = [ModContent.GetInstance<DemonSunBiome>().Type];
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo) => spawnInfo.Player.InModBiome<DemonSunBiome>() ? .5f : 0f;
        public override void ModifyNPCLoot(NPCLoot loot)
        {
            
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
            float dashChaseDuration = 600f; // this many ticks passed 
            //float dashStopDistance = 150f; // distance is greater than this 

            float dashStopSpeed = 8f; // also breaks the dash if the length of velocity vector is below this value 

            // the radius around the player where the ghoul tends to spin 
            float dashRadius = 60f;

            if (Main.expertMode)
                kbResist *= Main.GameModeInfo.KnockbackToEnemiesMultiplier;

            if (AI_State == ActionState.Chase)
                NPC.damage = NPC.defDamage;
            else
                NPC.damage = (int)(NPC.defDamage * 1.5);

            if (AI_State != ActionState.Spin)
                Utility.LookAt(playerActive ? Main.player[NPC.target].Center : NPC.Center + NPC.velocity, NPC, 0);

            if (AI_State == ActionState.Chase)
            {
                NPC.knockBackResist = kbResist;
                NPC.noGravity = true;

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
                    AI_State = ActionState.Accelerate;
                    NPC.ai[2] = positionDiff.X;
                    NPC.ai[3] = positionDiff.Y;
                    NPC.netUpdate = true;
                }
            }
            else if (AI_State == ActionState.Accelerate)
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
                NPC.velocity.Y+=0.1f;
                NPC.noGravity = false;

                //bool playerClose = Vector2.Distance(NPC.Center, Main.player[NPC.target].Center) > dashStopDistance; /* corites also check for player below : && NPC.Center.Y > Main.player[NPC.target].Center.Y;*/
                //if ((AI_Timer >= dashChaseDuration && playerClose) || NPC.velocity.Length() < dashStopSpeed)
                if (Utility.CastLength(NPC.Center,new Vector2(0,1), 60f, false) < 60f)
                {
                    NPC.velocity.Y -=9f;
                    NPC.velocity.X *=-0.9f;

                }
                    
                if ((AI_Timer >= dashChaseDuration))
                {
                    AI_State = ActionState.Decelerate;
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
                }

            }
            else if (AI_State == ActionState.Decelerate)
            {
                NPC.noGravity = true;
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

       

        

        public override void HitEffect(NPC.HitInfo hit)
        {
            for (int i = 0; i < 10; i++)
                {
                    int dustIndex = Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Blood);
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
                if (NPC.life <= 0)
                {
                   for (int i = 0; i < 3; i++)
                    {
                        NPC.NewNPCDirect(NPC.GetSource_FromAI(), (int)NPC.Center.X + Main.rand.Next(-15, 16), (int)NPC.Center.Y + Main.rand.Next(-5, 6), ModContent.NPCType<HellPustule>(), 0, 0f);
                    }
                }
                for (int i = 0; i < 40; i++)
                {
                    int dustIndex = Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Blood);
                    Dust dust = Main.dust[dustIndex];
                    dust.velocity.X *= dust.velocity.X * 1.25f * hit.HitDirection + Main.rand.Next(0, 100) * 0.015f;
                    dust.velocity.Y *= dust.velocity.Y * 0.25f + Main.rand.Next(-50, 51) * 0.01f;
                    dust.scale *= 1f + Main.rand.Next(-30, 31) * 0.01f;
                }
                
            }
        }
    }
}
