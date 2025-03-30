using Macrocosm.Common.DataStructures;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;

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

        public static RenderTargetBinding[] RenderTargetBindings => Main.graphics.GraphicsDevice.GetRenderTargets();
        public static void RenderTargetPreserveContents()
        {
            foreach (var rtb in Main.graphics.GraphicsDevice.GetRenderTargets())
                typeof(RenderTarget2D).SetPropertyValue("RenderTargetUsage", RenderTargetUsage.PreserveContents, rtb.RenderTarget);
            Main.graphics.GraphicsDevice.PresentationParameters.RenderTargetUsage = RenderTargetUsage.PreserveContents;
        }

        public static RenderTargetBinding[] SaveRenderTargets(this GraphicsDevice graphicsDevice)
        {
            RenderTargetBinding[] renderTargetBindings = graphicsDevice.GetRenderTargets();
            foreach (RenderTargetBinding binding in renderTargetBindings)
                typeof(RenderTarget2D).SetPropertyValue("RenderTargetUsage", RenderTargetUsage.PreserveContents, binding.RenderTarget);
            graphicsDevice.PresentationParameters.RenderTargetUsage = RenderTargetUsage.PreserveContents;

            return renderTargetBindings.Length > 0 ? renderTargetBindings : null;
        }

        private static RenderTarget2D _swapRT1, _swapRT2;
        private static SpriteBatchState _state;
        /// <summary>
        /// Applies multiple shader effects sequentially to a Texture2D.
        /// </summary>
        /// <param name="source">The source texture to apply effects on.</param>
        /// <param name="effects">An array of pre-configured shader Effects.</param>
        /// <returns>A Texture2D with all effects applied sequentially.</returns>
        public static Texture2D ApplyEffects(this Texture2D source, params Effect[] effects)
        {
            if (effects == null || effects.Length == 0)
                return source;

            var graphicsDevice = Main.graphics.GraphicsDevice;

            if (_swapRT1 == null || _swapRT1.Width != source.Width || _swapRT1.Height != source.Height)
            {
                _swapRT1?.Dispose();
                _swapRT1 = new RenderTarget2D(graphicsDevice, source.Width, source.Height, false, SurfaceFormat.Color, DepthFormat.None);
            }

            if (_swapRT2 == null || _swapRT2.Width != source.Width || _swapRT2.Height != source.Height)
            {
                _swapRT2?.Dispose();
                _swapRT2 = new RenderTarget2D(graphicsDevice, source.Width, source.Height, false, SurfaceFormat.Color, DepthFormat.None);
            }

            static void DrawToRenderTarget(Texture2D texture, Effect effect)
            {
                if (Main.spriteBatch.BeginCalled())
                {
                    _state.SaveState(Main.spriteBatch);
                    Main.spriteBatch.End();
                    Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Opaque, null, null, null, effect);
                    Main.spriteBatch.Draw(texture, new Rectangle(0, 0, texture.Width, texture.Height), Color.White);
                    Main.spriteBatch.End();
                    Main.spriteBatch.Begin(_state);
                }
                else
                {
                    Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Opaque, null, null, null, effect);
                    Main.spriteBatch.Draw(texture, new Rectangle(0, 0, texture.Width, texture.Height), Color.White);
                    Main.spriteBatch.End();
                }
            }

            var renders = graphicsDevice.SaveRenderTargets();
            graphicsDevice.SetRenderTarget(_swapRT1);
            graphicsDevice.Clear(Color.Transparent);
            DrawToRenderTarget(source, null);
            graphicsDevice.SetRenderTargets(renders);

            bool swapFlag = false;
            for (int i = 0; i < effects.Length; i++)
            {
                var input = swapFlag ? _swapRT2 : _swapRT1;
                var output = swapFlag ? _swapRT1 : _swapRT2;
                swapFlag = !swapFlag;

                graphicsDevice.SetRenderTarget(output);
                graphicsDevice.Clear(Color.Transparent);
                DrawToRenderTarget(input, effects[i]);
                graphicsDevice.SetRenderTargets(renders);
            }

            return swapFlag ? _swapRT2 : _swapRT1;
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