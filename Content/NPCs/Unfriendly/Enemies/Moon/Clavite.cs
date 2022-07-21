using System;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Microsoft.Xna.Framework;
using Terraria.GameContent.ItemDropRules;
using Macrocosm.Content.Buffs.Debuffs;
using Macrocosm.Content.Items.Materials;
using Terraria.GameContent.Bestiary;
using Macrocosm.Content.Biomes;
using Macrocosm.Base.BaseMod;

namespace Macrocosm.Content.NPCs.Unfriendly.Enemies.Moon
{
    public class Clavite : MoonEnemy
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();

            Main.npcFrameCount[NPC.type] = Main.npcFrameCount[NPCID.MeteorHead];
        }

        public override void SetDefaults()
        {

            base.SetDefaults();

            NPC.width = 60;
            NPC.height = 60;
            NPC.lifeMax = 2500;
            NPC.damage = 60;
            NPC.defense = 60;
            NPC.HitSound = SoundID.NPCHit2;
            NPC.DeathSound = SoundID.NPCDeath2;
            NPC.value = 60f;
            NPC.knockBackResist = 0f;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            AnimationType = NPCID.MeteorHead;
            SpawnModBiomes = new int[1] { ModContent.GetInstance<MoonBiome>().Type }; // Associates this NPC with the Moon Biome in Bestiary
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[]
            {
                new FlavorTextBestiaryInfoElement(
                    "A ravenous alien that prowls the surface of the Moon, attacking and charging at anything it finds.")
            });
        }

        public override void AI()
        {
            Player player = Main.player[NPC.target];
            if (NPC.target < 0 || NPC.target == 255 || Main.player[NPC.target].dead || !Main.player[NPC.target].active)
            {
                NPC.TargetClosest(true);
            }

            Move(Vector2.Zero);
            bool playerActive = player != null && player.active && !player.dead;
            BaseAI.LookAt(playerActive ? player.Center : NPC.Center + NPC.velocity, NPC, 0);
        }
        public void Move(Vector2 offset, float speed = 3f, float turnResistance = 0.5f)
        {
            Player player = Main.player[NPC.target];
            Vector2 moveTo = player.Center + offset; // Gets the point that the NPC will be moving to.
            Vector2 move = moveTo - NPC.Center;
            float magnitude = Magnitude(move);
            if (magnitude > speed)
            {
                move *= speed / magnitude;
            }
            move = (NPC.velocity * turnResistance + move) / (turnResistance + 1f);
            magnitude = Magnitude(move);
            if (magnitude > speed)
            {
                move *= speed / magnitude;
            }
            NPC.velocity = move;
        }

        private static float Magnitude(Vector2 mag) => (float)Math.Sqrt(mag.X * mag.X + mag.Y * mag.Y);

        public override void OnHitPlayer(Player player, int damage, bool crit)
        {
            if (player.GetModPlayer<MacrocosmPlayer>().accMoonArmor)
            {
                // Now only suit breaches players with said suit 
                player.AddBuff(ModContent.BuffType<SuitBreach>(), 600, true);
            }
        }
        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            return spawnInfo.Player.GetModPlayer<MacrocosmPlayer>().ZoneMoon && Main.dayTime && spawnInfo.SpawnTileY <= Main.worldSurface + 100 ? .1f : 0f;
        }

        public override void ModifyNPCLoot(NPCLoot loot)
        {
            loot.Add(ItemDropRule.Common(ModContent.ItemType<CosmicDust>()));             // Always drop 1 cosmic dust
            loot.Add(ItemDropRule.Common(ModContent.ItemType<ArtemiteOre>(), 16, 1, 6));  // 1/16 chance to drop 1-6 Artemite Ore
            loot.Add(ItemDropRule.Common(ModContent.ItemType<ChandriumOre>(), 16, 1, 6)); // 1/16 chance to drop 1-6 Chandrium Ore
            loot.Add(ItemDropRule.Common(ModContent.ItemType<SeleniteOre>(), 16, 1, 6));  // 1/16 chance to drop 1-6 Selenite Ore
            loot.Add(ItemDropRule.Common(ModContent.ItemType<DianiteOre>(), 16, 1, 6));   // 1/16 chance to drop 1-6 DianiteOre Ore
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            for (int i = 0; i < 10; i++)
            {
                int dustIndex = Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Stone);
                Dust dust = Main.dust[dustIndex];
                dust.velocity.X *= dust.velocity.X * 1.25f * hitDirection + Main.rand.Next(0, 100) * 0.015f;
                dust.velocity.Y *= dust.velocity.Y * 0.25f + Main.rand.Next(-50, 51) * 0.01f;
                dust.scale *= 1f + Main.rand.Next(-30, 31) * 0.01f;
            }

            if (Main.netMode == NetmodeID.Server)
            {
                return; // don't run on the server
            }

            if (NPC.life <= 0)
            {
                var entitySource = NPC.GetSource_Death();

                Gore.NewGore(entitySource, NPC.position, -NPC.velocity, Mod.Find<ModGore>("ClaviteGoreHead1").Type);
                Gore.NewGore(entitySource, NPC.position, -NPC.velocity, Mod.Find<ModGore>("ClaviteGoreHead2").Type);
                Gore.NewGore(entitySource, NPC.position, -NPC.velocity * 2, Mod.Find<ModGore>("ClaviteGoreJaw1").Type);
                Gore.NewGore(entitySource, NPC.position, -NPC.velocity, Mod.Find<ModGore>("ClaviteGoreJaw2").Type);
                Gore.NewGore(entitySource, NPC.position, -NPC.velocity * 1.5f, Mod.Find<ModGore>("ClaviteGoreEye1").Type);
                Gore.NewGore(entitySource, NPC.position, -NPC.velocity * 2, Mod.Find<ModGore>("ClaviteGoreEye2").Type);
            }
        }
    }
}
