using System;
using Microsoft.Xna.Framework;

namespace Macrocosm.Content.Rockets.Customization
{
	public readonly struct PatternColorData
	{
		public readonly bool IsUserChangeable { get; }
		public readonly Func<Color[], Color> ColorFunction { get; }
		public readonly Color DefaultColor { get; }

		public PatternColorData(Color defaultColor, bool isUserChangeable = false)
		{
			IsUserChangeable = isUserChangeable;
			ColorFunction = null;
			DefaultColor = defaultColor;
		}

		public PatternColorData(Func<Color[], Color> colorFunction = null)
		{
			IsUserChangeable = false;
			ColorFunction = colorFunction;
			DefaultColor = Color.Transparent;
		}
	}
}
