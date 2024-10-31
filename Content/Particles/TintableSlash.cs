using Macrocosm.Common.DataStructures;
using Macrocosm.Common.Drawing.Particles;
using Macrocosm.Common.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;

namespace Macrocosm.Content.Particles
{
    public class TintableSlash : Particle
    {
        public BlendState BlendState { get; set; }
        public Color? SecondaryColor { get; set; }

        public override int FrameCount => 13;
        public override bool DespawnOnAnimationComplete => true;

        public override void SetDefaults()
        {
            FrameSpeed = 3;
            BlendState = null;
            SecondaryColor = null;
        }

        public override void OnSpawn()
        {
            SecondaryColor ??= Color;
        }

        public override void UpdateFrame()
        {
            base.UpdateFrame();

            if (currentFrame >= 12)
            {
                Kill();
                return;
            }
        }

        public override void AI()
        {
            Lighting.AddLight(Position, Color.ToVector3() * 0.6f);
        }

        private SpriteBatchState state;
        public override void Draw(SpriteBatch spriteBatch, Vector2 screenPosition, Color lightColor)
        {
            bool nonDefaultBlendState = BlendState != null;

            if (nonDefaultBlendState)
            {
                state.SaveState(spriteBatch);
                spriteBatch.End();
                spriteBatch.Begin(BlendState, state);
            }

            Rectangle frame = GetFrame().Value;
            Rectangle glowFrame = Texture.Frame(1, 13, 0, 12);
            Vector2 origin = frame.Size() / 2f;
            Vector2 scale = new Vector2(0.7f) * Scale;
            float opacity = Utils.Remap(currentFrame, 0f, 3f, 0f, 1f) * Utils.Remap(currentFrame, 4f, 12f, 1f, 0f);
            spriteBatch.Draw(Texture.Value, Position - screenPosition, glowFrame, Color * 0.3f * opacity * FadeFactor, Rotation, origin, new Vector2(1f, 6f) * scale, SpriteEffects.None, 0f);
            spriteBatch.Draw(Texture.Value, Position - screenPosition, glowFrame, Color * 0.3f * opacity * FadeFactor, Rotation, origin, new Vector2(2f, 2f) * scale, SpriteEffects.None, 0f);
            spriteBatch.Draw(Texture.Value, Position - screenPosition, frame, Color * FadeFactor, Rotation, origin, scale, SpriteEffects.None, 0f);
            spriteBatch.Draw(Texture.Value, Position - screenPosition, frame, Color * FadeFactor, Rotation, origin, scale, SpriteEffects.None, 0f);
            spriteBatch.Draw(Texture.Value, Position - screenPosition, frame, Color * FadeFactor, Rotation, origin, scale, SpriteEffects.None, 0f);
            spriteBatch.Draw(Texture.Value, Position - screenPosition, frame, Color * FadeFactor, Rotation, origin, scale, SpriteEffects.None, 0f);

            if (nonDefaultBlendState)
            {
                spriteBatch.End();
                spriteBatch.Begin(state);
            }
        }

        public override void PostDrawAdditive(SpriteBatch spriteBatch, Vector2 screenPosition, Color lightColor)
        {
            Rectangle frame = GetFrame().Value;
            Vector2 origin = frame.Size() / 2f;
            Vector2 scale = new Vector2(0.7f) * Scale;
            spriteBatch.Draw(Texture.Value, Position - screenPosition, frame, SecondaryColor.Value, Rotation, origin, scale, SpriteEffects.None, 0f);
            spriteBatch.Draw(Texture.Value, Position - screenPosition, frame, SecondaryColor.Value, Rotation, origin, scale, SpriteEffects.None, 0f);
        }
    }
}
