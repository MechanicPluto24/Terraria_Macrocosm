using Macrocosm.Common.DataStructures;
using Macrocosm.Common.Utils;
using Macrocosm.Content.Rockets.Customization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using Terraria;
using Terraria.Graphics.Shaders;
using Terraria.Graphics;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.GameContent;

namespace Macrocosm.Content.Rockets.Modules
{
	public class EngineModule : AnimatedRocketModule
    {
		public override int DrawPriority => 0;
		public bool RearLandingLegRaised { get; set; } = false;
		public Nameplate Nameplate { get; set; } = new();

		public override int Width => 120;
		public override int Height => 302 + (RearLandingLegRaised ? 18 : 26);

		public EngineModule(Rocket rocket) : base(rocket)
		{
		}

		private SpriteBatchState state;
		public override void Draw(SpriteBatch spriteBatch, Vector2 screenPos, Color ambientColor)
        {
            state.SaveState(spriteBatch);
            spriteBatch.End();
            spriteBatch.Begin(SamplerState.PointClamp, state);

			// Draw the rear landing behind the rear booster 
			Texture2D rearLandingLeg = ModContent.Request<Texture2D>(TexturePath + "_LandingLeg", AssetRequestMode.ImmediateLoad).Value;
			spriteBatch.Draw(rearLandingLeg, Position + new Vector2(Texture.Width / 2f - rearLandingLeg.Width/2f, 314f) - screenPos, rearLandingLeg.Frame(1, NumberOfFrames, frameY: CurrentFrame), ambientColor);

			// Draw the rear booster behind the engine module 
			Texture2D boosterRear = ModContent.Request<Texture2D>(TexturePath + "_BoosterRear", AssetRequestMode.ImmediateLoad).Value;
			spriteBatch.Draw(boosterRear, Position + new Vector2(Texture.Width/2f - boosterRear.Width/2f, 293.5f) - screenPos, null, ambientColor, 0f, Origin, 1f, SpriteEffects.None, 0f);

			if (rocket.StaticFire || rocket.InFlight || rocket.ForcedFlightAppearance)
			{
				spriteBatch.End();
				spriteBatch.Begin(BlendState.Additive, state);
				//DrawSmokeTrail(spriteBatch, -1.2f);

				if(rocket.StaticFire)
					DrawTrail(spriteBatch, 0.5f * rocket.StaticFireProgress);

				if (rocket.InFlight)
					DrawTrail(spriteBatch, 1f);
			}

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

		private void DrawTrail(SpriteBatch spriteBatch, float intensity = 1f)
		{
			VertexStrip strip = new();
			int stripDataCount = (int)(58 * intensity);
			Vector2[] positions = new Vector2[stripDataCount];
			float[] rotations = new float[stripDataCount];
			Array.Fill(positions, new Vector2(Center.X, Position.Y + Height - 28) - Main.screenPosition);
			Array.Fill(rotations, MathHelper.Pi + MathHelper.PiOver2);

			for (int i = 0; i < stripDataCount; i++)
				positions[i] += new Vector2(0f, 4f * i * intensity);

			var shader = new MiscShaderData(Main.VertexPixelShaderRef, "MagicMissile")
							.UseProjectionMatrix(doUse: true)
							.UseSaturation(-2.2f)
							.UseImage0(ModContent.Request<Texture2D>(Macrocosm.TextureAssetsPath + "FadeOutMask"))
							.UseImage1(ModContent.Request<Texture2D>(Macrocosm.TextureAssetsPath + "RocketExhaustTrail2"))
							.UseImage2(ModContent.Request<Texture2D>(Macrocosm.TextureAssetsPath + "RocketExhaustTrail2"));

			shader.Apply();

			strip.PrepareStrip(
				positions, 
				rotations,
				(float progress) => Color.Lerp(new Color(255, 217, 120, (byte)(127 * (1 - intensity))), new Color(255, 0, 0, 0), Utility.QuadraticEaseIn(progress)),
				(float progress) => MathHelper.Lerp(40, 75, progress) * intensity
			);

			strip.DrawTrail();

			//var glow = ModContent.Request<Texture2D>(Macrocosm.TextureAssetsPath + "Circle6").Value;
			//spriteBatch.Draw(glow, new Vector2(Center.X, Position.Y + Height - 28) - Main.screenPosition, null, new Color(255, 69, 0).WithOpacity(0.35f), MathHelper.PiOver2, glow.Size() / 2f, 0.28f, SpriteEffects.None, 0f);

			//for (int i = 0; i < stripDataCount; i++)
			//	Lighting.AddLight(positions[i] + Main.screenPosition, new Color(255, 177, 65).ToVector3() * 2f);
		}

		private void DrawSmokeTrail(SpriteBatch spriteBatch, float offset = -28)
		{
			VertexStrip strip = new();
			int stripDataCount = 100; /*+ (int)(20 * Utility.CubicEaseInOut(Math.Abs(rocket.FlightProgress)));*/
			Vector2[] positions = new Vector2[stripDataCount];
			float[] rotations = new float[stripDataCount];

			Vector2 basePosition = new Vector2(Center.X, Position.Y + Height - 28) - Main.screenPosition;
			for (int i = 0; i < stripDataCount; i++)
			{
				positions[i] = basePosition + new Vector2(0f, 4f * i);
				rotations[i] = MathHelper.Pi + MathHelper.PiOver2;
			}

			var shader = new MiscShaderData(Main.VertexPixelShaderRef, "MagicMissile")
							.UseOpacity(0.9f + 0.1f * Utility.SineWave(300))
							.UseProjectionMatrix(doUse: true)
							.UseSaturation(offset)
							.UseImage0(ModContent.Request<Texture2D>(Macrocosm.TextureAssetsPath + "FadeOutMask"))
							.UseImage1(ModContent.Request<Texture2D>(Macrocosm.TextureAssetsPath + "SmokeTrail1"))
							.UseImage2(ModContent.Request<Texture2D>(Macrocosm.TextureAssetsPath + "SmokeTrail1"));

			shader.Apply();
			strip.PrepareStrip(
				positions,
				rotations,
				//(float progress) => (Color.White * (0.8f - 1.2f * Utility.QuintEaseInOut(progress))).WithOpacity(1f - Utility.QuartEaseOut(progress)),
				(float progress) => Color.White,
					 
				(float progress) => MathHelper.Lerp(35, 180, progress)
			);

			strip.DrawTrail();

			//var glow = ModContent.Request<Texture2D>(Macrocosm.TextureAssetsPath + "Circle6").Value;
			//spriteBatch.Draw(glow, new Vector2(Center.X, Position.Y + Height - 28) - Main.screenPosition, null, new Color(255, 69, 0).WithOpacity(0.35f), MathHelper.PiOver2, glow.Size() / 2f, 0.28f, SpriteEffects.None, 0f);

			//for (int i = 0; i < stripDataCount; i++)
			//	Lighting.AddLight(positions[i] + Main.screenPosition, new Color(255, 177, 65).ToVector3() * 2f);
		}

		protected override TagCompound SerializeModuleSpecificData()
		{
			return new()
			{
				[nameof(Nameplate)] = Nameplate,
			};
		}

		protected override void DeserializeModuleSpecificData(TagCompound tag, Rocket ownerRocket)
		{
			if (tag.ContainsKey(nameof(Nameplate)))
				Nameplate = tag.Get<Nameplate>(nameof(Nameplate));
		}
	}
}
