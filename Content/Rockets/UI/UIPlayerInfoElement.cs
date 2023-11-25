using Macrocosm.Common.DataStructures;
using Macrocosm.Common.UI;
using Macrocosm.Common.UI.Themes;
using Macrocosm.Common.Utils;
using Macrocosm.Content.Players;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;

namespace Macrocosm.Content.Rockets.UI
{
    internal class UIPlayerInfoElement : UIPanel
    {
        private Player player;
        private RocketPlayer RocketPlayer => player.GetModPlayer<RocketPlayer>();
        private UIText uIPlayerName;

        public bool LastInList { get; set; }

        public UIPlayerInfoElement(Player player)
        {
            this.player = player;
        }

        public override void OnInitialize()
        {
            Height.Set(74, 0f);

            BackgroundColor = UITheme.Current.InfoElementStyle.BackgroundColor;
            BorderColor = UITheme.Current.InfoElementStyle.BorderColor;

            if (Main.netMode == NetmodeID.SinglePlayer || RocketPlayer.IsCommander)
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

            Recalculate();
            CalculatedStyle dimensions = GetDimensions();

            // Uhh lotsa magic numbers -- Feldy
            if (!RocketPlayer.IsCommander)
            {
                UIConnectors.DrawConnectorHorizontal(spriteBatch, new Rectangle((int)(dimensions.X - dimensions.Width * 0.112f), (int)(dimensions.Y + dimensions.Height * 0.33f), 44, 23), BackgroundColor, BorderColor);

                if (LastInList)
                {
                    UIConnectors.DrawConnectorVertical(spriteBatch, new Rectangle((int)(dimensions.X + dimensions.Width * -0.15f), (int)(dimensions.Y - 6), 23, (int)dimensions.Height / 2), BackgroundColor, BorderColor);
                    UIConnectors.DrawConnectorLCorner(spriteBatch, new Vector2(dimensions.X - dimensions.Width * 0.152f, (int)(dimensions.Y + dimensions.Height * 0.33f)), BackgroundColor, BorderColor);
                }
                else
                {
                    UIConnectors.DrawConnectorVertical(spriteBatch, new Rectangle((int)(dimensions.X + dimensions.Width * -0.15f), (int)(dimensions.Y - 6), 23, (int)dimensions.Height + 4), BackgroundColor, BorderColor);
                    UIConnectors.DrawConnectorTJunction(spriteBatch, new Vector2(dimensions.X - dimensions.Width * 0.151f, (int)(dimensions.Y + dimensions.Height * 0.33f)), BackgroundColor, BorderColor);
                }
            }

            // TODO: move above 
            if (!player.active)
                return;

            state.SaveState(spriteBatch, true);

            Vector2 worldIconPosition = dimensions.Position() + new Vector2(dimensions.Width * 0.85f, dimensions.Height * 0.5f);
            Vector2 playerPosition = dimensions.Position() + new Vector2(dimensions.Width * 0.08f, dimensions.Height * 0.2f);

            if (RocketPlayer.IsCommander && ModContent.RequestIfExists(Macrocosm.TextureAssetsPath + "Icons/" + RocketPlayer.TargetSubworldID, out Asset<Texture2D> iconTexture))
                spriteBatch.Draw(iconTexture.Value, worldIconPosition, null, Color.White, 0f, iconTexture.Size() / 2f, 1f, SpriteEffects.None, 0);


            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, state);

            player.DrawPetDummy(playerPosition, true);
            var clonePlayer = player.PrepareDummy(playerPosition);
            Main.PlayerRenderer.DrawPlayer(Main.Camera, clonePlayer, clonePlayer.position, 0f, player.Size / 2f);

            spriteBatch.End();
            spriteBatch.Begin(state);
        }
    }
}

