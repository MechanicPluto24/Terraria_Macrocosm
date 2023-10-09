using Macrocosm.Common.DataStructures;
using Macrocosm.Common.Graphics;
using Macrocosm.Common.UI;
using Macrocosm.Common.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Drawing.Drawing2D;
using Terraria;
using Terraria.GameContent;
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

		public UIPlayerInfoElement(Player player)
		{
			this.player = player;
		}

		public override void OnInitialize()
		{
			Height.Set(80, 0f);

			if (RocketPlayer.IsCommander)
			{
				Width.Set(0f, 1f);
			}
			else
			{
				Width.Set(0f, 0.8f);
				Left.Set(0f, 0.2f);
			}

			BackgroundColor = new Color(43, 56, 101);
			BorderColor = BackgroundColor * 2f;

			uIPlayerName = new(player.name)
			{
				Left = new(80, 0),
				VAlign = 0.5f
			};
			Append(uIPlayerName);
		}

		SpriteBatchState state, state2;
		public override void Draw(SpriteBatch spriteBatch)
		{
			base.Draw(spriteBatch);

			Recalculate();
			CalculatedStyle dimensions = GetDimensions();

			if (!RocketPlayer.IsCommander)
			{
 				UIConnectors.DrawConnectorTJunction(spriteBatch, new Vector2(dimensions.X - dimensions.Width * 0.18f, (int)(dimensions.Y + 28)), BackgroundColor, BorderColor);
				//UIConnectors.DrawConnector(spriteBatch, new Rectangle((int)(dimensions.X + dimensions.Width * -0.15f), (int)(dimensions.Y - 6), 20, (int)dimensions.Height + 4), BackgroundColor, BorderColor);
				UIConnectors.DrawConnector(spriteBatch, new Rectangle((int)(dimensions.X - dimensions.Width * 0.112f), (int)(dimensions.Y + 28), 40, 23), BackgroundColor, BorderColor);
			}
 
			if (!player.active)
				return;

			state.SaveState(spriteBatch, true);

			Vector2 worldIconPosition = dimensions.Position() + new Vector2(dimensions.Width * 0.75f, dimensions.Height * 0.5f);
			Vector2 playerPosition = dimensions.Position() + new Vector2(dimensions.Width * 0.08f, dimensions.Height * 0.24f);


			Texture2D texture = Macrocosm.EmptyTex;
			Rectangle? sourceRect = null;

			if (RocketPlayer.IsCommander)
			{
				if (ModContent.RequestIfExists(Macrocosm.TextureAssetsPath + "Icons/Large/" + RocketPlayer.TargetSubworldID, out Asset<Texture2D> iconTexture))
				{
					texture = iconTexture.Value;
					var rect = new Rectangle(0, 0, texture.Width, texture.Height);

					// Hide texture overflow to the right
					// I had to do maths on paper for this lol -- Feldy
					int overflowX = (int)(worldIconPosition.X + rect.Width / 2f - (dimensions.X + dimensions.Width));
					if (overflowX > 0)
					{
						rect = rect with { Width = rect.Width - overflowX - 3 };
					}

					if (texture.Height > dimensions.Height)
					{
						worldIconPosition.Y += (int)(texture.Height - dimensions.Height) / 2 + 2;
						rect = rect with
						{
							Y = (int)((texture.Height - dimensions.Height) / 2f),
							Height = (int)dimensions.Height - 4
						};
					}


					sourceRect = rect;
				}

				spriteBatch.End();
				spriteBatch.Begin(state);

				spriteBatch.Draw(texture, worldIconPosition, sourceRect, Color.White, 0f, texture.Size() / 2f, 1f, SpriteEffects.None, 0);
			}
			
			spriteBatch.End();
			spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, state);

			var clonePlayer = player.PrepareDummy(playerPosition);
			Main.PlayerRenderer.DrawPlayer(Main.Camera, clonePlayer, clonePlayer.position, 0f, player.Size / 2f);

			spriteBatch.End();
			spriteBatch.Begin(state);
		}
	}
}

