using Microsoft.Xna.Framework;

namespace Macrocosm.Content.Rockets.Modules
{
    public class ServiceModule : RocketModule
    {
		public override int DrawPriority => 3;

		public override Rectangle Hitbox => base.Hitbox with { Y = base.Hitbox.Y + 2 };
	}
}
