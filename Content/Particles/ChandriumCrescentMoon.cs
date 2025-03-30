using Macrocosm.Common.Drawing.Particles;
using Macrocosm.Common.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;

namespace Macrocosm.Content.Particles
{
    public class ChandriumCrescentMoon : Particle
    {
        public override void SetDefaults()
        {
            ScaleVelocity = new(-0.016f);
            Color = new Color(180, 112, 226);
            RotationVelocity = 0.12f * (Main.rand.NextBool() ? 1f : -1f);
        }

        public override void OnSpawn()
        {
        }

        public override void AI()
        {
            if (Scale.X < 0.05f)
                Kill();

            Lighting.AddLight(Position, new Vector3(0.607f, 0.258f, 0.847f) * Scale.X);
        }

        public override void Draw(SpriteBatch spriteBatch, Vector2 screenPosition, Color lightColor)
        {
            spriteBatch.Draw(TextureAsset.Value, Position - screenPosition, null, Color.WithOpacity(0.45f), Rotation, TextureAsset.Size() / 2f, Scale, SpriteEffects.None, 0f);
        }
    }
}
