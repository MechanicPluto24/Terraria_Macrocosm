using Macrocosm.Common.Drawing.Particles;
using Macrocosm.Common.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using static Macrocosm.Macrocosm;
namespace Macrocosm.Content.Particles
{
    public class IceMist : Particle
    {
        public override int FrameNumber =>1;
     
        public override string TexturePath =>TextureEffectsPath+"Smoke1";
        public Color DrawColor=new Color(56,188,173,0);
        public float Opacity = 1f;
        public float ExpansionRate = -0.008f;

        public bool FadeIn;
        private bool fadedIn;

        public float WindFactor = 0f;

        public override void AI()
        {
            Scale += ExpansionRate;

            if (!fadedIn)
            {
                Opacity += 0.03f;
                if (Opacity >= 1f)
                    fadedIn = true;
            }
            else
            {
                if (Opacity > 0f)
                    Opacity -= 0.012f;
            }

            Velocity.X += WindFactor * Utility.WindSpeedScaled;

            if (Scale < 0.1 || (Opacity <= 0 && fadedIn))
                Kill();
        }

        public override void Draw(SpriteBatch spriteBatch, Vector2 screenPosition, Color lightColor)
        {
            spriteBatch.Draw(Texture.Value, Position - screenPosition, GetFrame(), Utility.Colorize(DrawColor, lightColor).WithAlpha(DrawColor.A) * Opacity , Rotation, Size * 0.5f, Scale, SpriteEffects.None, 0f);
        }

       
    }
}
