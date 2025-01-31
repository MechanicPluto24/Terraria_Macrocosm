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
        public override int Height => 136;

        public override Vector2 GetOffset(RocketModule[] modules)
        {
            return new
            (
                x: 89,
                y: modules[0..4].Sum(m => m.Height) - Height - 8
            );
        }

        protected override Vector2? LandingLegDrawOffset => new(28, 50);
        protected override int Direction => 1;

        public override AssemblyRecipe Recipe { get; } = new AssemblyRecipe().LinkWith<EngineModuleMk1>();
    }
}
