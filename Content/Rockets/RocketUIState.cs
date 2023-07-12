using Macrocosm.Common.UI;
using Macrocosm.Common.Utils;
using Macrocosm.Content.Rockets.Navigation.DebugPanel;
using Macrocosm.Content.Systems;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.Localization;
using Terraria.UI;

namespace Macrocosm.Content.Rockets.Navigation
{
    public class RocketUIState : UIState
    {
        public Rocket Rocket;

		private UIDragablePanel BackgroundPanel;
		private UIText Title;

		private UINavigationTab Navigation;

		UIRocketDebugPanel DebugPanel;

		public static void Show(Rocket rocket) => RocketUISystem.Instance.ShowUI(rocket);
        public static void Hide() => RocketUISystem.Instance.HideUI();
        public static bool Active => RocketUISystem.Instance.Interface?.CurrentState is not null;

		public RocketUIState() 
		{
		}

        public override void OnInitialize()
        {
            BackgroundPanel = new();
            BackgroundPanel.Width.Set(0, 0.435f);
            BackgroundPanel.Height.Set(0, 0.69f);
            BackgroundPanel.HAlign = 0.5f;
            BackgroundPanel.VAlign = 0.5f;
			BackgroundPanel.SetPadding(6f);
			BackgroundPanel.PaddingTop = 40f;
			//BackgroundPanel.PaddingTop *= 3;

			BackgroundPanel.BackgroundColor = (new Color(89, 116, 213) * 1f).NewAlpha(255);

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
			BackgroundPanel.Append(Navigation);

			//DebugPanel = new();
			//BackgroundPanel.Append(DebugPanel);
		}

		public override void Update(GameTime gameTime)
		{
			base.Update(gameTime);

			WorldDataSystem.Instance.FoundVulcan = true;

			Navigation.Rocket = Rocket;

			Player player = Main.LocalPlayer;
			player.mouseInterface = true;

			if (!Rocket.Active || !Rocket.InInteractionRange || Rocket.Launching || player.controlMount || player.UICloseConditions())
			{
				Hide();
				return;
			}
		}
	}
}
