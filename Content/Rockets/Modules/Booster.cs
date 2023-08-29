using Macrocosm.Common.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;

namespace Macrocosm.Content.Rockets.Modules
{
	public abstract class Booster : AnimatedRocketModule
    {
		public override int DrawPriority => 1;

		protected abstract Vector2 LandingLegDrawOffset { get; }

		public override void Draw(SpriteBatch spriteBatch, Vector2 screenPos, Color ambientColor)
		{
			// Draw the booster module with the base logic
			base.Draw(spriteBatch, screenPos, ambientColor);

			var state = spriteBatch.SaveState();
			spriteBatch.End();
			spriteBatch.Begin(SamplerState.PointClamp, state);

			Texture2D tex = ModContent.Request<Texture2D>(TexturePath + "_LandingLeg").Value;
			spriteBatch.Draw(tex, Position + LandingLegDrawOffset - screenPos, tex.Frame(1, NumberOfFrames, frameY: CurrentFrame), ambientColor);

			spriteBatch.End();
			spriteBatch.Begin(state);
		}
	}
}
