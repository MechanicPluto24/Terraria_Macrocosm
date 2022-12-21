using Macrocosm.Common.Drawing;
using Macrocosm.Content.Biomes;
using Macrocosm.Content.Buffs.Debuffs;
using Macrocosm.Content.Dusts;
using Macrocosm.Content.Items.Materials;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.NPCs.Enemies.Moon
{
	public class RadioactiveSlime : MoonEnemy
	{
		public override void SetStaticDefaults()
		{
			base.SetStaticDefaults();

			DisplayName.SetDefault("Radioactive Slime");
			Main.npcFrameCount[NPC.type] = Main.npcFrameCount[NPCID.BlueSlime];
		}

		public override void SetDefaults()
		{
			base.SetDefaults();

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
			SpawnModBiomes = new int[2] { ModContent.GetInstance<MoonBiome>().Type, ModContent.GetInstance<IrradiationBiome>().Type }; // Associates this NPC with the Moon Biome & Irradiation in Bestiary
		}
		public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
		{
			bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[]
			{
				new FlavorTextBestiaryInfoElement(
					"These slimes have absorbed high amounts of uranium, chromium, and other radioactive materials. Handle with care!")
			});
		}

		public override void OnHitPlayer(Player player, int damage, bool crit)
		{
			//player.AddBuff(ModContent.BuffType<Irradiated>(), 600, true);
		}

		public override float SpawnChance(NPCSpawnInfo spawnInfo)
		{
			return spawnInfo.SpawnTileType == ModContent.TileType<Tiles.IrradiatedRock>() ? 0.1f : 0f;
		}

		public override void ModifyNPCLoot(NPCLoot loot)
		{
			
		}

		public override void HitEffect(int hitDirection, double damage)
		{
			for (int i = 0; i < 10; i++)
			{
				int dustIndex = Dust.NewDust(NPC.position, NPC.width, NPC.height, ModContent.DustType<LuminiteDust>());
				Dust dust = Main.dust[dustIndex];
				dust.velocity.X *= dust.velocity.X * 1.25f * hitDirection + Main.rand.Next(0, 100) * 0.015f;
				dust.velocity.Y *= dust.velocity.Y * 0.25f + Main.rand.Next(-50, 51) * 0.01f;
				dust.scale *= 1f + Main.rand.Next(-30, 31) * 0.01f;
			}
		}

		public override Color? GetAlpha(Color drawColor)
			=> (Color.White * (0.3f + Main.DiscoColor.GetLuminance() * 0.7f)).NewAlpha(1f);


		private SpriteBatchState state;
		public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
		{
			state = spriteBatch.SaveState();
			spriteBatch.End();
			spriteBatch.Begin(BlendState.Additive, state);

			return true;
		}

		public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
		{
			spriteBatch.Restore(state);
		}
	}
}