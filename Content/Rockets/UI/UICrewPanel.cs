using Macrocosm.Common.UI;
using Macrocosm.Common.UI.Themes;
using Macrocosm.Content.Players;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.Localization;
using Terraria.UI;

namespace Macrocosm.Content.Rockets.UI
{
    public class UICrewPanel : UIListScrollablePanel, IRocketUIDataConsumer
    {
        public Rocket Rocket { get; set; }

        private Connector connector;
        private Player commander;
        private Player prevCommander = Main.LocalPlayer;
        private List<Player> crew = new();
        private List<Player> prevCrew = new();

        public UICrewPanel() : base(new LocalizedColorScaleText(Language.GetText("Mods.Macrocosm.UI.Rocket.Common.Crew"), scale: 1.2f))
        {
            connector = new(0);
        }

        public override void OnInitialize()
        {
            base.OnInitialize();
            BorderColor = UITheme.Current.PanelStyle.BorderColor;
            BackgroundColor = UITheme.Current.PanelStyle.BackgroundColor;
            ScrollbarHAlign = 1.015f;
            ListWidthWithScrollbar = new StyleDimension(0, 1f);
            SetPadding(0f);
            PaddingLeft = PaddingRight = ListOuterPadding = 12f;

            if (Main.netMode == NetmodeID.SinglePlayer)
            {
                Add(new UIPlayerInfoElement(Main.LocalPlayer));
            }
        }


        protected override void DrawChildren(SpriteBatch spriteBatch)
        {
            base.DrawChildren(spriteBatch);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (Main.netMode == NetmodeID.MultiplayerClient || true)
            {
                crew.Clear();

                for (int i = 0; i < Main.maxPlayers; i++)
                {
                    var player = Main.player[i];

                    if (!player.active)
                        continue;

                    var rocketPlayer = player.GetModPlayer<RocketPlayer>();

                    if (rocketPlayer.InRocket && rocketPlayer.RocketID == Rocket.WhoAmI)
                    {
                        if (rocketPlayer.IsCommander)
                            commander = player;
                        else
                            crew.Add(player);
                    }
                }

                if (!commander.Equals(prevCommander) || !crew.SequenceEqual(prevCrew))
                {
                    Deactivate();
                    ClearList();

                    connector = new(crew.Count) { };
                    Add(connector);

                    Add(new UIPlayerInfoElement(commander));

                    foreach (var player in crew)
                    {
                        Add(new UIPlayerInfoElement(player));
                    }

                    if (crew.Any())
                        OfType<UIPlayerInfoElement>().LastOrDefault().LastInList = true;

                    prevCommander = commander;
                    prevCrew = crew;

                    Activate();
                }
            }
        }

        private class Connector : UIElement
        {
            private int count;

            public Connector(int count)
            {
                this.count = count;
            }

            public override void Draw(SpriteBatch spriteBatch)
            {
                base.Draw(spriteBatch);

                var dimensions = Parent.GetDimensions();
                Rectangle rect = new((int)(dimensions.X + dimensions.Width * 0.1f), (int)(dimensions.Y + dimensions.Height * 0.35f), 20, 14 + 48 * count);
                spriteBatch.Draw(TextureAssets.BlackTile.Value, rect, Color.White);

                for (int i = 0; i < count; i++)
                {
                    rect = new((int)(dimensions.X + dimensions.Width * 0.1f), (int)(dimensions.Y + dimensions.Height * 0.438f + 64 * i), 68, 20);
                    spriteBatch.Draw(TextureAssets.BlackTile.Value, rect, Color.White);
                }
            }
        }
    }
}
