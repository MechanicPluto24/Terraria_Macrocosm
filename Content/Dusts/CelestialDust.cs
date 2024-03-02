using Macrocosm.Common.Drawing;
using Macrocosm.Common.Utils;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace Macrocosm.Content.Dusts
{
	public class CelestialDust : ModDust
	{
		public override void OnSpawn(Dust dust)
		{
			dust.noLight = true;
		}

		public override bool Update(Dust dust)
		{
			dust.velocity.Y += 0.01f;
			dust.position += dust.velocity;
			dust.scale -= 0.05f;
			dust.rotation += (dust.velocity.Y - dust.velocity.X) / 5;

			dust.color = CelestialDisco.CelestialColor.WithOpacity(0.8f);

			Lighting.AddLight(dust.position, dust.color.ToVector3() * 0.6f * dust.scale);

			if (dust.scale < 0.2f)
				dust.active = false;

			return false;
		}

		public override bool MidUpdate(Dust dust) => true;


		public override Color? GetAlpha(Dust dust, Color lightColor)
			=> Color.White;
	}
}