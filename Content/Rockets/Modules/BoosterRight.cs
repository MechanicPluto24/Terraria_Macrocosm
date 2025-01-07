using Microsoft.Xna.Framework;

namespace Macrocosm.Content.Rockets.Modules
{
    public class BoosterRight : Booster
    {
        public override int Width => 124;
        public override int Height => 314;

        public override Vector2 Offset => new(152, 284);

        public override float ExhaustOffsetX => 32f;

        protected override Vector2 LandingLegDrawOffset => new(28, 208);

        public override AssemblyRecipe Recipe { get; } = new AssemblyRecipe().LinkWith<EngineModule>();
    }
}
