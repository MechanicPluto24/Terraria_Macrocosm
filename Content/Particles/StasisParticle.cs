using Macrocosm.Common.Drawing.Particles;
using Macrocosm.Common.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.ModLoader;

namespace Macrocosm.Content.Particles
{
    public class StasisParticle : Particle
    {
        private static Asset<Texture2D> glow;
        public override int TrailCacheLength => 7;
        public override int MaxPoolCount => 1500;

        public override void SetDefaults()
        {
        }

        public override void OnSpawn()
        {
        }

        public override bool PreDrawAdditive(SpriteBatch spriteBatch, Vector2 screenPosition, Color lightColor)
        {
            glow ??= ModContent.Request<Texture2D>(Macrocosm.FancyTexturesPath + "Circle6");
            spriteBatch.Draw(glow.Value, Center - screenPosition, null, new Color(89, 151, 193, 127) * FadeFactor, 0f, glow.Size() / 2f, 0.0425f * Scale, SpriteEffects.None, 0f);
            return true;
        }

        public override void Draw(SpriteBatch spriteBatch, Vector2 screenPosition, Color lightColor)
        {
            this.DrawMagicPixelTrail(Vector2.Zero, 4f, 1f, new Color(104, 255, 255, 255), new Color(0, 0, 0, 0));
            spriteBatch.Draw(TextureAsset.Value, Center - screenPosition, null, Color.White * FadeFactor, Rotation, Size / 2, Scale, SpriteEffects.None, 0f);
        }

        public override void AI()
        {
            Lighting.AddLight(Center, new Vector3(0.407f, 1f, 1f) * Scale.X * 0.5f);

            float decelerationFactor = ((float)TimeToLive - TimeLeft) / TimeToLive;
            Velocity *= MathHelper.Lerp(0.9f, 0.85f, decelerationFactor);

            if (Scale.X < 0.1f)
                Kill();
        }
    }
}
