using Macrocosm.Common.Drawing;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace Macrocosm.Content.Dusts
{
	public class PlasmaBallDust : ModDust
	{

		float vanishSpeed;

		public override void OnSpawn(Dust dust)
		{
			//dust.noLight = true;
			dust.scale = Main.rand.NextFloat(1, 1.35f);
			vanishSpeed = Main.rand.NextFloat(0.04f, 0.01f);
		}

		public override bool Update(Dust dust)
		{
			Projectile owner = new Projectile();
			bool hasOwner = false;

			if (dust.customData is Projectile)
			{
				owner = dust.customData as Projectile;
				hasOwner = true;
			}

			if (!hasOwner || owner.active == false)
				dust.scale -= vanishSpeed * 3f;
			else
				dust.scale -= vanishSpeed * 0.7f;

			if (dust.scale < 0.2f)
 				dust.active = false;

			return false;
		}

		public override bool MidUpdate(Dust dust) => true;


		public override Color? GetAlpha(Dust dust, Color lightColor)
			=> Color.White;
 	}
}