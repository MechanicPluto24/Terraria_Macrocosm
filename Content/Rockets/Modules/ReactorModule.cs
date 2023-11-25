using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.ModLoader;

namespace Macrocosm.Content.Rockets.Modules
{
    public class ReactorModule : RocketModule
    {
        public ReactorModule(Rocket rocket) : base(rocket)
        {
        }

        public override int DrawPriority => 2;

        public override int Width => 84;
        public override int Height => 80;

        public override Rectangle Hitbox => base.Hitbox with { Y = base.Hitbox.Y + 4 };

        public override void Draw(SpriteBatch spriteBatch, Vector2 screenPos, Color ambientColor)
        {
            base.Draw(spriteBatch, screenPos, ambientColor);

            // Reactor glowmask
            spriteBatch.Draw(ModContent.Request<Texture2D>(TexturePath + "Glow").Value, Position - screenPos, null, Color.White, 0f, Origin, 1f, SpriteEffects.None, 0f);
        }
    }
}
