using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.ModLoader;
using Macrocosm.Content.Rockets.Customization;
using Terraria;
using Macrocosm.Common.Utils;
using Terraria.ModLoader.IO;
using ReLogic.Content;

namespace Macrocosm.Content.Rockets.Modules
{
    public class EngineModule : AnimatedRocketModule
    {
		public override int DrawPriority => 0;
		public bool RearLandingLegRaised { get; set; } = false;
		public Nameplate Nameplate { get; set; } = new();

		public override int Width => 120;
		public override int Height => 302 + (RearLandingLegRaised ? 18 : 26);

		public override void Draw(SpriteBatch spriteBatch, Vector2 screenPos, Color ambientColor)
        {
            var state = spriteBatch.SaveState();
            spriteBatch.End();
            spriteBatch.Begin(SamplerState.PointClamp, state);

			// Draw the rear landing behind the rear booster 
			Texture2D rearLandingLeg = ModContent.Request<Texture2D>(TexturePath + "_LandingLeg", AssetRequestMode.ImmediateLoad).Value;
			spriteBatch.Draw(rearLandingLeg, Position + new Vector2(Texture.Width / 2f - rearLandingLeg.Width/2f, 314f) - screenPos, rearLandingLeg.Frame(1, NumberOfFrames, frameY: CurrentFrame), ambientColor);

			// Draw the rear booster behind the engine module 
			Texture2D boosterRear = ModContent.Request<Texture2D>(TexturePath + "_BoosterRear", AssetRequestMode.ImmediateLoad).Value;
			spriteBatch.Draw(boosterRear, Position + new Vector2(Texture.Width/2f - boosterRear.Width/2f, 293.5f) - screenPos, null, ambientColor, 0f, Origin, 1f, SpriteEffects.None, 0f);


			spriteBatch.End();
			spriteBatch.Begin(state);

			// Draw the engine module with the base logic
			base.Draw(spriteBatch, screenPos, ambientColor);

			spriteBatch.End();
			spriteBatch.Begin(SamplerState.PointClamp, state);

			// Draw the nameplate
			Nameplate.Draw(spriteBatch, new Vector2(Center.X, Position.Y) - screenPos, ambientColor);

			spriteBatch.End();
			spriteBatch.Begin(state);
		}

		protected override TagCompound SerializeModuleData()
		{
			return new()
			{
				[nameof(Nameplate)] = Nameplate,
			};
		}

		protected override void DeserializeModuleData(TagCompound tag)
		{
			if (tag.ContainsKey(nameof(Nameplate)))
				Nameplate = tag.Get<Nameplate>(nameof(Nameplate));
		}
	}
}
