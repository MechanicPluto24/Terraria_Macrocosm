using System;
using Microsoft.Xna.Framework;

namespace Macrocosm.Content.Rockets.Customization
{
	public class PatternColorData
	{
		public bool IsUserChangeable { get; }
		public Color DefaultColor { get; }
		public Color UserColor { get; set; }
		public PatternColorFunction ColorFunction { get; set; }

		public bool HasColorFunction => ColorFunction != null;

		public PatternColorData()
		{
			IsUserChangeable = false;
			DefaultColor = Color.Transparent;
			UserColor = Color.Transparent;
			ColorFunction = null;
		}

		public PatternColorData(Color defaultColor, bool isUserChangeable = true)
		{
			IsUserChangeable = isUserChangeable;
			DefaultColor = defaultColor;
			UserColor = defaultColor;
			ColorFunction = null;
		}

		public PatternColorData(PatternColorFunction colorFunction)
		{
			IsUserChangeable = false;
			DefaultColor = Color.Transparent;
			UserColor = Color.Transparent;
			ColorFunction = colorFunction;
		}

		public PatternColorData Clone()
		{
			PatternColorData clonedData = new(this.DefaultColor, this.IsUserChangeable)
			{
				UserColor = this.UserColor,
				ColorFunction = this.ColorFunction
			};

			return clonedData;
		}
	}
}
