using Macrocosm.Common.DataStructures;
using Macrocosm.Common.Drawing.Particles;
using Macrocosm.Common.Utils;
using Macrocosm.Content.NPCs.Enemies.Moon;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;

namespace Macrocosm.Content.Particles
{
    public class TintableFlash : Particle
    {
        public override string TexturePath => Macrocosm.TextureEffectsPath + "Flare2";

        public override void OnSpawn()
        {
            TimeToLive = 6;
        }

        public override void AI()
        {
            Lighting.AddLight(Position, Color.ToVector3());
        }

        public override bool PreDrawAdditive(SpriteBatch spriteBatch, Vector2 screenPosition, Color lightColor)
        {
            spriteBatch.Draw(Texture.Value, Position - screenPosition, null, Color, 0f, Texture.Size() / 2f, Scale, SpriteEffects.None, 0f);
            return false;
        }
    }
}
