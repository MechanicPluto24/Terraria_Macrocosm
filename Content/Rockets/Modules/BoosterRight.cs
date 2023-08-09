using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;

namespace Macrocosm.Content.Rockets.Modules
{
    public class BoosterRight : Booster
    {
		public override Rectangle Hitbox => base.Hitbox with { Width = base.Hitbox.Width + 78, Height = base.Hitbox.Height + 8 };

		protected override Vector2 LandingLegDrawOffset => new(28, 208);
	}
}
