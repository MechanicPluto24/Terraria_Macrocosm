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

        private static FieldInfo spriteBatch_sortMode_fieldInfo;
        private static FieldInfo spriteBatch_blendState_fieldInfo;
        private static FieldInfo spriteBatch_samplerState_fieldInfo;
        private static FieldInfo spriteBatch_customEffect_fieldInfo;
        private static FieldInfo spriteBatch_transformMatrix_fieldInfo;

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
                spriteBatch_sortMode_fieldInfo ??= type.GetField("sortMode", BindingFlags.Instance | BindingFlags.NonPublic);
                spriteBatch_blendState_fieldInfo ??= type.GetField("blendState", BindingFlags.Instance | BindingFlags.NonPublic);
                spriteBatch_samplerState_fieldInfo ??= type.GetField("samplerState", BindingFlags.Instance | BindingFlags.NonPublic);
                spriteBatch_customEffect_fieldInfo ??= type.GetField("customEffect", BindingFlags.Instance | BindingFlags.NonPublic);
                spriteBatch_transformMatrix_fieldInfo ??= type.GetField("transformMatrix", BindingFlags.Instance | BindingFlags.NonPublic);

                SpriteSortMode = (SpriteSortMode)spriteBatch_sortMode_fieldInfo.GetValue(spriteBatch);
                BlendState = (BlendState)spriteBatch_blendState_fieldInfo.GetValue(spriteBatch);
                SamplerState = (SamplerState)spriteBatch_samplerState_fieldInfo.GetValue(spriteBatch);
                Effect = (Effect)spriteBatch_customEffect_fieldInfo.GetValue(spriteBatch);
                initialized = true;
            }

            RasterizerState = spriteBatch.GraphicsDevice.RasterizerState;
            Matrix = (Matrix)spriteBatch_transformMatrix_fieldInfo.GetValue(spriteBatch);
        }
    }
}
