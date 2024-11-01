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
        public delegate Color ColorMethod(Vector2 uv, Vector2 position);

        /// <summary>
        /// Creates a rectangular vertex mesh
        /// </summary>
        /// <param name="position">The position of the mesh</param>
        /// <param name="width">The width of the mesh</param>
        /// <param name="height">The height of the mesh</param>
        /// <param name="horizontalResolution">How many vertices to use for the horizontal resolution</param>
        /// <param name="verticalResolution">How many vertices to use for the vertical resolution</param>
        /// <param name="colorFunction">The function that returns a color to use for each vertex. Provides the UV and position of the current vertex</param>
        /// <param name="vertices">The resulting vertices array</param>
        /// <param name="indices">The resulting indices array</param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static void CreateMeshRectangle(Vector2 position, float width, float height, int horizontalResolution, int verticalResolution, ColorMethod colorFunction, out VertexPositionColorTexture[] vertices, out short[] indices, bool debug = false)
        {
            // We can only work with a minimum resolution of 2x2 because we need at least 4 vertices

            if (horizontalResolution < 2)
            {
                throw new ArgumentOutOfRangeException(nameof(horizontalResolution), "Horizontal resolution must be a minimum of 2");
            }

            if (verticalResolution < 2)
            {
                throw new ArgumentOutOfRangeException(nameof(verticalResolution), "Vertical resolution must be a minimum of 2");
            }

            // Initialize arrays

            // vertex count = x * y
            vertices = new VertexPositionColorTexture[horizontalResolution * verticalResolution];

            // x - 1 is the number of horizontal quads
            // y - 1 is the number of vertical quads
            // we get the total amount of quads then multiply by 6, because we need 6 indices for each quad
            indices = new short[(horizontalResolution - 1) * (verticalResolution - 1) * 6];

            // Iterate over vertices
            for (int i = 0; i < horizontalResolution; i++)
            {
                for (var j = 0; j < verticalResolution; j++)
                {
                    // Calculate variables
                    float u = i / (float)(horizontalResolution - 1);
                    float v = i / (float)(verticalResolution - 1);
                    Vector2 pos = new(position.X + u * width, position.Y + v * height);

                    vertices[(i * verticalResolution) + j] = new VertexPositionColorTexture(new Vector3(pos, 0f), colorFunction?.Invoke(pos, position) ?? Color.White, new Vector2(u, v));
                }
            }

            int index = 0;

            // Iterate over indices
            for (int i = 0; i < horizontalResolution - 1; i++)
            {
                for (int j = 0; j < verticalResolution - 1; j++)
                {
                    // 0 --- 2
                    // |  \  |
                    // 1 --- 3

                    // A --- B
                    // |  \  |
                    // D --- C

                    // 0 > 1 > 3
                    // A > B > C
                    indices[index++] = (short)(i * verticalResolution + j);
                    indices[index++] = (short)(i * verticalResolution + j + 1);
                    indices[index++] = (short)((i + 1) * verticalResolution + j + 1);

                    // 0 > 3 > 2
                    // A > C > D
                    indices[index++] = (short)(i * verticalResolution + j);
                    indices[index++] = (short)((i + 1) * verticalResolution + j + 1);
                    indices[index++] = (short)((i + 1) * verticalResolution + j);

                    // The example numbers given aren't static, they increase in increments of 2 for each quad
                }
            }

            if (debug)
            {
                bool beginCalled = Main.spriteBatch.BeginCalled();

                if (!beginCalled)
                    Main.spriteBatch.Begin();

                var tex = ModContent.Request<Texture2D>(Macrocosm.TextureEffectsPath + "Circle5", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
                foreach (VertexPositionColorTexture vertex in vertices)
                    Main.spriteBatch.Draw(tex, new Vector2(vertex.Position.X, vertex.Position.Y), tex.Bounds, new Color(1f, 1f, 1f, 0f), 0f, Vector2.Zero, 0.01f, SpriteEffects.None, 0f);

                if (!beginCalled)
                    Main.spriteBatch.End();
            }
        }

        /// <summary> Saves the SpriteBatch parameters. Prefer to use <see cref="SpriteBatchState.SaveState(SpriteBatch)"/> instead</summary>
        public static SpriteBatchState SaveState(this SpriteBatch spriteBatch)
        {
            if (spriteBatch.BeginCalled())
            {
                var type = spriteBatch.GetType();
                return new SpriteBatchState(
                   true,
                   (SpriteSortMode)type.GetField("sortMode", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(spriteBatch),
                   (BlendState)type.GetField("blendState", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(spriteBatch),
                   (SamplerState)type.GetField("samplerState", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(spriteBatch),
                   default,
                   (RasterizerState)spriteBatch.GetType().GetField("rasterizerState", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(spriteBatch),
                   (Effect)type.GetField("customEffect", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(spriteBatch),
                   (Matrix)type.GetField("transformMatrix", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(spriteBatch)
               );
            }

            else
                return new SpriteBatchState(
                    false,
                    SpriteSortMode.Deferred,
                    BlendState.AlphaBlend,
                    Main.DefaultSamplerState,
                    DepthStencilState.Default,
                    RasterizerState.CullNone,
                    null,
                    Main.GameViewMatrix.TransformationMatrix
                );
        }

        public static void End(this SpriteBatch spriteBatch, out SpriteBatchState state)
        {
            state = spriteBatch.SaveState();
            spriteBatch.End();
        }

        public static void Begin(this SpriteBatch spriteBatch, SpriteSortMode sortMode, BlendState blendState, Effect effect, SpriteBatchState state)
            => spriteBatch.Begin(sortMode, blendState, state.SamplerState, state.DepthStencilState, state.RasterizerState, effect, state.Matrix);

        public static void Begin(this SpriteBatch spriteBatch, SpriteSortMode sortMode, BlendState blendState, SpriteBatchState state)
            => spriteBatch.Begin(sortMode, blendState, state.SamplerState, state.DepthStencilState, state.RasterizerState, state.Effect, state.Matrix);

        public static void Begin(this SpriteBatch spriteBatch, BlendState blendState, SpriteBatchState state)
            => spriteBatch.Begin(state.SpriteSortMode, blendState, state.SamplerState, state.DepthStencilState, state.RasterizerState, state.Effect, state.Matrix);

        public static void Begin(this SpriteBatch spriteBatch, BlendState blendState, SamplerState samplerState, SpriteBatchState state)
            => spriteBatch.Begin(state.SpriteSortMode, blendState, samplerState, state.DepthStencilState, state.RasterizerState, state.Effect, state.Matrix);

        public static void Begin(this SpriteBatch spriteBatch, SamplerState samplerState, SpriteBatchState state)
           => spriteBatch.Begin(state.SpriteSortMode, state.BlendState, samplerState, state.DepthStencilState, state.RasterizerState, state.Effect, state.Matrix);

        public static void Begin(this SpriteBatch spriteBatch, BlendState blendState, Effect effect, SpriteBatchState state)
            => spriteBatch.Begin(state.SpriteSortMode, blendState, state.SamplerState, state.DepthStencilState, state.RasterizerState, effect, state.Matrix);

        public static void Begin(this SpriteBatch spriteBatch, Effect effect, SpriteBatchState state)
            => spriteBatch.Begin(state.SpriteSortMode, state.BlendState, state.SamplerState, state.DepthStencilState, state.RasterizerState, effect, state.Matrix);

        public static void Begin(this SpriteBatch spriteBatch, SpriteBatchState state, Matrix matrix)
               => spriteBatch.Begin(state.SpriteSortMode, state.BlendState, state.SamplerState, state.DepthStencilState, state.RasterizerState, state.Effect, matrix);

        public static void Begin(this SpriteBatch spriteBatch, RasterizerState rasterizerState, SpriteBatchState state)
            => spriteBatch.Begin(state.SpriteSortMode, state.BlendState, state.SamplerState, state.DepthStencilState, rasterizerState, state.Effect, state.Matrix);

        /// <summary> Begins the SpriteBatch with the parameters stored in a SpriteBatchState </summary>
        public static void Begin(this SpriteBatch spriteBatch, SpriteBatchState state)
                => spriteBatch.Begin(state.SpriteSortMode, state.BlendState, state.SamplerState, state.DepthStencilState, state.RasterizerState, state.Effect, state.Matrix);

        public static void EndIfBeginCalled(this SpriteBatch spriteBatch)
        {
            if (spriteBatch.BeginCalled())
                spriteBatch.End();
        }

        public static bool BeginCalled(this SpriteBatch spriteBatch)
            => (bool)spriteBatch.GetType().GetField("beginCalled", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(spriteBatch);
    }
}