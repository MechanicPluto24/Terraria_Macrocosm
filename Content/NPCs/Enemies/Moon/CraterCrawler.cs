using Macrocosm.Common.Bases;
using Macrocosm.Common.Sets;
using Macrocosm.Content.Biomes;
using Macrocosm.Content.Dusts;
using Macrocosm.Content.Items.Drops;
using Microsoft.Xna.Framework;
using System.IO;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.NPCs.Enemies.Moon
{
    // Adapted from Example Mod
    public class CraterCrawlerHead : WormHead
    {
        public override int BodyType => ModContent.NPCType<CraterCrawlerBody>();
        public override int TailType => ModContent.NPCType<CraterCrawlerTail>();

        public override void SetStaticDefaults()
        {
            var drawModifier = new NPCID.Sets.NPCBestiaryDrawModifiers()    // Influences how the NPC looks in the Bestiary
            {
                CustomTexturePath = "Macrocosm/Content/NPCs/Enemies/Moon/CraterCrawler_Bestiary", // If the NPC is multiple parts like a worm, a custom texture for the Bestiary is encouraged.
                Position = new Vector2(40f, 24f),
                PortraitPositionXOverride = 0f,
                PortraitPositionYOverride = 12f
            };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(NPC.type, drawModifier);

            NPCSets.MoonNPC[Type] = true;
            NPCSets.DropsMoonstone[Type] = true;
        }

        public override void SetDefaults()
        {
            NPC.CloneDefaults(NPCID.DiggerHead);
            NPC.lifeMax = 900;
            NPC.damage = 90;
            NPC.defense = 40;
            NPC.width = 20;
            NPC.height = 20;
            SpawnModBiomes = [ModContent.GetInstance<NightMoonBiome>().Type];
            NPC.aiStyle = -1;
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            return spawnInfo.Player.InModBiome<MoonBiome>() && !Main.dayTime && spawnInfo.SpawnTileY <= Main.worldSurface + 100 ? .1f : 0f;
        }

        public override void ModifyNPCLoot(NPCLoot loot)
        {
            loot.Add(ItemDropRule.Common(ModContent.ItemType<AlienResidue>(), 2, 1, 2));
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            /*
            bestiaryEntry.Info.Add(
				BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Times.NightTime
			);
            */
        }

        public override void Init()
        {
            // Set the segment variance
            // If you want the segment length to be constant, set these two properties to the same value
            MinSegmentLength = 16;
            MaxSegmentLength = 24;

            CommonWormInit(this);
        }

        public static void CommonWormInit(Worm worm)
        {
            // These two properties handle the movement of the worm
            worm.MoveSpeed = 15.5f;
            worm.Acceleration = 0.12f;
        }

        private int attackCounter;
        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(attackCounter);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            attackCounter = reader.ReadInt32();
        }

        public override void AI()
        {
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                // tick down the attack counter.
                if (attackCounter > 0)
                    attackCounter--;

                Player target = Main.player[NPC.target];
                // If the attack counter is 0, this NPC is less than 12.5 tiles away from its target, and has a path to the target unobstructed by blocks, summon a projectile.
                if (attackCounter <= 0 && Vector2.Distance(NPC.Center, target.Center) < 200 && Collision.CanHit(NPC.Center, 1, 1, target.Center, 1, 1))
                {
                    // some projectile attack here?
                }
            }
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            for (int i = 0; i < 10; i++)
            {
                int dustIndex = Dust.NewDust(NPC.position, NPC.width, NPC.height, i % 2 == 0 ? ModContent.DustType<RegolithDust>() : DustID.GreenBlood);
                Dust dust = Main.dust[dustIndex];
                dust.velocity.X *= dust.velocity.X * 1.25f * hit.HitDirection + Main.rand.Next(0, 100) * 0.015f;
                dust.velocity.Y *= dust.velocity.Y * 0.25f + Main.rand.Next(-50, 51) * 0.01f;
                dust.scale *= 1f + Main.rand.Next(-30, 31) * 0.01f;
            }

            if (NPC.life <= 0)
            {
                Gore.NewGore(NPC.GetSource_Death(), NPC.position, -NPC.velocity, Mod.Find<ModGore>("CraterCrawlerHeadGore").Type);

                for (int i = 0; i < 20; i++)
                {
                    int dustIndex = Dust.NewDust(NPC.position, NPC.width, NPC.height, i % 2 == 0 ? ModContent.DustType<RegolithDust>() : DustID.GreenBlood);
                    Dust dust = Main.dust[dustIndex];
                    dust.velocity.X *= dust.velocity.X * 1.25f * hit.HitDirection + Main.rand.Next(0, 100) * 0.015f;
                    dust.velocity.Y *= dust.velocity.Y * 0.25f + Main.rand.Next(-50, 51) * 0.01f;
                    dust.scale *= 1f + Main.rand.Next(-30, 31) * 0.01f;
                }
            }
        }
    }

    public class CraterCrawlerBody : WormBody
    {
        public override void SetStaticDefaults()
        {
            NPCID.Sets.NPCBestiaryDrawModifiers value = new NPCID.Sets.NPCBestiaryDrawModifiers() { Hide = true }; // Hides this NPC from the Bestiary, useful for multi-part NPCs whom you only want one entry.
            NPCID.Sets.NPCBestiaryDrawOffset.Add(NPC.type, value);
        }

        public override void SetDefaults()
        {
            NPC.CloneDefaults(NPCID.DiggerBody);
            NPC.damage = 60;
            NPC.defense = 45;
            NPC.npcSlots = 0f;
            NPC.width = 16;
            NPC.height = 16;
            NPC.aiStyle = -1;
            Main.npcFrameCount[Type] = 1;
        }

        public override void Init()
        {
            CraterCrawlerHead.CommonWormInit(this);
        }

        public override void OnSpawn(IEntitySource source)
        {
        }

        public override void FindFrame(int frameHeight)
        {
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            for (int i = 0; i < 10; i++)
            {
                int dustIndex = Dust.NewDust(NPC.position, NPC.width, NPC.height, i % 2 == 0 ? ModContent.DustType<RegolithDust>() : DustID.GreenBlood);
                Dust dust = Main.dust[dustIndex];
                dust.velocity.X *= dust.velocity.X * 1.25f * hit.HitDirection + Main.rand.Next(0, 100) * 0.015f;
                dust.velocity.Y *= dust.velocity.Y * 0.25f + Main.rand.Next(-50, 51) * 0.01f;
                dust.scale *= 1f + Main.rand.Next(-30, 31) * 0.01f;
            }

            if (NPC.life <= 0)
            {
                Gore.NewGore(NPC.GetSource_Death(), NPC.position, -NPC.velocity, Mod.Find<ModGore>("CraterCrawlerBodyGore").Type);

                for (int i = 0; i < 20; i++)
                {
                    int dustIndex = Dust.NewDust(NPC.position, NPC.width, NPC.height, i % 2 == 0 ? ModContent.DustType<RegolithDust>() : DustID.GreenBlood);
                    Dust dust = Main.dust[dustIndex];
                    dust.velocity.X *= dust.velocity.X * 1.25f * hit.HitDirection + Main.rand.Next(0, 100) * 0.015f;
                    dust.velocity.Y *= dust.velocity.Y * 0.25f + Main.rand.Next(-50, 51) * 0.01f;
                    dust.scale *= 1f + Main.rand.Next(-30, 31) * 0.01f;
                }
            }
        }
    }

    public class CraterCrawlerTail : WormTail
    {
        public override void SetStaticDefaults()
        {
            NPCID.Sets.NPCBestiaryDrawModifiers value = new()
            {
                Hide = true // Hides this NPC from the Bestiary, useful for multi-part NPCs whom you only want one entry.
            };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(NPC.type, value);
        }

        public override void SetDefaults()
        {
            NPC.CloneDefaults(NPCID.DiggerTail);
            NPC.damage = 50;
            NPC.defense = 50;
            NPC.width = 30;
            NPC.height = 30;
            NPC.npcSlots = 0f;
            NPC.aiStyle = -1;
        }

        public override void Init()
        {
            FlipSprite = false;
            CraterCrawlerHead.CommonWormInit(this);
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            for (int i = 0; i < 10; i++)
            {
                int dustIndex = Dust.NewDust(NPC.position, NPC.width, NPC.height, i % 2 == 0 ? ModContent.DustType<RegolithDust>() : DustID.GreenBlood);
                Dust dust = Main.dust[dustIndex];
                dust.velocity.X *= dust.velocity.X * 1.25f * hit.HitDirection + Main.rand.Next(0, 100) * 0.015f;
                dust.velocity.Y *= dust.velocity.Y * 0.25f + Main.rand.Next(-50, 51) * 0.01f;
                dust.scale *= 1f + Main.rand.Next(-30, 31) * 0.01f;
            }

            if (NPC.life <= 0)
            {
                Gore.NewGore(NPC.GetSource_Death(), NPC.position, -NPC.velocity, Mod.Find<ModGore>("CraterCrawlerTailGore").Type);

                for (int i = 0; i < 20; i++)
                {
                    int dustIndex = Dust.NewDust(NPC.position, NPC.width, NPC.height, i % 2 == 0 ? ModContent.DustType<RegolithDust>() : DustID.GreenBlood);
                    Dust dust = Main.dust[dustIndex];
                    dust.velocity.X *= dust.velocity.X * 1.25f * hit.HitDirection + Main.rand.Next(0, 100) * 0.015f;
                    dust.velocity.Y *= dust.velocity.Y * 0.25f + Main.rand.Next(-50, 51) * 0.01f;
                    dust.scale *= 1f + Main.rand.Next(-30, 31) * 0.01f;
                }
            }
        }
    }
}
