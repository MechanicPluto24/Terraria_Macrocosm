using Macrocosm.Common.Drawing.Particles;
using Macrocosm.Common.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;

namespace Macrocosm.Content.Particles
{
    public class ArtemiteStar : Particle
    {
        public override string TexturePath => Macrocosm.EmptyTexPath;
        public override int MaxPoolCount => 100;

        public int StarPointCount { get; set; }
        public float FadeInFactor { get; set; }
        public float FadeOutFactor { get; set; }

        private bool fadeIn;
        private float defScale;
        private float actualScale;

        public override void SetDefaults()
        {
            Color = new(130, 220, 199);
            StarPointCount = 2;
            FadeInFactor = 1.25f;
            FadeOutFactor = 0.775f;
            fadeIn = true;
            actualScale = 0.1f;
        }

        public override void OnSpawn()
        {
            defScale = Scale.X;
        }

        public override void AI()
        {
            if (fadeIn)
            {
                actualScale *= FadeInFactor;

                if (actualScale > defScale)
                    fadeIn = false;
            }
            else
            {
                actualScale *= FadeOutFactor;
            }

            Lighting.AddLight(Center, Color.ToVector3() * actualScale * 0.5f);

            if (actualScale < 0.1f && !fadeIn)
                Kill();
        }

        public override bool PreDrawAdditive(SpriteBatch spriteBatch, Vector2 screenPosition, Color lightColor)
        {
            Utility.DrawStar(Position - screenPosition, StarPointCount, Color * FadeFactor, new Vector2(0.5f * actualScale, actualScale), Rotation);
            return false;
        }
    }
}
