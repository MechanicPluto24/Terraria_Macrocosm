using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.ModLoader;
using Macrocosm.Content.Rockets.Customization;
using Terraria;
using Macrocosm.Common.Utils;
using Macrocosm.Common.Drawing;

namespace Macrocosm.Content.Rockets.Modules
{
    public class EngineModule : RocketModule
    {
		public Nameplate Nameplate = new();

		public override void Draw(SpriteBatch spriteBatch, Vector2 screenPos, Color ambientColor)
        {
            var state = spriteBatch.SaveState();

            // Draw the rear booster behind the engine module (no paintjobs applicable)
            Texture2D boosterRear = ModContent.Request<Texture2D>(TexturePath + "_BoosterRear").Value;
            spriteBatch.Draw(boosterRear, Position + new Vector2(0, 18) - screenPos, null, ambientColor, 0f, Origin, 1f, SpriteEffects.None, 0f);

            // Draw the engine module with the base logic
            base.Draw(spriteBatch, screenPos, ambientColor);

			// Draw the nameplate
			Nameplate.Draw(spriteBatch, Position + new Vector2(6, 61) - screenPos, ambientColor);
		}
    }
}
