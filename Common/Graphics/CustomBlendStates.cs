using Microsoft.Xna.Framework.Graphics;

namespace Macrocosm.Common.Graphics
{
	public class CustomBlendStates
	{
		public static readonly BlendState Multiplicative = new()
		{
			ColorBlendFunction = BlendFunction.Add,
			ColorSourceBlend = Blend.DestinationColor,
			ColorDestinationBlend = Blend.Zero,
		};

		public static readonly BlendState Subtractive = new()
		{
			ColorSourceBlend = Blend.One,
			AlphaSourceBlend = Blend.One,
			ColorDestinationBlend = Blend.One,
			AlphaDestinationBlend = Blend.One,
			ColorBlendFunction = BlendFunction.ReverseSubtract,
			AlphaBlendFunction = BlendFunction.ReverseSubtract,
		};
	}
}
