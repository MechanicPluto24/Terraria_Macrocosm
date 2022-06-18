using Macrocosm.Content.Items.Currency;
using Macrocosm.Content.Items.Materials;
using System.ComponentModel;
using System.Media;
using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.NPCs.Unfriendly.Enemies
{
	public class MoonZombie : ModNPC
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Moon Zombie");	
			Main.npcFrameCount[Type] = 9;
		}

		public override void SetDefaults()
		{
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
			return spawnInfo.SpawnTileType == ModContent.TileType<Tiles.Regolith>() && !Main.dayTime ? 0.1f : 0f;
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
					NPC.frame.Y += 44;
					if (NPC.frame.Y > 304)
					{
						NPC.frame.Y = 0;
					}
				}
			}
			else
			{
				NPC.frame.Y = 352;
			}
		}

		public override void ModifyNPCLoot(NPCLoot loot)
        {
			loot.Add(ItemDropRule.Common(ModContent.ItemType<CosmicDust>()));             // Always drop 1 cosmic dust
			loot.Add(ItemDropRule.Common(ModContent.ItemType<ArtemiteOre>(), 16, 1, 6));  // 16% chance to drop 1-6 Artemite Ore
			loot.Add(ItemDropRule.Common(ModContent.ItemType<ChandriumOre>(), 16, 1, 6)); // 16% chance to drop 1-6 Chandrium Ore
			loot.Add(ItemDropRule.Common(ModContent.ItemType<SeleniteOre>(), 16, 1, 6));  // 16% chance to drop 1-6 Selenite Ore
			loot.Add(ItemDropRule.Common(ModContent.ItemType<DianiteOre>(), 16, 1, 6));   // 16% chance to drop 1-6 DianiteOre Ore
		}

		public override void HitEffect(int hitDirection, double damage)
		{
            
            if(NPC.life > 0)
            {
				for (int i = 0; i < 10; i++)
				{
					int dustIndex = Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.TintableDust);
					Dust dust = Main.dust[dustIndex];
					dust.velocity.X = dust.velocity.X + Main.rand.Next(-50, 51) * 0.01f;
					dust.velocity.Y = dust.velocity.Y + Main.rand.Next(-50, 51) * 0.01f;
					dust.scale *= 1f + Main.rand.Next(-30, 31) * 0.01f;
				}
			}

			if (Main.netMode == NetmodeID.Server)
			{
                return; // don't run on the server
            }

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
