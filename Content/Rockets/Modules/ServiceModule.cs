using Microsoft.Xna.Framework;

namespace Macrocosm.Content.Rockets.Modules
{
    public class ServiceModule : RocketModule
    {
        public ServiceModule(Rocket rocket) : base(rocket)
        {
        }

        public override int DrawPriority => 3;

        public override int Width => 80;
        public override int Height => 110;

        public override Rectangle Hitbox => base.Hitbox with { Y = base.Hitbox.Y + 2 };
    }
}
