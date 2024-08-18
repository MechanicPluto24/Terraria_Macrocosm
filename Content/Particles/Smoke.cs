using Macrocosm.Common.Drawing.Particles;
using Macrocosm.Common.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;

namespace Macrocosm.Content.Particles
{
    public class Smoke : Particle
    {
        public override int FrameNumber => 3;
        public override bool SetRandomFrameOnSpawn => true;

        public Color DrawColor;
        public float Opacity = 1f;
        public float ExpansionRate = -0.005f;

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
            spriteBatch.Draw(Texture.Value, Position - screenPosition, GetFrame(), Utility.Colorize(DrawColor, lightColor).WithAlpha(DrawColor.A) * Opacity, Rotation, Size * 0.5f, Scale, SpriteEffects.None, 0f);
        }

        public static Color GetTileHitColor(Point coords) => GetTileHitColor(coords.X, coords.Y);
        public static Color GetTileHitColor(int i, int j)
        {
            Tile hitTile = Main.tile[i, j];
            Color mapColor = Utility.GetTileColor(i, j);
            Color paintColor = Utility.GetPaintColor(hitTile);
            Color lerpedColor = Color.Lerp(mapColor, paintColor == Color.White ? Color.Gray : paintColor, 0.5f);
            return lerpedColor * Main.rand.NextFloat(0.2f, 0.8f);
        }
    }
}
