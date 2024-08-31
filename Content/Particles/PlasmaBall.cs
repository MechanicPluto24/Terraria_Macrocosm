using Macrocosm.Common.Drawing.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.ModLoader;

namespace Macrocosm.Content.Particles
{
    public class PlasmaBall : Particle
    {
        private static Asset<Texture2D> glow;

        public override int SpawnTimeLeft => 135;
        public override int TrailCacheLength => 7;

        public override bool PreDrawAdditive(SpriteBatch spriteBatch, Vector2 screenPosition, Color lightColor)
        {
            glow ??= ModContent.Request<Texture2D>(Macrocosm.TextureEffectsPath + "Circle6");
            spriteBatch.Draw(glow.Value, Center - screenPosition, null, new Color(89, 151, 193, 127), 0f, glow.Size()/2f, 0.0425f * ScaleV, SpriteEffects.None, 0f);
            return true;
        }

        public override void Draw(SpriteBatch spriteBatch, Vector2 screenPosition, Color lightColor)
        {
            DrawMagicPixelTrail(Vector2.Zero, 4f, 1f, new Color(104, 255, 255, 15), new Color(104, 255, 255, 255));
            spriteBatch.Draw(Texture.Value, Center - screenPosition, null, Color.White, Rotation, Size / 2, ScaleV, SpriteEffects.None, 0f);
        }

        public override void AI()
        {
            Lighting.AddLight(Center, new Vector3(0.407f, 1f, 1f) * Scale * 0.5f);

            float decelerationFactor = ((float)SpawnTimeLeft - TimeLeft) / SpawnTimeLeft;
            Velocity *= MathHelper.Lerp(0.9f, 0.85f, decelerationFactor);

            if (Scale >= 0)
                Scale -= 0.008f;
        }
    }
}
