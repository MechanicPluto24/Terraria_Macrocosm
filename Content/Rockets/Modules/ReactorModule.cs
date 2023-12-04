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

        public override void Draw(SpriteBatch spriteBatch, Vector2 position)
        {
            base.Draw(spriteBatch, position);

            // Reactor glowmask
            spriteBatch.Draw(ModContent.Request<Texture2D>(TexturePath + "Glow").Value, position, null, Color.White, 0f, Origin, 1f, SpriteEffects.None, 0f);
        }
    }
}
