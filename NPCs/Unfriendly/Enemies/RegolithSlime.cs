using Macrocosm.Items.Currency;
using Macrocosm.Items.Materials;
using System.ComponentModel;
using System.Media;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.NPCs.Unfriendly.Enemies
{
	// Party Zombie is a pretty basic clone of a vanilla NPC. To learn how to further adapt vanilla NPC behaviors, see https://github.com/tModLoader/tModLoader/wiki/Advanced-Vanilla-Code-Adaption#example-npc-npc-clone-with-modified-projectile-hoplite
	public class RegolithSlime : ModNPC
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Regolith Slime");
			Main.npcFrameCount[npc.type] = Main.npcFrameCount[NPCID.BlueSlime];
		}

		public override void SetDefaults()
		{
			npc.width = 18;
			npc.height = 40;
			npc.damage = 50;
			npc.defense = 60;
			npc.lifeMax = 2000;
			npc.HitSound = SoundID.NPCHit1;
			npc.DeathSound = SoundID.NPCDeath1;
			npc.value = 60f;
			npc.knockBackResist = 0.5f;
			npc.aiStyle = 1;
			aiType = NPCID.BlueSlime;
			animationType = NPCID.BlueSlime;
			banner = Item.NPCtoBanner(NPCID.BlueSlime);
			bannerItem = Item.BannerToItem(banner);
		}

		public override void OnHitPlayer(Player player, int damage, bool crit)
        {
			player.AddBuff(mod.BuffType("SuitBreach"), 600, true);
        }

		public override float SpawnChance(NPCSpawnInfo spawnInfo)
		{
			return spawnInfo.spawnTileType == ModContent.TileType<Tiles.Regolith>() ? .1f : 0f;
		}

		public override void NPCLoot()
        {
			Item.NewItem(npc.getRect(), ModContent.ItemType<CosmicDust>());
			if (Main.rand.NextFloat() < .0625)
				Item.NewItem(npc.getRect(), ModContent.ItemType<ArtemiteOre>(), 1 + Main.rand.Next(5));
			if (Main.rand.NextFloat() < .0625)
				Item.NewItem(npc.getRect(), ModContent.ItemType<ChandriumOre>(), 1 + Main.rand.Next(5));
			if (Main.rand.NextFloat() < .0625)
				Item.NewItem(npc.getRect(), ModContent.ItemType<SeleniumOre>(), 1 + Main.rand.Next(5));
			if (Main.rand.NextFloat() < .0625)
				Item.NewItem(npc.getRect(), ModContent.ItemType<DianiteOre>(), 1 + Main.rand.Next(5));
			Item.NewItem(npc.getRect(), ModContent.ItemType<UnuCredit>(), 1);
		}

		public override void HitEffect(int hitDirection, double damage)
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