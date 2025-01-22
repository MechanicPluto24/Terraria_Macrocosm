using Macrocosm.Common.Drawing.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;

namespace Macrocosm.Content.Particles
{
    public class PhantasmalSkullSpawnEffect : Particle
    {
        public override string TexturePath => Macrocosm.TextureEffectsPath + "Circle3";

        private bool fadeIn;
        private float defScale;
        private float actualScale;
        public float Opacity { get; set; }

        public override void SetDefaults()
        {
            Color = new Color(30, 255, 105, 255);
            actualScale = 0f;
            Color = new(30, 255, 105);
            Opacity = 0.8f;
        }

        public override void OnSpawn()
        {
            defScale = Scale.X;
        }

        public override void AI()
        {
            Opacity -= 0.05f;
            actualScale += 0.03f;

            Velocity = Vector2.Zero;

            Lighting.AddLight(Position, Color.ToVector3() * (actualScale / defScale));

            if (actualScale > 0.7f)
                Kill();
        }

        public override bool PreDrawAdditive(SpriteBatch spriteBatch, Vector2 screenPosition, Color lightColor)
        {
            spriteBatch.Draw(Texture.Value, Position - screenPosition, GetFrame(), Color * Opacity * FadeFactor, Rotation, Size * 0.5f, actualScale * 0.5f, SpriteEffects.None, 0f);
            return false;
        }
    }
}
