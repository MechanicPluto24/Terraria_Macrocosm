using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace Macrocosm.Content.Dusts
{
	public class LuminiteSparkDust : ModDust
	{
		public override void OnSpawn(Dust dust)
		{
		}

		public override bool Update(Dust dust)
		{
			dust.position += dust.velocity;

			dust.rotation += dust.dustIndex % 2 == 0 ? 0.5f : -0.5f; 

			float clampedScale = dust.scale;

			if (clampedScale > 1f)
 				clampedScale = 1f;

			if (!dust.noLight)
				Lighting.AddLight((int)(dust.position.X / 16f), (int)(dust.position.Y / 16f), clampedScale * 0.2f, clampedScale * 0.725f, clampedScale * 0.51f);

			if (dust.noGravity)
				dust.velocity *= 0.93f;

			dust.scale -= 0.01f;

			if (dust.scale < 0.2f)
				dust.active = false;

			return false;
		}

		public override bool MidUpdate(Dust dust) => false;

		public override Color? GetAlpha(Dust dust, Color lightColor)
			=> Color.White;
 
	}
}