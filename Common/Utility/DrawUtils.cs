using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Macrocosm.Common.Utility
{
	public static class DrawUtils
	{
		/// <summary>
		/// Returns the RGB grayscale of a color using the NTSC formula
		/// </summary>
		/// <param name="rgbColor"></param>
		/// <returns></returns>
		public static Color ToGrayscale(this Color rgbColor)
		{
			Color result = new();
			result.R = result.G = result.B = (byte)((0.299f * (float)rgbColor.R / 255 + 0.587f * (float)rgbColor.G / 255 + 0.114f * (float)rgbColor.B / 255) * 255);
			result.A = rgbColor.A;
			return result;
		}

		public static Texture2D PremultiplyTexture(this Texture2D texture)
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
	}
}