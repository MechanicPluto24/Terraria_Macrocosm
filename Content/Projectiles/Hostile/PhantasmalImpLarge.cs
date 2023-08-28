using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Projectiles.Hostile
{
    internal class PhantasmalImpLarge : PhantasmalImpSmall
	{
		public override void SetDefaults()
		{
			base.SetDefaults();
			Projectile.width = 66;
			Projectile.height = 74;
		}
	}
}
