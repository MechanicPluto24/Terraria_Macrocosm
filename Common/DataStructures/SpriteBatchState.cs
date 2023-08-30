using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Macrocosm.Common.DataStructures
{
	public class SpriteBatchState
	{
		public bool BeginCalled { get; init; }
		public SpriteSortMode SpriteSortMode { get; init; }
		public BlendState BlendState { get; init; }
		public SamplerState SamplerState { get; init; }
		public DepthStencilState DepthStencilState { get; init; }
		public RasterizerState RasterizerState { get; init; }
		public Effect Effect { get; init; }
		public Matrix Matrix { get; init; }

		public SpriteBatchState(bool beginCalled, SpriteSortMode spriteSortMode, BlendState blendState, SamplerState samplerState, DepthStencilState depthStencilState, RasterizerState rasterizerState, Effect effect, Matrix matrix)
		{
			BeginCalled = beginCalled;
			SpriteSortMode = spriteSortMode;
			BlendState = blendState;
			SamplerState = samplerState;
			DepthStencilState = depthStencilState;
			RasterizerState = rasterizerState;
			Effect = effect;
			Matrix = matrix;
		}
	}
}
