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
		public static readonly BlendState Multiplicative = new BlendState
		{
			ColorBlendFunction = BlendFunction.Add,
			ColorSourceBlend = Blend.DestinationColor,
			ColorDestinationBlend = Blend.Zero,
		};

		public static readonly BlendState Subtractive = new BlendState
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
