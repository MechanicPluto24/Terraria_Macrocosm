using Microsoft.Xna.Framework;

namespace Macrocosm.Content.Rockets.Customization
{
    public readonly struct PatternColorData
    {
        public readonly bool IsUserModifiable { get; }
        public readonly Color Color { get; }
        public readonly ColorFunction ColorFunction { get; }

        public bool HasColorFunction => ColorFunction != null;

        public PatternColorData()
        {
            IsUserModifiable = false;
            Color = Color.Transparent;
            ColorFunction = null;
        }

        public PatternColorData(Color defaultColor, bool isUserModifiable = true)
        {
            IsUserModifiable = isUserModifiable;
            Color = defaultColor;
            ColorFunction = null;
        }

        public PatternColorData(ColorFunction colorFunction)
        {
            IsUserModifiable = false;
            Color = Color.Transparent;
            ColorFunction = colorFunction;
        }


        public PatternColorData WithUserColor(Color newUserColor)
        {
            if (!IsUserModifiable || HasColorFunction)
                return this;

            return new PatternColorData(newUserColor);
        }

        public PatternColorData WithColorFunction(ColorFunction function)
        {
            return new PatternColorData(function);
        }
    }
}
