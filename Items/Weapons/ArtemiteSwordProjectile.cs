using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Items.Weapons
{
	public class ArtemiteSwordProjectile : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Crescent Moon");
		}

		public override void SetDefaults()
		{
			projectile.width = 10;
			projectile.height = 10;
			projectile.aiStyle = 56;
			projectile.friendly = true;
			projectile.melee = true;
			projectile.tileCollide = false;
			projectile.ignoreWater = true;
			projectile.light = 1f;
		}

		// Additional hooks/methods here.
	}
}