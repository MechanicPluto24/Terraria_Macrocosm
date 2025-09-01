using Macrocosm.Common.Drawing.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;

namespace Macrocosm.Content.Particles
{
    public class TintableExplosionFiery : Particle
    {
        public override int FrameCount => 5;
        public override bool DespawnOnAnimationComplete => true;

        public override void SetDefaults()
        {
            FrameSpeed = 3;
        }

        public override void OnSpawn()
        {
        }

        public override void AI()
        {
            Lighting.AddLight(Center, Color.ToVector3());
        }

        public override void Draw(SpriteBatch spriteBatch, Vector2 screenPosition, Color lightColor)
        {
            //spriteBatch.Draw(Texture.Value, Position - screenPosition, GetFrame(), Color, Rotation, Size * 0.5f, Scale, SpriteEffects.None, 0f);
            base.Draw(spriteBatch, screenPosition, lightColor);
        }
    }
}
