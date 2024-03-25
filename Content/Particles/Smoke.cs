using Macrocosm.Common.Drawing.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Macrocosm.Content.Particles
{
	public class Smoke : Particle
	{
		public override int FrameNumber => 3;
		public override bool SetRandomFrameOnSpawn => true;

		public Color? Color;

		public bool FadeIn;
		private bool fadedIn;

		public float Opacity = 1f;

		public override void AI()
		{
			Velocity *= 0.98f;
			Scale -= 0.005f;

			if(!fadedIn)
			{
                Opacity += 0.03f;

				if (Opacity >= 1f)
					fadedIn = true;
            }
			else
			{
                if (Opacity > 0f)
                    Opacity -= 0.015f;
            }

            if (Scale < 0.1)
				Kill();
		}

		public override void Draw(SpriteBatch spriteBatch, Vector2 screenPosition, Color lightColor)
		{
			spriteBatch.Draw(Texture, Position - screenPosition, GetFrame(), (Color ?? lightColor) * Opacity, Rotation, Size * 0.5f, Scale, SpriteEffects.None, 0f);
		}

	}
}
