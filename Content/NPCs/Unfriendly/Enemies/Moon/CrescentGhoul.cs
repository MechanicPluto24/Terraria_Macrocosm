using Macrocosm.Base.BaseMod;
using Macrocosm.Common.Utility;
using Macrocosm.Content.Biomes;
using Macrocosm.Content.Buffs.Debuffs;
using Macrocosm.Content.Dusts;
using Macrocosm.Content.Items.Materials;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.NPCs.Unfriendly.Enemies.Moon
{
	public class CrescentGhoul : MoonEnemy
	{

		public enum ActionState
		{
			Chase,
			Dash
		}

		public ActionState AI_State  
		{
			get => (ActionState)NPC.ai[0];
			set => NPC.ai[0] = (int)value;
		}
		public ref float AI_Timer => ref NPC.ai[1];

		public override void SetStaticDefaults()
		{
			base.SetStaticDefaults();

			Main.npcFrameCount[NPC.type] = 4;
			NPCID.Sets.TrailCacheLength[NPC.type] = 4;
			NPCID.Sets.TrailingMode[NPC.type] = 0;
		}

		public override void SetDefaults()
		{

			base.SetDefaults();

			NPC.width = 72;
			NPC.height = 84;
			NPC.lifeMax = 2500;
			NPC.damage = 60;
			NPC.defense = 60;
			NPC.HitSound = SoundID.NPCHit2;
			NPC.DeathSound = SoundID.NPCDeath2;
			NPC.value = 60f;
			NPC.knockBackResist = 0f;
			NPC.noGravity = true;
			NPC.noTileCollide = true;
			SpawnModBiomes = new int[1] { ModContent.GetInstance<MoonBiome>().Type }; // Associates NPC NPC with the Moon Biome in Bestiary
		}

		public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
		{
			bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[]
			{
				new FlavorTextBestiaryInfoElement(
					" ")
			});
		}

		public override void AI()
		{
			Player player = Main.player[NPC.target];

			if (NPC.target < 0 || NPC.target == 255 || Main.player[NPC.target].dead || !Main.player[NPC.target].active)
			{
				NPC.TargetClosest(true);
			}

			AI_Timer++;

			switch (AI_State)
			{
				case ActionState.Chase:
					NPC.Move(player.Center, Vector2.Zero);
					bool playerActive = player != null && player.active && !player.dead;
					BaseAI.LookAt(playerActive ? player.Center : NPC.Center + NPC.velocity, NPC, 0);

					if(AI_Timer > 200 && Vector2.Distance(NPC.Center, player.Center) < 300f)
					{
						AI_Timer = 0;
						AI_State = ActionState.Dash;
					}
					break;

				case ActionState.Dash:

					NPC.rotation += 0.36f;

					NPC.Move(player.Center, Vector2.Zero, 7f);

					if (AI_Timer > 100 || Vector2.Distance(NPC.Center, player.Center) < 30f)
					{
						AI_Timer = 0;
						AI_State = ActionState.Chase;
					}

					break;
			}
		}

		public override void OnHitPlayer(Player player, int damage, bool crit)
		{
			if (player.GetModPlayer<MacrocosmPlayer>().accMoonArmor)
			{
				player.AddBuff(ModContent.BuffType<SuitBreach>(), 600, true);
			}
		}
		public override float SpawnChance(NPCSpawnInfo spawnInfo)
		{
			return spawnInfo.Player.GetModPlayer<MacrocosmPlayer>().ZoneMoon && !Main.dayTime && spawnInfo.SpawnTileY <= Main.worldSurface + 100 ? .1f : 0f;
		}

		public override void ModifyNPCLoot(NPCLoot loot)
		{
			loot.Add(ItemDropRule.Common(ModContent.ItemType<CosmicDust>()));             // Always drop 1 cosmic dust
			loot.Add(ItemDropRule.Common(ModContent.ItemType<ArtemiteOre>(), 16, 1, 6));  // 1/16 chance to drop 1-6 Artemite Ore
			loot.Add(ItemDropRule.Common(ModContent.ItemType<ChandriumOre>(), 16, 1, 6)); // 1/16 chance to drop 1-6 Chandrium Ore
			loot.Add(ItemDropRule.Common(ModContent.ItemType<SeleniteOre>(), 16, 1, 6));  // 1/16 chance to drop 1-6 Selenite Ore
			loot.Add(ItemDropRule.Common(ModContent.ItemType<DianiteOre>(), 16, 1, 6));   // 1/16 chance to drop 1-6 DianiteOre Ore
		}

		public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
		{
			if (AI_State != ActionState.Dash)
				return true;

			for (int i = 0; i < NPC.oldPos.Length; i++)
			{
				Vector2 drawPos = NPC.oldPos[i] + NPC.Size / 2 - Main.screenPosition;
				Color color = NPC.GetAlpha(drawColor) * (((float)NPC.oldPos.Length - i) / NPC.oldPos.Length);
				spriteBatch.Draw(TextureAssets.Npc[NPC.type].Value, drawPos, NPC.frame, color * 0.6f, NPC.rotation - 0.36f * i, NPC.Size/2, NPC.scale, SpriteEffects.None, 0f);
			}

			return true;
		}

		public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
		{
			Texture2D glowmask = ModContent.Request<Texture2D>("Macrocosm/Content/NPCs/Unfriendly/Enemies/Moon/CrescentGhoulGlow").Value;
			SpriteEffects effect = NPC.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
			NPC.DrawGlowmask(spriteBatch, glowmask, screenPos, effect: effect);
		}

		public override void FindFrame(int frameHeight)
		{
			NPC.frame.Y = (int)(NPC.frameCounter / 10) * frameHeight;

			if(NPC.localAI[0] == 0f)
			{
				NPC.frameCounter++;

				if (NPC.frameCounter >= 39)
					NPC.localAI[0] = 1f;
			} 
			else if(NPC.localAI[0] == 1f)
			{
				NPC.frameCounter--;

				if(NPC.frameCounter <= 0)
					NPC.localAI[0] = 0f;
			}
			else
			{
				NPC.localAI[0] = 0f;
			}
		}

		public override void HitEffect(int hitDirection, double damage)
		{
			for (int i = 0; i < 10; i++)
			{
				int dustIndex = Dust.NewDust(NPC.position, NPC.width, NPC.height, ModContent.DustType<RegolithDust>());
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

				for (int i = 0; i < 50; i++)
				{
					int dustIndex = Dust.NewDust(NPC.position, NPC.width, NPC.height, ModContent.DustType<RegolithDust>());
					Dust dust = Main.dust[dustIndex];
					dust.velocity.X *= dust.velocity.X * 1.25f * hitDirection + Main.rand.Next(0, 100) * 0.015f;
					dust.velocity.Y *= dust.velocity.Y * 0.25f + Main.rand.Next(-50, 51) * 0.01f;
					dust.scale *= 1f + Main.rand.Next(-30, 31) * 0.01f;
				}
			}
		}
	}
}
