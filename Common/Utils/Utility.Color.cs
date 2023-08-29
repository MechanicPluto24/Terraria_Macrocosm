using Microsoft.Xna.Framework;
using System;
using System.Globalization;
using Terraria;

namespace Macrocosm.Common.Utils
{
	public static partial class Utility
    {
		public static string GetHexText(this Color color) => "#" + color.Hex3().ToUpper();

		public static bool TryGetColorFromHex(string hexString, out Color color)
		{
			if (hexString.StartsWith("#"))
				hexString = hexString[1..];

			if (hexString.Length <= 6 && uint.TryParse(hexString, NumberStyles.HexNumber, CultureInfo.CurrentCulture, out var result))
			{
				uint b = result & 0xFFu;
				uint g = (result >> 8) & 0xFFu;
				uint r = (result >> 16) & 0xFFu;
				color = new Color((int)r, (int)g, (int)b);
				return true;
			}

			color = Color.White;
			return false;
		}

		public static bool TryGetColorFromHex(string hexString, out Vector3 hsl, float luminanceFactor = 1f)
		{
			if (TryGetColorFromHex(hexString, out Color rgb))
			{
				hsl = rgb.ToScaledHSL(luminanceFactor);
				return true;
			}
			else
			{
				hsl = Vector3.Zero;
				return false;
			}
		}


		public static Vector3 RGBToHSL(Color color) => color.ToHSL();
		public static Vector3 ToHSL(this Color color) => Main.rgbToHsl(color);

		public static Color HSLToRGB(Vector3 hsl) => Main.hslToRgb(hsl);
		public static Color HSLToRGB(float hue, float saturation, float luminance) => Main.hslToRgb(hue, saturation, luminance);

		public static Vector3 ToScaledHSL(this Color color, float luminanceFactor)
		{
			float invFactor = 1f - luminanceFactor;
			Vector3 value = color.ToHSL();
			value.Z = (value.Z - invFactor) / luminanceFactor;
			return Vector3.Clamp(value, Vector3.Zero, Vector3.One);
		}

		public static Color ScaledHSLToRGB(Vector3 hsl, float luminanceFactor) => ScaledHSLToRGB(hsl.X, hsl.Y, hsl.Z, luminanceFactor);
		public static Color ScaledHSLToRGB(float hue, float saturation, float luminance, float luminanceFactor)
		{
			float invFactor = 1f - luminanceFactor;
			return HSLToRGB(hue, saturation, luminance * luminanceFactor + invFactor);
		}

		public static Vector3 ScaleHSL(float hue, float saturation, float luminance, float luminanceFactor)
		{
			float invFactor = 1f - luminanceFactor;
			return new Vector3(hue, saturation, luminance * luminanceFactor + invFactor);
		}

		public static Vector3 ScaleHSL(Vector3 hsl, float luminanceFactor) => ScaleHSL(hsl.X, hsl.Y, hsl.Z, luminanceFactor);

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

        public static Vector3[] ToVector3Array(this Color[] colors)
        {
            Vector3[] vectors = new Vector3[colors.Length];

            for (int i = 0; i < colors.Length; i++) 
 				vectors[i] = colors[i].ToVector3();    

             return vectors;
        }

		public static Vector4[] ToVector4Array(this Color[] colors)
		{
			Vector4[] vectors = new Vector4[colors.Length];

			for (int i = 0; i < colors.Length; i++)
 				vectors[i] = colors[i].ToVector4();

			return vectors;
		}


        /// <summary> Gets the perceived luminance of a color using the NTSC standard as a normalized value </summary>
        public static float GetLuminance(this Color rgbColor)
            => 0.299f * rgbColor.R / 255 + 0.587f * rgbColor.G / 255 + 0.114f * rgbColor.B / 255;


        /// <summary> Gets the perceived luminance of a color using the NTSC standard as a byte </summary>
        public static byte GetLuminance_Byte(this Color rgbColor) => (byte)(rgbColor.GetLuminance() * 255);


        /// <summary> Returns the RGB grayscale of a color using the NTSC standard </summary>
        public static Color ToGrayscale(this Color rgbColor)
        {
            Color result = new();
            result.R = result.G = result.B = rgbColor.GetLuminance_Byte();
            result.A = rgbColor.A;
            return result;
        }

        public static Color NewAlpha(this Color color, float alpha)
            => new(color.R, color.G, color.B, (byte)(alpha * 255));

        public static Color NewAlpha(this Color color, byte alpha)
            => new(color.R, color.G, color.B, alpha);
        
	}
}