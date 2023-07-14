using Macrocosm.Common.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.UI.Elements;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.UI;

namespace Macrocosm.Content.Rockets.Navigation
{
    public class UIRocketPreviewLarge : UIPanel
    {
        public Rocket Rocket;

		private int currentModule;
		private int lastModule;
		private float animationCounter;
		private float moduleOffsetX;
		private float moduleOffsetY;
		private float zoom;

		private float[] moduleZooms = { 0.35f, 0.35f, 0.35f, 0.55f, 0.52f, 0.52f };
		private float[] moduleOffsetsX = { 0f, 0f, 0f, 40f, 118f, -51f };
		private float[] moduleOffsetsY = { 140f, -40f, -320f, -440f, -520f, -520f };

		public UIRocketPreviewLarge()
		{
		}

		public override void OnInitialize()
        {
            Width.Set(0, 1f);
            Height.Set(0, 1f);
            BackgroundColor = new Color(53, 72, 135);
			BorderColor = Color.Transparent;	
        }

		public void UpdateModule(string moduleName)
		{
			lastModule = currentModule;
			currentModule = Rocket.ModuleNames.IndexOf(moduleName);
		}

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

			if(animationCounter >= 1f)
			{
				animationCounter = 0f;
				lastModule = currentModule;
			}

			if(lastModule != currentModule)
			{
				animationCounter += 0.05f;
			}

			float animation = Utility.QuadraticEaseInOut(animationCounter);
			moduleOffsetX = MathHelper.Lerp(moduleOffsetsX[lastModule], moduleOffsetsX[currentModule], animation);
			moduleOffsetY = MathHelper.Lerp(moduleOffsetsY[lastModule], moduleOffsetsY[currentModule], animation);
			zoom = MathHelper.Lerp(moduleZooms[lastModule], moduleZooms[currentModule], animation);
		}

		public override void Draw(SpriteBatch spriteBatch)
		{
            base.Draw(spriteBatch);

			var overflowHiddenRasterizerState = new RasterizerState
			{
				CullMode = CullMode.None,
				ScissorTestEnable = true
			};

			Matrix matrix = Matrix.CreateScale(Main.UIScale / zoom, Main.UIScale / zoom, 1f);

            var state = spriteBatch.SaveState();
            spriteBatch.End();
            spriteBatch.Begin(state.SpriteSortMode, BlendState.AlphaBlend, SamplerState.AnisotropicClamp, state.DepthStencilState, overflowHiddenRasterizerState, state.Effect, matrix);

            Rocket.DrawDummy(spriteBatch, (GetDimensions().Position() + new Vector2(moduleOffsetX, moduleOffsetY)) * zoom, Color.White);

			spriteBatch.End();
			spriteBatch.Begin(state);
		}

		public override Rectangle GetViewCullingArea()
		{
			return base.GetViewCullingArea();
		}
	}
}
