using Macrocosm.Common.UI;
using Macrocosm.Common.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System.Linq;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.UI.Elements;
using Terraria.ID;
using Terraria.Localization;

namespace Macrocosm.Content.Rockets.Navigation.CrewPanel
{
    public sealed class UICrewPanel : UIListScrollablePanel, IRocketDataConsumer
    {
        public Rocket Rocket { get; set; }

        public UICrewPanel() : base(new LocalizedColorScaleText(Language.GetText("Mods.Macrocosm.UI.Rocket.Common.Crew"), scale: 1.2f))
        {
        }

        public override void OnInitialize()
        {
            base.OnInitialize();
            BorderColor = new Color(89, 116, 213, 255);
            BackgroundColor = new Color(53, 72, 135);

            if (Main.netMode == NetmodeID.SinglePlayer)
                Add(new UIPlayerInfoElement(Main.LocalPlayer));
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

                    if (rocketPlayer.InRocket && rocketPlayer.RocketID == Rocket.WhoAmI)
                         Add(new UIPlayerInfoElement(player));
                }

                Activate();

            }
        }
    }
}
