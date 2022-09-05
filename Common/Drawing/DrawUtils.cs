using System;
using System.Reflection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Macrocosm.Common.Drawing
{
	public struct SpriteBatchState
	{
		public bool beginCalled;
		public SpriteSortMode sortMode;
		public BlendState blendState;
		public SamplerState samplerState;
		public DepthStencilState depthStencilState;
		public RasterizerState rasterizerState;
		public Effect effect;
		public Matrix matrix;
	}

	public static class DrawUtils
	{
		public static void ManipulateColor(ref Color color, byte amount)
		{
			color.R += amount;
			color.G += amount;
			color.B += amount;
		}
		public static void ManipulateColor(ref Color color, float amount)
		{
			color.R *= (byte)Math.Round(color.R * amount);
			color.G += (byte)Math.Round(color.G * amount);
			color.B += (byte)Math.Round(color.B * amount);
		}

		/// <summary> Saves the SpriteBatch parameters in a struct </summary>
		public static SpriteBatchState SaveState(this SpriteBatch spriteBatch)
		{
			if (spriteBatch.BeginCalled())
				return new SpriteBatchState()
				{
					beginCalled = true,
					sortMode = (SpriteSortMode)spriteBatch.GetType().GetField("sortMode", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(spriteBatch),
					blendState = (BlendState)spriteBatch.GetType().GetField("blendState", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(spriteBatch),
					samplerState = (SamplerState)spriteBatch.GetType().GetField("samplerState", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(spriteBatch),
					depthStencilState = (DepthStencilState)spriteBatch.GetType().GetField("depthStencilState", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(spriteBatch),
					rasterizerState = (RasterizerState)spriteBatch.GetType().GetField("rasterizerState", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(spriteBatch),
					effect = (Effect)spriteBatch.GetType().GetField("customEffect", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(spriteBatch),
					matrix = (Matrix)spriteBatch.GetType().GetField("transformMatrix", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(spriteBatch)
				};
			else
				return new SpriteBatchState()
				{
					// some defaults 
					beginCalled = false,
					sortMode = SpriteSortMode.Deferred, 
					blendState =  BlendState.AlphaBlend,
					samplerState = Terraria.Main.DefaultSamplerState,
					depthStencilState = DepthStencilState.Default,
					rasterizerState = RasterizerState.CullCounterClockwise,
					effect = null,
					matrix = Terraria.Main.GameViewMatrix.TransformationMatrix
				};
		}

		public static void Begin(this SpriteBatch spriteBatch, SpriteSortMode sortMode, BlendState blendState, SpriteBatchState state)
			=>	spriteBatch.Begin(sortMode, blendState, state.samplerState, state.depthStencilState, state.rasterizerState, state.effect, state.matrix);

		public static void Begin(this SpriteBatch spriteBatch, BlendState blendState, SpriteBatchState state)
			=> spriteBatch.Begin(state.sortMode, blendState, state.samplerState, state.depthStencilState, state.rasterizerState, state.effect, state.matrix);
 

		/// <summary> Restores the SpriteBatch parameters stored in a SpriteBatchState </summary>
		public static void Restore(this SpriteBatch spriteBatch, SpriteBatchState state)
		{
			spriteBatch.EndIfBeginCalled();

			if(state.beginCalled)
				spriteBatch.Begin(state.sortMode, state.blendState, state.samplerState, state.depthStencilState, state.rasterizerState, state.effect, state.matrix);
		}

		public static void EndIfBeginCalled(this SpriteBatch spriteBatch)
		{
			if(spriteBatch.BeginCalled())
				spriteBatch.End();
		}

		public static bool BeginCalled(this SpriteBatch spriteBatch) => (bool)spriteBatch.GetType().GetField("beginCalled", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(spriteBatch);


		/// <summary> Returns the RGB grayscale of a color using the NTSC formula </summary>
		public static Color ToGrayscale(this Color rgbColor)
		{
			Color result = new();
			result.R = result.G = result.B = (byte)((0.299f * rgbColor.R / 255 + 0.587f * rgbColor.G / 255 + 0.114f * rgbColor.B / 255) * 255);
			result.A = rgbColor.A;
			return result;
		}

		public static Color NewAlpha(this Color color, float alpha)
			=> new Color(color.R, color.G, color.B, (byte)(alpha * 255));

		public static Color NewAlpha(this Color color, byte alpha)
			=> new Color(color.R, color.G, color.B, alpha);

		/// <summary> Returns a premultiplied copy of a texture </summary>
		public static Texture2D ToPremultiplied(this Texture2D texture)
		{
			Terraria.Main.QueueMainThreadAction(() =>
			{
				Color[] buffer = new Color[texture.Width * texture.Height];
				texture.GetData(buffer);
				for (int i = 0; i < buffer.Length; i++)
				{
					buffer[i] = Color.FromNonPremultiplied(
						buffer[i].R, buffer[i].G, buffer[i].B, buffer[i].A);
				}
				texture.SetData(buffer);
			});

			return texture;
		}
	}
}