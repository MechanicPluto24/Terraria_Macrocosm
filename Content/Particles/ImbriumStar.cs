using Macrocosm.Common.Drawing.Particles;
using Macrocosm.Common.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;

namespace Macrocosm.Content.Particles
{
    public class ImbriumStar : Particle
    {
        public override string TexturePath => Macrocosm.TextureAssetsPath + "Star2";

        public float Alpha = 0.8f;
        Color color = Color.White;

        public override void OnSpawn()
        {
            color = Color.Lerp(Color.White, new Color(0, 217, 102, 255), 0.1f + 0.7f * Main.rand.NextFloat());
        }

        public override void AI()
        {
            Scale -= 0.003f;

            if (Scale < 0.0001f)
                Kill();
        }

        public override bool PreDrawAdditive(SpriteBatch spriteBatch, Vector2 screenPosition, Color lightColor)
        {
            spriteBatch.Draw(Texture, Position - screenPosition, null, color.WithOpacity(Alpha), Rotation, Texture.Size() / 2f, ScaleV, SpriteEffects.None, 0f);
            return false;
        }
    }
}
