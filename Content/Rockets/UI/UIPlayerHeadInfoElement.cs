using Macrocosm.Common.DataStructures;
using Macrocosm.Common.UI;
using Macrocosm.Common.Utils;
using Macrocosm.Content.Players;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;

namespace Macrocosm.Content.Rockets.UI
{
    internal class UIPlayerHeadInfoElement : UIInfoElement
    {
        private Player player;

        public UIPlayerHeadInfoElement(Player player) : base(player.name)
        {
            this.player = player;
        }

        SpriteBatchState state;
        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);

            if (!player.active)
                return;

            Recalculate();
            CalculatedStyle dimensions = GetDimensions();
            Vector2 worldIconPosition = dimensions.Position() + new Vector2(dimensions.Width * 0.8f, dimensions.Height * 0.1f);
            Vector2 headIconPosition = dimensions.Position() + new Vector2(dimensions.Width * 0.08f, dimensions.Height * 0.42f);

            var rocketPlayer = player.GetModPlayer<RocketPlayer>();
            Texture2D texture = Macrocosm.EmptyTex;
            if (ModContent.RequestIfExists(Macrocosm.TextureAssetsPath + "Icons/" + rocketPlayer.TargetSubworldID, out Asset<Texture2D> iconTexture))
                texture = iconTexture.Value;

            spriteBatch.Draw(texture, worldIconPosition, Color.White);

            state.SaveState(spriteBatch);
            spriteBatch.End();
            spriteBatch.Begin(state);
            Main.PlayerRenderer.DrawPlayerHead(Main.Camera, player, headIconPosition);
            spriteBatch.End();
            spriteBatch.Begin(state);
        }
    }
}