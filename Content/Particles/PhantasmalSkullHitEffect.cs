using Macrocosm.Common.Drawing.Particles;
using Macrocosm.Common.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;

namespace Macrocosm.Content.Particles
{
    public class PhantasmalSkullHitEffect : Particle
    {
        public override string TexturePath => Macrocosm.TextureEffectsPath+"Star6";
        public override int MaxPoolCount => 100;

        public int StarPointCount { get; set; }
        public float FadeInFactor { get; set; }
        public float FadeOutFactor { get; set; }

        private bool fadeIn;
        private float defScale;
        private float actualScale;
        public float Opacity { get; set; }
        public override void SetDefaults()
        {
            Color = new(30, 255, 105);
            StarPointCount = 2;
            FadeInFactor = 1.25f;
            FadeOutFactor = 0.775f;
            fadeIn = true;
            actualScale = 0.1f;
            Opacity=0.1f;
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
                Opacity*= FadeInFactor;
                if (actualScale > defScale)
                    fadeIn = false;
            }
            else
            {
                actualScale *= FadeOutFactor;
                Opacity*=FadeOutFactor;
            }

            Lighting.AddLight(Center, Color.ToVector3() * actualScale * 0.5f);

            if (actualScale < 0.1f && !fadeIn)
                Kill();
        }
        public override void Draw(SpriteBatch spriteBatch, Vector2 screenPosition, Color lightColor)
        {
            spriteBatch.Draw(Texture.Value, Position - screenPosition, GetFrame(),new Color(30, 255, 105,0) * Opacity*1.5f, Rotation, Size * 0.5f, actualScale, SpriteEffects.None, 0f);
        }

        
    }
}
