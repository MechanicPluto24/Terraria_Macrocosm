using Macrocosm.Content.Biomes;
using Macrocosm.Content.Items.Materials;
using Macrocosm.Content.NPCs.Global;
using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.NPCs.Enemies.Moon
{
    public class MoonZombie : ModNPC, IMoonEnemy
    {
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[Type] = 9;
        }

        public override void SetDefaults()
        {
            base.SetDefaults();

            NPC.width = 18;
            NPC.height = 44;
            NPC.damage = 60;
            NPC.defense = 60;
            NPC.lifeMax = 2200;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath2;
            NPC.knockBackResist = 0.5f;
            NPC.aiStyle = 3;
            AIType = NPCID.ZombieMushroom;
            Banner = Item.NPCtoBanner(NPCID.Zombie);
            BannerItem = Item.BannerToItem(Banner);
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            return spawnInfo.Player.InModBiome<MoonBiome>() && !Main.dayTime ? .1f : 0f;
        }

        public override void AI()
        {
            if (NPC.velocity.Y < 0f)
                NPC.velocity.Y += 0.1f;
            base.AI();
        }

        public override void FindFrame(int frameHeight)
        {
            NPC.spriteDirection = NPC.direction;
            if (NPC.velocity.Y == 0)
            {
                NPC.frameCounter += 10;
                if (NPC.frameCounter >= 48)
                {
                    NPC.frameCounter -= 48;
                    NPC.frame.Y += frameHeight;
                    if (NPC.frame.Y > 304)
                    {
                        NPC.frame.Y = 0;
                    }
                }
            }
            else
            {
                NPC.frame.Y = 8 * frameHeight;
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
                for (int i = 0; i < 30; i++)
                {
                    int dustType = Utils.SelectRandom<int>(Main.rand, DustID.TintableDust, DustID.Blood);

                    Dust dust = Dust.NewDustDirect(NPC.position, NPC.width, NPC.height, dustType);
                    dust.velocity.X *= (dust.velocity.X + +Main.rand.Next(0, 100) * 0.015f) * hit.HitDirection;
                    dust.velocity.Y = 3f + Main.rand.Next(-50, 51) * 0.01f;
                    dust.scale *= 1f + Main.rand.Next(-30, 31) * 0.01f;
                    dust.noGravity = true;
                }
            }

            if (Main.dedServ)
                return; // don't run on the server

            if (NPC.life <= 0)
            {
                var entitySource = NPC.GetSource_Death();

                Gore.NewGore(entitySource, NPC.position, NPC.velocity, Mod.Find<ModGore>("MoonZombieHead").Type);
                Gore.NewGore(entitySource, NPC.position, NPC.velocity, Mod.Find<ModGore>("MoonZombieArm").Type);
                Gore.NewGore(entitySource, NPC.position, NPC.velocity, Mod.Find<ModGore>("MoonZombieArm").Type);
                Gore.NewGore(entitySource, NPC.position, NPC.velocity, Mod.Find<ModGore>("MoonZombieLeg").Type);
                Gore.NewGore(entitySource, NPC.position, NPC.velocity, Mod.Find<ModGore>("MoonZombieLeg").Type);
            }
        }
    }
}
