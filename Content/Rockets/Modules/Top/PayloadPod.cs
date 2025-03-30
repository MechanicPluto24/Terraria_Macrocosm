using Macrocosm.Content.Items.Tech;
using Microsoft.Xna.Framework;
using System.Linq;
using Terraria.ModLoader;

namespace Macrocosm.Content.Rockets.Modules.Top
{
    public class PayloadPod : RocketModule
    {
        public override SlotType Slot => SlotType.Top;
        public override int Tier => 1;
        public override ConfigurationType Configuration => ConfigurationType.Unmanned;

        public override int DrawPriority => 4;

        public override int Width => 88;
        public override int Height => 78;

        public override Vector2 GetDynamicOffset(int[] widths, int[] heights, Vector2 globalOffsetAggregate)
        {
            return new
            (
                x: ((widths[0..4].Max() - Width) / 2) + globalOffsetAggregate.X,
                y: 0
            );
        }

        public override AssemblyRecipe Recipe { get; } = new AssemblyRecipe()
        {
            new(ModContent.ItemType<RocketPlating>(), 20)
        };

    }
}
