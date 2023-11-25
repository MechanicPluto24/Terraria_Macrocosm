using Macrocosm.Common.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Reflection;

namespace Macrocosm.Common.DataStructures
{
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
                SpriteSortMode = (SpriteSortMode)type.GetField("sortMode", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(spriteBatch);
                BlendState = (BlendState)type.GetField("blendState", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(spriteBatch);
                SamplerState = (SamplerState)type.GetField("samplerState", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(spriteBatch);
                DepthStencilState = default;
                RasterizerState = (RasterizerState)spriteBatch.GetType().GetField("rasterizerState", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(spriteBatch);
                Effect = (Effect)type.GetField("customEffect", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(spriteBatch);

                initialized = true;
            }

            Matrix = (Matrix)type.GetField("transformMatrix", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(spriteBatch);
        }
    }
}
