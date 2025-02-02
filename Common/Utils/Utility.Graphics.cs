using Macrocosm.Common.DataStructures;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Reflection;
using Terraria;
using Terraria.ModLoader;

namespace Macrocosm.Common.Utils
{
    public static partial class Utility
    {
        public static Vector2 ScreenCenter => new(Main.screenWidth / 2f, Main.screenHeight / 2f);

        public static Vector2 ScreenCenterInWorld => Main.screenPosition + ScreenCenter;

        public static Rectangle ScreenRectangle => new(0, 0, Main.screenWidth, Main.screenHeight);

        public static void Draw(this SpriteBatch spriteBatch, Texture2D texture, System.Drawing.RectangleF destinationRectangle, Color color)
        {
            Vector2 position = new(destinationRectangle.X, destinationRectangle.Y);
            Vector2 scale = new(destinationRectangle.Width / texture.Width, destinationRectangle.Height / texture.Height);
            Vector2 origin = Vector2.Zero;
            spriteBatch.Draw(texture, position, null, color, 0.0f, origin, scale, SpriteEffects.None, 0.0f);
        }

        public static bool BeginCalled(this SpriteBatch spriteBatch) => typeof(SpriteBatch).GetFieldValue<bool>("beginCalled", spriteBatch);

        /// <summary> Saves the SpriteBatch parameters </summary>
        public static SpriteBatchState SaveState(this SpriteBatch spriteBatch)
        {
            if (!spriteBatch.BeginCalled())
                return new();

            SpriteBatchState state = new();
            state.SaveState(spriteBatch);
            return state;
        }

        /// <summary> End the SpriteBatch but save the SpriteBatchState </summary>
        public static void End(this SpriteBatch spriteBatch, out SpriteBatchState state)
        {
            state = spriteBatch.SaveState();
            spriteBatch.End();
        }

        public static void EndIfBeginCalled(this SpriteBatch spriteBatch)
        {
            if (spriteBatch.BeginCalled())
                spriteBatch.End();
        }

        /// <summary> Begins the SpriteBatch with parameters from SpriteBatchState </summary>
        public static void Begin(this SpriteBatch spriteBatch, SpriteBatchState state)
                => spriteBatch.Begin(state.SpriteSortMode, state.BlendState, state.SamplerState, state.DepthStencilState, state.RasterizerState, state.Effect, state.Matrix);

        /// <inheritdoc cref="Begin(SpriteBatch, SpriteBatchState)"/>
        public static void Begin(this SpriteBatch spriteBatch, SpriteSortMode sortMode, BlendState blendState, Effect effect, SpriteBatchState state)
            => spriteBatch.Begin(sortMode, blendState, state.SamplerState, state.DepthStencilState, state.RasterizerState, effect, state.Matrix);

        /// <inheritdoc cref="Begin(SpriteBatch, SpriteBatchState)"/>
        public static void Begin(this SpriteBatch spriteBatch, SpriteSortMode sortMode, BlendState blendState, SpriteBatchState state)
            => spriteBatch.Begin(sortMode, blendState, state.SamplerState, state.DepthStencilState, state.RasterizerState, state.Effect, state.Matrix);

        /// <inheritdoc cref="Begin(SpriteBatch, SpriteBatchState)"/>
        public static void Begin(this SpriteBatch spriteBatch, BlendState blendState, SpriteBatchState state)
            => spriteBatch.Begin(state.SpriteSortMode, blendState, state.SamplerState, state.DepthStencilState, state.RasterizerState, state.Effect, state.Matrix);

        /// <inheritdoc cref="Begin(SpriteBatch, SpriteBatchState)"/>
        public static void Begin(this SpriteBatch spriteBatch, BlendState blendState, SamplerState samplerState, SpriteBatchState state)
            => spriteBatch.Begin(state.SpriteSortMode, blendState, samplerState, state.DepthStencilState, state.RasterizerState, state.Effect, state.Matrix);

        /// <inheritdoc cref="Begin(SpriteBatch, SpriteBatchState)"/>
        public static void Begin(this SpriteBatch spriteBatch, SamplerState samplerState, SpriteBatchState state)
           => spriteBatch.Begin(state.SpriteSortMode, state.BlendState, samplerState, state.DepthStencilState, state.RasterizerState, state.Effect, state.Matrix);

        /// <inheritdoc cref="Begin(SpriteBatch, SpriteBatchState)"/>
        public static void Begin(this SpriteBatch spriteBatch, BlendState blendState, Effect effect, SpriteBatchState state)
            => spriteBatch.Begin(state.SpriteSortMode, blendState, state.SamplerState, state.DepthStencilState, state.RasterizerState, effect, state.Matrix);

        /// <inheritdoc cref="Begin(SpriteBatch, SpriteBatchState)"/>
        public static void Begin(this SpriteBatch spriteBatch, Effect effect, SpriteBatchState state)
            => spriteBatch.Begin(state.SpriteSortMode, state.BlendState, state.SamplerState, state.DepthStencilState, state.RasterizerState, effect, state.Matrix);

        /// <inheritdoc cref="Begin(SpriteBatch, SpriteBatchState)"/>
        public static void Begin(this SpriteBatch spriteBatch, SpriteBatchState state, Matrix matrix)
               => spriteBatch.Begin(state.SpriteSortMode, state.BlendState, state.SamplerState, state.DepthStencilState, state.RasterizerState, state.Effect, matrix);

        /// <inheritdoc cref="Begin(SpriteBatch, SpriteBatchState)"/>
        public static void Begin(this SpriteBatch spriteBatch, RasterizerState rasterizerState, SpriteBatchState state)
            => spriteBatch.Begin(state.SpriteSortMode, state.BlendState, state.SamplerState, state.DepthStencilState, rasterizerState, state.Effect, state.Matrix);

    }
}