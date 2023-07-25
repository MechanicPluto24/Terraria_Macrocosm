using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.ModLoader;
using Macrocosm.Content.Rockets.Customization;
using Terraria;
using Macrocosm.Common.Utils;

namespace Macrocosm.Content.Rockets.Modules
{
    public class EngineModule : RocketModule
    {
		public override int DrawPriority => 0;
		public bool RearLandingLegRaised { get; set; } = false;
		public Nameplate Nameplate { get; set; } = new();

		// Add rear booster and its read landing leg to hixbox
		public override Rectangle Hitbox => base.Hitbox with { Height = base.Hitbox.Height + (RearLandingLegRaised ? 18 : 26) };

		public override void Draw(SpriteBatch spriteBatch, Vector2 screenPos, Color ambientColor)
        {
            var state = spriteBatch.SaveState();
            spriteBatch.End();
            spriteBatch.Begin(SamplerState.PointClamp, state);

            // Draw the rear booster behind the engine module (no paintjobs applicable)
			int frameX = RearLandingLegRaised ? 1 : 0;
			Texture2D boosterRear = ModContent.Request<Texture2D>(TexturePath + "_BoosterRear").Value;
			spriteBatch.Draw(boosterRear, Position + new Vector2(0, 18) - screenPos, boosterRear.Frame(2, 1, frameX), ambientColor, 0f, Origin, 1f, SpriteEffects.None, 0f);

			spriteBatch.End();
			spriteBatch.Begin(state);

			// Draw the engine module with the base logic
			base.Draw(spriteBatch, screenPos, ambientColor);

			spriteBatch.End();
			spriteBatch.Begin(SamplerState.PointClamp, state);

			// Draw the nameplate
			Nameplate.Draw(spriteBatch, new Vector2(Center.X, Position.Y) + new Vector2(6, 61) - screenPos, ambientColor);

			spriteBatch.End();
			spriteBatch.Begin(state);
		}
    }
}
