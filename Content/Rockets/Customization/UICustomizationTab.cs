using Macrocosm.Common.UI;
using Macrocosm.Common.Utils;
using Macrocosm.Content.Rockets.Customization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.UI;

namespace Macrocosm.Content.Rockets.Navigation
{
    public class UICustomizationTab : UIPanel
    {
        public Rocket Rocket;
        public Rocket RocketDummy;

		public UIRocketPreviewLarge RocketPreview;
		public UIPanel RocketPreviewBackground;

		public UIRocketModulePicker ModulePicker;
		public UIPanel CustomizationPanelBackground;

		public UINameplateConfig NameplateConfig;
		public UIDetailConfig DetailConfig;
		public UIPatternConfig PatternConfig;

		public UIColorHSLProvider HSLMenu;

		public UICustomizationTab()
		{
		}

		public override void OnInitialize()
        {
			Width.Set(0, 1f);
			Height.Set(0, 1f);
			HAlign = 0.5f;
			VAlign = 0.5f;

			SetPadding(6f);

			BackgroundColor = new Color(13, 23, 59, 127);
			BorderColor = new Color(15, 15, 15, 255);

			RocketPreviewBackground = new()
			{
				Width = new(0, 0.4f),
				Height = new(0, 1f),
				Left = new (0,0.605f),
				HAlign = 0f,
				BackgroundColor = new Color(53, 72, 135),
				BorderColor = new Color(89, 116, 213, 255)
			};
			RocketPreviewBackground.SetPadding(6f);
			RocketPreviewBackground.OverflowHidden = true;
			Append(RocketPreviewBackground);

			RocketPreview = new();
			RocketPreviewBackground.Append(RocketPreview);
			RocketPreview.Activate();

			CustomizationPanelBackground = new()
			{
				Width = new(0, 0.6f),
				Height = new(0, 1f),
				HAlign = 0f,
				BackgroundColor = new Color(53, 72, 135),
				BorderColor = new Color(89, 116, 213, 255)
			};
			CustomizationPanelBackground.SetPadding(6f);
			Append(CustomizationPanelBackground);

			ModulePicker = new();
			ModulePicker.Activate();
			ModulePicker.LeftButton.OnLeftClick += LeftButton_OnLeftClick;
			ModulePicker.LeftButton.CheckInteractible = () => !RocketPreview.AnimationActive;
			ModulePicker.RightButton.OnLeftClick += RightButton_OnLeftClick;
			ModulePicker.RightButton.CheckInteractible = () => !RocketPreview.AnimationActive;

			CustomizationPanelBackground.Append(ModulePicker);

			NameplateConfig = new()
			{
				OnFocusSet = () => 
				{
					RocketPreview.UpdateModule("EngineModule");
					ModulePicker.CurrentModuleName = "EngineModule";
					HSLMenu.SetColorHSL(RocketDummy.EngineModule.Nameplate.TextColor.ToHSL());
				},

				CheckTextSubmit = () =>
				{
					if (Main.keyState.IsKeyDown(Keys.Enter))
					{
						Rocket.SetName(NameplateConfig.Text);
						Rocket.EngineModule.Nameplate.TextColor = HSLMenu.PendingColor;
						RocketDummy = Rocket.Clone();
						return true;
					}

					return false;
				}
			};

			DetailConfig = new();
			PatternConfig = new();

			NameplateConfig.Activate();
			DetailConfig.Activate();
			PatternConfig.Activate();

			CustomizationPanelBackground.Append(NameplateConfig);
			CustomizationPanelBackground.Append(DetailConfig);
			CustomizationPanelBackground.Append(PatternConfig);

			HSLMenu = new();
			UIPanel hslPanel = HSLMenu.ProvideHSLMenu();
			CustomizationPanelBackground.Append(hslPanel);
			hslPanel.Activate();
			HSLMenu.SetColorSetEvent(HSLMenu_OnSliderClick);
		}

		private void LeftButton_OnLeftClick(Terraria.UI.UIMouseEvent evt, Terraria.UI.UIElement listeningElement)
		{
			if (RocketPreview.AnimationActive)
				return;

			if (RocketPreview.CurrentModuleIndex == 0)
				RocketPreview.UpdateModule(Rocket.Modules.Count - 1);
			else
				RocketPreview.UpdateModule(RocketPreview.CurrentModuleIndex - 1);

			ModulePicker.CurrentModuleName = RocketPreview.CurrentModuleName;

		}
		private void RightButton_OnLeftClick(Terraria.UI.UIMouseEvent evt, Terraria.UI.UIElement listeningElement)
		{
			if (RocketPreview.AnimationActive)
				return;

			if (RocketPreview.CurrentModuleIndex == Rocket.Modules.Count - 1)
				RocketPreview.UpdateModule(0);
			else
				RocketPreview.UpdateModule(RocketPreview.CurrentModuleIndex + 1);

			ModulePicker.CurrentModuleName = RocketPreview.CurrentModuleName;
		}

		private void HSLMenu_OnSliderClick(Terraria.UI.UIMouseEvent evt, Terraria.UI.UIElement listeningElement)
		{
			
		}

		public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

			RocketPreview.Rocket = RocketDummy;
			NameplateConfig.Rocket = RocketDummy;

			if (!NameplateConfig.TextBoxHasFocus)
			{
				NameplateConfig.SetText(RocketDummy.AssignedName);
			}
			else
			{
				RocketDummy.EngineModule.Nameplate.TextColor = HSLMenu.PendingColor;
				RocketDummy.SetName(NameplateConfig.Text);
			}

			DetailConfig.Rocket = RocketDummy;
			PatternConfig.Rocket = RocketDummy;
		}

		public override void Draw(SpriteBatch spriteBatch)
		{
			base.Draw(spriteBatch);
		}
	}
}
