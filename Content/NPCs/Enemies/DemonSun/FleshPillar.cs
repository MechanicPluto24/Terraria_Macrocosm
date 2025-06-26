using Macrocosm.Common.CrossMod;
using Macrocosm.Common.Enums;
using Macrocosm.Common.Sets;
using Macrocosm.Common.Utils;
using Macrocosm.Content.Biomes;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using Macrocosm.Content.Projectiles.Hostile;

namespace Macrocosm.Content.NPCs.Enemies.DemonSun
{
    public class FleshPillar : ModNPC
    {

        public enum ActionState
        {
            Idle,
            Rain,
            Pulse,
            Target
        };

        // ai[0] and ai[3] are used by fighter ai
        public ref float AI_Timer => ref NPC.ai[1];
        public ref float AI_State => ref NPC.ai[2];
        public ref float AI_Direction => ref NPC.localAI[0];

 
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[Type] = 1;

            SpawnModBiomes = [ModContent.GetInstance<DemonSunBiome>().Type];

            NPC.ApplyBuffImmunity
            (
                BuffID.Confused
            );

            NPCSets.MoonNPC[Type] = true;
            NPCSets.DemonSunNPC[Type] = true;
            NPC.despawnEncouraged = false;

            NPCSets.Material[Type] = NPCMaterial.Organic;
            Redemption.AddElementToNPC(Type, Redemption.ElementID.Blood);
            Redemption.AddNPCToElementList(Type, Redemption.NPCType.Blood);
        }

        public override void SetDefaults()
        {
            NPC.noGravity=true;
            NPC.noTileCollide=true;
            NPC.width = 170;
            NPC.height = 360;
            NPC.damage = 0;
            NPC.defense = 60;
            NPC.lifeMax = 35000;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath2;
            NPC.knockBackResist = 0.0f;
            NPC.aiStyle = -1;
            NPC.npcSlots=5f;
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[]
            {
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Events.BloodMoon,
            });
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo) => spawnInfo.Player.InModBiome<DemonSunBiome>() ? 0.03f : 0f;
        public override void AI()
        {

            if (AI_Timer <= 0)
            {
                AI_Timer = 0;
            }
           
            
            NPC.TargetClosest(true);

            NPC.despawnEncouraged = false;
            Player player = Main.player[NPC.target];

            if(AI_State==(float)ActionState.Idle)
                NPC.velocity+=(GetTargetPostion(NPC.Center)-NPC.Center).SafeNormalize(Vector2.UnitX)*0.1f;
            else
                NPC.velocity+=(GetTargetPostion(player.Center)-NPC.Center).SafeNormalize(Vector2.UnitX)*0.1f;

            if(NPC.velocity.Length()>3f)
                NPC.velocity=NPC.velocity.SafeNormalize(Vector2.UnitX)*3f;
            switch (AI_State)
            {
                case (float)ActionState.Idle:
                    AI_Idle();
                    break;

                case (float)ActionState.Rain:
                    AI_Rain();
                    break;
                case (float)ActionState.Pulse:
                    AI_Pulse();
                    break;
                case (float)ActionState.Target:
                    AI_Target();
                    break;
            }
        }

        private Vector2 GetTargetPostion(Vector2 targetPos)
        {
            int tries = 0;
            int x = (int)(targetPos.X / 16);
            int y = (int)(targetPos.Y / 16);
            while(Main.tile[x,y].HasTile==false && tries<100)
            {
                y++;
                tries++;
            }
            return new Vector2(x*16,(y*16)-(int)((Math.Sin(Main.time/100)*20)+400));
        }
        private void AI_Idle()
        {
            Player player = Main.player[NPC.target];
            if (Vector2.Distance(NPC.Center, player.Center) < 500f || NPC.life < NPC.lifeMax)
            {   
                AI_Timer=0;

                if(Main.rand.NextBool(3))
                    AI_State=(float)ActionState.Rain;
                else if(Main.rand.NextBool(2))
                    AI_State=(float)ActionState.Pulse;
                else
                    AI_State=(float)ActionState.Target;
            }
        }
        private void AI_Rain()
        {
            Player player = Main.player[NPC.target];
            AI_Timer++;
            if(AI_Timer%30==0)
            {
                Projectile.NewProjectile(NPC.GetSource_FromAI(), new Vector2(NPC.Center.X,NPC.position.Y+40), new Vector2(Main.rand.NextFloat(-10f,10f),-13f), ModContent.ProjectileType<HellBlood>(),30, 2f);
            }
            if (AI_Timer>500)
            {   
                AI_Timer=0;
                if(Main.rand.NextBool(3))
                    AI_State=(float)ActionState.Rain;
                else 
                {
                    if(Main.rand.NextBool(2))
                        AI_State=(float)ActionState.Pulse;
                    else
                        AI_State=(float)ActionState.Target;
                }
            }
        }

        private void AI_Pulse()
        {
            Player player = Main.player[NPC.target];
            AI_Timer++;
            if(AI_Timer==50)
            {
                for(int count=0;count<10;count++)
                    Projectile.NewProjectile(NPC.GetSource_FromAI(), new Vector2(NPC.Center.X,NPC.position.Y+40), new Vector2(Main.rand.NextFloat(-10f,10f),-13f), ModContent.ProjectileType<HellBlood>(),30, 2f);
            }
            if (AI_Timer>100)
            {   
                AI_Timer=0;
                if(Main.rand.NextBool(3))
                    AI_State=(float)ActionState.Rain;
                else 
                {
                    if(Main.rand.NextBool(2))
                        AI_State=(float)ActionState.Pulse;
                    else
                        AI_State=(float)ActionState.Target;
                }
            }
        }

        private void AI_Target()
        {
            Player player = Main.player[NPC.target];
            AI_Timer++;
            if(AI_Timer%20==0)
            {
                Projectile.NewProjectile(NPC.GetSource_FromAI(), new Vector2(NPC.Center.X,NPC.position.Y+40), (player.Center-new Vector2(NPC.Center.X,NPC.position.Y+10)).SafeNormalize(Vector2.UnitX)*15f, ModContent.ProjectileType<HellBlood>(),30, 2f);
            }
            if (AI_Timer>200)
            {   
                AI_Timer=0;
                if(Main.rand.NextBool(3))
                    AI_State=(float)ActionState.Rain;
                else 
                {
                    if(Main.rand.NextBool(2))
                        AI_State=(float)ActionState.Pulse;
                    else
                        AI_State=(float)ActionState.Target;
                }
            }
        }

      
        

        public override void ModifyNPCLoot(NPCLoot loot)
        {
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            if (NPC.life > 0)
            {
                for (int i = 0; i < 10; i++)
                {
                    int dustIndex = Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Blood);
                    Dust dust = Main.dust[dustIndex];
                    dust.velocity.X *= dust.velocity.X * 1.25f * hit.HitDirection + Main.rand.Next(0, 100) * 0.015f;
                    dust.velocity.Y *= dust.velocity.Y * 0.25f + Main.rand.Next(-50, 51) * 0.01f;
                    dust.scale *= 1f + Main.rand.Next(-30, 31) * 0.01f;
                }
            }

            if (Main.dedServ)
                return; // don't run on the server

            var entitySource = NPC.GetSource_Death();


            if (NPC.life <= 0)
            {

                for (int i = 0; i < 50; i++)
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
