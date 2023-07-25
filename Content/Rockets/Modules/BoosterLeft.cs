using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;

namespace Macrocosm.Content.Rockets.Modules
{
    public class BoosterLeft : BoosterRight
    {
		public override Rectangle Hitbox => base.Hitbox with { X = base.Hitbox.X - 78 };

		/// <summary> The draw offset of this booster's landing leg </summary>
		protected override Vector2 LandingLegDrawOffset => base.LandingLegDrawOffset with { X = -78 };
	}
}
