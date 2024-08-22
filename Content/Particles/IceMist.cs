using Macrocosm.Common.Drawing.Particles;
using Macrocosm.Common.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Macrocosm.Content.Particles
{
    public class IceMist : Particle
    {
        public override int FrameNumber => 1;

        public override string TexturePath => Macrocosm.TextureEffectsPath + "Smoke1";
        public Color DrawColor = new Color(56, 188, 173, 0);
        public float Opacity = 0.3f;
        public float ExpansionRate = -0.008f;
        
        public bool FadeIn;
        private bool fadedIn=false;

        public float WindFactor = 0f;

        public override void AI()
        {
            if (!fadedIn)
            {
                Scale -= ExpansionRate/3;
                Opacity += 0.05f;
                if (Opacity >= 1f)
                    fadedIn = true;
            }
            else
            {
                Scale += ExpansionRate/3;
                if (Opacity > 0f)
                    Opacity -= 0.012f;
            }

            Velocity.X += WindFactor * Utility.WindSpeedScaled;

            if (Scale < 0.1f || (Opacity <= 0 && fadedIn))
                Kill();
        }

        public override void Draw(SpriteBatch spriteBatch, Vector2 screenPosition, Color lightColor)
        {
            spriteBatch.Draw(Texture.Value, Position - screenPosition, GetFrame(), Utility.Colorize(DrawColor, lightColor).WithAlpha(DrawColor.A) * Opacity, Rotation, Size * 0.5f, Scale, SpriteEffects.None, 0f);
        }
    }
}
