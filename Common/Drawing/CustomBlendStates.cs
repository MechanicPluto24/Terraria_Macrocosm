using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Macrocosm.Common.Drawing
{
	internal class CustomBlendStates
	{
		public static readonly BlendState Multiply = new BlendState
		{
			ColorSourceBlend = Blend.DestinationColor,
			AlphaSourceBlend = Blend.DestinationColor,
			ColorDestinationBlend = Blend.Zero,
			AlphaDestinationBlend = Blend.Zero,
		};

		public static readonly BlendState Screen = new BlendState
		{
			ColorSourceBlend = Blend.One,
			AlphaSourceBlend = Blend.One,
			ColorDestinationBlend = Blend.InverseSourceColor,
			AlphaDestinationBlend = Blend.InverseSourceColor,
		};

		public static readonly BlendState Overlay = new BlendState
		{
			ColorSourceBlend = Blend.One,
			AlphaSourceBlend = Blend.One,
			ColorDestinationBlend = Blend.InverseSourceAlpha,
			AlphaDestinationBlend = Blend.InverseSourceAlpha,
		};

		public static readonly BlendState Subtract = new BlendState
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
