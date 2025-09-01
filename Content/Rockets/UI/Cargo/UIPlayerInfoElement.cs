using Macrocosm.Common.DataStructures;
using Macrocosm.Common.Players;
using Macrocosm.Common.Subworlds;
using Macrocosm.Common.UI;
using Macrocosm.Common.UI.Themes;
using Macrocosm.Common.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;

namespace Macrocosm.Content.Rockets.UI.Cargo
{
    public class UIPlayerInfoElement : UIPanel
    {
        private readonly Player player;
        private readonly bool large;

        private UIText uIPlayerName;

        public bool AddTargetWorld { get; set; } = true;
        public bool DrawInListWithConnectors { get; set; } = true;
        public bool LastInList { get; set; } = false;

        private RocketPlayer RocketPlayer => player.GetModPlayer<RocketPlayer>();

        public UIPlayerInfoElement(Player player, bool large = true)
        {
            this.player = player;
            this.large = large;
        }

        public override void OnInitialize()
        {
            if (large)
                Height.Set(74, 0f);
            else
                Height.Set(40, 0f);

            BackgroundColor = UITheme.Current.InfoElementStyle.BackgroundColor;
            BorderColor = UITheme.Current.InfoElementStyle.BorderColor;

            if (Main.netMode == NetmodeID.SinglePlayer || RocketPlayer.IsCommander || !DrawInListWithConnectors)
            {
                Width.Set(0f, 0.98f);
                Left.Set(0f, 0f);
            }
            else
            {
                Width.Set(0f, 0.78f);
                Left.Set(0f, 0.20f);
            }

            uIPlayerName = new(player.name)
            {
                Left = new(100, 0),
                VAlign = 0.5f
            };
            Append(uIPlayerName);
        }

        SpriteBatchState state;
        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);

            if (!player.active)
                return;

            Recalculate();
            CalculatedStyle dimensions = GetDimensions();

            // Uhh lotsa magic numbers -- Feldy
            if (!RocketPlayer.IsCommander && DrawInListWithConnectors)
            {
                UIConnectors.DrawConnectorHorizontal(spriteBatch, new Rectangle((int)(dimensions.X - dimensions.Width * 0.112f), (int)(dimensions.Y + dimensions.Height * 0.33f), 44, 23), BackgroundColor, BorderColor, out _, out _);

                if (LastInList)
                {
                    UIConnectors.DrawConnectorVertical(spriteBatch, new Rectangle((int)(dimensions.X + dimensions.Width * -0.15f), (int)(dimensions.Y - 6), 23, (int)dimensions.Height / 2), BackgroundColor, BorderColor, out _, out _);
                    UIConnectors.DrawConnectorLCorner(spriteBatch, new Vector2(dimensions.X - dimensions.Width * 0.152f, (int)(dimensions.Y + dimensions.Height * 0.33f)), BackgroundColor, BorderColor);
                }
                else
                {
                    UIConnectors.DrawConnectorVertical(spriteBatch, new Rectangle((int)(dimensions.X + dimensions.Width * -0.15f), (int)(dimensions.Y - 6), 23, (int)dimensions.Height + 4), BackgroundColor, BorderColor, out _, out _);
                    UIConnectors.DrawConnectorTJunction(spriteBatch, new Vector2(dimensions.X - dimensions.Width * 0.151f, (int)(dimensions.Y + dimensions.Height * 0.33f)), BackgroundColor, BorderColor);
                }
            }

            state.SaveState(spriteBatch, true);

            Vector2 worldIconPosition = dimensions.Position() + new Vector2(dimensions.Width * 0.85f, dimensions.Height * 0.5f);
            Vector2 playerPosition = dimensions.Position() + new Vector2(dimensions.Width * 0.08f, dimensions.Height * 0.2f);
            Vector2 headIconPosition = dimensions.Position() + new Vector2(dimensions.Width * 0.08f, dimensions.Height * 0.42f);

            string targetWorld = MacrocosmSubworld.SanitizeID(RocketPlayer.TargetWorld, out _);
            if (AddTargetWorld && RocketPlayer.IsCommander && ModContent.RequestIfExists(Macrocosm.TexturesPath + "Icons/" + targetWorld, out Asset<Texture2D> iconTexture))
                spriteBatch.Draw(iconTexture.Value, worldIconPosition, null, Color.White, 0f, iconTexture.Size() / 2f, 1f, SpriteEffects.None, 0);


            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, state);

            if (large)
            {
                player.DrawPetDummy(playerPosition, true);
                var clonePlayer = player.PrepareDummy(playerPosition);
                Main.PlayerRenderer.DrawPlayer(Main.Camera, clonePlayer, clonePlayer.position, 0f, player.Size / 2f);
            }
            else
            {
                Main.PlayerRenderer.DrawPlayerHead(Main.Camera, player, headIconPosition);
            }

            spriteBatch.End();
            spriteBatch.Begin(state);
        }
    }

}
