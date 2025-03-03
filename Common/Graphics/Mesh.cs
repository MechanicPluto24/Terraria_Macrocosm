using Macrocosm.Common.DataStructures;
using Macrocosm.Common.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace Macrocosm.Common.Graphics
{
    // TODO: add RT export and shader chaining support
    public class Mesh : IDisposable
    {
        private readonly GraphicsDevice graphicsDevice;
        private readonly Asset<Effect> effect;

        private DynamicVertexBuffer vertexBuffer;
        private DynamicIndexBuffer indexBuffer;

        private VertexPositionColorTexture[] vertices;
        private short[] indices;

        private CullMode cullMode = CullMode.None;

        public Mesh(GraphicsDevice graphicsDevice)
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

        public void CreateRectangle(Vector2 position, float width, float height, int horizontalResolution, int verticalResolution, float rotation, Vector2 origin, Func<Vector2, Color> colorFunction = null)
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

                    Vector2 vertexPosition = new(position.X + u * width, position.Y + v * height);
                    Vector2 rotatedPosition = (vertexPosition - origin).RotatedBy(rotation) + origin;

                    Color vertexColor = colorFunction?.Invoke(rotatedPosition) ?? Color.White;

                    vertices[i * verticalResolution + j] = new VertexPositionColorTexture(
                        position: new Vector3(rotatedPosition, 0f),
                        color: vertexColor,
                        textureCoordinate: new Vector2(u, v)
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

            UpdateBuffers();
        }

        public enum SphereProjectionType { Mercator, Cylindrical }
        public void CreateSphere(Vector2 position, float radius, int horizontalResolution, int verticalResolution, float depthFactor, Vector3 rotation, Func<Vector2, Color> colorFunction = null, SphereProjectionType projectionType = SphereProjectionType.Mercator)
        {
            if (horizontalResolution < 2 || verticalResolution < 2)
                throw new ArgumentOutOfRangeException(nameof(horizontalResolution), "Resolution must be at least 2.");

            int vertexCount = horizontalResolution * verticalResolution;
            int indexCount = (horizontalResolution - 1) * (verticalResolution - 1) * 6;

            if (vertices == null || vertices.Length != vertexCount)
                vertices = new VertexPositionColorTexture[vertexCount];

            if (indices == null || indices.Length != indexCount)
                indices = new short[indexCount];

            Matrix rotationMatrix = Matrix.CreateRotationX(rotation.X) * Matrix.CreateRotationY(rotation.Y) * Matrix.CreateRotationZ(rotation.Z);
            for (int i = 0; i < horizontalResolution; i++)
            {
                for (int j = 0; j < verticalResolution; j++)
                {
                    float u = i / (float)(horizontalResolution - 1);
                    float v = j / (float)(verticalResolution - 1);

                    Vector3 spherePosition = default;
                    switch (projectionType)
                    {
                        case SphereProjectionType.Mercator:
                            {
                                float longitude = MathHelper.TwoPi * u;
                                float latitude = MathHelper.Pi * (v - 0.5f);
                                spherePosition = new(
                                    x: radius * MathF.Cos(latitude) * MathF.Cos(longitude),
                                    y: radius * MathF.Sin(latitude),
                                    z: radius * MathF.Cos(latitude) * MathF.Sin(longitude)
                                );
                                break;
                            }
                        case SphereProjectionType.Cylindrical:
                            {
                                //TODO
                                break;
                            }
                        }
                    spherePosition = Vector3.Transform(spherePosition, rotationMatrix);

                    float depth = 1f / (depthFactor - spherePosition.Z / radius);
                    Vector2 projectedPosition = new Vector2(spherePosition.X * depth, spherePosition.Y * depth) + position;
                    Color vertexColor = colorFunction?.Invoke(projectedPosition) ?? Color.White;

                    vertices[i * verticalResolution + j] = new VertexPositionColorTexture(
                        position: new Vector3(projectedPosition, 0),
                        color: vertexColor,
                        textureCoordinate: new(1 - u, v)
                    );
                }
            }

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

            cullMode = CullMode.CullCounterClockwiseFace;
            UpdateBuffers();
        }

        /// <summary> Updates the vertex and index buffers on the GPU, resizing if needed. </summary>
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

        public void Draw(Texture2D texture, Matrix transformMatrix, Rectangle? sourceRect = null, BlendState blendState = null, SamplerState samplerState = null)
        {
            if (vertices == null || indices == null)
                return;

            Effect shader = effect.Value;
            var oldBlendState = graphicsDevice.BlendState;
            var oldSamplerState = graphicsDevice.SamplerStates[0];

            graphicsDevice.BlendState = blendState ?? BlendState.AlphaBlend;
            graphicsDevice.SamplerStates[0] = samplerState ?? SamplerState.LinearClamp;
            graphicsDevice.RasterizerState = new RasterizerState
            {
                CullMode = cullMode,
                ScissorTestEnable = true
            };

            graphicsDevice.Textures[0] = texture;
            graphicsDevice.SetVertexBuffer(vertexBuffer);
            graphicsDevice.Indices = indexBuffer;

            Matrix worldViewProjection = Matrix.CreateOrthographicOffCenter(0, graphicsDevice.Viewport.Width, graphicsDevice.Viewport.Height, 0, 0, 1);
            Matrix matrix = transformMatrix * worldViewProjection;
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


        private SpriteBatchState state;
        public void DebugDraw() => DebugDraw(Main.GameViewMatrix.ZoomMatrix);
        public void DebugDraw(Matrix matrix, Color? color = null, Color? backFaceColor = null)
        {
            bool beginCalled = Main.spriteBatch.BeginCalled();

            if (beginCalled)
            {
                state.SaveState(Main.spriteBatch);
                Main.spriteBatch.End();
            }

            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, SamplerState.PointClamp, null, RasterizerState.CullNone, null, matrix);

            var frontColor = color ?? Color.White * 0.5f;
            var backColor = backFaceColor ?? Color.Transparent;

            for (int i = 0; i < indices.Length; i += 3)
            {
                var v1 = vertices[indices[i]].Position;
                var v2 = vertices[indices[i + 1]].Position;
                var v3 = vertices[indices[i + 2]].Position;

                Vector3 edge1 = v2 - v1;
                Vector3 edge2 = v3 - v1;
                Vector3 normal = Vector3.Cross(edge1, edge2);
                Vector3 cameraDirection = new(0, 0, 1);
                bool isBackface = Vector3.Dot(normal, cameraDirection) < 0;
                Color lineColor = isBackface ? backColor : frontColor;

                static void DrawLine(SpriteBatch spriteBatch, Vector3 start, Vector3 end, Color color)
                {
                    Vector2 start2D = new(start.X, start.Y);
                    Vector2 end2D = new(end.X, end.Y);
                    Vector2 edge = end2D - start2D;
                    float angle = (float)Math.Atan2(edge.Y, edge.X);
                    spriteBatch.Draw(TextureAssets.MagicPixel.Value, start2D, new(0, 0, 1, 1), color, angle, Vector2.Zero, new Vector2(edge.Length(), 0.5f), SpriteEffects.None, 0);
                }

                DrawLine(Main.spriteBatch, v1, v2, lineColor);
                DrawLine(Main.spriteBatch, v2, v3, lineColor);
                DrawLine(Main.spriteBatch, v3, v1, lineColor);
            }

            Main.spriteBatch.End();
            if (beginCalled)
                Main.spriteBatch.Begin(state);
        }


        public void Dispose()
        {
            vertexBuffer?.Dispose();
            indexBuffer?.Dispose();
        }
    }
}
