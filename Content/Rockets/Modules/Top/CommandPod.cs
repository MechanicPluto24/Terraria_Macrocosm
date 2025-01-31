using Macrocosm.Content.Items.Blocks;
using Macrocosm.Content.Items.Tech;
using Microsoft.Xna.Framework;
using System.Linq;
using Terraria.ModLoader;

namespace Macrocosm.Content.Rockets.Modules.Top
{
    public class CommandPod : RocketModule
    {
        public override SlotType Slot => SlotType.Top;
        public override int Tier => 1;
        public override ConfigurationType Configuration => ConfigurationType.Manned;

        public override int DrawPriority => 4;

        public override int Width => 68;
        public override int Height => 80;

        public override Vector2 GetOffset(RocketModule[] modules)
        {
            int avgW = modules[0..4].Sum(m => m.Width) / 4;
            return new
            (
                x: avgW - Width / 2,
                y: 0
            );
        }

        public override AssemblyRecipe Recipe { get; } = new AssemblyRecipe()
        {
            new(ModContent.ItemType<RocketPlating>(), 10),
            new(ModContent.ItemType<LexanGlass>(), 6),
            new(ModContent.ItemType<Computer>(), 3),
        };

    }
}
