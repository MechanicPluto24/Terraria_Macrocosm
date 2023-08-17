using System;
using Microsoft.Xna.Framework;

namespace Macrocosm.Content.Rockets.Customization
{
	public readonly struct PatternColorData
	{
		public readonly bool IsUserModifiable { get; }
		public readonly Color DefaultColor { get; }
		public readonly Color UserColor { get; }
		public readonly PatternColorFunction ColorFunction { get; }

		public bool HasColorFunction => ColorFunction != null;

		public PatternColorData()
		{
			IsUserModifiable = false;
			DefaultColor = Color.Transparent;
			UserColor = Color.Transparent;
			ColorFunction = null;
		}

		public PatternColorData(Color defaultColor, bool isUserModifiable = true)
		{
			IsUserModifiable = isUserModifiable;
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

		private PatternColorData(Color defaultColor, Color userColor)
		{
			DefaultColor = defaultColor;
			UserColor = userColor;
			IsUserModifiable = true;
		}

		public PatternColorData WithUserColor(Color newUserColor)
		{
			if (!IsUserModifiable || HasColorFunction)
 				return this;
 
			return new PatternColorData(DefaultColor, newUserColor);
		}

		public PatternColorData WithColorFunction(PatternColorFunction function)
		{
			if (!IsUserModifiable)
				return this;

			return new PatternColorData(function);
		}
	}
}
