using System;
using System.Reflection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;

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
			=> spriteBatch.Begin(sortMode, blendState, state.samplerState, state.depthStencilState, state.rasterizerState, state.effect, state.matrix);

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

		public static bool BeginCalled(this SpriteBatch spriteBatch)
			=> (bool)spriteBatch.GetType().GetField("beginCalled", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(spriteBatch);


		/// <summary>
		/// Draw a MagicPixel trail behind a projectile, with length based on the trail cache length  
		/// </summary>
		/// <param name="rotatableOffsetFromCenter"> offset from projectile center when rotation is 0 </param>
		/// <param name="startWidth"> The trail width near the projectile </param>
		/// <param name="endWidth"> The trail width at its end </param>
		/// <param name="startColor"> The trail color near the projectile  </param>
		/// <param name="endColor"> The trail color at its end </param>
		public static void DrawTrail(this Projectile proj, Vector2 rotatableOffsetFromCenter, float startWidth, float endWidth, Color startColor, Color? endColor = null)
			=> DrawTrail(proj.Size / 2f, proj.oldPos, proj.oldRot, rotatableOffsetFromCenter, startWidth, endWidth, startColor, endColor);

		/// <summary>
		/// Draw a MagicPixel trail behind a NPC, with length based on the trail cache length  
		/// </summary>
		/// <param name="rotatableOffsetFromCenter"> offset from NPC center when rotation is 0 </param>
		/// <param name="startWidth"> The trail width near the NPC </param>
		/// <param name="endWidth"> The trail width at its end </param>
		/// <param name="startColor"> The trail color near the NPC </param>
		/// <param name="endColor"> The trail color at its end </param>
		public static void DrawTrail(this NPC npc, Vector2 rotatableOffsetFromCenter, float startWidth, float endWidth, Color startColor, Color? endColor = null)
			=> DrawTrail(npc.Size / 2f, npc.oldPos, npc.oldRot, rotatableOffsetFromCenter, startWidth, endWidth, startColor, endColor);


		/// <summary> Adapted from Terraria.Main </summary>
		private static void DrawTrail(Vector2 origin, Vector2[] oldPos, float[] oldRot, Vector2 rotatableOffsetFromCenter, float startWidth, float endWidth, Color startColor, Color? endColor = null)
		{
			Rectangle rect = new(0, 0, 1, 1);
 			for (int k = oldPos.Length - 1; k > 0; k--)
			{
				if (!(oldPos[k] == Vector2.Zero))
				{
					Vector2 v1 = oldPos[k] + origin + rotatableOffsetFromCenter.RotatedBy(oldRot[k]);
					Vector2 v2 = oldPos[k - 1] + origin + rotatableOffsetFromCenter.RotatedBy(oldRot[k - 1]) - v1;
 					float brightness = Utils.Remap(k, 0f, oldPos.Length, 1f, 0f);
					Color color = (endColor is null) ? startColor * brightness : Color.Lerp((Color)endColor, startColor, brightness);

					SpriteBatch spriteBatch = Main.spriteBatch;
					SpriteBatchState state = spriteBatch.SaveState();
					spriteBatch.EndIfBeginCalled();
					spriteBatch.Begin(SpriteSortMode.Deferred, blendState: BlendState.NonPremultiplied, state);
					Main.spriteBatch.Draw(TextureAssets.MagicPixel.Value, v1 - Main.screenPosition, rect, color, v2.ToRotation() + (float)Math.PI / 2f, new Vector2((float)rect.Width / 2f, rect.Height), new Vector2(MathHelper.Lerp(startWidth, endWidth, (float)k/oldPos.Length), v2.Length()), SpriteEffects.None, 1);
					spriteBatch.Restore(state);
				}
			}
		}

		/// <summary> Gets the perceived luminance of a color using the NTSC standard as a normalized value </summary>
		public static float GetLuminance(this Color rgbColor)
			=> 0.299f * rgbColor.R / 255 + 0.587f * rgbColor.G / 255 + 0.114f * rgbColor.B / 255;


		/// <summary> Gets the perceived luminance of a color using the NTSC standard as a byte </summary>
		public static byte GetLuminance_Byte(this Color rgbColor) => (byte)(GetLuminance(rgbColor) * 255);


		/// <summary> Returns the RGB grayscale of a color using the NTSC standard </summary>
		public static Color ToGrayscale(this Color rgbColor)
		{
			Color result = new();
			result.R = result.G = result.B = GetLuminance_Byte(rgbColor);
			result.A = rgbColor.A;
			return result;
		}

		public static Color NewAlpha(this Color color, float alpha)
			=> new(color.R, color.G, color.B, (byte)(alpha * 255));


		public static Color NewAlpha(this Color color, byte alpha)
			=> new(color.R, color.G, color.B, alpha);


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