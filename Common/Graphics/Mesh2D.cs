using Macrocosm.Common.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using Terraria;
using Terraria.ModLoader;

namespace Macrocosm.Common.Graphics
{
    public class Mesh2D : IDisposable
    {
        private readonly GraphicsDevice graphicsDevice;
        private readonly Asset<Effect> effect;

        private DynamicVertexBuffer vertexBuffer;
        private DynamicIndexBuffer indexBuffer;

        private VertexPositionColorTexture[] vertices;
        private short[] indices;

        public Mesh2D(GraphicsDevice graphicsDevice)
        {
            this.graphicsDevice = graphicsDevice;
            effect = ModContent.Request<Effect>(Macrocosm.ShadersPath + "Mesh", AssetRequestMode.ImmediateLoad);
        }

        public void Create(VertexPositionColorTexture[] vertices, short[] indices)
        {
            this.vertices = vertices;
            this.indices = indices;
            UpdateBuffers();
        }

        /// <summary>
        /// Creates or updates the mesh with rectangle geometry.
        /// Resizes buffers only if necessary.
        /// </summary>
        public void CreateRectangle(Vector2 origin, float width, float height, int horizontalResolution, int verticalResolution, Func<Vector2, Color> colorFunction = null)
        {
            if (horizontalResolution < 2 || verticalResolution < 2)
                throw new ArgumentOutOfRangeException(nameof(horizontalResolution), "Resolution must be at least 2.");

            int vertexCount = horizontalResolution * verticalResolution;
            int indexCount = (horizontalResolution - 1) * (verticalResolution - 1) * 6;

            // Resize vertex and index arrays if needed
            if (vertices == null || vertices.Length != vertexCount)
                vertices = new VertexPositionColorTexture[vertexCount];

            if (indices == null || indices.Length != indexCount)
                indices = new short[indexCount];

            // Populate vertices
            for (int i = 0; i < horizontalResolution; i++)
            {
                for (int j = 0; j < verticalResolution; j++)
                {
                    float u = i / (float)(horizontalResolution - 1);
                    float v = j / (float)(verticalResolution - 1);

                    Vector2 vertexPosition = new(
                        origin.X + u * width,
                        origin.Y + v * height
                    );

                    Color vertexColor = colorFunction?.Invoke(vertexPosition) ?? Color.White;

                    vertices[i * verticalResolution + j] = new VertexPositionColorTexture(
                        new Vector3(vertexPosition, 0f),
                        vertexColor,
                        new Vector2(u, v)
                    );
                }
            }

            // Populate indices
            int index = 0;
            for (int i = 0; i < horizontalResolution - 1; i++)
            {
                for (int j = 0; j < verticalResolution - 1; j++)
                {
                    indices[index++] = (short)(i * verticalResolution + j);
                    indices[index++] = (short)(i * verticalResolution + j + 1);
                    indices[index++] = (short)((i + 1) * verticalResolution + j + 1);

                    indices[index++] = (short)(i * verticalResolution + j);
                    indices[index++] = (short)((i + 1) * verticalResolution + j + 1);
                    indices[index++] = (short)((i + 1) * verticalResolution + j);
                }
            }

            // Update GPU buffers
            UpdateBuffers();
        }

        /// <summary>
        /// Updates the vertex and index buffers on the GPU, resizing if needed.
        /// </summary>
        private void UpdateBuffers()
        {
            if (vertexBuffer == null || vertexBuffer.VertexCount != vertices.Length)
            {
                vertexBuffer?.Dispose();
                vertexBuffer = new DynamicVertexBuffer(graphicsDevice, typeof(VertexPositionColorTexture), vertices.Length, BufferUsage.WriteOnly);
            }

            if (indexBuffer == null || indexBuffer.IndexCount != indices.Length)
            {
                indexBuffer?.Dispose();
                indexBuffer = new DynamicIndexBuffer(graphicsDevice, IndexElementSize.SixteenBits, indices.Length, BufferUsage.WriteOnly);
            }

            vertexBuffer.SetData(vertices);
            indexBuffer.SetData(indices);
        }

        public void Draw(Texture2D texture, Matrix transformMatrix, Rectangle? sourceRect = null, float rotation = 0f, Vector2 origin = default, BlendState blendState = null, SamplerState samplerState = null)
        {
            Effect shader = effect.Value;
            var oldBlendState = graphicsDevice.BlendState;
            var oldSamplerState = graphicsDevice.SamplerStates[0];

            graphicsDevice.BlendState = blendState ?? BlendState.AlphaBlend;
            graphicsDevice.SamplerStates[0] = samplerState ?? SamplerState.LinearClamp;
            graphicsDevice.RasterizerState = new RasterizerState
            {
                CullMode = CullMode.None,
                ScissorTestEnable = true
            };

            graphicsDevice.Textures[0] = texture;
            graphicsDevice.SetVertexBuffer(vertexBuffer);
            graphicsDevice.Indices = indexBuffer;

            Matrix rotationMatrix =  Matrix.CreateTranslation(-origin.X, -origin.Y, 0)  * Matrix.CreateRotationZ(rotation) * Matrix.CreateTranslation(origin.X, origin.Y, 0);
            Matrix worldViewProjection = Matrix.CreateOrthographicOffCenter(0, graphicsDevice.Viewport.Width, graphicsDevice.Viewport.Height, 0, 0, 1);
            Matrix matrix = rotationMatrix * transformMatrix * worldViewProjection;
            shader.Parameters["uTransformMatrix"].SetValue(matrix);

            Vector4 sourceRectV4 = sourceRect is Rectangle source
                ? new Vector4(source.X / (float)texture.Width, source.Y / (float)texture.Height, source.Width / (float)texture.Width, source.Height / (float)texture.Height)
                : new Vector4(0, 0, 1, 1);
            shader.Parameters["uSourceRect"].SetValue(sourceRectV4);

            foreach (var pass in shader.CurrentTechnique.Passes)
            {
                pass.Apply();
                graphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, vertices.Length, 0, indices.Length / 3);
            }

            graphicsDevice.BlendState = oldBlendState;
            graphicsDevice.SamplerStates[0] = oldSamplerState;
        }

        private Matrix GetScreenProjectedMatrix(Matrix transformMatrix)
        {
            return default;
        }

        public void DebugDraw()
        {
            bool beginCalled = Main.spriteBatch.BeginCalled();

            if (!beginCalled)
                Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, null, RasterizerState.CullNone, null, Main.GameViewMatrix.ZoomMatrix);

            var tex = ModContent.Request<Texture2D>(Macrocosm.TextureEffectsPath + "Circle5", AssetRequestMode.ImmediateLoad).Value;
            foreach (VertexPositionColorTexture vertex in vertices)
                Main.spriteBatch.Draw(tex, new Vector2(vertex.Position.X, vertex.Position.Y), null, new Color(1f, 1f, 1f, 0f), 0f, tex.Size() / 2f, 0.01f, SpriteEffects.None, 0f);

            if (!beginCalled)
                Main.spriteBatch.End();
        }

        public void Dispose()
        {
            vertexBuffer?.Dispose();
            indexBuffer?.Dispose();
        }
    }
}
