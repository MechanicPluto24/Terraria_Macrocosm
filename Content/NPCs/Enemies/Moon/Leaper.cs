using Macrocosm.Common.Utils;
using Macrocosm.Content.Biomes;
using Macrocosm.Content.Items.Materials;
using Macrocosm.Content.NPCs.Global;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.NPCs.Enemies.Moon
{
    // incomplete
    // (TODO: leaps)
    public class Leaper : ModNPC, IMoonEnemy
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            Main.npcFrameCount[Type] = 24;
        }
        public override void SetDefaults()
        {
            base.SetDefaults();

            NPC.width = 36;
            NPC.height = 44;
            NPC.damage = 100;
            NPC.defense = 75;
            NPC.lifeMax = 3000;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath2;
            NPC.knockBackResist = 0.5f;
            NPC.aiStyle = -1;
            AIType = NPCID.ZombieMushroom;
            Banner = Item.NPCtoBanner(NPCID.Zombie);
            BannerItem = Item.BannerToItem(Banner);

            SpawnModBiomes = new int[1] { ModContent.GetInstance<UndergroundMoonBiome>().Type };
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[]
            {
                new FlavorTextBestiaryInfoElement(
                    " ")
            });
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            return spawnInfo.Player.InModBiome<MoonBiome>() && spawnInfo.Player.ZoneRockLayerHeight ? .1f : 0f;
        }

        public override void AI()
        {
            // add slimeAI for leaps? 
            Utility.AIZombie(NPC, ref NPC.ai, false, true, velMax: 4, maxJumpTilesX: 15, maxJumpTilesY: 10, moveInterval: 0.07f);

            if (NPC.velocity.Y < 0f)
                NPC.velocity.Y += 0.1f;
        }

        // frames 0 - 9: idle 
        // frames 10 - 15: start leap
        // frame 16 - leap while mid-air (mid-vacuum?)
        // frames 17 - 23: run 

        public override void FindFrame(int frameHeight)
        {

            int ticksPerFrame = 5;

            int idleFrameInitial = 0;
            int idleFrameCount = 10;

            //int leapFrameInitial = 10;
            //int leapFrameCount = 7;

            int midAirFrame = 16;

            int runFrameInitial = 17;
            int runFrameCount = 7;

            if (NPC.velocity.Y < -0.1f ||
                     !Main.tile[(int)(NPC.Center.X / 16), (int)((NPC.Center.Y + NPC.height / 2) / 16)].HasTile &&
                     !Main.tile[(int)(NPC.Center.X / 16), (int)((NPC.Center.Y + NPC.height / 2) / 16) + 1].HasTile &&
                     !Main.tile[(int)(NPC.Center.X / 16), (int)((NPC.Center.Y + NPC.height / 2) / 16) + 2].HasTile)
            {
                NPC.frame.Y = midAirFrame * frameHeight;
            }

            NPC.frameCounter++;
            NPC.spriteDirection = NPC.direction;

            if (NPC.velocity == Vector2.Zero)
            {
                if (NPC.frameCounter >= ticksPerFrame)
                {
                    NPC.frameCounter = 0;
                    NPC.frame.Y += frameHeight;

                    if (NPC.frame.Y >= idleFrameCount * frameHeight - 1)
                        NPC.frame.Y = idleFrameInitial * frameHeight;
                }
            }
            else
            {
                NPC.frame.Y = (int)(NPC.frameCounter / ticksPerFrame + runFrameInitial) * frameHeight;

                if (NPC.frameCounter >= ticksPerFrame * runFrameCount)
                {
                    NPC.frameCounter = 0;
                    NPC.frame.Y = runFrameInitial * frameHeight;
                }
            }
        }

        public override void ModifyNPCLoot(NPCLoot loot)
        {
            loot.Add(ItemDropRule.Common(ModContent.ItemType<SpaceDust>()));             // Always drop 1 cosmic dust
            loot.Add(ItemDropRule.Common(ModContent.ItemType<ArtemiteOre>(), 16, 1, 6));  // 1/16 chance to drop 1-6 Artemite Ore
            loot.Add(ItemDropRule.Common(ModContent.ItemType<ChandriumOre>(), 16, 1, 6)); // 1/16 chance to drop 1-6 Chandrium Ore
            loot.Add(ItemDropRule.Common(ModContent.ItemType<SeleniteOre>(), 16, 1, 6));  // 1/16 chance to drop 1-6 Selenite Ore
            loot.Add(ItemDropRule.Common(ModContent.ItemType<DianiteOre>(), 16, 1, 6));   // 1/16 chance to drop 1-6 DianiteOre Ore
        }

        public override void HitEffect(NPC.HitInfo hit)
        {

            if (NPC.life > 0)
            {
                for (int i = 0; i < 20; i++)
                {
                    Dust dust = Dust.NewDustDirect(NPC.position, NPC.width, NPC.height, DustID.Wraith);
                    dust.velocity.X = (dust.velocity.X + Main.rand.Next(0, 100) * 0.02f) * hit.HitDirection;
                    dust.velocity.Y = 1f + Main.rand.Next(-50, 51) * 0.01f;
                    dust.scale *= 1f + Main.rand.Next(-30, 31) * 0.01f;
                    dust.noGravity = true;
                }
            }

            if (Main.netMode == NetmodeID.Server)
                return; // don't run on the server

            if (NPC.life <= 0)
            {
                var entitySource = NPC.GetSource_Death();

                Gore.NewGore(entitySource, NPC.position, NPC.velocity, Mod.Find<ModGore>("LeaperGoreHead").Type);
                Gore.NewGore(entitySource, NPC.position, NPC.velocity, Mod.Find<ModGore>("LeaperGoreEye").Type);
                Gore.NewGore(entitySource, NPC.position, NPC.velocity, Mod.Find<ModGore>("LeaperGoreArm").Type);
                Gore.NewGore(entitySource, NPC.position, NPC.velocity, Mod.Find<ModGore>("LeaperGoreArm").Type);
                Gore.NewGore(entitySource, NPC.position, NPC.velocity, Mod.Find<ModGore>("LeaperGoreLeg").Type);
                Gore.NewGore(entitySource, NPC.position, NPC.velocity, Mod.Find<ModGore>("LeaperGoreLeg").Type);
            }
        }
    }
}
