using Microsoft.Xna.Framework;

namespace Macrocosm.Content.Rockets.Modules
{
    public class ReactorModule : RocketModule
    {
		public override int DrawPriority => 2;

		public override Rectangle Hitbox => base.Hitbox with { Y = base.Hitbox.Y + 4 };
	}
}
