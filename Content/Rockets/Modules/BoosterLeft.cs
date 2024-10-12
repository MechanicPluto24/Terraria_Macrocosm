using Microsoft.Xna.Framework;

namespace Macrocosm.Content.Rockets.Modules
{
    public class BoosterLeft : Booster
    {
        public override int Width => 46;
        public override int Height => 304;

        public override Rectangle Hitbox => base.Hitbox with { X = base.Hitbox.X - 78, Width = base.Hitbox.Width + 78, Height = base.Hitbox.Height + 8 };
        public override float ExhaustOffsetX => 14f;

        protected override Vector2 LandingLegDrawOffset => new(-78, 208);

        public override AssemblyRecipe Recipe { get; } = new AssemblyRecipe().LinkWith<EngineModule>();

    }
}
