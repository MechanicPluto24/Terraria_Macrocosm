using Macrocosm.Common.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent.UI.Elements;


namespace Macrocosm.Content.Rockets.Navigation
{
    public class UIRocketPreviewLarge : UIPanel
    {
        public Rocket RocketDummy = new();

		public string CurrentModuleName { get; set; } = "CommandPod";

		public int CurrentModuleIndex 
		{
			get => RocketDummy.ModuleNames.IndexOf(CurrentModuleName);
			set => CurrentModuleName = RocketDummy.ModuleNames[value];
		}

		public bool AnimationActive => lastModuleIndex != CurrentModuleIndex;

		private int lastModuleIndex;
		private float animationCounter;
		private float moduleOffsetX;
		private float moduleOffsetY;
		private float zoom;

		private float[] moduleZooms = { 0.35f, 0.35f, 0.35f, 0.55f, 0.52f, 0.52f };
		private float[] moduleOffsetsX =  { -220f, -220f, -220f, -80f, 40f, -250f };
		private float[] moduleOffsetsY = { 140f, -40f, -320f, -460f, -520f, -520f };

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
			lastModuleIndex = CurrentModuleIndex;
			CurrentModuleName = moduleName;
		}

		public void UpdateModule(int moduleIndex)
		{
			lastModuleIndex = CurrentModuleIndex;
			CurrentModuleIndex = moduleIndex;
		}

		public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

			if (animationCounter >= 1f)
			{
				animationCounter = 0f;
				lastModuleIndex = CurrentModuleIndex;
			}

			if(AnimationActive)
			{
				animationCounter += 0.05f;
			}

			float animation = Utility.QuadraticEaseInOut(animationCounter);
			moduleOffsetX = MathHelper.Lerp(moduleOffsetsX[lastModuleIndex], moduleOffsetsX[CurrentModuleIndex], animation);
			moduleOffsetY = MathHelper.Lerp(moduleOffsetsY[lastModuleIndex], moduleOffsetsY[CurrentModuleIndex], animation);
			zoom = MathHelper.Lerp(moduleZooms[lastModuleIndex], moduleZooms[CurrentModuleIndex], animation);
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

            RocketDummy.DrawDummy(spriteBatch, (GetDimensions().Position() + new Vector2(moduleOffsetX, moduleOffsetY)) * zoom, Color.White);

			spriteBatch.End();
			spriteBatch.Begin(state);
		}
	}
}
