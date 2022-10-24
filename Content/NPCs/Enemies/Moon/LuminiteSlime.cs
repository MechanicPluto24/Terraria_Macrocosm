using Macrocosm.Common.Base;
using Macrocosm.Common.Utility;
using Macrocosm.Content.Biomes;
using Macrocosm.Content.Buffs.Debuffs;
using Macrocosm.Content.Dusts;
using Macrocosm.Content.Items.Materials;
using Macrocosm.Content.Projectiles.Unfriendly;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.NPCs.Enemies.Moon
{
	public class LuminiteSlime : MoonEnemy
	{
		public override void SetStaticDefaults()
		{
			base.SetStaticDefaults();

			DisplayName.SetDefault("Luminite Slime");
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
			AIType = NPCID.Crimslime;
			AnimationType = NPCID.BlueSlime;
			SpawnModBiomes = new int[1] { ModContent.GetInstance<MoonBiome>().Type }; // Associates this NPC with the Moon Biome in Bestiary
		}

		const float attackCooldown = 280f;

 		// Not used for this particular AIType
 		public ref float AI_AttackTimer => ref NPC.ai[1];

		public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
		{
			bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[]
			{
				new FlavorTextBestiaryInfoElement(
					"")
			});
		}

		public override float SpawnChance(NPCSpawnInfo spawnInfo)
		{
			return (spawnInfo.SpawnTileY > Main.rockLayer) ? 0.1f : 0f;
		}

		public override void OnSpawn(IEntitySource source)
		{
			AI_AttackTimer = attackCooldown;
		}

		public override void PostAI()
		{
			// fall down faster (better for underground enemies)
			if (NPC.velocity.Y < 0f)
				NPC.velocity.Y += 0.1f;

			if (Main.rand.NextBool(25))
				SpawnDusts();

			Main.NewText(AI_AttackTimer);

 			if (Main.netMode == NetmodeID.MultiplayerClient || !NPC.HasPlayerTarget)
				return;

			if(AI_AttackTimer > 0f)
				AI_AttackTimer--;

			if (AI_AttackTimer == 0f && NPC.velocity == Vector2.Zero)
			{
				Player target = Main.player[NPC.target];
				bool clearLineOfSight = Collision.CanHitLine(NPC.position, NPC.width, NPC.height, target.position, target.width, target.height);

				if (clearLineOfSight && target.active && !target.dead)
				{
					// successful attack, reset counter 
					AI_AttackTimer = attackCooldown;
					ProjectileAttack();
				}
			}
 		}

		private void ProjectileAttack()
		{
			for (int i = 0; i < Main.rand.Next(3, 7); i++)
			{
				Vector2 projVelocity = MathUtils.PolarVector(2.6f, Main.rand.NextFloat(-MathHelper.Pi + MathHelper.PiOver4, -MathHelper.PiOver4));
				Projectile proj = Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center, projVelocity, ModContent.ProjectileType<LuminiteSlimeProjectile>(), (int)(NPC.damage * 0.75f), 1f, NPC.whoAmI, ai1: NPC.target);
				proj.netUpdate = true;
			}

			for (int i = 0; i < 5; i++)
				SpawnDusts();
		}

		public override void ModifyNPCLoot(NPCLoot loot)
		{
			loot.Add(ItemDropRule.Common(ModContent.ItemType<CosmicDust>()));  
			loot.Add(ItemDropRule.Common(ItemID.LunarOre, 1, 3, 13));         
		}

		public override void HitEffect(int hitDirection, double damage)
		{
			for (int i = 0; i < 3; i++)
			{
				int dustIndex = Dust.NewDust(NPC.position, NPC.width, NPC.height, ModContent.DustType<LuminiteDust>());
				Dust dust = Main.dust[dustIndex];
				dust.velocity.X *= dust.velocity.X * 1.1f * hitDirection + Main.rand.Next(0, 100) * 0.015f;
				dust.velocity.Y *= dust.velocity.Y * 0.25f + Main.rand.Next(-50, 51) * 0.01f;
				dust.scale *= 1f + Main.rand.Next(-30, 31) * 0.01f;
			}
		}

		private void SpawnDusts()
		{
			Vector2 dustVelocity = MathUtils.PolarVector(0.01f, MiscUtils.RandomRotation());
			Dust.NewDust(NPC.position, NPC.width, NPC.height, ModContent.DustType<LuminiteDust>(), dustVelocity.X, dustVelocity.Y, newColor: Color.White * 0.1f);
		}
	}
}