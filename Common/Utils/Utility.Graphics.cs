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
        public static (VertexPositionColorTexture[] vertices, short[] indices) CreateRectangularQuadMesh(Vector2 position, float width, float height, int countX, int countY, Func<Vector2, Color> colorFunction, bool debug = false)
        {
            VertexPositionColorTexture[] vertices = new VertexPositionColorTexture[(countX + 1) * (countY + 1)];
            short[] indices = new short[countX * countY * 6];

            float horizontalStep = width / countX;
            float verticalStep = height / countY;

            for (int y = 0; y <= countY; y++)
            {
                for (int x = 0; x <= countX; x++)
                {
					Vector2 samplePosition = position + new Vector2(x * horizontalStep, y * verticalStep);
					vertices[y * (countX + 1) + x] = new VertexPositionColorTexture()
					{
						Position = new Vector3(samplePosition, 0f),
						TextureCoordinate = new Vector2(x / (float)countX, y / (float)countY),
						Color = colorFunction(samplePosition)
                    };
                }
            }

            int index = 0;
            for (int y = 0; y < countY; y++)
            {
                for (int x = 0; x < countX; x++)
                {
                    indices[index++] = (short)(y * countX + x);
                    indices[index++] = (short)(y * countX + x + 1);
                    indices[index++] = (short)((y + 1) * countX + x + 1);

                    indices[index++] = (short)(y * countX + x);
                    indices[index++] = (short)((y + 1) * countX + x + 1);
                    indices[index++] = (short)((y + 1) * countX + x);
                }
            }

			if (debug)
			{
				bool beginCalled = Main.spriteBatch.BeginCalled();

				if(!beginCalled) 
					Main.spriteBatch.Begin();

                var tex = ModContent.Request<Texture2D>(Macrocosm.TextureAssetsPath + "Circle5", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
                foreach (VertexPositionColorTexture vertex in vertices)
                 	Main.spriteBatch.Draw(tex, new Vector2(vertex.Position.X, vertex.Position.Y), tex.Bounds, new Color(1f, 1f, 1f, 0f), 0f, Vector2.Zero, 0.01f, SpriteEffects.None, 0f);
       			
				if(!beginCalled)
					Main.spriteBatch.End();
            }

            return (vertices, indices);
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
					RasterizerState.CullCounterClockwise,
					null,
					Main.GameViewMatrix.TransformationMatrix
				);
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