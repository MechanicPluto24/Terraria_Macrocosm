using Macrocosm.Content.Items.Tech;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria.ModLoader;

namespace Macrocosm.Content.Rockets.Modules
{
    public class ReactorModule : RocketModule
    {
        private static Asset<Texture2D> glowmask;
        public override int DrawPriority => 2;

        public override int Width => 84;
        public override int Height => 80;

        public override Vector2 Offset => new(96, 190);

        public override AssemblyRecipe Recipe { get; } = new AssemblyRecipe()
        {
            new(ModContent.ItemType<RocketPlating>(), 20),
            new(ModContent.ItemType<ReactorComponent>(), 8),
            new(ModContent.ItemType<ReactorHousing>(), 10),
        };

        // Reactor glowmask
        public override void PostDraw(SpriteBatch spriteBatch, Vector2 position, bool inWorld)
        {
            if (inWorld)
            {
                glowmask ??= ModContent.Request<Texture2D>(TexturePath + "_Glow");
                spriteBatch.Draw(glowmask.Value, position, null, Color.White * rocket.Transparency, 0f, Origin, 1f, SpriteEffects.None, 0f);
            }
        }
    }
}
