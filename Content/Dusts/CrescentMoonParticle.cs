using Macrocosm.Common.Drawing;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace Macrocosm.Content.Dusts
{
	public class CrescentMoonParticle : ModDust
	{
		bool fadeIn;

		public override void OnSpawn(Dust dust)
		{
			dust.noGravity = true;
			dust.alpha = 255;
			fadeIn = true;
			dust.frame = new Rectangle(0, 0, 26, 26);
		}

		public override bool Update(Dust dust)
		{
			dust.rotation += 0.08f * (dust.dustIndex % 2 == 0 ? 1f : -1f);
			dust.position += dust.velocity;

			if (dust.alpha <= 24)
				fadeIn = false;

			if (fadeIn)
			{
				dust.alpha -= 15;
				dust.scale += 0.01f;
			}
			else
			{
				dust.alpha++;
				dust.scale -= 0.01f;
			}

			dust.scale -= 0.01f;
			dust.alpha++;

			if (dust.scale < 0.3f)
				dust.active = false;

			Lighting.AddLight(dust.position, new Vector3(0.607f, 0.258f, 0.847f) * dust.scale);

			return false;
		}
	}
}
