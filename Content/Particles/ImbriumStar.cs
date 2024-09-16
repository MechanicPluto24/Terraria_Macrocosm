using Macrocosm.Common.Drawing.Particles;
using Macrocosm.Common.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;

namespace Macrocosm.Content.Particles
{
    public class ImbriumStar : Particle
    {
        public override string TexturePath => Macrocosm.EmptyTexPath;

        public float Alpha = 0.8f;
        Color color = Color.White;

        public override void OnSpawn()
        {
            color = Color.Lerp(Color.White, new Color(0, 217, 102, 255), 0.1f + 0.7f * Main.rand.NextFloat());
            ScaleVelocity = new(-0.035f);
            //Rotation = Utility.RandomRotation();
        }

        public override void AI()
        {
            if (Scale.X < 0.002f)
                Kill();
        }

        public override bool PreDrawAdditive(SpriteBatch spriteBatch, Vector2 screenPosition, Color lightColor)
        {
            Utility.DrawStar(Position - screenPosition, 2, color.WithOpacity(Alpha), Scale);
            return false;
        }
    }
}
