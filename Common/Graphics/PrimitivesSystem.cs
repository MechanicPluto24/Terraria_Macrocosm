using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;
using static Basic.Reference.Assemblies.Net60;

namespace Macrocosm.Common.Graphics
{
	public class PrimitivesSystem : ModSystem
	{
		public static GraphicsDevice GraphicsDevice => Main.graphics.GraphicsDevice;

		public static Matrix WorldViewProjection { get; private set; }

		public static BasicEffect WorldEffect { get; private set; }
		public static BasicEffect InterfaceEffect { get; private set; }

		public static event Action<Matrix> OnResolutionChanged;

		private static Matrix oldZoomMatrix;

		public override void Load()
		{
			if (Main.netMode is NetmodeID.Server)
			{
				return;
			}

			Main.OnResolutionChanged += ResolutionChanged;

			Main.QueueMainThreadAction(() =>
			{
				WorldEffect = new(GraphicsDevice)
				{
					VertexColorEnabled = true
				};

				InterfaceEffect = new(GraphicsDevice)
				{
					VertexColorEnabled = true
				};

				ResolutionChanged(Main.ScreenSize.ToVector2());
			});
		}

        public override void PreUpdateEntities()
        {
			if (Main.netMode == NetmodeID.Server)
				return;

			if (oldZoomMatrix != Main.GameViewMatrix.ZoomMatrix)
			{
				oldZoomMatrix = Main.GameViewMatrix.ZoomMatrix;
                ResolutionChanged(Vector2.Zero);
            }
        }

        private void ResolutionChanged(Vector2 size)
		{
			int width = Main.graphics.GraphicsDevice.Viewport.Width;
			int height = Main.graphics.GraphicsDevice.Viewport.Height;
			Vector2 zoom = Main.GameViewMatrix.Zoom;

            Matrix view = Matrix.CreateLookAt(Vector3.Zero, Vector3.UnitZ, Vector3.Up) *
				Matrix.CreateTranslation(width / 2, height / -2, 0) *
				Matrix.CreateRotationZ(MathHelper.Pi) *
				Matrix.CreateScale(zoom.X, zoom.Y, 1f);

			Matrix projection = Matrix.CreateOrthographic(width, height, 0, 1000);

			WorldViewProjection = view * projection;

			WorldEffect.Projection = WorldViewProjection;
			InterfaceEffect.Projection = Matrix.CreateOrthographicOffCenter(0, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height, 0, 0, 1);

			Main.QueueMainThreadAction(() =>
			{
				OnResolutionChanged?.Invoke(WorldViewProjection);
			});
		}
	}
}
