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

			if (rocket.InFlight || rocket.ForcedFlightAppearance)
			{
				spriteBatch.End();
				spriteBatch.Begin(BlendState.Additive, state);
				DrawTrail2(spriteBatch);
				DrawTrail(spriteBatch);
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

		private void DrawTrail(SpriteBatch spriteBatch)
		{
			VertexStrip strip = new();
			int stripDataCount = 58; /*+ (int)(20 * Utility.CubicEaseInOut(Math.Abs(rocket.FlightProgress)));*/
			Vector2[] positions = new Vector2[stripDataCount];
			float[] rotations = new float[stripDataCount];
			Array.Fill(positions, new Vector2(Center.X, Position.Y + Height - 28) - Main.screenPosition);
			Array.Fill(rotations, MathHelper.Pi + MathHelper.PiOver2);

			for (int i = 0; i < stripDataCount; i++)
				positions[i] += new Vector2(0f, 4f * i);

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
				(float progress) => Color.Lerp(new Color(255, 217, 120, 0), new Color(255, 0, 0, 0), Utility.QuadraticEaseIn(progress)),
				(float progress) => MathHelper.Lerp(40, 75, progress)
			);

			strip.DrawTrail();

			//var glow = ModContent.Request<Texture2D>(Macrocosm.TextureAssetsPath + "SimpleGlow").Value;
			//spriteBatch.Draw(glow, new Vector2(Center.X, Position.Y + Height - 28) - Main.screenPosition, null, new Color(255, 69, 0).WithOpacity(0.35f), MathHelper.PiOver2, glow.Size() / 2f, 0.28f, SpriteEffects.None, 0f);

			//for (int i = 0; i < stripDataCount; i++)
			//	Lighting.AddLight(positions[i] + Main.screenPosition, new Color(255, 177, 65).ToVector3() * 2f);
		}

		private void DrawTrail2(SpriteBatch spriteBatch)
		{
			VertexStrip strip = new();
			int stripDataCount = 180; /*+ (int)(20 * Utility.CubicEaseInOut(Math.Abs(rocket.FlightProgress)));*/
			Vector2[] positions = new Vector2[stripDataCount];
			float[] rotations = new float[stripDataCount];
			Array.Fill(positions, new Vector2(Center.X, Position.Y + Height - 28) - Main.screenPosition);
			Array.Fill(rotations, MathHelper.Pi + MathHelper.PiOver2);

			for (int i = 0; i < stripDataCount; i++)
				positions[i] += new Vector2(0f, 4f * i);

			var shader = new MiscShaderData(Main.VertexPixelShaderRef, "MagicMissile")
							.UseProjectionMatrix(doUse: true)
							.UseSaturation(-1f)
							.UseImage0(ModContent.Request<Texture2D>(Macrocosm.TextureAssetsPath + "FadeOutMask"))
							.UseImage1(ModContent.Request<Texture2D>(Macrocosm.TextureAssetsPath + "FadeOutMask"))
							.UseImage2(ModContent.Request<Texture2D>(Macrocosm.TextureAssetsPath + "RocketExhaustTrail1"));

			shader.Apply();
			strip.PrepareStrip(
				positions,
				rotations,
				(float progress) => progress < 0.4f ? 
								Color.Lerp(
									Color.Gray.WithOpacity(0.5f), 
									Color.DarkGray.WithOpacity(0.4f), 
									progress
								) : 
								Color.Lerp(
									Color.DarkGray.WithOpacity(0.3f),
									Color.Transparent, 
									Utility.ExpoEaseOut(Utils.Remap(progress - 0.385f, 0f, 0.4f, 0f, 1f))
								),
				(float progress) => MathHelper.Lerp(40, 125, progress)
			);

			strip.DrawTrail();

			//var glow = ModContent.Request<Texture2D>(Macrocosm.TextureAssetsPath + "SimpleGlow").Value;
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
