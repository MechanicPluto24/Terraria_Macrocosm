using Macrocosm.Common.Sets;
using Macrocosm.Common.Utils;
using Macrocosm.Content.Items.Machines;
using Macrocosm.Content.Items.Machines.Consumers.Oxygen;
using Macrocosm.Content.Items.Tech;
using Microsoft.Xna.Framework;
using System.Linq;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Macrocosm.Content.Rockets.Modules.Service
{
    public class ServiceModule : RocketModule
    {
        public override SlotType Slot => SlotType.Service;
        public override int Tier => 1;
        public override ConfigurationType Configuration => ConfigurationType.Manned;

        public override int DrawPriority => 3;

        public override int Width => 80;
        public override int Height => 110;

        public override Vector2 GetDynamicOffset(int[] widths, int[] heights, Vector2 globalOffsetAggregate)
        {
            return new
            (
                x: ((widths[0..4].Max() - Width) / 2) + globalOffsetAggregate.X,
                y: heights[0]
            );
        }

        public override AssemblyRecipe Recipe { get; } = new AssemblyRecipe()
        {
            new(ModContent.ItemType<RocketPlating>(), 25),
            new(ModContent.ItemType<PowerJunction>(), 3),
            new(ModContent.ItemType<OxygenSystem>(), 3),
            new((item) => item.IsChest(), Language.GetText("Mods.Macrocosm.UI.LaunchPad.AnyChest"), 1)
        };
    }
}
