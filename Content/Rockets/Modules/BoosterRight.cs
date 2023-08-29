using Microsoft.Xna.Framework;

namespace Macrocosm.Content.Rockets.Modules
{
	public class BoosterRight : Booster
    {
		public override int Width => 46;
		public override int Height => 304;

		public override Rectangle Hitbox => base.Hitbox with { Width = base.Hitbox.Width + 78, Height = base.Hitbox.Height + 8 };

		protected override Vector2 LandingLegDrawOffset => new(28, 208);
	}
}
