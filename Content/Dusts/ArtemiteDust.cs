using Terraria;
using Terraria.ModLoader;

namespace Macrocosm.Content.Dusts
{
	public class ArtemiteDust : ModDust
	{
		public override bool Update(Dust dust)
		{
			float lightMultiplier = 0.25f;

			dust.position += dust.velocity;
			dust.rotation += 0.1f * (dust.dustIndex % 2 == 0 ? -1 : 1);
			dust.scale -= 0.05f;

			float light = 0.25f * dust.scale;

			Lighting.AddLight(dust.position, 1 * lightMultiplier, 1 * lightMultiplier, 1 * lightMultiplier);

			if (dust.scale < 0.1f)
			{
				dust.active = false;
			}

			return true;
		}
	}
}