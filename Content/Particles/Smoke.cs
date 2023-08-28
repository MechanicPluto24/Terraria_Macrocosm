using Macrocosm.Common.Drawing.Particles;
using Macrocosm.Common.Netcode;
using Macrocosm.Common.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Graphics.PackedVector;
using Terraria;

namespace Macrocosm.Content.Particles
{
    internal class Smoke : Particle
    {
        public override int FrameNumber => 3;
        public override bool SetRandomFrameOnSpawn => true;
        float Alpha = 255;

		public override void AI()
        {
			Velocity *= 0.98f;
			Scale -= 0.007f;

            if (Alpha > 0)
                Alpha -= 4;

			if (Scale < 0.1)
 				Kill();
        }

		public override void Draw(SpriteBatch spriteBatch, Vector2 screenPosition, Color lightColor)
		{
			spriteBatch.Draw(Texture, Position - screenPosition, GetFrame(), lightColor * ((float)Alpha/255f), Rotation, Size * 0.5f, Scale, SpriteEffects.None, 0f);
		}

	}
}
