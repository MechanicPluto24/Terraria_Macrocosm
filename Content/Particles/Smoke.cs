using Macrocosm.Common.Drawing.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Macrocosm.Content.Particles
{
    public class Smoke : Particle
    {
        public override int FrameNumber => 3;
        public override bool SetRandomFrameOnSpawn => true;
        float Alpha = 255;

        public override void AI()
        {
            Velocity *= 0.98f;
            Scale -= 0.005f;

            if (Alpha > 0)
                Alpha -= 4;

            if (Scale < 0.1)
                Kill();
        }

        public override void Draw(SpriteBatch spriteBatch, Vector2 screenPosition, Color lightColor)
        {
            spriteBatch.Draw(Texture, Position - screenPosition, GetFrame(), lightColor * ((float)Alpha / 255f), Rotation, Size * 0.5f, Scale, SpriteEffects.None, 0f);
        }

    }
}
