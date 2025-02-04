using Macrocosm.Content.Rockets.Modules.Engine;
using Microsoft.Xna.Framework;
using System.Linq;

namespace Macrocosm.Content.Rockets.Modules.Boosters
{
    public class LegModuleRight : BaseBooster
    {
        public override SlotType Slot => SlotType.RightSide;
        public override int Tier => 1;
        public override ConfigurationType Configuration => ConfigurationType.Any;

        public override int Width => 46;
        public override int Height => 136 + 20;

        public override Vector2 GetDynamicOffset(int[] widths, int[] heights, Vector2 globalOffsetAggregate)
        {
            return new
            (
                x: 66,
                y: heights[0..4].Sum() - 136 - 8
            );
        }

        public override Rectangle ModifyRenderBounds(Rectangle bounds, Rocket.DrawMode drawMode)
        {
            if (drawMode == Rocket.DrawMode.Dummy)
            {
                int extra = LandingLegFrame.Width - (int)LandingLegDrawOffset.Value.X;
                return bounds with
                {
                    Width = bounds.Width + extra
                };
            }

            return bounds;
        }

        protected override Vector2? LandingLegDrawOffset => new(28, 50);
        protected override int Direction => 1;

        public override AssemblyRecipe Recipe { get; } = new AssemblyRecipe().LinkWith<EngineModuleMk1>();
    }
}
