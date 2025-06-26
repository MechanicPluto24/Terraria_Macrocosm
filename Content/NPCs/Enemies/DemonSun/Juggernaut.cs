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
    public class Juggernaut : ModNPC
    {

        public enum ActionState
        {
            Idle,
            Roar,
            Walk,
            Sprint,
            Jump,
            Slam
        };

        // ai[0] and ai[3] are used by fighter ai
        public ref float AI_Timer => ref NPC.ai[1];
        public ref float AI_State => ref NPC.ai[2];
        public ref float AI_Direction => ref NPC.localAI[0];

        private float punchCooldown = 180f; // min ticks between attacks 
        private float dashSpeed = 12f; // initial dash speed and cap 
        private float dashDeceleration = 8f; // deceleration factor of the dash

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
            Redemption.AddNPCToElementList(Type, Redemption.NPCType.Humanoid);
        }

        public override void SetDefaults()
        {
            base.SetDefaults();

            NPC.width = 300;
            NPC.height = 270;
            NPC.damage = 150;
            NPC.defense = 80;
            NPC.lifeMax = 30000;
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
            NPC.spriteDirection = NPC.velocity.X > 0f ? -1 : 1;

            if (AI_Timer <= 0)
            {
                AI_Timer = 0;
            }
            if(AI_State!=(float)ActionState.Sprint)
                CurrentSpeed=0f;
            
            // actively targets closest player
            NPC.TargetClosest(true);

            NPC.despawnEncouraged = false;

            switch (AI_State)
            {
                case (float)ActionState.Idle:
                    AI_Idle();
                    break;

                case (float)ActionState.Roar:
                    AI_Roar();
                    break;

                case (float)ActionState.Walk:
                    AI_Walk();
                    break;
                case (float)ActionState.Sprint:
                    AI_Sprint();
                    break;
                case (float)ActionState.Jump:
                    AI_Jump();
                    break;
                case (float)ActionState.Slam:
                    AI_Slam();
                    break;
            }
        }

        Vector2 idleDirection=new Vector2(1, 0);
        private void AI_Idle()
        {
            NPC.damage = 150;
            AI_Timer++;
            if (AI_Timer % 120f == 0f && Main.rand.NextBool(5) || NPC.velocity.X == 0f)
            {
                if (Main.rand.NextBool(2))
                    idleDirection = new Vector2(1, 0);
                else
                    idleDirection = new Vector2(-1, 0);
            }

            Utility.AIFighter(NPC, ref NPC.ai, NPC.Center + idleDirection);
            Player player = Main.player[NPC.target];
            if (Vector2.Distance(NPC.Center, player.Center) < 500f || NPC.life < NPC.lifeMax)
            {   
                AI_State=(float)ActionState.Roar;
                AI_Timer=0;
                SoundEngine.PlaySound(SoundID.NPCDeath10);
                for (int i = 0; i < 255; i++)
                {
                    Player playerforthing = Main.player[i];
                    if (playerforthing.active)
                    {
                        float distance = Vector2.Distance(playerforthing.Center, NPC.Center);
                        if (distance < 2000f)
                        {
                            playerforthing.AddScreenshake(200f - distance / 2000f * 200f, context: FullName + NPC.whoAmI.ToString());
                        }
                    }
                }

            }
        }
        private void AI_Roar()
        {
            NPC.damage = 150;
            AI_Timer++;
            NPC.velocity.X*=0.8f;
            if (AI_Timer>120)
            {   
                AI_State=(float)ActionState.Walk;
                AI_Timer=0;
            }
        }

        private void AI_Walk()
        {
            NPC.damage = 200;
            AI_Timer++;
            Player player = Main.player[NPC.target];
            if(AI_Timer%100<80)
                Utility.AIFighter(NPC, ref NPC.ai, player.Center,accelerationFactor: 0.1f, velMax: 2f, maxJumpTilesX: 3, maxJumpTilesY: 1);
            else
                Utility.AIFighter(NPC, ref NPC.ai, player.Center,accelerationFactor: 2f, velMax: 10f, maxJumpTilesX: 4, maxJumpTilesY: 2);
            if(AI_Timer>500)
            {
                AI_Timer=0;
                if(Vector2.Distance(player.Center, NPC.Center)>1500f)
                    AI_State=(float)ActionState.Jump;
                else
                {
                    if(Main.rand.NextBool(2))
                        AI_State=(float)ActionState.Sprint;
                    else
                        AI_State=(float)ActionState.Slam;
                }
            }
        }

        float CurrentSpeed=0f;
        private void AI_Sprint()
        {
            NPC.damage = 250;
            AI_Timer++;
            CurrentSpeed+=0.01f;
            if(CurrentSpeed>10f)
                CurrentSpeed=10f;
            Player player = Main.player[NPC.target];
            Utility.AIFighter(NPC, ref NPC.ai, player.Center,accelerationFactor: CurrentSpeed, velMax: 10f, maxJumpTilesX: 6, maxJumpTilesY: 4);
            NPC.velocity.Y+=0.1f;
            for (int i = 0; i < 255; i++)
                {
                    Player playerforthing = Main.player[i];
                    if (playerforthing.active)
                    {
                        float distance = Vector2.Distance(playerforthing.Center, NPC.Center);
                        if (distance < 2000f)
                        {
                            playerforthing.AddScreenshake(10f, context: FullName + NPC.whoAmI.ToString());
                        }
                    }
                }
            if(AI_Timer>600)
            {
                AI_Timer=0;
                if(Vector2.Distance(player.Center, NPC.Center)>1500f)
                    AI_State=(float)ActionState.Jump;
                else
                {
                    AI_State=(float)ActionState.Walk;
                }
            }
        }

        private void AI_Jump()
        {
            NPC.damage = 250;
            AI_Timer++;
            Player player = Main.player[NPC.target];
            NPC.velocity.Y=-10f;
            NPC.velocity.X=player.Center.X<NPC.Center.X ? -20f : 20f;
            if(player.Center.X-NPC.Center.X<1000f||AI_Timer>30)
                NPC.velocity.Y+=0.5f;
            if(AI_Timer>60 ||Vector2.Distance(player.Center, NPC.Center)<500f)
            {
                AI_Timer=0;
                
                AI_State=(float)ActionState.Walk;
                
            }
        }
        private void AI_Slam()
        {
            NPC.damage = 150;
            AI_Timer++;
            Player player = Main.player[NPC.target];
            NPC.velocity.X*=0.9f;
            NPC.velocity.Y+=0.3f;
            if(AI_Timer==60)
            {
                for (int i = 0; i < 255; i++)
                {
                    Player playerforthing = Main.player[i];
                    if (playerforthing.active)
                    {
                        float distance = Vector2.Distance(playerforthing.Center, NPC.Center);
                        if (distance < 2000f)
                        {
                            playerforthing.AddScreenshake(200f, context: FullName + NPC.whoAmI.ToString());
                        }
                    }
                }
                Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, new Vector2(player.Center.X<NPC.Center.X ? -1 : 1,0), ModContent.ProjectileType<JuggernautShockwave>(),50, 10f);
            }
            if(AI_Timer>180)
            {
                AI_Timer=0;
                if(Vector2.Distance(player.Center, NPC.Center)>1500f)
                    AI_State=(float)ActionState.Jump;
                else
                {
                    AI_State=(float)ActionState.Walk;
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
