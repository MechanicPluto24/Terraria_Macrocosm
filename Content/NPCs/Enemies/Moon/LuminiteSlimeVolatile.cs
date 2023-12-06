using Macrocosm.Common.Utils;
using Macrocosm.Content.Projectiles.Hostile;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace Macrocosm.Content.NPCs.Enemies.Moon
{
	public class LuminiteSlimeVolatile : LuminiteSlime
	{
		public override void SetDefaults()
		{
			base.SetDefaults();
		}

		public override float SpawnChance(NPCSpawnInfo spawnInfo)
		{
			return base.SpawnChance(spawnInfo);
		}

		public override void HitEffect(NPC.HitInfo hit)
		{
			SpawnDusts(4);

			if (NPC.life <= 0)
				SpawnDusts(45);
		}

		public override bool PreAI()
		{
			Utility.AISlime(NPC, ref NPC.ai, false, false, 140, 5, -8, 6, -12);

			if (NPC.velocity.Y < 0f)
				NPC.velocity.Y += 0.25f;

			return true;
		}

		public override void AI()
		{
			base.AI();
		}

		public override void OnKill()
		{
			Projectile.NewProjectile(NPC.GetSource_Death(), NPC.Center, default, ModContent.ProjectileType<LuminiteExplosion>(), (int)(NPC.damage * 0.65f), 3, Main.myPlayer);

			for (int i = 0; i < 5; i++)
			{
				Projectile.NewProjectile(NPC.GetSource_Death(), NPC.Center, (-Vector2.UnitY).RotatedByRandom(MathHelper.PiOver2) * Main.rand.NextFloat(5f, 10f), ModContent.ProjectileType<LuminiteShard>(), (int)(NPC.damage * 0.5f), 1f, Main.myPlayer, ai1: -1, ai2: 1);
			}
		}

		protected override void ProjectileAttack()
		{
			for (int i = 0; i < Main.rand.Next(3, 7); i++)
			{
				Vector2 projVelocity = Utility.PolarVector(2.6f, Main.rand.NextFloat(-MathHelper.Pi + MathHelper.PiOver4, -MathHelper.PiOver4));
				Projectile proj = Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center, projVelocity, ModContent.ProjectileType<LuminiteStar>(), (int)(NPC.damage * 0.75f), 1f, Main.myPlayer, ai1: NPC.target);
				proj.netUpdate = true;
			}

			SpawnDusts(5);
		}
	}
}