using Macrocosm.Content.Rockets.Modules.Engine;
using Microsoft.Xna.Framework;

namespace Macrocosm.Content.Rockets.Modules.Boosters
{
    public class BoosterLeft : BaseBooster
    {
        public override SlotType Slot => SlotType.LeftSide;
        public override int Tier => 2;
        public override ConfigurationType Configuration => ConfigurationType.Any;

        public override int Width => 46;
        public override int Height => 314;

        public override Vector2 GetOffset(RocketModule[] modules)
        {
            return new
            (
                x: 28,
                y: 280
            );
        }

        public override float? ExhaustOffsetX => 14f;
        protected override Vector2? LandingLegDrawOffset => new(-78, 208);
        protected override int Direction => -1;


        public override AssemblyRecipe Recipe { get; } = new AssemblyRecipe().LinkWith<EngineModuleMk2>();

    }
}
