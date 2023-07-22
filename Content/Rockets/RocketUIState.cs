using Macrocosm.Common.UI;
using Macrocosm.Common.Utils;
using Macrocosm.Content.Rockets.Navigation.DebugPanel;
using Macrocosm.Content.Systems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Linq;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.UI;

namespace Macrocosm.Content.Rockets.Navigation
{
    public class RocketUIState : UIState
    {
        public Rocket Rocket;

		private UIText Title;
		private UIDragablePanel BackgroundPanel;
		private UIHoverImageButton TabLeftButton;
		private UIHoverImageButton TabRightButton;

		private UINavigationTab Navigation;
		private UICustomizationTab Customization;
		private UICargoTab Cargo;

		//private UIRocketDebugPanel DebugPanel;

		public RocketUIState() 
		{
		}

		private const string buttonsPath = "Macrocosm/Content/Rockets/Textures/Buttons/";


		public override void OnInitialize()
        {
			var mode = ReLogic.Content.AssetRequestMode.ImmediateLoad;

            BackgroundPanel = new();
            BackgroundPanel.Width.Set(875f, 0f);
            BackgroundPanel.Height.Set(720f, 0f);
            BackgroundPanel.HAlign = 0.5f;
            BackgroundPanel.VAlign = 0.5f;
			BackgroundPanel.SetPadding(6f);
			BackgroundPanel.PaddingTop = 40f;

			BackgroundPanel.BackgroundColor = new Color(89, 116, 213);

			Append(BackgroundPanel);

			Title = new(Language.GetText("Mods.Macrocosm.RocketUI.Common.Navigation"), 0.6f, true)
			{
				IsWrapped = false,
				HAlign = 0.5f,
				VAlign = 0.005f,
				Top = new(-34, 0),
				TextColor = Color.White
			};
			BackgroundPanel.Append(Title);


			Navigation = new();
			Customization = new();
			Cargo = new();

			Navigation.Activate();
			Customization.Activate();
			Cargo.Activate();

			BackgroundPanel.Append(Navigation);
			Navigation.CustomizationPreview.OnLeftClick += SetTab_Customization;
			//Navigation.CargoFuelPreview.OnLeftClick += SetTab_Cargo;


			TabLeftButton = new(ModContent.Request<Texture2D>(buttonsPath + "BackArrow", mode), ModContent.Request<Texture2D>(buttonsPath + "BackArrowBorder", mode), Language.GetText("Mods.Macrocosm.RocketUI.Common.Customization"))
			{
				Top = new(-38,0f),
				Left = new(0, 0.005f),
				
				CheckInteractible = () => !BackgroundPanel.Children.Contains(Customization)
			};
			TabLeftButton.SetVisibility(1f, 0f, 1f);
			TabLeftButton.OnLeftClick += SetTab_Customization;
			BackgroundPanel.Append(TabLeftButton);

			TabRightButton = new(ModContent.Request<Texture2D>(buttonsPath + "ForwardArrow", mode), ModContent.Request<Texture2D>(buttonsPath + "ForwardArrowBorder", mode), Language.GetText("Mods.Macrocosm.RocketUI.Common.Cargo"))
			{
				Top = new(-38, 0f),
				Left = new(0, 0.955f),

				CheckInteractible = () => !BackgroundPanel.Children.Contains(Cargo)
			};
			TabRightButton.SetVisibility(1f, 0f, 1f);
			TabRightButton.OnLeftClick += SetTab_Cargo;
			BackgroundPanel.Append(TabRightButton);

			//DebugPanel = new();
			//BackgroundPanel.Append(DebugPanel);
		}

		private void SetTab_Customization(UIMouseEvent evt, UIElement listeningElement)
		{
			BackgroundPanel.RemoveChild(Navigation);
			BackgroundPanel.Append(Customization);
			Title.SetText(Language.GetText("Mods.Macrocosm.RocketUI.Common.Customization"));

			TabRightButton.OnLeftClick -= SetTab_Cargo;
			TabRightButton.OnLeftClick += SetTab_Navigation;
			TabRightButton.HoverText = Language.GetText("Mods.Macrocosm.RocketUI.Common.Navigation");
		}

		private void SetTab_Cargo(UIMouseEvent evt, UIElement listeningElement)
		{
			BackgroundPanel.RemoveChild(Navigation);
			BackgroundPanel.Append(Cargo);
			Title.SetText(Language.GetText("Mods.Macrocosm.RocketUI.Common.Cargo"));

			TabLeftButton.OnLeftClick -= SetTab_Customization;
			TabLeftButton.OnLeftClick += SetTab_Navigation;
			TabLeftButton.HoverText = Language.GetText("Mods.Macrocosm.RocketUI.Common.Navigation");
		}

		private void SetTab_Navigation(UIMouseEvent evt, UIElement listeningElement)
		{
			BackgroundPanel.RemoveChild(Customization);
			BackgroundPanel.RemoveChild(Cargo);  

			BackgroundPanel.Append(Navigation);
			Title.SetText(Language.GetText("Mods.Macrocosm.RocketUI.Common.Navigation"));

			TabLeftButton.OnLeftClick -= SetTab_Navigation;
			TabLeftButton.OnLeftClick += SetTab_Customization;
			TabLeftButton.HoverText = Language.GetText("Mods.Macrocosm.RocketUI.Common.Customization");

			TabRightButton.OnLeftClick -= SetTab_Navigation;
			TabRightButton.OnLeftClick += SetTab_Cargo;
			TabRightButton.HoverText = Language.GetText("Mods.Macrocosm.RocketUI.Common.Cargo");
		}

		public override void Update(GameTime gameTime)
		{
			base.Update(gameTime);

			WorldDataSystem.Instance.FoundVulcan = false;

			Navigation.Rocket = Rocket;
			Customization.Rocket = Rocket;
			Cargo.Rocket = Rocket;

			Player player = Main.LocalPlayer;
			player.mouseInterface = true;

			if (!Rocket.Active || !Rocket.InInteractionRange() || Rocket.InFlight || player.controlMount || player.UICloseConditions())
			{
				RocketUISystem.Hide();
				return;
			}
		}
	}
}
