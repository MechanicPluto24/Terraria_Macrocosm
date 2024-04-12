using Macrocosm.Content.Items.Blocks;
using Macrocosm.Content.Items.Materials.Tech;
using Terraria.ModLoader;

namespace Macrocosm.Content.Rockets.Modules
{
    public class CommandPod : RocketModule
    {
        public override int DrawPriority => 4;

        public override int Width => 68;
        public override int Height => 78;

        public override AssemblyRecipe Recipe { get; } = new AssemblyRecipe()
        {
            new(ModContent.ItemType<RocketPlating>(), 50),
            new(ModContent.ItemType<LexanGlass>(), 45),
            new(ModContent.ItemType<Computer>(), 5),
        };

    }
}
