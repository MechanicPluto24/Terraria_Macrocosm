using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using Terraria;

namespace Macrocosm.Content.UI.Themes
{
	// Not yet sure on how we'll do the implementation
    internal class UITheme
    {
		public Color PanelBackgroundColor { get; set; }
        public Color PanelBorderColor { get; set; }
        public Asset<Texture2D> PanelBackgroundTexture { get; set; }
        public Asset<Texture2D> PanelBorderTexture { get; set; }
        public Color DefaultTextColor { get; set; }

		public UITheme(Color panelBackgroundColor, Color panelBorderColor, Color defaultTextColor = default, Asset<Texture2D> panelBackgroundTexture = null, Asset<Texture2D> panelBorderTexture = null)
		{
			PanelBackgroundColor = panelBackgroundColor;
			PanelBorderColor = panelBorderColor;

			DefaultTextColor = defaultTextColor == default ? Color.White : defaultTextColor;

			PanelBackgroundTexture = (panelBackgroundTexture is null) ? Main.Assets.Request<Texture2D>("Images/UI/PanelBackground") : panelBackgroundTexture;
			PanelBorderTexture = (panelBorderTexture is null) ? Main.Assets.Request<Texture2D>("Images/UI/PanelBorder") : panelBorderTexture;
		}
	}
}
