using Macrocosm.Common.UI;
using Macrocosm.Common.Utils;
using Macrocosm.Content.Rockets.Payload;
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
    public class RocketUIState : UIState, IRocketDataConsumer
    {
        public Rocket Rocket { get; set; } 

		private UIText title;
		private UIDragablePanel window;
		private UIHoverImageButton tabLeftButton;
		private UIHoverImageButton tabRightButton;

		private UINavigationTab navigation;
		private UICustomizationTab customization;
		private UIPayloadTab payload;

		public RocketUIState() 
		{
		}

		private const string buttonsPath = "Macrocosm/Content/Rockets/Textures/Buttons/";


		public override void OnInitialize()
        {
			var mode = ReLogic.Content.AssetRequestMode.ImmediateLoad;

            window = new();
            window.Width.Set(875f, 0f);
            window.Height.Set(720f, 0f);
            window.HAlign = 0.5f;
            window.VAlign = 0.5f;
			window.SetPadding(6f);
			window.PaddingTop = 40f;

			window.BackgroundColor = new Color(89, 116, 213);

			Append(window);

			title = new(Language.GetText("Mods.Macrocosm.UI.Rocket.Common.Navigation"), 0.6f, true)
			{	
				IsWrapped = false,
				HAlign = 0.5f,
				VAlign = 0.005f,
				Top = new(-34, 0),
				TextColor = Color.White
			};
			window.Append(title);


			navigation = new();
			customization = new();
			payload = new();

			navigation.Activate();
			customization.Activate();
			payload.Activate();

			window.Append(navigation);
			navigation.CustomizationPreview.OnLeftClick += SetTab_Customization;
			//Navigation.PayloadFuelPreview.OnLeftClick += SetTab_Payload;

			tabLeftButton = new(ModContent.Request<Texture2D>(buttonsPath + "BackArrow", mode), ModContent.Request<Texture2D>(buttonsPath + "BackArrowBorder", mode), Language.GetText("Mods.Macrocosm.UI.Rocket.Common.Customization"))
			{
				Top = new(-38,0f),
				Left = new(0, 0.005f),
				
				CheckInteractible = () => !window.Children.Contains(customization)
			};
			tabLeftButton.SetVisibility(1f, 0f, 1f);
			tabLeftButton.OnLeftClick += SetTab_Customization;
			window.Append(tabLeftButton);

			tabRightButton = new(ModContent.Request<Texture2D>(buttonsPath + "ForwardArrow", mode), ModContent.Request<Texture2D>(buttonsPath + "ForwardArrowBorder", mode), Language.GetText("Mods.Macrocosm.UI.Rocket.Common.Payload"))
			{
				Top = new(-38, 0f),
				Left = new(0, 0.955f),

				CheckInteractible = () => !window.Children.Contains(payload)
			};
			tabRightButton.SetVisibility(1f, 0f, 1f);
			tabRightButton.OnLeftClick += SetTab_Payload;
			window.Append(tabRightButton);
		}

		public void OnShow()
		{
			window.ExecuteRecursively((uIElement) => 
			{
				if (uIElement is IRocketDataConsumer rocketDataConsumer)
					rocketDataConsumer.Rocket = Rocket;

				if (uIElement is ITabUIElement tab)
					tab.OnTabOpen();
			});
 		}

		public void OnHide()
		{
			window.ExecuteRecursively((uIElement) =>
			{
				if (uIElement is ITabUIElement tab)
					tab.OnTabClose();
			});
		}

		private void SwitchTab(ITabUIElement newTab)
		{
			//window.GetChildrenWhere((child) => child is ITabUIElement).Cast<ITabUIElement>().ToList().ForEach((tab) => tab.OnTabClose());
			(window.Children.FirstOrDefault(child => child is ITabUIElement) as ITabUIElement).OnTabClose();
			window.RemoveAllChildrenWhere((child) => child is ITabUIElement && child != newTab as UIElement);

			newTab.OnTabOpen();
			window.Append(newTab as UIElement);

			window.ExecuteRecursively((uIElement) =>
			{
				if (uIElement is IRocketDataConsumer rocketDataConsumer)
					rocketDataConsumer.Rocket = Rocket;
			});
		}


		// TODO: There must be a way to unhardcode all of these. -- Feldy
		// The tabbing logic with interface could use some linked list behavior (?) 
		private void SetTab_Customization(UIMouseEvent evt, UIElement listeningElement)
		{
			SwitchTab(customization);
			title.SetText(Language.GetText("Mods.Macrocosm.UI.Rocket.Common.Customization"));

			tabRightButton.OnLeftClick -= SetTab_Payload;
			tabRightButton.OnLeftClick += SetTab_Navigation;
			tabRightButton.HoverText = Language.GetText("Mods.Macrocosm.UI.Rocket.Common.Navigation");
		}

		private void SetTab_Payload(UIMouseEvent evt, UIElement listeningElement)
		{
			SwitchTab(payload);
			title.SetText(Language.GetText("Mods.Macrocosm.UI.Rocket.Common.Payload"));

			tabLeftButton.OnLeftClick -= SetTab_Customization;
			tabLeftButton.OnLeftClick += SetTab_Navigation;
			tabLeftButton.HoverText = Language.GetText("Mods.Macrocosm.UI.Rocket.Common.Navigation");
		}

		private void SetTab_Navigation(UIMouseEvent evt, UIElement listeningElement)
		{
			SwitchTab(navigation);
			title.SetText(Language.GetText("Mods.Macrocosm.UI.Rocket.Common.Navigation"));

			tabLeftButton.OnLeftClick -= SetTab_Navigation;
			tabLeftButton.OnLeftClick += SetTab_Customization;
			tabLeftButton.HoverText = Language.GetText("Mods.Macrocosm.UI.Rocket.Common.Customization");

			tabRightButton.OnLeftClick -= SetTab_Navigation;
			tabRightButton.OnLeftClick += SetTab_Payload;
			tabRightButton.HoverText = Language.GetText("Mods.Macrocosm.UI.Rocket.Common.Payload");
		}

		public override void Update(GameTime gameTime)
		{
			base.Update(gameTime);

			Player player = Main.LocalPlayer;
	
			if (Rocket is not null && !Rocket.Active || !Rocket.Bounds.InPlayerInteractionRange() || Rocket.Launched || player.controlMount || player.UICloseConditions())
 				RocketUISystem.Hide();
 		}
	}
}
