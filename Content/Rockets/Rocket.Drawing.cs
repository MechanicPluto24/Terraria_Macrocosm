using Macrocosm.Common.DataStructures;
using Macrocosm.Common.Graphics;
using Macrocosm.Common.Utils;
using Macrocosm.Content.Rockets.Modules;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Linq;
using Terraria;
using Terraria.Graphics.Capture;
using Terraria.ModLoader;

namespace Macrocosm.Content.Rockets
{
    public partial class Rocket
    {
        public enum DrawMode
        {
            World,
            Dummy,
            Blueprint
        }

        private Effect meshEffect;
        private EffectParameter meshTransform;
        private RenderTarget2D renderTarget;
        private SpriteBatchState state, state2;
        private DynamicVertexBuffer vertexBuffer;
        private DynamicIndexBuffer indexBuffer;
        private VertexPositionColorTexture[] vertices;
        private short[] indices;

        private bool firstDraw = true;

        public bool HasRenderTarget => renderTarget is not null && !renderTarget.IsDisposed;

        public void ResetRenderTarget()
        {
            renderTarget?.Dispose();
        }

        /// <summary> Draw the rocket </summary>
        /// <param name="drawMode"> 
        /// The drawing mode: <list type="bullet">
        /// <item> <see cref="DrawMode.World"/>: Draws the rocket in world </item>
        /// <item> <see cref="DrawMode.Dummy"/>: Draws the rocket as a dummy, for use in UI </item>
        /// <item> <see cref="DrawMode.Blueprint"/>:  Draws the rocket as a blueprint, for use in the assembly UI </item>
        ///</list> </param>
        /// <param name="spriteBatch"> The spritebatch </param>
        /// <param name="position"> The draw position </param>
        /// <param name="useRenderTarget"> 
        /// Whether to use a render target or draw the rocket directly.
        /// For performance considerations, prefer to use a render target for drawing, except where visual changes happen very often.
        /// NOTE: In world, you must use a render target in order for the lighting to work.
        /// </param>
        public void Draw(DrawMode drawMode, SpriteBatch spriteBatch, Vector2 position, bool useRenderTarget = true)
        {
            if (useRenderTarget)
            {
                // Prepare our RenderTarget
                renderTarget = GetRenderTarget(drawMode);

                // Save our SpriteBatch state
                state.SaveState(spriteBatch);
                spriteBatch.EndIfBeginCalled();

                switch (drawMode)
                {
                    // Only DrawMode.World consumes the lighting buffers
                    case DrawMode.World:
                        PrepareEffect(drawMode);
                        PrepareLightingBuffers(Width, Height, out int numVertices, out int primitiveCount);
                        PresentLightingBuffers(numVertices, primitiveCount);
                        break;

                    // All other cases draw the RenderTarget directly
                    default:

                        spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, state.DepthStencilState, state.RasterizerState, state.Effect, state.Matrix);
                        spriteBatch.Draw(renderTarget, position, Color.White);
                        spriteBatch.End();

                        break;
                }

                if(firstDraw)
                {
                    ResetRenderTarget();
                    firstDraw = false;
                }

                // Reset our SpriteBatch to its previous state
                spriteBatch.Begin(state);
            }
            else
            {
                switch (drawMode)
                {
                    case DrawMode.World:
                        DrawWorld(spriteBatch, position);
                        break;

                    case DrawMode.Dummy:
                        DrawDummy(spriteBatch, position);
                        break;

                    case DrawMode.Blueprint:
                        DrawBlueprint(spriteBatch, position);
                        break;
                }
            }
        }

        public void PreDrawBeforeTiles(SpriteBatch spriteBatch, Vector2 position, bool inWorld = true)
        {
            foreach (RocketModule module in ModulesByDrawPriority)
            {
                module.PreDrawBeforeTiles(spriteBatch, GetModuleRelativePosition(module, position), inWorld);
            }
        }

        public void PostDraw(SpriteBatch spriteBatch, Vector2 position, bool inWorld = true)
        {
            foreach (RocketModule module in ModulesByDrawPriority)
            {
                module.PostDraw(spriteBatch, GetModuleRelativePosition(module, position), inWorld);
            }
        }

        public void DrawOverlay(SpriteBatch spriteBatch, Vector2 position)
        {
            if (StaticFire || InFlight || Landing || ForcedFlightAppearance)
            {
                float scale = 1.2f * Main.rand.NextFloat(0.85f, 1f);

                if (ForcedFlightAppearance)
                    scale *= 1.25f;

                if (StaticFire)
                    scale *= Utility.QuadraticEaseOut(StaticFireProgress);

                if(Landing && LandingProgress > 0.9f)
                    scale *= Utility.QuadraticEaseOut((1f - LandingProgress) * 10f);

                var flare = ModContent.Request<Texture2D>(Macrocosm.TextureEffectsPath + "Flare2").Value;
                spriteBatch.Draw(flare, position + new Vector2(Bounds.Width / 2, Bounds.Height), null, new Color(255, 69, 0), 0f, flare.Size() / 2f, scale, SpriteEffects.None, 0f);
            }
        }

        public RenderTarget2D GetRenderTarget(DrawMode drawMode)
        {
            // We only need to prepare our RenderTarget if it's not ready to use
            if (renderTarget is null || renderTarget.IsDisposed)
            {
                // Initialize our RenderTarget
                renderTarget = new(Main.spriteBatch.GraphicsDevice, Bounds.Width, Bounds.Height, mipMap: false, SurfaceFormat.Color, DepthFormat.None, 0, RenderTargetUsage.PreserveContents);
                FillRenderTarget(Main.spriteBatch, drawMode);
            }

            return renderTarget;
        }

        // Draw types
        private void DrawWorld(SpriteBatch spriteBatch, Vector2 position)
        {
            foreach (RocketModule module in ModulesByDrawPriority)
            {
                module.Draw(spriteBatch, GetModuleRelativePosition(module, position));
            }
        }

        private void DrawDummy(SpriteBatch spriteBatch, Vector2 position)
        {
            PreDrawBeforeTiles(spriteBatch, position, inWorld: false);
            DrawWorld(spriteBatch, position);
            PostDraw(spriteBatch, position, inWorld: false);

            state2.SaveState(spriteBatch);
            spriteBatch.End();
            spriteBatch.Begin(state.SpriteSortMode, BlendState.Additive, state.SamplerState, state.DepthStencilState, state.RasterizerState, state.Effect, Main.UIScaleMatrix);
            DrawOverlay(spriteBatch, position);
            spriteBatch.End();
            spriteBatch.Begin(state);
        }

        private void DrawBlueprint(SpriteBatch spriteBatch, Vector2 position)
        {
            foreach (RocketModule module in ModulesByDrawPriority.OrderBy(module => module.BlueprintHighlighted))
            {
                Vector2 drawPosition = GetModuleRelativePosition(module, position);

                if (module.IsBlueprint)
                {
                    if (module is BoosterLeft)
                        drawPosition.X -= 78;

                    module.DrawBlueprint(spriteBatch, drawPosition);
                }
                else
                {
                    module.PreDrawBeforeTiles(spriteBatch, drawPosition, inWorld: false);
                    module.Draw(spriteBatch, drawPosition);
                }
            }
        }

        // Draw preparation
        private void PrepareEffect(DrawMode drawMode)
        {
            // Guard against effect being null
            if (meshEffect is null || meshEffect.IsDisposed)
            {
                meshEffect = ModContent.Request<Effect>(Macrocosm.ShadersPath + "Mesh", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
                meshTransform = meshEffect.Parameters["TransformMatrix"];
            }

            // We want our vertices to scale correctly
            if (drawMode is DrawMode.World)
            {
                meshTransform.SetValue(PrimitivesSystem.WorldViewProjection);           
            }
            else
            {
                meshTransform.SetValue(Main.UIScaleMatrix);
            }
        }

        private void FillRenderTarget(SpriteBatch spriteBatch, DrawMode drawMode)
        {
            // Store previous settings
            var scissorRectangle = PrimitivesSystem.GraphicsDevice.ScissorRectangle;
            var rasterizerState = PrimitivesSystem.GraphicsDevice.RasterizerState;

            // Capture original RenderTargets and preserve their contents
            PrimitivesSystem.GraphicsDevice.PresentationParameters.RenderTargetUsage = RenderTargetUsage.PreserveContents;
            RenderTargetBinding[] originalRenderTargets = spriteBatch.GraphicsDevice.GetRenderTargets();
            foreach (var binding in originalRenderTargets)
                typeof(RenderTarget2D).SetPropertyValue("RenderTargetUsage", RenderTargetUsage.PreserveContents, binding.RenderTarget);

            // Draw our modules
            state = spriteBatch.SaveState();
            spriteBatch.EndIfBeginCalled();

            spriteBatch.GraphicsDevice.SetRenderTarget(renderTarget);
            spriteBatch.GraphicsDevice.Clear(Color.Transparent);

            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, state.DepthStencilState, state.RasterizerState, state.Effect, Matrix.CreateScale(1f));

            switch (drawMode)
            {
                case DrawMode.World:
                    DrawWorld(spriteBatch, default);
                    break;

                case DrawMode.Dummy:
                    DrawDummy(spriteBatch, default);
                    break;

                case DrawMode.Blueprint:
                    DrawBlueprint(spriteBatch, default);
                    break;
            }

            spriteBatch.End();

            // Revert our RenderTargets back to the vanilla ones
            if (originalRenderTargets.Length > 0)
            {
                PrimitivesSystem.GraphicsDevice.SetRenderTargets(originalRenderTargets);
            }
            else
            {
                PrimitivesSystem.GraphicsDevice.SetRenderTarget(null);
            }

            // Reset our settings back to the previous ones
            PrimitivesSystem.GraphicsDevice.ScissorRectangle = scissorRectangle;
            PrimitivesSystem.GraphicsDevice.RasterizerState = rasterizerState;
        }

        private void PrepareLightingBuffers(int width, int height, out int numVertices, out int primitiveCount)
        {
            // The rocket RenderTarget bounds are not perfectly divisible by 8f (half of a tile)
            // So we approximate, and this yields a result almost indistinguishable anyway.
            int w = 3;
            int h = 6;

            // There are always more vertices than quads
            // In our case we want a two-dimensional mesh, so the formula for vertex count is `(w + 1) * (h + 1)`
            // For a one-dimensional string of quads, the formula is `(n * 2) + 2` where `n` is the number of points you're creating quads from
            // These rules apply only for indexed vertice. Unindexed quads don't reuse vertices, and always need four per quad
            int vertexCount = (w + 1) * (h + 1);

            // Every quad needs six indices, so the formula is just w * h * 6
            int indexCount = w * h * 6;

            // Output stuff we need for drawing the mesh
            numVertices = vertexCount;
            primitiveCount = w * h * 2; // Each quad consists of two primitives (triangles)

            // Now we create our vertex buffer
            vertexBuffer ??= new(Main.graphics.GraphicsDevice, typeof(VertexPositionColorTexture), vertexCount, BufferUsage.WriteOnly);

            // Now we create our vertex buffer
            // As a note, IndexElementSize is there in case your indices need to be larger than 16 bits (short), for high polygon counts
            indexBuffer ??= new(Main.graphics.GraphicsDevice, IndexElementSize.SixteenBits, indexCount, BufferUsage.WriteOnly);

            // Create our vertices
            vertices ??= new VertexPositionColorTexture[vertexCount];

            for (int y = 0; y < h; y++)
            {
                for (int x = 0; x < w; x++)
                {
                    float xQuotient = x / ((float)w - 1);
                    float yQuotient = y / ((float)h - 1);

                    Vector2 worldPosition = Position + new Vector2(xQuotient * width, yQuotient * height);
                    Vector2 screenPosition = worldPosition - Main.screenPosition;
                    Vector2 uv = new(xQuotient, yQuotient);
                    //Color lighting = Color.White;
                    Color lighting = new(Lighting.GetSubLight(worldPosition));

                    vertices[(y * w) + x] = new VertexPositionColorTexture(new Vector3(screenPosition, 0f), lighting, uv);
                }
            }

            // Create our indices
            indices ??= new short[indexCount];
            int index = 0;

            for (int y = 0; y < h - 1; y++)
            {
                for (int x = 0; x < w - 1; x++)
                {
                    // I know the math here is very weird, but essentially we're iterating
                    // over each quad and following the primitive pattern below

                    // Our vertices are a one dimensional array
                    // Each row is consecutive starting from the top left

                    // Example, starting at quad 0, top left corner
                    // The first vertex is at index 0, the second at index 1
                    // The third will be in the next row, which is `1 + width`, where width in our case is 3, so 4
                    // The fourth will be 0
                    // The fifth will be `1 + width`, or `1 + 3` = 4 for us
                    // THe sixth will be at `width`, or `3` for us

                    // 0 --- 1
                    // |  \  |
                    // 3 --- 4

                    // A --- B
                    // |  \  |
                    // D --- C

                    // 0 > 1 > 4
                    // A > B > C
                    indices[index++] = (short)(y * w + x);
                    indices[index++] = (short)(y * w + x + 1);
                    indices[index++] = (short)((y + 1) * w + x + 1);

                    // 0 > 4 > 3
                    // A > C > D
                    indices[index++] = (short)(y * w + x);
                    indices[index++] = (short)((y + 1) * w + x + 1);
                    indices[index++] = (short)((y + 1) * w + x);

                    // These numbers will change depending on the state of the loop
                    // We repeat this process with all quads
                }
            }

            vertexBuffer.SetData(vertices, SetDataOptions.Discard);
            indexBuffer.SetData(indices);
        }

        private void PresentLightingBuffers(int numVertices, int primitiveCount)
        {
            // Store previous settings
            var scissorRectangle = PrimitivesSystem.GraphicsDevice.ScissorRectangle;
            var rasterizerState = PrimitivesSystem.GraphicsDevice.RasterizerState;
            var previousVertices = PrimitivesSystem.GraphicsDevice.GetVertexBuffers();
            var previousIndices = PrimitivesSystem.GraphicsDevice.Indices;

            // Assign our own buffers to the GPU
            PrimitivesSystem.GraphicsDevice.SetVertexBuffer(vertexBuffer);
            PrimitivesSystem.GraphicsDevice.Indices = indexBuffer;
            PrimitivesSystem.GraphicsDevice.Textures[0] = renderTarget;

            // Apply our effect and send our draw call to the GPU
            meshEffect.CurrentTechnique.Passes[0].Apply();
            PrimitivesSystem.GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, numVertices, 0, primitiveCount);
            //DebugVertices();

            // Reset our settings back to the previosu ones
            PrimitivesSystem.GraphicsDevice.ScissorRectangle = scissorRectangle;
            PrimitivesSystem.GraphicsDevice.RasterizerState = rasterizerState;
            PrimitivesSystem.GraphicsDevice.SetVertexBuffers(previousVertices);
            PrimitivesSystem.GraphicsDevice.Indices = previousIndices;

            // Reset texture register
            PrimitivesSystem.GraphicsDevice.Textures[0] = null;
        }

        private void DebugVertices()
        {
            // Vertex debug
            bool beginCalled = Main.spriteBatch.BeginCalled();

            if (!beginCalled)
                Main.spriteBatch.Begin();

            var tex = ModContent.Request<Texture2D>(Macrocosm.TextureEffectsPath + "Circle5", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
            foreach (VertexPositionColorTexture vertex in vertices)
                Main.spriteBatch.Draw(tex, new Vector2(vertex.Position.X, vertex.Position.Y), tex.Bounds, new Color(1f, 1f, 1f, 0f), 0f, Vector2.Zero, 0.01f, SpriteEffects.None, 0f);

            if (!beginCalled)
                Main.spriteBatch.End();
        }

        // Events
        private void OnResolutionChanged(Matrix matrix)
        {

        }
    }
}