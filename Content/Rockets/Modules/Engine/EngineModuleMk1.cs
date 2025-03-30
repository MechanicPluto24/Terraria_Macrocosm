using Macrocosm.Content.Items.LiquidContainers;
using Macrocosm.Content.Items.Tech;
using Microsoft.Xna.Framework;
using System.Linq;
using Terraria.ModLoader;

namespace Macrocosm.Content.Rockets.Modules.Engine
{
    public class EngineModuleMk1 : BaseEngineModule
    {
        public override SlotType Slot => SlotType.Engine;
        public override int Tier => 2;
        public override ConfigurationType Configuration => ConfigurationType.Any;

        public override int DrawPriority => 0;

        public override int Width => 72;
        public override int Height => 268;

        protected override Vector2? LandingLegDrawOffset => new(0, -6);

        public override Vector2 GetDynamicOffset(int[] widths, int[] heights, Vector2 globalOffsetAggregate)
        {
            return new
            (
                x: ((widths[0..4].Max() - Width) / 2) + globalOffsetAggregate.X,
                y: heights[0..3].Sum()
            );
        }

        public override AssemblyRecipe Recipe { get; } = new AssemblyRecipe()
        {
            new(ModContent.ItemType<RocketPlating>(), 35),
            new(ModContent.ItemType<Canister>(), 10),
            new(ModContent.ItemType<EngineComponentMk1>(), 1)
        };
    }
}
