using Macrocosm.Content.Rockets.Modules.Engine;
using Microsoft.Xna.Framework;

namespace Macrocosm.Content.Rockets.Modules.Boosters
{
    public class BoosterLeft : BaseBooster
    {
        public override int Slot => 4;
        public override int Tier => 2;
        public override ConfigurationType Configuration => ConfigurationType.Any;

        public override int Width => 124;
        public override int Height => 314;

        public override Vector2 Offset => new(78, 284);

        public override bool DrawExhaust => true;
        public override float ExhaustOffsetX => 14f;
        protected override Vector2 LandingLegDrawOffset => new(-78, 208);
        protected override int Direction => -1;


        public override AssemblyRecipe Recipe { get; } = new AssemblyRecipe().LinkWith<EngineModuleMk2>();

    }
}
