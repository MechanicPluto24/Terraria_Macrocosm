using Macrocosm.Common.Drawing;
using Macrocosm.Common.Drawing.Particles;
using Macrocosm.Common.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;

namespace Macrocosm.Content.Particles
{
	public class CelestialStar : Particle
	{
		public override string TexturePath => Macrocosm.EmptyTexPath;

		public float Alpha = 0.3f;
		private Color color = Color.White;

		bool fadeIn = true;
		float defScale;
		float actualScale;

		public override void OnSpawn()
		{
			defScale = Scale;
			actualScale = 0.5f;

        }

		public override void AI()
		{
			if (fadeIn)
			{
                actualScale *= 1.2f;

				Alpha = MathHelper.Clamp(Alpha * 1.1f, 0f, 1f);

				if (actualScale > defScale)
					fadeIn = false;
			}
			else
			{
                Alpha = MathHelper.Clamp(Alpha * 0.9f, 0f, 1f);
                actualScale *= 0.9f;
            }

            color = CelestialDisco.CelestialColor;

            if (actualScale < 0.1f)
				Kill();
		}

		public override bool PreDrawAdditive(SpriteBatch spriteBatch, Vector2 screenPosition, Color lightColor)
		{
			spriteBatch.DrawStar(Position - screenPosition, 2, color.WithOpacity(Alpha), actualScale, Rotation);
			return false;
		}
	}
}
