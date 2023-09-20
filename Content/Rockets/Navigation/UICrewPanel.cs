using Macrocosm.Common.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System.Linq;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.UI.Elements;
using Terraria.ID;
using Terraria.Localization;

namespace Macrocosm.Content.Rockets.Navigation
{
	public sealed class UICrewPanel : UIListScrollablePanel, IRocketDataConsumer
    {
		public Rocket Rocket { get; set; }

		public UICrewPanel() : base(Language.GetText("Mods.Macrocosm.UI.Rocket.Common.Crew"))
        {
		}

		public override void Update(GameTime gameTime)
		{
			base.Update(gameTime);

			Deactivate();
            ClearList();

            //if(Main.netMode == NetmodeID.MultiplayerClient)
            {
				for (int i = 0; i < Main.maxPlayers; i++)
				{
					var player = Main.player[i];

					if (!player.active)
						continue;

					var rocketPlayer = player.GetModPlayer<RocketPlayer>();

					if (rocketPlayer.InRocket && rocketPlayer.RocketID == Rocket.WhoAmI)
					{
						Add(new UIInfoElement(new LocalizedColorScaleText(Language.GetText(player.name)))
						{
							Width = new(0f, 1f),
							ExtraDraw = (Vector2 iconPosition) => Main.PlayerRenderer.DrawPlayerHead(Main.Camera, player, iconPosition)
						});
					}
				}
			}

			Activate();
		}

		public override void OnInitialize()
        {
            Width.Set(0, 0.34f);
            Height.Set(0f, 0.4f);
			HAlign = 0.5f;
            Top.Set(0f, 0.365f);
            SetPadding(0f);
            BorderColor = new Color(89, 116, 213, 255);
			BackgroundColor = new Color(53, 72, 135);
        }
    }
}
