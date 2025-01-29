using Macrocosm.Content.Items.Blocks;
using Macrocosm.Content.Items.Tech;
using Microsoft.Xna.Framework;
using Terraria.ModLoader;

namespace Macrocosm.Content.Rockets.Modules
{
    public class PayloadPod : RocketModule
    {
        public override int Slot => 0;
        public override int Tier => 1;
        public override ConfigurationType Configuration => ConfigurationType.Unmanned;

        public override int DrawPriority => 4;

        public override int Width => 88;
        public override int Height => 78;

        public override Vector2 Offset => new(94, 0);

        public override AssemblyRecipe Recipe { get; } = new AssemblyRecipe()
        {
            new(ModContent.ItemType<RocketPlating>(), 20)
        };

    }
}
