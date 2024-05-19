using Macrocosm.Common.UI;
using Macrocosm.Common.UI.Themes;
using Macrocosm.Common.Utils;
using Macrocosm.Content.Rockets;
using Macrocosm.Content.Rockets.LaunchPads;
using Macrocosm.Content.Rockets.UI.Assembly;
using Microsoft.Xna.Framework;
using MonoMod.Core.Utils;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.UI.Elements;
using Terraria.Localization;
using Terraria.UI;

namespace Macrocosm.Common.Systems.UI
{
    public class AssemblyUIState : UIState
    {
        public LaunchPad LaunchPad { get; set; }

        private UIText title;
        private UIDragablePanel window;
        private UIAssemblyTab assemblyTab;

        public AssemblyUIState()
        {
        }

        public override void OnInitialize()
        {
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

            title = new(Language.GetText("Mods.Macrocosm.UI.Rocket.Common.Assembly"), 0.6f, true)
            {
                IsWrapped = false,
                HAlign = 0.49f,
                VAlign = 0.005f,
                Top = new(-34, 0),
                TextColor = Color.White
            };
            window.Append(title);

            assemblyTab = new();
            window.Append(assemblyTab);
        }

        public void OnShow()
        {
            assemblyTab.LaunchPad = LaunchPad;

            window.ExecuteRecursively((uIElement) =>
            {
                if (uIElement is IRocketUIDataConsumer rocketDataConsumer)
                {
                    rocketDataConsumer.Rocket = LaunchPad.Rocket;
                    rocketDataConsumer.OnRocketChanged();
                }

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

            LaunchPad.Inventory.InteractingPlayer = 255;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            Player player = Main.LocalPlayer;

            if (LaunchPad is not null && (!LaunchPad.Hitbox.InPlayerInteractionRange(TileReachCheckSettings.Simple) || player.UICloseConditions()))
                UISystem.Hide();
        }
    }
}
