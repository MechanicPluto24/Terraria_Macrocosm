using Macrocosm.Content.Rockets.Modules.Engine;
using Microsoft.Xna.Framework;
using System.Linq;

namespace Macrocosm.Content.Rockets.Modules.Boosters
{
    public class BoosterRight : BaseBooster
    {
        public override SlotType Slot => SlotType.RightSide;
        public override int Tier => 2;
        public override ConfigurationType Configuration => ConfigurationType.Any;

        public override int Width => 46;
        public override int Height => 314;

        public override Vector2 GetOffset(RocketModule[] modules)
        {
            return new
            (
                x: 56,
                y: modules[0..4].Sum(m => m.Height) - Height + 8
            );
        }

        public override float? ExhaustOffsetX => 32f;
        protected override Vector2? LandingLegDrawOffset => new(28, 208);
        protected override int Direction => 1;


        public override AssemblyRecipe Recipe { get; } = new AssemblyRecipe().LinkWith<EngineModuleMk2>();
    }
}
