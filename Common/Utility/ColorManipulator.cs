using System;
using Microsoft.Xna.Framework;

namespace Macrocosm.Common.Utility
{
    public static class ColorManipulator 
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
	}
}