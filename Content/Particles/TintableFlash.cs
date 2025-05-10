using Macrocosm.Common.Drawing.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;

namespace Macrocosm.Content.Particles
{
    public class TintableFlash : Particle
    {
        public override string Texture => Macrocosm.FancyTexturesPath + "Flare2";

        public override void SetDefaults()
        {
            TimeToLive = 6;
        }

        public override void OnSpawn()
        {
        }

        public override void AI()
        {
            Lighting.AddLight(Position, Color.ToVector3());
        }

        public override bool PreDrawAdditive(SpriteBatch spriteBatch, Vector2 screenPosition, Color lightColor)
        {
            spriteBatch.Draw(TextureAsset.Value, Position - screenPosition, null, Color, Rotation, TextureAsset.Size() / 2f, Scale, SpriteEffects.None, 0f);
            return false;
        }
    }
}
