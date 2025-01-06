using Microsoft.Xna.Framework;

namespace Macrocosm.Content.Rockets.Modules
{
    public class BoosterLeft : Booster
    {
        public override int Width => 124;
        public override int Height => 314;

        public override Vector2 Offset => new(78, 284); 

        public override float ExhaustOffsetX => 14f;

        protected override Vector2 LandingLegDrawOffset => new(-78, 208);

        public override AssemblyRecipe Recipe { get; } = new AssemblyRecipe().LinkWith<EngineModule>();

    }
}
