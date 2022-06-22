using Macrocosm.Content.Items.Currency;
using Macrocosm.Content.Items.Materials;
using Macrocosm.Content.Buffs.Debuffs;
using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.ModLoader;
using Macrocosm.Content.Biomes;

namespace Macrocosm.Content.NPCs.Unfriendly.Enemies
{
	public class RegolithSlime : ModNPC
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Regolith Slime");
			Main.npcFrameCount[NPC.type] = Main.npcFrameCount[NPCID.BlueSlime];
		}

		public override void SetDefaults()
		{
			NPC.width = 36;  
			NPC.height = 22;
			NPC.damage = 50;
			NPC.defense = 60;
			NPC.lifeMax = 2000;
			NPC.HitSound = SoundID.NPCHit1;
			NPC.DeathSound = SoundID.NPCDeath1;
			NPC.value = 60f;
			NPC.knockBackResist = 0.5f;
			NPC.aiStyle = 1;
			AIType = NPCID.BlueSlime;
			AnimationType = NPCID.BlueSlime;
			Banner = Item.NPCtoBanner(NPCID.BlueSlime);
			BannerItem = Item.BannerToItem(Banner);
			SpawnModBiomes = new int[1] { ModContent.GetInstance<MoonBiome>().Type }; // Associates this NPC with the Moon Biome in Bestiary
		}
		public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
		{
			bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[]
			{
				new FlavorTextBestiaryInfoElement(
					"Even extreme environments such as the Moon can host equally extreme types of slimes, mixing with the Moon's silvery regolith.")
			});
		}

		public override void OnHitPlayer(Player player, int damage, bool crit)
        {
			player.AddBuff(ModContent.BuffType<SuitBreach>(), 600, true);
        }

		public override float SpawnChance(NPCSpawnInfo spawnInfo)
		{
			return spawnInfo.SpawnTileType == ModContent.TileType<Tiles.Regolith>() && Main.dayTime ? 0.1f : 0f;
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
				int dustIndex = Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.TintableDust);
				Dust dust = Main.dust[dustIndex];
				dust.velocity.X *= dust.velocity.X * 1.25f * hitDirection + Main.rand.Next(0, 100) * 0.015f;
				dust.velocity.Y *= dust.velocity.Y * 0.25f + Main.rand.Next(-50, 51) * 0.01f;
				dust.scale *= 1f + Main.rand.Next(-30, 31) * 0.01f;
			}
		}
	}
}