using System;
using System.Reflection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;

namespace Macrocosm.Common.Utils
{
    public struct SpriteBatchState
    {
        public bool beginCalled;
        public SpriteSortMode sortMode;
        public BlendState blendState;
        public SamplerState samplerState;
        public DepthStencilState depthStencilState;
        public RasterizerState rasterizerState;
        public Effect effect;
        public Matrix matrix;
    }

    public static partial class Utility
    {
        /// <summary> Saves the SpriteBatch parameters in a struct </summary>
        public static SpriteBatchState SaveState(this SpriteBatch spriteBatch)
        {
            if (spriteBatch.BeginCalled())
                return new SpriteBatchState()
                {
                    beginCalled = true,
                    sortMode = (SpriteSortMode)spriteBatch.GetType().GetField("sortMode", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(spriteBatch),
                    blendState = (BlendState)spriteBatch.GetType().GetField("blendState", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(spriteBatch),
                    samplerState = (SamplerState)spriteBatch.GetType().GetField("samplerState", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(spriteBatch),
                    depthStencilState = (DepthStencilState)spriteBatch.GetType().GetField("depthStencilState", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(spriteBatch),
                    rasterizerState = (RasterizerState)spriteBatch.GetType().GetField("rasterizerState", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(spriteBatch),
                    effect = (Effect)spriteBatch.GetType().GetField("customEffect", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(spriteBatch),
                    matrix = (Matrix)spriteBatch.GetType().GetField("transformMatrix", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(spriteBatch)
                };
            else
                return new SpriteBatchState()
                {
                    // some defaults 
                    beginCalled = false,
                    sortMode = SpriteSortMode.Deferred,
                    blendState = BlendState.AlphaBlend,
                    samplerState = Main.DefaultSamplerState,
                    depthStencilState = DepthStencilState.Default,
                    rasterizerState = RasterizerState.CullCounterClockwise,
                    effect = null,
                    matrix = Main.GameViewMatrix.TransformationMatrix
                };
        }

        public static void Begin(this SpriteBatch spriteBatch, SpriteSortMode sortMode, BlendState blendState, Effect effect, SpriteBatchState state)
            => spriteBatch.Begin(sortMode, blendState, state.samplerState, state.depthStencilState, state.rasterizerState, effect, state.matrix);

        public static void Begin(this SpriteBatch spriteBatch, SpriteSortMode sortMode, BlendState blendState, SpriteBatchState state)
            => spriteBatch.Begin(sortMode, blendState, state.samplerState, state.depthStencilState, state.rasterizerState, state.effect, state.matrix);

        public static void Begin(this SpriteBatch spriteBatch, BlendState blendState, SpriteBatchState state)
            => spriteBatch.Begin(state.sortMode, blendState, state.samplerState, state.depthStencilState, state.rasterizerState, state.effect, state.matrix);

        public static void Begin(this SpriteBatch spriteBatch, BlendState blendState, Effect effect, SpriteBatchState state)
            => spriteBatch.Begin(state.sortMode, blendState, state.samplerState, state.depthStencilState, state.rasterizerState, effect, state.matrix);

        public static void Begin(this SpriteBatch spriteBatch, Effect effect, SpriteBatchState state)
            => spriteBatch.Begin(state.sortMode, state.blendState, state.samplerState, state.depthStencilState, state.rasterizerState, effect, state.matrix);

        /// <summary> Begins the SpriteBatch with the parameters stored in a SpriteBatchState </summary>
        public static void Begin(this SpriteBatch spriteBatch, SpriteBatchState state)
                => spriteBatch.Begin(state.sortMode, state.blendState, state.samplerState, state.depthStencilState, state.rasterizerState, state.effect, state.matrix);

        public static void EndIfBeginCalled(this SpriteBatch spriteBatch)
        {
            if (spriteBatch.BeginCalled())
                spriteBatch.End();
        }

        public static bool BeginCalled(this SpriteBatch spriteBatch)
            => (bool)spriteBatch.GetType().GetField("beginCalled", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(spriteBatch);
    }
}