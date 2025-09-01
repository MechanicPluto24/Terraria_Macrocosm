using Macrocosm.Common.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Macrocosm.Common.DataStructures;

public struct SpriteBatchState
{
    public bool BeginCalled { get; private set; }

    public SpriteSortMode SpriteSortMode { get; private set; }
    public BlendState BlendState { get; private set; }
    public SamplerState SamplerState { get; private set; }
    public DepthStencilState DepthStencilState { get; private set; }

    public RasterizerState RasterizerState { get; private set; }
    public Effect Effect { get; private set; }
    public Matrix Matrix { get; set; }

    private bool initialized;

    public SpriteBatchState()
    {
        BeginCalled = false;
        SpriteSortMode = SpriteSortMode.Deferred;
        BlendState = default;
        SamplerState = default;
        DepthStencilState = default;
        RasterizerState = default;
        Effect = default;
        Matrix = default;

        initialized = false;
    }

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

        initialized = true;
    }

    public void SaveState(SpriteBatch spriteBatch, bool continuous = false)
    {
        var type = spriteBatch.GetType();
        BeginCalled = spriteBatch.BeginCalled();

        if (!initialized || continuous)
        {
            SpriteSortMode = type.GetFieldValue<SpriteSortMode>("sortMode", spriteBatch);
            BlendState = type.GetFieldValue<BlendState>("blendState", spriteBatch);
            DepthStencilState = DepthStencilState.None;
            Effect = type.GetFieldValue<Effect>("customEffect", spriteBatch);

            initialized = true;
        }

        SamplerState = type.GetFieldValue<SamplerState>("samplerState", spriteBatch);
        RasterizerState = type.GetFieldValue<RasterizerState>("rasterizerState", spriteBatch);
        Matrix = type.GetFieldValue<Matrix>("transformMatrix", spriteBatch);
    }
}
