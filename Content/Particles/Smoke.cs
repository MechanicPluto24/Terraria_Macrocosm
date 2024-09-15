using Macrocosm.Common.Drawing.Particles;
using Macrocosm.Common.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;

namespace Macrocosm.Content.Particles
{
    public class Smoke : Particle
    {
        public override int FrameCount => 3;
        public override bool SetRandomFrameOnSpawn => true;

        public float Opacity = 1f;

        public bool FadeIn;
        private bool fadedIn;

        public float WindFactor = 0f;

        public override void OnSpawn()
        {
            if(ScaleVelocity == default)
                ScaleVelocity = new(-0.005f);
        }

        public override void AI()
        {
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

            if (Scale.X < 0.1f || (Opacity <= 0 && fadedIn))
                Kill();
        }

        public override void Draw(SpriteBatch spriteBatch, Vector2 screenPosition, Color lightColor)
        {
            spriteBatch.Draw(Texture.Value, Position - screenPosition, GetFrame(), Utility.Colorize(Color, lightColor).WithAlpha(Color.A) * Opacity, Rotation, Size * 0.5f, Scale, SpriteEffects.None, 0f);
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
