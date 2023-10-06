using Macrocosm.Common.DataStructures;
using Macrocosm.Common.UI;
using Macrocosm.Common.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader;
using Terraria.UI;

namespace Macrocosm.Content.Rockets.UI
{
	internal class UIPlayerInfoElement : UIPanel
	{
		private Player player;

		public UIPlayerInfoElement(Player player)
		{
			this.player = player;
		}

		public override void OnInitialize()
		{
			Width.Set(0f, 1f);
			Height.Set(200f, 0f);

			BackgroundColor = new Color(43, 56, 101);
			BorderColor = BackgroundColor * 2f;
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
			Vector2 playerPosition = dimensions.Position() + new Vector2(dimensions.Width * 0.08f, dimensions.Height * 0.42f);

			var rocketPlayer = player.GetModPlayer<RocketPlayer>();
			Texture2D texture = Macrocosm.EmptyTex;

			if (ModContent.RequestIfExists(Macrocosm.TextureAssetsPath + "Icons/" + rocketPlayer.TargetSubworldID, out Asset<Texture2D> iconTexture))
				texture = iconTexture.Value;

			spriteBatch.Draw(texture, worldIconPosition, Color.White);

			state.SaveState(spriteBatch);
			spriteBatch.End();
			spriteBatch.Begin(state);
			Main.PlayerRenderer.DrawPlayer(Main.Camera, player, playerPosition, 0f, player.Size / 2f);
			spriteBatch.End();
			spriteBatch.Begin(state);
		}
	}
}

