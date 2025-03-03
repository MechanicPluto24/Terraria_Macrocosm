using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System;
using Terraria;
using Macrocosm.Common.Utils;
using static Terraria.GameContent.TextureAssets;
using Macrocosm.Common.DataStructures;
using Macrocosm.Common.Graphics;
using Terraria.Graphics.Effects;

namespace Macrocosm.Common.Drawing
{
    // WIP
    public class Rift
    {
        public Vector2 Position { get; set; }
        public float Width { get; set; }
        public float Height { get; set; }
        public Color InteriorColor { get; set; }
        public Color BorderColor { get; set; }

        private List<Vector2> borderPoints;
        private List<Vector2> lastBorderPoints;
        private Mesh mesh;
        private GraphicsDevice graphicsDevice;

        public Rift(GraphicsDevice graphicsDevice, Vector2 position, float width, float height, Color interiorColor, Color borderColor, int borderPointCount = 64)
        {
            this.graphicsDevice = graphicsDevice;
            Position = position;
            Width = width;
            Height = height;
            InteriorColor = interiorColor;
            BorderColor = borderColor;

            borderPoints = new List<Vector2>();
            mesh = new Mesh(graphicsDevice);

            GenerateBorderPoints(borderPointCount);
            UpdateMesh();
        }

        public void Update()
        {
            GenerateBorderPoints(borderPoints.Count);

            for (int i = 0; i < borderPoints.Count; i++)
                borderPoints[i] = Vector2.Lerp(lastBorderPoints[i], borderPoints[i], 0.01f);

            UpdateMesh();
        }

        private void GenerateBorderPoints(int pointCount)
        {
            lastBorderPoints = new List<Vector2>(borderPoints);
            borderPoints.Clear();

            float angleStep = MathHelper.TwoPi / pointCount;
            float noiseSeedX = Main.rand.NextFloat(0f, 1000f); // Random seed for Perlin noise
            float noiseSeedY = Main.rand.NextFloat(0f, 1000f);

            for (int i = 0; i < pointCount; i++)
            {
                float angle = i * angleStep;

                float radiusX = Width / 2;
                float radiusY = Height / 2;

                float x = Position.X + radiusX * (float)Math.Cos(angle) + Main.rand.NextFloat(-50, 50);
                float y = Position.Y + radiusY * (float)Math.Sin(angle) + Main.rand.NextFloat(-50, 50);

                borderPoints.Add(new Vector2(x, y));
            }

            if (lastBorderPoints.Count != borderPoints.Count)
                lastBorderPoints = new List<Vector2>(borderPoints); // Initialize previous points if needed
        }

        private float PerlinNoise(float x)
        {
            return 0f;
        }

        // TODO: add this to mesh (CreateTriangleFan?)
        private void UpdateMesh()
        {
            if (borderPoints.Count < 3)
                return;

            int vertexCount = borderPoints.Count + 1;
            int indexCount = borderPoints.Count * 3;

            VertexPositionColorTexture[] vertices = new VertexPositionColorTexture[vertexCount];
            short[] indices = new short[indexCount];

            vertices[0] = new VertexPositionColorTexture(new Vector3(Position, 0f), InteriorColor, new Vector2(0.5f, 0.5f));
            for (int i = 0; i < borderPoints.Count; i++)
            {
                Vector2 point = borderPoints[i];

                float u = (point.X - (Position.X - Width / 2)) / Width;
                float v = (point.Y - (Position.Y - Height / 2)) / Height;

                vertices[i + 1] = new VertexPositionColorTexture(
                    new Vector3(point, 0f),
                    new Color(u, v, 0f),
                    new Vector2(u, v)
                );
            }

            int index = 0;
            for (int i = 1; i <= borderPoints.Count; i++)
            {
                indices[index++] = 0;
                indices[index++] = (short)i;
                indices[index++] = (short)(i % borderPoints.Count + 1);
            }
            mesh.Create(vertices, indices);
        }


        public void Draw(Texture2D texture, Matrix transformMatrix)
        {
            Update();

            DrawRT();
            mesh.Draw(renderTarget, transformMatrix);
            renderTarget.Dispose();
            for (int i = 0; i < borderPoints.Count; i++)
            {
                Vector2 start = borderPoints[i];
                Vector2 end = borderPoints[(i + 1) % borderPoints.Count];

                DrawLine(Main.spriteBatch, start, end, BorderColor, 1f);
            }
            mesh.DebugDraw();
        }

        private SpriteBatchState state;
        private RenderTarget2D renderTarget;
        public void DrawRT()
        {
            var spriteBatch = Main.spriteBatch;

            // Store previous settings
            var scissorRectangle = spriteBatch.GraphicsDevice.ScissorRectangle;
            var rasterizerState = spriteBatch.GraphicsDevice.RasterizerState;

            // Capture original RenderTargets and preserve their contents
            spriteBatch.GraphicsDevice.PresentationParameters.RenderTargetUsage = RenderTargetUsage.PreserveContents;
            RenderTargetBinding[] originalRenderTargets = spriteBatch.GraphicsDevice.GetRenderTargets();
            foreach (var binding in originalRenderTargets)
                typeof(RenderTarget2D).SetPropertyValue("RenderTargetUsage", RenderTargetUsage.PreserveContents, binding.RenderTarget);

            state = spriteBatch.SaveState();
            spriteBatch.EndIfBeginCalled();

            renderTarget = new(Main.spriteBatch.GraphicsDevice, 1000, 1000, mipMap: false, SurfaceFormat.Color, DepthFormat.None, 0, RenderTargetUsage.PreserveContents);
            spriteBatch.GraphicsDevice.SetRenderTarget(renderTarget);
            spriteBatch.GraphicsDevice.Clear(Color.Transparent);

            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, null, state.RasterizerState, state.Effect, Matrix.CreateScale(1f));

            spriteBatch.Draw(
                Main.Assets.Request<Texture2D>("Images/Misc/noise").Value,
                new Rectangle(0, 0, renderTarget.Width, renderTarget.Height),
                Color.White
            );

            spriteBatch.End();
            spriteBatch.Begin(state);

            // Revert our RenderTargets back to the vanilla ones
            if (originalRenderTargets.Length > 0)
                spriteBatch.GraphicsDevice.SetRenderTargets(originalRenderTargets);
            else
                spriteBatch.GraphicsDevice.SetRenderTarget(null);

            spriteBatch.GraphicsDevice.ScissorRectangle = scissorRectangle;
            spriteBatch.GraphicsDevice.RasterizerState = rasterizerState;
        }

        private void DrawLine(SpriteBatch spriteBatch, Vector2 start, Vector2 end, Color color, float thickness)
        {
            Vector2 edge = end - start;
            float angle = (float)Math.Atan2(edge.Y, edge.X);
            float length = edge.Length();

            spriteBatch.Draw(
                MagicPixel.Value,
                start,
                new Rectangle(0, 0, 1, 1),
                color,
                angle,
                new Vector2(0, 0.5f),
                new Vector2(length, thickness),
                SpriteEffects.None,
                0f
            );
        }
    }
}