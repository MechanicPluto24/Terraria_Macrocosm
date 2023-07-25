using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;

namespace Macrocosm.Content.Rockets.Modules
{
    public class BoosterRight : RocketModule
    {
		public override int DrawPriority => 1;

		public override Rectangle Hitbox => base.Hitbox with { Width = base.Hitbox.Width + 78, Height = base.Hitbox.Height + 8 };

		protected virtual Vector2 LandingLegDrawOffset => new(28, 208);

		/// <summary> The current landing leg animation frame </summary>
		public int CurrentFrame { get; set; } = MaxFrames - 1;
		public const int MaxFrames = 10;

		public override void Draw(SpriteBatch spriteBatch, Vector2 screenPos, Color ambientColor)
		{
			// Draw the booster module with the base logic
			base.Draw(spriteBatch, screenPos, ambientColor);

			Texture2D tex = ModContent.Request<Texture2D>(TexturePath + "_LandingLegs").Value;
			spriteBatch.Draw(tex, Position + LandingLegDrawOffset - screenPos, tex.Frame(1, MaxFrames, frameY: CurrentFrame), ambientColor);
		}
	}
}
