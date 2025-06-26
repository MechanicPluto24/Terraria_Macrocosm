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

namespace Macrocosm.Content.NPCs.Enemies.DemonSun
{
    public class Bloodstrider : ModNPC
    {

        // ai[0] and ai[3] are used by fighter ai
        public ref float AI_Timer => ref NPC.ai[1];
        public ref float AI_State => ref NPC.ai[2];
        public ref float AI_Direction => ref NPC.localAI[0];

        private float punchCooldown = 180f; // min ticks between attacks 
        private float dashSpeed = 12f; // initial dash speed and cap 
        private float dashDeceleration = 8f; // deceleration factor of the dash

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[Type] = 7;

            SpawnModBiomes = [ModContent.GetInstance<DemonSunBiome>().Type];

            NPC.ApplyBuffImmunity
            (
                BuffID.Confused
            );

            NPCSets.MoonNPC[Type] = true;
            NPCSets.DemonSunNPC[Type] = true;

            NPCSets.Material[Type] = NPCMaterial.Organic;
            Redemption.AddElementToNPC(Type, Redemption.ElementID.Blood);
            Redemption.AddNPCToElementList(Type, Redemption.NPCType.Blood);
            Redemption.AddNPCToElementList(Type, Redemption.NPCType.Humanoid);
        }

        public override void SetDefaults()
        {
            base.SetDefaults();

            NPC.width = 30;
            NPC.height = 50;
            NPC.damage = 60;
            NPC.defense = 10;
            NPC.lifeMax = 2500;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath2;
            NPC.knockBackResist = 1f;
            NPC.aiStyle = -1;
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[]
            {
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Events.BloodMoon,
            });
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo) => spawnInfo.Player.InModBiome<DemonSunBiome>() ? 1f : 0f;

        public override void AI()
        {
            NPC.TargetClosest();
            Player player = Main.player[NPC.target];
            Utility.AIFighter(NPC, ref NPC.ai, player.Center, accelerationFactor: 0.1f, velMax: 8f, maxJumpTilesX: 2, maxJumpTilesY: 1);        
        }

        public override void FindFrame(int frameHeight)
        {
            NPC.spriteDirection = NPC.direction;
            
                NPC.frameCounter += 10;
                if (NPC.frameCounter >= 48)
                {
                    NPC.frameCounter -= 48;
                    NPC.frame.Y += frameHeight;
                    if (NPC.frame.Y > frameHeight*6)
                    {
                        NPC.frame.Y = 0;
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
                for (int i = 0; i < 3; i++)
                {
                    NPC.NewNPCDirect(NPC.GetSource_FromAI(), (int)NPC.Center.X + Main.rand.Next(-15, 16), (int)NPC.Center.Y + Main.rand.Next(-5, 6), ModContent.NPCType<HellPustule>(), 0, 0f);
                }
            }
        }
    }
}
