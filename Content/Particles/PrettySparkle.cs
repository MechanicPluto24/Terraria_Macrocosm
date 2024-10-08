using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria.GameContent;
using Terraria.Graphics.Renderers;
using Terraria;
using Macrocosm.Common.Drawing.Particles;
using Microsoft.Xna.Framework;
using Terraria.ID;

namespace Macrocosm.Content.Particles
{
    /// <summary> Adapted from vanilla <see cref="PrettySparkleParticle"/> </summary>
    public class PrettySparkle : Particle
    {
        public override string TexturePath => Macrocosm.EmptyTexPath;

        public float AdditiveAmount;
        public float FadeInEnd;
        public float FadeOutStart;
        public float FadeOutEnd;
        public bool DrawHorizontalAxis;
        public bool DrawVerticalAxis;

        public override void SetDefaults()
        {
            FadeInNormalizedTime = 0.05f;
            FadeOutNormalizedTime = 0.9f;
            TimeToLive = 60;
            AdditiveAmount = 1f;
            FadeInEnd = 20f;
            FadeOutStart = 30f;
            FadeOutEnd = 45f;
            DrawVerticalAxis = DrawHorizontalAxis = true;
        }

        public override void AI()
        {
        }

        public override void Draw(SpriteBatch spriteBatch, Vector2 screenPosition, Color lightColor)
        {
            Texture2D texture = TextureAssets.Extra[ExtrasID.SharpTears].Value;
            Vector2 position = Position - screenPosition;
            Vector2 origin = texture.Size() / 2f;
            SpriteEffects effects = SpriteEffects.None;

            float t = ((TimeToLive - TimeLeft) / (float)TimeToLive) * 60f;
            float fade = Utils.GetLerpValue(0f, FadeInEnd, t, clamped: true) * Utils.GetLerpValue(FadeOutEnd, FadeOutStart, t, clamped: true);

            Color baseColor = Color.White * FadeFactor * 0.9f;
            baseColor.A /= 2;

            Color tintColor = Color * FadeFactor * 0.5f;
            tintColor.A = (byte)(tintColor.A * (1f - AdditiveAmount));

            Color innerColor = baseColor * 0.5f;

            Vector2 scaleX = new Vector2(0.3f, 1f) * fade * Scale;
            Vector2 scaleY = new Vector2(0.3f, 1f) * fade * Scale;

            tintColor *= fade;
            innerColor *= fade;

            if (DrawHorizontalAxis)
                spriteBatch.Draw(texture, position, null, tintColor, (float)Math.PI / 2f + Rotation, origin, scaleX, effects, 0f);

            if (DrawVerticalAxis)
                spriteBatch.Draw(texture, position, null, tintColor, 0f + Rotation, origin, scaleY, effects, 0f);

            if (DrawHorizontalAxis)
                spriteBatch.Draw(texture, position, null, innerColor, (float)Math.PI / 2f + Rotation, origin, scaleX * 0.6f, effects, 0f);

            if (DrawVerticalAxis)
                spriteBatch.Draw(texture, position, null, innerColor, 0f + Rotation, origin, scaleY * 0.6f, effects, 0f);
        }
    }
}
