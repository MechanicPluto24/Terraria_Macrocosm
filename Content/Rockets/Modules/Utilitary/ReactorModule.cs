using Macrocosm.Content.Items.Tech;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System.Linq;
using Terraria.ModLoader;

namespace Macrocosm.Content.Rockets.Modules.Utilitary
{
    public class ReactorModule : RocketModule
    {
        public override SlotType Slot => SlotType.Utilitary;
        public override int Tier => 2;
        public override ConfigurationType Configuration => ConfigurationType.Any;

        private static Asset<Texture2D> glowmask;
        public override int DrawPriority => 2;

        public override int Width => 84;
        public override int Height => 80;

        public override Vector2 GetDynamicOffset(int[] widths, int[] heights, Vector2 globalOffsetAggregate)
        {
            return new
            (
                x: ((widths[0..4].Max() - Width) / 2) + globalOffsetAggregate.X,
                y: heights[0..2].Sum()
            );
        }

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
                spriteBatch.Draw(glowmask.Value, position, null, Color.White * Rocket.Transparency, 0f, Origin, 1f, SpriteEffects.None, 0f);
            }
        }
    }
}
