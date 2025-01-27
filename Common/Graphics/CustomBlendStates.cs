using Microsoft.Xna.Framework.Graphics;

namespace Macrocosm.Common.Graphics
{
    public class CustomBlendStates
    {
        private static BlendState _multiplicative;
        private static BlendState _subtractive;
        private static BlendState _alphaSubtractive;
        private static BlendState _negative;

        public static BlendState Multiplicative => _multiplicative ??= new BlendState
        {
            ColorBlendFunction = BlendFunction.ReverseSubtract,
            ColorDestinationBlend = Blend.One,
            ColorSourceBlend = Blend.SourceAlpha,
            AlphaBlendFunction = BlendFunction.ReverseSubtract,
            AlphaDestinationBlend = Blend.One,
            AlphaSourceBlend = Blend.SourceAlpha
        };

        public static BlendState Subtractive => _subtractive ??= new BlendState
        {
            ColorSourceBlend = Blend.One,
            AlphaSourceBlend = Blend.One,
            ColorDestinationBlend = Blend.One,
            AlphaDestinationBlend = Blend.One,
            ColorBlendFunction = BlendFunction.ReverseSubtract,
            AlphaBlendFunction = BlendFunction.ReverseSubtract,
        };

        public static BlendState AlphaSubtractive => _subtractive ??= new BlendState
        {
            ColorSourceBlend = Blend.SourceAlpha,
            AlphaSourceBlend = Blend.SourceAlpha,
            ColorDestinationBlend = Blend.One,
            AlphaDestinationBlend = Blend.One,
            ColorBlendFunction = BlendFunction.ReverseSubtract,
            AlphaBlendFunction = BlendFunction.ReverseSubtract
        };

        public static BlendState Negative => _negative ??= new BlendState
        {
            ColorBlendFunction = BlendFunction.ReverseSubtract,
            ColorSourceBlend = Blend.One,
            ColorDestinationBlend = Blend.One,
            AlphaBlendFunction = BlendFunction.ReverseSubtract,
            AlphaSourceBlend = Blend.One,
            AlphaDestinationBlend = Blend.One,
        };
    }
}
