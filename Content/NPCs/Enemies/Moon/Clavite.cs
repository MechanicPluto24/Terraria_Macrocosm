using Macrocosm.Common.Utils;
using Macrocosm.Content.Biomes;
using Macrocosm.Content.Items.Materials;
using Macrocosm.Content.NPCs.Global;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.NPCs.Enemies.Moon
{
	public class Clavite : ModNPC, IMoonEnemy
	{
		public override void SetStaticDefaults()
		{
			base.SetStaticDefaults();

			Main.npcFrameCount[NPC.type] = 2;
		}

		public override void SetDefaults()
		{

			base.SetDefaults();

			NPC.width = 56;
			NPC.height = 56;
			NPC.lifeMax = 2500;
			NPC.damage = 60;
			NPC.defense = 60;
			NPC.HitSound = SoundID.NPCHit2;
			NPC.DeathSound = SoundID.NPCDeath2;
			NPC.value = 60f;
			NPC.knockBackResist = 0f;
			NPC.noGravity = true;
			NPC.noTileCollide = true;
 		}

		public override void AI()
		{
			Player player = Main.player[NPC.target];
			if (NPC.target < 0 || NPC.target == 255 || Main.player[NPC.target].dead || !Main.player[NPC.target].active)
			{
				NPC.TargetClosest(true);
			}

			NPC.Move(player.Center, Vector2.Zero);
			bool playerActive = player != null && player.active && !player.dead;
			Utility.LookAt(playerActive ? player.Center : NPC.Center + NPC.velocity, NPC, 0);
		}

		public override void FindFrame(int frameHeight)
		{
			int frameSpeed = 15;

			NPC.frameCounter++;   

			if (NPC.frameCounter >= frameSpeed)
			{
				NPC.frameCounter = 0;
				NPC.frame.Y += frameHeight;

				if (NPC.frame.Y >= Main.npcFrameCount[Type] * frameHeight)
				{
					NPC.frame.Y = 0;
				}
			}
		}

		public override void OnHitPlayer(Player target, Player.HurtInfo hurtInfo)
		{
			//if (player.Macrocosm().AccMoonArmor)
			//{
			//	// Now only suit breaches players with said suit 
			//	player.AddBuff(ModContent.BuffType<SuitBreach>(), 600, true);
			//}
		}
		public override float SpawnChance(NPCSpawnInfo spawnInfo)
		{
			return spawnInfo.Player.InModBiome<MoonBiome>() && Main.dayTime && spawnInfo.SpawnTileY <= Main.worldSurface + 100 ? .1f : 0f;
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
			for (int i = 0; i < 10; i++)
			{
				int dustIndex = Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Stone);
				Dust dust = Main.dust[dustIndex];
				dust.velocity.X *= dust.velocity.X * 1.25f * hit.HitDirection + Main.rand.Next(0, 100) * 0.015f;
				dust.velocity.Y *= dust.velocity.Y * 0.25f + Main.rand.Next(-50, 51) * 0.01f;
				dust.scale *= 1f + Main.rand.Next(-30, 31) * 0.01f;
			}

			if (Main.dedServ)
				return;

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
