using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace Macrocosm.Content.Projectiles.Friendly.Weapons
{
	public class ArtemiteGreatswordProjectile : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			//DisplayName.SetDefault("Crescent Moon");
			DisplayName.SetDefault("Artemite Greatsword");
		}

		public override void SetDefaults()
		{
			Projectile.width = 34;
			Projectile.height = 32;
			Projectile.aiStyle = -1;
			Projectile.friendly = true;
			Projectile.DamageType = DamageClass.Melee;
			Projectile.tileCollide = false;
			Projectile.ignoreWater = true;
		}

		private int initialTimeleft;
		private bool saved = false;

		public override void AI()
		{

			if (!saved)
				initialTimeleft = Projectile.timeLeft;

			Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
		}
	}
}