using Macrocosm.Content.Items.Blocks;
using Macrocosm.Content.Items.Tech;
using Microsoft.Xna.Framework;
using Terraria.ModLoader;

namespace Macrocosm.Content.Rockets.Modules
{
    public class CommandPod : RocketModule
    {
        public override int DrawPriority => 4;

        public override int Width => 68;
        public override int Height => 78;

        public override Vector2 Offset => new(104, 2);

        public override AssemblyRecipe Recipe { get; } = new AssemblyRecipe()
        {
            new(ModContent.ItemType<RocketPlating>(), 10),
            new(ModContent.ItemType<LexanGlass>(), 6),
            new(ModContent.ItemType<Computer>(), 3),
        };

    }
}
