using Macrocosm.Common.DataStructures;
using Macrocosm.Common.Drawing;
using Macrocosm.Common.Drawing.Particles;
using Macrocosm.Common.Graphics;
using Macrocosm.Common.Netcode;
using Macrocosm.Common.Subworlds;
using Macrocosm.Common.UI.Themes;
using Macrocosm.Common.Utils;
using Macrocosm.Content.Items.CursorIcons;
using Macrocosm.Content.Particles;
using Macrocosm.Content.Players;
using Macrocosm.Content.Rockets.Customization;
using Macrocosm.Content.Rockets.LaunchPads;
using Macrocosm.Content.Rockets.Modules;
using Macrocosm.Content.Rockets.Storage;
using Macrocosm.Content.Rockets.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using static Macrocosm.Content.Tiles.Furniture.MoonBase.MoonBaseChest;
using Terraria.ModLoader;
using Macrocosm.Common.Graphics;

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
		private SpriteBatchState state;
		private DynamicVertexBuffer vertexBuffer;
		private DynamicIndexBuffer indexBuffer;

		public void ResetRenderTarget() => renderTarget?.Dispose();

		/// <summary> Draw the rocket to a RenderTarget and then in the world </summary>
		public void Draw(DrawMode drawMode, SpriteBatch spriteBatch, Vector2 position)
		{
			state.SaveState(spriteBatch);
			renderTarget = GetOrPrepareRenderTarget(drawMode);

			spriteBatch.EndIfBeginCalled();

			Main.NewText(drawMode.ToString());
			if (drawMode is DrawMode.Dummy)
			{
				spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, state.DepthStencilState, state.RasterizerState, state.Effect, state.Matrix);
				spriteBatch.Draw(renderTarget, position, Color.White);
				spriteBatch.End();
			}

			spriteBatch.Begin(state);
		}

		public void PreDrawBeforeTiles(SpriteBatch spriteBatch, Vector2 position)
		{
			foreach (RocketModule module in ModulesByDrawPriority)
			{
				module.PreDrawBeforeTiles(spriteBatch, GetModuleRelativePosition(module, position));
			}
		}

		public void DrawOverlay(SpriteBatch spriteBatch, Vector2 position)
		{
			if (InFlight || ForcedFlightAppearance)
			{
				float scale = 1.2f * Main.rand.NextFloat(0.85f, 1f);
				if (FlightProgress < 0.1f)
					scale *= Utility.QuadraticEaseOut(FlightProgress * 10f);

				if (renderTarget is null || renderTarget.IsDisposed)
				{
					var scissorRectangle = PrimitivesSystem.GraphicsDevice.ScissorRectangle;
					var rasterizerState = PrimitivesSystem.GraphicsDevice.RasterizerState;
					RenderTargetBinding[] originalRenderTargets = spriteBatch.GraphicsDevice.GetRenderTargets();

					foreach (var binding in originalRenderTargets)
						typeof(RenderTarget2D).SetPropertyValue("RenderTargetUsage", RenderTargetUsage.PreserveContents, binding.RenderTarget);
				}
			}
		}

		public RenderTarget2D GetOrPrepareRenderTarget(DrawMode drawMode)
		{
			SpriteBatch spriteBatch = Main.spriteBatch;

			PrepareEffect(drawMode);
			FillRenderTarget(spriteBatch, drawMode);

			// DrawMode.Dummy does not consume the vertex mesh
			if (drawMode is DrawMode.Dummy)
			{
				return renderTarget;
			}

			PrepareLightingBuffers(Width, Height, out int numVertices, out int primitiveCount);
			PresentBuffers(numVertices, primitiveCount);

			return renderTarget;
		}

		// Draw types
		private void DrawDirect(SpriteBatch spriteBatch, Vector2 position)
		{
			foreach (RocketModule module in ModulesByDrawPriority)
			{
				module.Draw(spriteBatch, GetModuleRelativePosition(module, position));
			}
		}

		private void DrawDummy(SpriteBatch spriteBatch, Vector2 position)
		{
			PreDrawBeforeTiles(spriteBatch, position);
			DrawDirect(spriteBatch, position);
			DrawOverlay(spriteBatch, position);
		}

		private void DrawBlueprint(SpriteBatch spriteBatch, Vector2 position)
		{
			foreach (RocketModule module in ModulesByDrawPriority)
			{
				Vector2 drawPosition = GetModuleRelativePosition(module, position);

				if (module is BoosterLeft)
					drawPosition.X -= 78;

				module.DrawBlueprint(spriteBatch, drawPosition);
			}
		}

		// Draw preparation
		private void PrepareEffect(DrawMode drawMode)
		{
			// Guard against effect being null
			if (meshEffect is null || meshEffect.IsDisposed)
			{
				meshEffect = ModContent.Request<Effect>(Macrocosm.EffectAssetsPath + "Mesh", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
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
			RenderTargetBinding[] originalRenderTargets = spriteBatch.GraphicsDevice.GetRenderTargets();
			foreach (var binding in originalRenderTargets)
				typeof(RenderTarget2D).SetPropertyValue("RenderTargetUsage", RenderTargetUsage.PreserveContents, binding.RenderTarget);

			// We only need to prepare our RenderTarget if it's not ready to use
			if (renderTarget is null || renderTarget.IsDisposed)
			{
				// Initialize our RenderTarget
				renderTarget = new(spriteBatch.GraphicsDevice, Bounds.Width, Bounds.Height, mipMap: false, SurfaceFormat.Color, DepthFormat.None, 0, RenderTargetUsage.PreserveContents);
			}

			// Draw our modules
			state = spriteBatch.SaveState();
			spriteBatch.EndIfBeginCalled();

			spriteBatch.GraphicsDevice.SetRenderTarget(renderTarget);
			spriteBatch.GraphicsDevice.Clear(Color.Transparent);

			spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, state.DepthStencilState, state.RasterizerState, state.Effect, Matrix.CreateScale(1f));

			switch (drawMode)
			{
				case DrawMode.World:
					DrawDirect(spriteBatch, default);
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

			// Reset our settings back to the previosu ones
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
			VertexPositionColorTexture[] vertices = new VertexPositionColorTexture[vertexCount];

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

			// Vertex debug
			Main.spriteBatch.EndIfBeginCalled();
			Main.spriteBatch.Begin();

			var tex = ModContent.Request<Texture2D>(Macrocosm.TextureAssetsPath + "Circle5", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;

			foreach (VertexPositionColorTexture vertex in vertices)
			{
				Main.spriteBatch.Draw(tex, new Vector2(vertex.Position.X, vertex.Position.Y), tex.Bounds, new Color(1f, 1f, 1f, 0f), 0f, Vector2.Zero, 0.01f, SpriteEffects.None, 0f);
			}

			Main.spriteBatch.End();

			short[] indices = new short[indexCount];
			int index = 0;

			for (int y = 0; y < h - 1; y++)
			{
				for (int x = 0; x < w - 1; x++)
				{
					// A > B > C
					indices[index++] = (short)(y * w + x);
					indices[index++] = (short)(y * w + x + 1);
					indices[index++] = (short)((y + 1) * w + x + 1);

					// A > C > D
					indices[index++] = (short)(y * w + x);
					indices[index++] = (short)((y + 1) * w + x + 1);
					indices[index++] = (short)((y + 1) * w + x);
				}
			}

			vertexBuffer.SetData(vertices, SetDataOptions.Discard);
			indexBuffer.SetData(indices);
		}

		private void PresentBuffers(int numVertices, int primitiveCount)
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

			// Reset our settings back to the previosu ones
			PrimitivesSystem.GraphicsDevice.ScissorRectangle = scissorRectangle;
			PrimitivesSystem.GraphicsDevice.RasterizerState = rasterizerState;
			PrimitivesSystem.GraphicsDevice.SetVertexBuffers(previousVertices);
			PrimitivesSystem.GraphicsDevice.Indices = previousIndices;

			// Reset texture register
			PrimitivesSystem.GraphicsDevice.Textures[0] = null;
		}

		// Events
		private void OnResolutionChanged(Matrix matrix)
		{

		}
	}
}