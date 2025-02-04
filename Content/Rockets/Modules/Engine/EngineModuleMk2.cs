using Macrocosm.Common.DataStructures;
using Macrocosm.Common.Utils;
using Macrocosm.Content.Items.LiquidContainers;
using Macrocosm.Content.Items.Tech;
using Microsoft.Xna.Framework;
using System.Linq;
using Terraria.ModLoader;

namespace Macrocosm.Content.Rockets.Modules.Engine
{
    [LegacyName("EngineModule")]
    public class EngineModuleMk2 : BaseEngineModule
    {
        public override SlotType Slot => SlotType.Engine;
        public override int Tier => 2;
        public override ConfigurationType Configuration => ConfigurationType.Any;

        public override int DrawPriority => 0;

        public override int Width => 84;
        public override int Height => 302;

        protected override Vector2? LandingLegDrawOffset => new(0, 10);
        protected override Vector2? BoosterRearDrawOffset => new(0, -6);

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
            new(ModContent.ItemType<RocketPlating>(), 45),
            new(ModContent.ItemType<Canister>(), 15),
            new(ModContent.ItemType<EngineComponentMk2>(), 4),
            new(ModContent.ItemType<LandingGear>(), 3)
        };
    }
}
