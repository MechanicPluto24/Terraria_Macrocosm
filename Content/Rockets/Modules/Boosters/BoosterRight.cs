using Macrocosm.Content.Rockets.Modules.Engine;
using Microsoft.Xna.Framework;

namespace Macrocosm.Content.Rockets.Modules.Boosters
{
    public class BoosterRight : BaseBooster
    {
        public override int Slot => 5;
        public override int Tier => 2;
        public override ConfigurationType Configuration => ConfigurationType.Any;

        public override int Width => 124;
        public override int Height => 314;

        public override Vector2 Offset => new(152, 284);

        public override bool DrawExhaust => true;
        public override float ExhaustOffsetX => 32f;
        protected override Vector2 LandingLegDrawOffset => new(28, 208);
        protected override int Direction => 1;


        public override AssemblyRecipe Recipe { get; } = new AssemblyRecipe().LinkWith<EngineModuleMk2>();
    }
}
