using Macrocosm.Common.Drawing.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;

namespace Macrocosm.Content.Particles
{
    public class TintableFlash : Particle
    {
        public override string TexturePath => Macrocosm.TextureEffectsPath + "Flare2";

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
            spriteBatch.Draw(Texture.Value, Position - screenPosition, null, Color, Rotation, Texture.Size() / 2f, Scale, SpriteEffects.None, 0f);
            return false;
        }
    }
}
