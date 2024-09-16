using Macrocosm.Common.Drawing.Particles;
using Macrocosm.Common.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;

namespace Macrocosm.Content.Particles
{
    public class ChandriumCrescentMoon : Particle
    {
        bool rotateClockwise = false;

        public override void OnSpawn()
        {
            rotateClockwise = Main.rand.NextBool();
            ScaleVelocity = new(-0.016f);
            Color = new Color(180, 112, 226);
        }

        public override void AI()
        {
            Rotation += 0.12f * (rotateClockwise ? 1f : -1f);

            if (Scale.X < 0.05f)
                Kill();

            Lighting.AddLight(Position, new Vector3(0.607f, 0.258f, 0.847f) * Scale.X);
        }

        public override void Draw(SpriteBatch spriteBatch, Vector2 screenPosition, Color lightColor)
        {
            spriteBatch.Draw(Texture.Value, Position - screenPosition, null, Color.WithOpacity(0.45f), Rotation, Texture.Size() / 2f, Scale, SpriteEffects.None, 0f);
        }
    }
}
