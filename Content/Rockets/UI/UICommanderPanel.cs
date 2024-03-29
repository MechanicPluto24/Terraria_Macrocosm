using Macrocosm.Common.UI;
using Macrocosm.Common.UI.Themes;
using Macrocosm.Content.Players;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.Localization;

namespace Macrocosm.Content.Rockets.UI
{
    public class UICommanderPanel : UIListScrollablePanel, IRocketUIDataConsumer
    {
        public Rocket Rocket { get; set; }

        public UICommanderPanel() : base(new LocalizedColorScaleText(Language.GetText("Mods.Macrocosm.UI.Rocket.Common.Commander"), scale: 1.2f))
        {
        }

        public override void OnInitialize()
        {
            base.OnInitialize();
            BorderColor = UITheme.Current.PanelStyle.BorderColor;
            BackgroundColor = UITheme.Current.PanelStyle.BackgroundColor;

            if (Main.netMode == NetmodeID.SinglePlayer)
                Add(new UIPlayerHeadInfoElement(Main.LocalPlayer));
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (Main.netMode == NetmodeID.MultiplayerClient)
            {
                Deactivate();
                ClearList();

                for (int i = 0; i < Main.maxPlayers; i++)
                {
                    var player = Main.player[i];

                    if (!player.active)
                        continue;

                    var rocketPlayer = player.GetModPlayer<RocketPlayer>();

                    if (rocketPlayer.InRocket && rocketPlayer.IsCommander && rocketPlayer.RocketID == Rocket.WhoAmI)
                        Add(new UIPlayerHeadInfoElement(player));
                }

                Activate();
            }
        }
    }
}
