using Macrocosm.Common.DataStructures;
using Macrocosm.Common.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.GameContent;

namespace Macrocosm.Common.Graphics;

public class Mesh : IDisposable
{
    private DynamicVertexBuffer vertexBuffer;
    private DynamicIndexBuffer indexBuffer;

    private VertexPositionColorTexture[] vertices;
    private short[] indices;

    private CullMode cullMode = CullMode.None;

    private RenderTarget2D renderTarget;

    public GraphicsDevice GraphicsDevice => Main.graphics.GraphicsDevice;

    public Mesh()
    {
    }

    #region Create methods
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
    #endregion

    #region Render
    public void Draw(Texture2D texture, Matrix transformMatrix, Rectangle? sourceRect = null, BlendState blendState = null, SamplerState samplerState = null, bool scissorTestEnable = false)
    {
        if (vertices == null || indices == null)
            return;

        GraphicsDevice.BlendState = blendState ?? BlendState.AlphaBlend;
        GraphicsDevice.SamplerStates[0] = samplerState ?? SamplerState.LinearClamp;
        GraphicsDevice.RasterizerState = new RasterizerState
        {
            CullMode = cullMode,
            ScissorTestEnable = scissorTestEnable
        };
        GraphicsDevice.SetVertexBuffer(vertexBuffer);
        GraphicsDevice.Indices = indexBuffer;
        GraphicsDevice.Textures[0] = texture;

        Effect meshShader = Macrocosm.GetShader("Mesh");
        Matrix worldViewProjection = Matrix.CreateOrthographicOffCenter(0, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height, 0, 0, 1);
        Matrix matrix = transformMatrix * worldViewProjection;
        meshShader.Parameters["uTransformMatrix"].SetValue(matrix);

        Vector4 rectV = sourceRect is Rectangle r ? new(r.X / (float)texture.Width, r.Y / (float)texture.Height, r.Width / (float)texture.Width, r.Height / (float)texture.Height) : new(0, 0, 1, 1);
        meshShader.Parameters["uSourceRect"].SetValue(rectV);

        foreach (var pass in meshShader.CurrentTechnique.Passes)
        {
            pass.Apply();
            GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, vertices.Length, 0, indices.Length / 3);
        }

        GraphicsDevice.Textures[0] = texture;
    }

    public RenderTarget2D DrawToRenderTarget(Texture2D texture, Matrix transformMatrix, Rectangle? sourceRect = null, BlendState blendState = null, SamplerState samplerState = null, bool scissorTestEnable = false)
    {
        if (vertices == null || indices == null || vertices.Length == 0)
            return null;

        List<Vector3> projectedPoints = vertices.Select(v => Vector3.Transform(v.Position, transformMatrix)).ToList();
        float minX = projectedPoints.Min(v => v.X);
        float minY = projectedPoints.Min(v => v.Y);
        float maxX = projectedPoints.Max(v => v.X);
        float maxY = projectedPoints.Max(v => v.Y);

        // Calculate RT size
        int width = Math.Max(1, (int)Math.Ceiling(maxX - minX));
        int height = Math.Max(1, (int)Math.Ceiling(maxY - minY));

        // Create and bind RT if needed
        if (renderTarget == null || renderTarget.Width != width || renderTarget.Height != height || renderTarget.IsDisposed)
        {
            renderTarget?.Dispose();
            renderTarget = new RenderTarget2D(GraphicsDevice, width, height);
        }

        var renders = GraphicsDevice.SaveRenderTargets();
        Viewport originalViewport = GraphicsDevice.Viewport;
        GraphicsDevice.SetRenderTarget(renderTarget);
        GraphicsDevice.Viewport = new Viewport(0, 0, width, height);
        GraphicsDevice.Clear(Color.Transparent);

        // Translate and draw to RT
        Matrix traslatedMatrix = Matrix.CreateTranslation(-minX, -minY, 0) * transformMatrix;
        Draw(texture, traslatedMatrix, sourceRect, blendState, samplerState, scissorTestEnable);

        // Restore state
        GraphicsDevice.SetRenderTargets(renders);
        GraphicsDevice.Viewport = originalViewport;

        return renderTarget;
    }

    #endregion

    #region Debug
    private SpriteBatchState _state;
    public void DebugDraw() => DebugDraw(Main.GameViewMatrix.ZoomMatrix);
    public void DebugDraw(Matrix matrix, Color? color = null, Color? backFaceColor = null)
    {
        bool beginCalled = Main.spriteBatch.BeginCalled();

        if (beginCalled)
        {
            _state.SaveState(Main.spriteBatch);
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
            Main.spriteBatch.Begin(_state);
    }
    #endregion

    /// <summary> Updates the vertex and index buffers on the GPU, resizing if needed. </summary>
    private void UpdateBuffers()
    {
        if (vertexBuffer == null || vertexBuffer.VertexCount != vertices.Length)
        {
            vertexBuffer?.Dispose();
            vertexBuffer = new DynamicVertexBuffer(GraphicsDevice, typeof(VertexPositionColorTexture), vertices.Length, BufferUsage.WriteOnly);
        }

        if (indexBuffer == null || indexBuffer.IndexCount != indices.Length)
        {
            indexBuffer?.Dispose();
            indexBuffer = new DynamicIndexBuffer(GraphicsDevice, IndexElementSize.SixteenBits, indices.Length, BufferUsage.WriteOnly);
        }

        vertexBuffer.SetData(vertices);
        indexBuffer.SetData(indices);
    }

    public void Dispose()
    {
        vertexBuffer?.Dispose();
        indexBuffer?.Dispose();
        renderTarget?.Dispose();
    }
}
