using Macrocosm.Content.Items.Currency;
using Macrocosm.Content.Items.Materials;
using System.ComponentModel;
using System.Media;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.NPCs.Unfriendly.Enemies
{
	public class MoonZombie : ModNPC
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Moon Zombie");
			Main.npcFrameCount[npc.type] = 9;
		}

		public override void SetDefaults()
		{
			npc.width = 18;
			npc.height = 44;
			npc.damage = 60;
			npc.defense = 60;
			npc.lifeMax = 2200;
			npc.HitSound = SoundID.NPCHit1;
			npc.DeathSound = SoundID.NPCDeath2;
			npc.knockBackResist = 0.5f;
			npc.aiStyle = 3;
			aiType = NPCID.ZombieMushroom;
			banner = Item.NPCtoBanner(NPCID.Zombie);
			bannerItem = Item.BannerToItem(banner);
		}

		public override float SpawnChance(NPCSpawnInfo spawnInfo)
		{
			return spawnInfo.spawnTileType == ModContent.TileType<Tiles.Regolith>() && !Main.dayTime ? 0.1f : 0f;
		}

		public override void AI()
		{
			if (npc.velocity.Y < 0f)
				npc.velocity.Y += 0.1f;
			base.AI();
		}

		public override void FindFrame(int frameHeight)
		{
			npc.spriteDirection = npc.direction;
			if (npc.velocity.Y == 0)
			{
				npc.frameCounter += 10;
				if (npc.frameCounter >= 48)
				{
					npc.frameCounter -= 48;
					npc.frame.Y += 44;
					if (npc.frame.Y > 304)
					{
						npc.frame.Y = 0;
					}
				}
			}
			else
			{
				npc.frame.Y = 352;
			}
		}

		public override void NPCLoot()
        {
			Item.NewItem(npc.getRect(), ModContent.ItemType<CosmicDust>());
			if (Main.rand.NextFloat() < .0625)
				Item.NewItem(npc.getRect(), ModContent.ItemType<ArtemiteOre>(), 1 + Main.rand.Next(5));
			if (Main.rand.NextFloat() < .0625)
				Item.NewItem(npc.getRect(), ModContent.ItemType<ChandriumOre>(), 1 + Main.rand.Next(5));
			if (Main.rand.NextFloat() < .0625)
				Item.NewItem(npc.getRect(), ModContent.ItemType<SeleniteOre>(), 1 + Main.rand.Next(5));
			if (Main.rand.NextFloat() < .0625)
				Item.NewItem(npc.getRect(), ModContent.ItemType<DianiteOre>(), 1 + Main.rand.Next(5));
		}

		public override void HitEffect(int hitDirection, double damage)
		{
			if (npc.life <= 0)
			{
				Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Content/Gores/MoonZombieHead"), 1f);
				Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Content/Gores/MoonZombieArm"), 1f);
				Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Content/Gores/MoonZombieArm"), 1f);
				Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Content/Gores/MoonZombieLeg"), 1f);
				Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Content/Gores/MoonZombieLeg"), 1f);
			}
			else
			{
				for (int i = 0; i < 10; i++)
				{
					int dustType = 4;
					int dustIndex = Dust.NewDust(npc.position, npc.width, npc.height, dustType);
					Dust dust = Main.dust[dustIndex];
					dust.velocity.X = dust.velocity.X + Main.rand.Next(-50, 51) * 0.01f;
					dust.velocity.Y = dust.velocity.Y + Main.rand.Next(-50, 51) * 0.01f;
					dust.scale *= 1f + Main.rand.Next(-30, 31) * 0.01f;
				}
			}
		}
	}
}
