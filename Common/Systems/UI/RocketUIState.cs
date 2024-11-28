using Macrocosm.Common.UI;
using Macrocosm.Common.UI.Themes;
using Macrocosm.Common.Utils;
using Macrocosm.Content.Rockets;
using Macrocosm.Content.Rockets.UI.Cargo;
using Macrocosm.Content.Rockets.UI.Customization;
using Macrocosm.Content.Rockets.UI.Navigation;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Linq;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.UI.Elements;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.UI;

namespace Macrocosm.Common.Systems.UI
{
    public class RocketUIState : UIState, IRocketUIDataConsumer
    {
        public Rocket Rocket { get; set; }

        private UINavigationTab navigation;
        private UICustomizationTab customization;
        private UICargoTab cargo;

        private UIText title;
        private UIDragablePanel window;
        private UIHoverImageButton tabLeftButton;
        private UIHoverImageButton tabRightButton;


        public RocketUIState()
        {
        }

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

            window.BackgroundColor = UITheme.Current.WindowStyle.BackgroundColor;
            window.BorderColor = UITheme.Current.WindowStyle.BorderColor;

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
            cargo = new();

            navigation.Activate();
            customization.Activate();
            cargo.Activate();

            window.Append(navigation);
            navigation.CustomizationPreview.OnLeftClick += SetTab_Customization;
            navigation.CargoPreview.OnLeftClick += SetTab_Cargo;

            tabLeftButton = new
            (
                ModContent.Request<Texture2D>(Macrocosm.ButtonsPath + "ShortArrow", mode),
                ModContent.Request<Texture2D>(Macrocosm.ButtonsPath + "ShortArrowBorder", mode),
                Language.GetText("Mods.Macrocosm.UI.Rocket.Common.Customization"))
            {
                Top = new(-38, 0f),
                Left = new(0, 0.005f),
                SpriteEffects = SpriteEffects.FlipHorizontally,
                CheckInteractible = () => !window.Children.Contains(customization)
            };
            tabLeftButton.SetVisibility(1f, 0f, 1f);
            tabLeftButton.OnLeftClick += SetTab_Customization;
            window.Append(tabLeftButton);

            tabRightButton = new
            (
                ModContent.Request<Texture2D>(Macrocosm.ButtonsPath + "ShortArrow", mode),
                ModContent.Request<Texture2D>(Macrocosm.ButtonsPath + "ShortArrowBorder", mode),
                Language.GetText("Mods.Macrocosm.UI.Rocket.Common.Payload"))
            {
                Top = new(-38, 0f),
                Left = new(0, 0.955f),

                CheckInteractible = () => !window.Children.Contains(cargo)
            };
            tabRightButton.SetVisibility(1f, 0f, 1f);
            tabRightButton.OnLeftClick += SetTab_Cargo;
            window.Append(tabRightButton);
        }

        public void OnRocketChanged()
        {
            navigation.ExecuteRecursively(OnRocketChanged());
            cargo.ExecuteRecursively(OnRocketChanged());
            customization.ExecuteRecursively(OnRocketChanged());

            UIElementAction OnRocketChanged()
            {
                return (uIElement) =>
                {
                    if (uIElement is IRocketUIDataConsumer rocketDataConsumer)
                        rocketDataConsumer.Rocket = Rocket;
                };
            }
        }

        public void OnShow()
        {
            navigation.ExecuteRecursively(OnShow());
            cargo.ExecuteRecursively(OnShow());
            customization.ExecuteRecursively(OnShow());

            UIElementAction OnShow()
            {
                return (uIElement) =>
                {
                    if (uIElement is IRocketUIDataConsumer rocketDataConsumer)
                    {
                        rocketDataConsumer.Rocket = Rocket;
                        rocketDataConsumer.OnRocketChanged();
                    }

                    if (uIElement is ITabUIElement tab)
                        tab.OnTabOpen();
                };
            }
        }

        public void OnHide()
        {
            navigation.ExecuteRecursively(OnHide());
            cargo.ExecuteRecursively(OnHide());
            customization.ExecuteRecursively(OnHide());

            static UIElementAction OnHide()
            {
                return (uIElement) =>
                {
                    if (uIElement is ITabUIElement tab)
                        tab.OnTabClose();
                };
            }
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
                if (uIElement is IRocketUIDataConsumer rocketDataConsumer)
                {
                    rocketDataConsumer.Rocket = Rocket;
                    rocketDataConsumer.OnRocketChanged();
                }
            });
        }


        // TODO: There must be a way to unhardcode all of these. -- Feldy
        // The tabbing logic with interface could use some linked list behavior (?) 
        private void SetTab_Customization(UIMouseEvent evt, UIElement listeningElement)
        {
            tabRightButton.OnLeftClick -= SetTab_Cargo;
            tabRightButton.OnLeftClick += SetTab_Navigation;

            SwitchTab(customization);

            title.SetText(Language.GetText("Mods.Macrocosm.UI.Rocket.Common.Customization"));
            tabRightButton.HoverText = Language.GetText("Mods.Macrocosm.UI.Rocket.Common.Navigation");
        }

        private void SetTab_Cargo(UIMouseEvent evt, UIElement listeningElement)
        {
            tabLeftButton.OnLeftClick -= SetTab_Customization;
            tabLeftButton.OnLeftClick += SetTab_Navigation;

            SwitchTab(cargo);

            title.SetText(Language.GetText("Mods.Macrocosm.UI.Rocket.Common.Cargo"));
            tabLeftButton.HoverText = Language.GetText("Mods.Macrocosm.UI.Rocket.Common.Navigation");
        }

        private void SetTab_Navigation(UIMouseEvent evt, UIElement listeningElement)
        {

            tabLeftButton.OnLeftClick -= SetTab_Navigation;
            tabLeftButton.OnLeftClick += SetTab_Customization;

            tabRightButton.OnLeftClick -= SetTab_Navigation;
            tabRightButton.OnLeftClick += SetTab_Cargo;

            SwitchTab(navigation);

            title.SetText(Language.GetText("Mods.Macrocosm.UI.Rocket.Common.Navigation"));

            tabLeftButton.HoverText = Language.GetText("Mods.Macrocosm.UI.Rocket.Common.Customization");
            tabRightButton.HoverText = Language.GetText("Mods.Macrocosm.UI.Rocket.Common.Cargo");
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            Player player = Main.LocalPlayer;

            if (Rocket is not null && !Rocket.Active || !Rocket.Bounds.InPlayerInteractionRange(TileReachCheckSettings.Simple) || Rocket.State != Rocket.ActionState.Idle || player.controlMount || player.UICloseConditions())
                UISystem.Hide();
        }
    }
}
