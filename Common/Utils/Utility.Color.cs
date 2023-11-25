using Microsoft.Xna.Framework;
using System;
using System.Globalization;
using Terraria;

namespace Macrocosm.Common.Utils
{
    public static partial class Utility
    {
        #region Alpha manipulation

        public static Color WithOpacity(this Color color, float opacity)
            => new(color.R, color.G, color.B, (byte)(opacity * 255));

        public static Color WithAlpha(this Color color, byte alpha)
            => new(color.R, color.G, color.B, alpha);

        #endregion



        #region Hex format conversion
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
        #endregion

        #region HSL conversion

        public static Color WithHue(this Color color, float hue) => HSLToRGB(ToHSL(color) with { X = hue });
        public static Color WithSaturation(this Color color, float saturation) => HSLToRGB(ToHSL(color) with { Y = saturation });
        public static Color WithLuminance(this Color color, float luminance) => HSLToRGB(ToHSL(color) with { Z = luminance });

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

        #endregion

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

        /// <summary>
        ///	 Returns a monochrome version of the given color.        ///	</summary>

        public static Color ColorMonochrome(Color color)
        {
            int average = color.R + color.G + color.B;
            average /= 3;
            return new Color(average, average, average, color.A);
        }

        /// <summary> Gets the perceived luminance of a color using the NTSC standard as a normalized value </summary>
        public static float GetLuminanceNTSC(this Color rgbColor)
            => 0.299f * rgbColor.R / 255 + 0.587f * rgbColor.G / 255 + 0.114f * rgbColor.B / 255;

        /// <summary> Returns the RGB grayscale of a color using the NTSC standard </summary>
        public static Color ToGrayscaleNTSC(this Color rgbColor)
        {
            Color result = new();
            result.R = result.G = result.B = (byte)(rgbColor.GetLuminanceNTSC() * 255);
            result.A = rgbColor.A;
            return result;
        }
        ///<summary>
        /// Alters the brightness of the color by the amount of the factor. If factor is negative, it darkens it. Else, it brightens it.        ///</summary>
        public static Color Brighten(this Color color, int factor)
        {
            int r = Math.Max(0, Math.Min(255, color.R + factor));
            int g = Math.Max(0, Math.Min(255, color.G + factor));
            int b = Math.Max(0, Math.Min(255, color.B + factor));
            return new Color(r, g, b, color.A);
        }

        ///<summary>
        /// Alters the brightness of the color by the multiplier.        ///</summary>
        public static Color ColorMult(Color color, float mult)
        {
            int r = Math.Max(0, Math.Min(255, (int)(color.R * mult)));
            int g = Math.Max(0, Math.Min(255, (int)(color.G * mult)));
            int b = Math.Max(0, Math.Min(255, (int)(color.B * mult)));
            return new Color(r, g, b, color.A);
        }

        ///<summary>
        /// Clamps the first color to be no lower then the values of the second color.        ///</summary>
        public static Color ColorClamp(Color color1, Color color2)
        {
            int r = color1.R;
            int g = color1.G;
            int b = color1.B;
            int a = color1.A;
            if (r < color2.R) { r = color2.R; }
            if (g < color2.G) { g = color2.G; }
            if (b < color2.B) { b = color2.B; }
            if (a < color2.A) { a = color2.A; }
            return new Color(r, g, b, a);
        }

        ///<summary>
        /// Clamps the first color to be no lower then the brightness of the second color.        ///</summary>
        public static Color ColorBrightnessClamp(Color color1, Color color2)
        {
            float r = color1.R / 255f;
            float g = color1.G / 255f;
            float b = color1.B / 255f;
            float r2 = color2.R / 255f;
            float g2 = color2.G / 255f;
            float b2 = color2.B / 255f;
            float brightness = r2 > g2 ? r2 : g2 > b2 ? g2 : b2;
            r *= brightness; g *= brightness; b *= brightness;
            return new Color(r, g, b, color1.A / 255f);
        }

        ///<summary>
        /// Tints the light color according to the buff color given. (prevents 'darkness' occuring if more than one is applied)        ///</summary>
        public static Color BuffColorize(Color buffColor, Color lightColor)
        {
            Color color2 = ColorBrightnessClamp(buffColor, lightColor);
            return ColorClamp(Colorize(buffColor, lightColor), color2);
        }

        ///<summary>
        /// Tints the light color according to the tint color given.        ///</summary>
        public static Color Colorize(Color tint, Color lightColor)
        {
            float r = lightColor.R / 255f;
            float g = lightColor.G / 255f;
            float b = lightColor.B / 255f;
            float a = lightColor.A / 255f;
            Color newColor = tint;
            float nr = (byte)(newColor.R * r);
            float ng = (byte)(newColor.G * g);
            float nb = (byte)(newColor.B * b);
            float na = (byte)(newColor.A * a);
            newColor.R = (byte)nr;
            newColor.G = (byte)ng;
            newColor.B = (byte)nb;
            newColor.A = (byte)na;
            return newColor;
        }

        ///<summary>
        ///Returns a color of the rainbow. Percent goes from 0 to 1.
        /// </summary>
        public static Color Rainbow(float percent)
        {
            Color r = new(255, 50, 50);
            Color g = new(50, 255, 50);
            Color b = new(90, 90, 255);
            Color y = new(255, 255, 50);
            if (percent <= 0.25f)
            {
                return Color.Lerp(r, b, percent / 0.25f);
            }

            if (percent <= 0.5f)
            {
                return Color.Lerp(b, g, (percent - 0.25f) / 0.25f);
            }

            if (percent <= 0.75f)
            {
                return Color.Lerp(g, y, (percent - 0.5f) / 0.25f);
            }

            return Color.Lerp(y, r, (percent - 0.75f) / 0.25f);
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
    }
}