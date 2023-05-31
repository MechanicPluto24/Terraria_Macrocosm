using Microsoft.Xna.Framework;
using System;
using Terraria;

namespace Macrocosm.Common.Utils
{
	public static partial class Utility
	{
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
		
		public static float RandomRotation() 
			=> Main.rand.NextFloat(-MathHelper.Pi, MathHelper.Pi);

		public static float TriangleWave(float period, float phase)
		{
			float time = ((float)Main.timeForVisualEffects + phase) % period;
			float position = time / period;
			float slope = position < 0.5 ? 4 * position - 1 : -4 * position + 3;
			return 2 * (slope - 0.5f);
		}

		public static float SineWave(float period, float phase)
		{
			float time = ((float)Main.timeForVisualEffects) % period;
			float angle = (2 * MathF.PI * (1/period) * time) + phase;
			return MathF.Sin(angle);
		}

		public static float PositiveSineWave(float period, float phase)
			=> (SineWave(period, phase) + 1f) * 0.5f;

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

		// Thank you code from TerrariaAmbience
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
	}
}