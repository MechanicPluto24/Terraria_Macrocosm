using Macrocosm.Content.Rockets.Modules.Engine;
using Microsoft.Xna.Framework;
using System.Linq;

namespace Macrocosm.Content.Rockets.Modules.Boosters
{
    public class LegModuleLeft : BaseBooster
    {
        public override SlotType Slot => SlotType.LeftSide;
        public override int Tier => 1;
        public override ConfigurationType Configuration => ConfigurationType.Any;

        public override int Width => 46;
        public override int Height => 136 + 20;

        public override Vector2 GetOffset(RocketModule[] modules)
        {
            return new
            (
                x: 25,
                y: modules[0..4].Sum(m => m.Height) - 136 - 8
            );
        }

        public override float? ExhaustOffsetX => 14f;
        protected override Vector2? LandingLegDrawOffset => new(-77.5f, 50);
        protected override int Direction => -1;


        public override AssemblyRecipe Recipe { get; } = new AssemblyRecipe().LinkWith<EngineModuleMk1>();

    }
}
