using System;
using Microsoft.Xna.Framework;

namespace Macrocosm.Common.Utility {
    public static class ColorManipulator  {
		public static void ManipulateColor(ref Color color, byte amount) {
			color.R += amount;
			color.G += amount;
			color.B += amount;
		}
		public static void ManipulateColor(ref Color color, float amount) {
			color.R *= (byte)Math.Round(color.R * amount);
			color.G += (byte)Math.Round(color.G * amount);
			color.B += (byte)Math.Round(color.B * amount);
		}

		/// <summary>
		/// Returns the RGB grayscale of a color using the NTSC formula
		/// </summary>
		/// <param name="rgbColor"></param>
		/// <returns></returns>
		public static Color ToGrayscale(Color rgbColor)
		{
			Color result = new Color();
			result.R = (byte)((0.299f * (float)rgbColor.R / 255 + 0.587f * (float)rgbColor.G / 255 + 0.114f * (float)rgbColor.B / 255) * 255);
			result.G = result.R;
			result.B = result.R;
			result.A = rgbColor.A;
			return result;
		}

	}
}