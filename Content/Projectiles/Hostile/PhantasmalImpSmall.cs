using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Projectiles.Hostile
{
    public class PhantasmalImpSmall : ModProjectile
	{
		public override void SetStaticDefaults()
		{
		}

		public override void SetDefaults()
		{
			Projectile.width = 26;
			Projectile.height = 26;
			Projectile.aiStyle = ProjAIStyleID.Arrow;
			Projectile.alpha = 255;
			Projectile.hostile = true;
			Projectile.penetrate = 3;
		}


		bool spawned = false;
		public override bool PreAI()
		{
			return true;
		}

		public override bool PreDraw(ref Color lightColor)
		{
			return true;
		}
	}
}
