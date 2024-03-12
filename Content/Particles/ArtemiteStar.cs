using Macrocosm.Common.Drawing;
using Macrocosm.Common.Drawing.Particles;
using Macrocosm.Common.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;

namespace Macrocosm.Content.Particles
{
	public class ArtemiteStar : Particle
	{
		public override string TexturePath => Macrocosm.EmptyTexPath;

		public float Alpha = 0.3f;
        public Color Color = new(130, 220, 199);

		bool fadeIn = true;
		float defScale;
		float actualScale;

		public override void OnSpawn()
		{
			defScale = Scale;
			actualScale = 0.1f;
        }

		public override void AI()
		{
			if (fadeIn)
			{
                actualScale *= 1.25f;

				if (actualScale > defScale)
					fadeIn = false;
			}
			else
			{
                actualScale *= 0.775f;
            }

			Lighting.AddLight(Center, Color.ToVector3() * actualScale);

            if (actualScale < 0.2f && !fadeIn)
				Kill();
		}

		public override bool PreDrawAdditive(SpriteBatch spriteBatch, Vector2 screenPosition, Color lightColor)
		{
			spriteBatch.DrawStar(Position - screenPosition, 2, Color, actualScale, Rotation);
			return false;
		}
	}
}
