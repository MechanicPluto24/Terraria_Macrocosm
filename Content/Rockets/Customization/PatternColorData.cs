using System;
using Microsoft.Xna.Framework;

namespace Macrocosm.Content.Rockets.Customization
{
	public class PatternColorData
	{
		public bool IsUserModifiable { get; }
		public Color DefaultColor { get; }
		public Color UserColor { get; set; }
		public PatternColorFunction ColorFunction { get; set; }

		public bool HasColorFunction => ColorFunction != null;

		public PatternColorData()
		{
			IsUserModifiable = true;
			DefaultColor = Color.Transparent;
			UserColor = Color.Transparent;
			ColorFunction = null;
		}

		public PatternColorData(Color defaultColor, bool isUserChangeable = true)
		{
			IsUserModifiable = isUserChangeable;
			DefaultColor = defaultColor;
			UserColor = defaultColor;
			ColorFunction = null;
		}

		public PatternColorData(Func<Color[], Color> colorFunction)
		{
			IsUserModifiable = false;
			DefaultColor = Color.Transparent;
			UserColor = Color.Transparent;
			ColorFunction = new(colorFunction);
		}

		public PatternColorData(PatternColorFunction colorFunction)
		{
			IsUserModifiable = false;
			DefaultColor = Color.Transparent;
			UserColor = Color.Transparent;
			ColorFunction = colorFunction;
		}

		public PatternColorData Clone()
		{
			PatternColorData clonedData = new(this.DefaultColor, this.IsUserModifiable)
			{
				UserColor = this.UserColor,
				ColorFunction = this.ColorFunction
			};

			return clonedData;
		}
	}
}
