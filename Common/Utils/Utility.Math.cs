using Microsoft.Xna.Framework;
using System;
using System.Drawing.Drawing2D;
using Terraria;

namespace Macrocosm.Common.Utils
{
	public static partial class Utility
	{
		#region Interpolation

		/// <summary>
		/// Calculates the inverse lerp value representing the relative position of the input value between the specified range.
		/// </summary>
		/// <param name="from">The starting value.</param>
		/// <param name="to">The ending value.</param>
		/// <param name="t">The input value to calculate the inverse lerp for.</param>
		/// <param name="clamped">Specifies whether the result should be clamped between 0 and 1.</param>
		/// <returns>Specifies whether the result should be clamped between 0 and 1.</returns>
		public static float InverseLerp(float from, float to, float value, bool clamped = false)
			=> Terraria.Utils.GetLerpValue(from, to, value, clamped);

		/// <summary> Performs a logarithmic interpolation between 0 and 1 based on the given value and logarithmic base. </summary>
		/// /// <param name="t">The input value between 0 and 1.</param>
		/// <param name="logBase">The logarithmic base to use (optional, default = 10).</param>
		/// <returns>The interpolated value between 0 and 1 based on the logarithmic scale.</returns>
		public static float LogarithmicLerp(float t, int logBase = 10)
		{
			t = MathHelper.Clamp(t, 0, 1);
			float lerpValue = MathF.Pow(logBase, t) - 1f;
			float maxLerpValue = MathF.Pow(logBase, 1f) - 1f;
			return lerpValue / maxLerpValue;
		}

		#endregion

		#region Vectors

		public static Vector2 Absolute(this Vector2 vector)
		{
			float x = Math.Abs(vector.X);
			float y = Math.Abs(vector.Y);
			return new Vector2(x, y);
		}

		#endregion

		#region Rotation & angles

		/// <summary>
		/// Converts the polar coordinates of a point to it's cartesian ones
		/// </summary>
		/// <param name="radius"> The radius, i.e. the L2 distance between the point and the origin </param>
		/// <param name="theta"> The angle (in radians) with respect to the positive X axis </param>
		/// <returns> The point's cartesian coordinates relative to the origin  </returns>
		public static Vector2 PolarVector(float radius, float theta)
			=> new Vector2((float)Math.Cos(theta), (float)Math.Sin(theta)) * radius;

		/// <summary>
		/// Gets the position of a point rotating about an origin using a rotation matrix
		/// </summary>
		/// <param name="origin"> The rotation origin </param>
		/// <param name="offset"> The offset of the point from the origin when there is no rotation (<paramref name="theta"> = 0f) </param>
		/// <param name="theta"> The rotation angle (in radians) </param>
		/// <returns> The rotated point coordinates </returns>
		public static Vector2 RotatingPoint(Vector2 origin, Vector2 offset, float theta)
		{
			offset += origin;
			return new(
				origin.X + (offset.X - origin.X) * (float)Math.Cos(theta) - (offset.Y - origin.Y) * (float)Math.Sin(theta),
				origin.Y + (offset.X - origin.X) * (float)Math.Sin(theta) + (offset.Y - origin.Y) * (float)Math.Cos(theta)
			);
		}

		/// <summary> Generates a random rotation value within the range of -pi to pi. </summary>
		/// <returns> A random rotation value in radians. </returns>
		public static float RandomRotation()
			=> Main.rand.NextFloat(-MathHelper.Pi, MathHelper.Pi);

		#endregion

		#region Signals

		/// <summary> Generates a triangle wave based on the given period and phase. </summary>
		/// <param name="period">The duration of one complete cycle of the wave.</param>
		/// <param name="phase">The starting phase offset of the wave (optional).</param>
		/// <returns>The value of the triangle wave ranging from -1 to 1.</returns>
		public static float TriangleWave(float period, float phase = 0)
		{
			float time = ((float)Main.timeForVisualEffects + phase) % period;
			float position = time / period;
			float slope = position < 0.5 ? 4 * position - 1 : -4 * position + 3;
			return 2 * (slope - 0.5f);
		}

		/// <summary> Generates a sine wave based on the given period and phase. </summary>
		/// <param name="period">The duration of one complete cycle of the wave.</param>
		/// <param name="phase">The starting phase offset of the wave (optional).</param>
		/// <returns>The value of the sine wave ranging from -1 to 1.</returns>
		public static float SineWave(float period, float phase = 0)
		{
			float time = ((float)Main.timeForVisualEffects) % period;
			float angle = (2 * MathF.PI * (1 / period) * time) + phase;
			return MathF.Sin(angle);
		}

		/// <summary> Generates a positive-only sine wave based on the given period and phase.</summary>
		/// <param name="period">The duration of one complete cycle of the wave.</param>
		/// <param name="phase">The starting phase offset of the wave (optional).</param>
		/// <returns> The value of the positive-only sine wave ranging from 0 to 1.</returns>
		public static float PositiveSineWave(float period, float phase = 0)
			=> (SineWave(period, phase) + 1f) * 0.5f;

		#endregion

		#region Easing functions

		/// <summary> Applies quadratic easing-in to the input value.  </summary>
		/// <param name="t">The input value to apply easing to.</param>
		/// <returns>The eased value based on quadratic easing-in.</returns>
		public static float QuadraticEaseIn(float t) => t * t;

		/// <summary> Applies quadratic easing-out to the input value. </summary>
		/// <param name="t">The input value to apply easing to.</param>
		/// <returns>The eased value based on quadratic easing-out.</returns>
		public static float QuadraticEaseOut(float t) => t * (2f - t);

		/// <summary>  Applies quadratic easing-in-out to the input value.  </summary>
		/// <param name="t">The input value to apply easing to.</param>
		/// <returns>The eased value based on quadratic easing-in-out.</returns>
		public static float QuadraticEaseInOut(float t)
			=> (t < 0.5) ? (2f * t * t) : (-1f + (4f - 2f * t) * t);

		/// <summary> Applies cubic easing-in to the input value.  </summary>
		/// <param name="t">The input value to apply easing to.</param>
		/// <returns>The eased value based on cubic easing-in.</returns>
		public static float CubicEaseIn(float t) => t * t * t;

		/// <summary> Applies cubic easing-out to the input value. </summary>
		/// <param name="t">The input value to apply easing to.</param>
		/// <returns>The eased value based on cubic easing-out.</returns>
		public static float CubicEaseOut(float t)
			=> (t - 1f) * (t - 1f) * (t - 1f) + 1f;

		/// <summary>  Applies cubic easing-in-out to the input value.  </summary>
		/// <param name="t">The input value to apply easing to.</param>
		/// <returns>The eased value based on cubic easing-in-out.</returns>
		public static float CubicEaseInOut(float t)
			=> (t < 0.5) ? (4f * t * t * t) : 0.5f + CubicEaseOut(t);

		#endregion

		#region Derivatives

		/// <summary>
		/// Applies a logarithmic derivative to <paramref name="value"/>
		/// </summary>
		/// <param name="value">The initial value</param>
		/// <param name="targetValue">The target value</param>
		/// <param name="scaleFactor">The factor (commonly notated as <c>k</c>) which affects how quickly or slowly <paramref name="value"/> reaches <paramref name="targetValue"/></param>
		/// <param name="deltaTime">The change in time</param>
		/// <returns>The updated value</returns>
		public static float ScaleLogarithmic(float value, float targetValue, float scaleFactor, float deltaTime)
		{
			float dt = scaleFactor * (targetValue - value) * deltaTime;
			value += dt;
			return value;
		}

		// Thank you code from TerrariaAmbience - Ryan
		/// <summary> Creates a gradient value based on the given input value and range. </summary>
		/// <param name="value">The input value.</param>
		/// <param name="min">The minimum value of the range.</param>
		/// <param name="max">The maximum value of the range.</param>
		/// <returns>The gradient value between 0 and 1 based on the input value's position within the range.</returns>
		public static double CreateGradientValue(double value, double min, double max)
		{
			double mid = (max + min) / 2;
			double returnValue;

			if (value > mid)
			{
				var thing = 1f - (value - min) / (max - min) * 2;
				returnValue = 1f + thing;
				return returnValue;
			}
			returnValue = (value - min) / (max - min) * 2;
			returnValue = Terraria.Utils.Clamp(returnValue, 0, 1);
			return returnValue;
		}

		/// <summary> Creates a gradient value based on the given input value and range. </summary>
		/// <param name="value">The input value.</param>
		/// <param name="min">The minimum value of the range.</param>
		/// <param name="max">The maximum value of the range.</param>
		/// <returns>The gradient value between 0 and 1 based on the input value's position within the range.</returns>
		public static float CreateGradientValue(float value, float min, float max)
		{
			float mid = (max + min) / 2;
			float returnValue;

			if (value > mid)
			{
				var thing = 1f - (value - min) / (max - min) * 2;
				returnValue = 1f + thing;
				return returnValue;
			}
			returnValue = (value - min) / (max - min) * 2;
			returnValue = Terraria.Utils.Clamp(returnValue, 0, 1);
			return returnValue;
		}

		#endregion
	}
}