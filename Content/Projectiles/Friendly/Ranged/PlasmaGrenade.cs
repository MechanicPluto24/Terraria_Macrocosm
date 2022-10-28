using Macrocosm.Common.Utility;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Projectiles.Friendly.Ranged
{
	public class PlasmaGrenade : ModProjectile
	{
		public int PlasmaBallCount
		{
			get => (int)Projectile.ai[0];
			set => Projectile.ai[0] = value;
		}

		public override void SetStaticDefaults()
		{
			 
		}

		public override void SetDefaults()
		{
			Projectile.DamageType = DamageClass.Ranged;

			Projectile.width = 16;
			Projectile.height = 16;

			Projectile.timeLeft = 180;

			Projectile.tileCollide = true;

			Projectile.friendly = true;
			Projectile.hostile = false;
 			Projectile.penetrate = -1;
			Projectile.localNPCHitCooldown = -1;
			Projectile.usesLocalNPCImmunity = true;

			Projectile.aiStyle = ProjAIStyleID.Explosive;
			AIType = ProjectileID.Grenade;
		}

		public override bool PreKill(int timeLeft)
		{
			PlasmaBallCount = 100;

			if (Main.netMode == NetmodeID.MultiplayerClient)
				return true;

			if (PlasmaBallCount <= 0)
				return true;

			for (float i = 0; i < MathHelper.TwoPi; i += MathHelper.TwoPi / PlasmaBallCount)
			{
				float speed = Main.rand.NextFloat(5f, 15f);
				float theta = i + Main.rand.NextFloat(-MathHelper.PiOver4, MathHelper.PiOver4) * 0.5f;
				Vector2 velocity = MathUtils.PolarVector(speed, theta);
				Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, velocity, ModContent.ProjectileType<PlasmaBall>(), (int)(Projectile.damage * 0.5f), 0f, Projectile.owner, ai0: 200);
			}

			return true;
		}

		public override void Kill(int timeLeft)
		{

			
		}

		public override bool PreDraw(ref Color lightColor)
		{
			return true;
		}

		public override void OnHitNPC(NPC npc, int damage, float knockback, bool crit)
		{
		} 
	}
}
