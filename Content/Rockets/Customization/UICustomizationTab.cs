using Macrocosm.Common.UI;
using Macrocosm.Common.Utils;
using Macrocosm.Content.Rockets.Customization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader;
using Terraria.UI;

namespace Macrocosm.Content.Rockets.Navigation
{
    public class UICustomizationTab : UIPanel
    {
        public Rocket Rocket;

		public UIRocketPreviewLarge RocketPreview;
		public UIPanel RocketPreviewBackground;

		public UIPanelIconButton ApplyButton;
		public UIPanelIconButton CancelButton;
		public UIPanel ControlButtonsBackground;

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
			RocketPreviewBackground.Activate();
			Append(RocketPreviewBackground);

			RocketPreview = new();
			RocketPreviewBackground.Append(RocketPreview);

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
				OnFocusGain = () => 
				{
					RocketPreview.UpdateModule("EngineModule");
					ModulePicker.CurrentModuleName = "EngineModule";
					HSLMenu.SetColorHSL(Rocket.CustomizationDummy.Nameplate.TextColor.ToHSL());
				}
			};

			CustomizationPanelBackground.Append(NameplateConfig);

			DetailConfig = new();
			CustomizationPanelBackground.Append(DetailConfig);

			PatternConfig = new();
			CustomizationPanelBackground.Append(PatternConfig);

			HSLMenu = new();
			UIPanel hslPanel = HSLMenu.ProvideHSLMenu();
			CustomizationPanelBackground.Append(hslPanel);
			HSLMenu.SetColorSetEvent(HSLMenu_OnSliderClick);

			ControlButtonsBackground = new()
			{
				Width = new(0f, 0.17f),
				Height = new(0f, 0.08f),
				Left = new(0f, 0.8f),
				Top = new(0f, 0.45f),
				BackgroundColor = new Color(53, 72, 135),
				BorderColor = new Color(89, 116, 213, 255)
			};
			ControlButtonsBackground.SetPadding(2f);
			CustomizationPanelBackground.Append(ControlButtonsBackground);

			ApplyButton = new(ModContent.Request<Texture2D>("Macrocosm/Content/Rockets/Textures/Symbols/GreenCheckmark"))
			{
				VAlign = 0.5f,
				Left = new(0f, 0.5f)
			};
			ApplyButton.OnLeftClick += (_, _) => Rocket.ApplyCustomizationChanges();
			ControlButtonsBackground.Append(ApplyButton);

			CancelButton = new(ModContent.Request<Texture2D>("Macrocosm/Content/Rockets/Textures/Symbols/RedCrossmark"))
			{
				VAlign = 0.5f,
				Left = new(0f, 0.05f)
			};

			CancelButton.OnLeftClick += (_, _) =>
			{
				NameplateConfig.HasFocus = false;
				Rocket.RefreshCustomizationDummy();
			};
			ControlButtonsBackground.Append(CancelButton);

			CustomizationPanelBackground.Activate();
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

			NameplateConfig.HasFocus = false;
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

			NameplateConfig.HasFocus = false;
		}

		private void HSLMenu_OnSliderClick(Terraria.UI.UIMouseEvent evt, Terraria.UI.UIElement listeningElement)
		{
			
		}

		public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

			RocketPreview.Rocket = Rocket;
			NameplateConfig.Rocket = Rocket;
			DetailConfig.Rocket = Rocket;
			PatternConfig.Rocket = Rocket;

			if (NameplateConfig.HasFocus)
 				Rocket.CustomizationDummy.Nameplate.TextColor = HSLMenu.PendingColor;
		}

		public override void Draw(SpriteBatch spriteBatch)
		{
			base.Draw(spriteBatch);
		}
	}
}
