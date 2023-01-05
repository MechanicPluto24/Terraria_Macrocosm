using Macrocosm.Common.Drawing;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.UI;

namespace Macrocosm.Content.UI.Rocket
{
	public class UINavigationZoomButton : UIElement
	{
		public Texture2D ButtonTexture;
		public Texture2D BorderTexture;

		public UINavigationZoomButton(Texture2D buttonTex, Texture2D borderTex, Vector2 position)
		{
			ButtonTexture = buttonTex;
			BorderTexture = borderTex;

			Left.Set(position.X, 0f);
			Top.Set(position.Y, 0f);
		}

		public override void OnInitialize()
		{
			Width.Set(ButtonTexture.Width, 0);
			Height.Set(ButtonTexture.Height, 0);
		}

		public override void Draw(SpriteBatch spriteBatch)
		{
			Color buttonColor;
			Color borderColor;

			if (IsMouseHovering)
			{
				buttonColor = new Color(230, 230, 230);
				borderColor = Color.White;
			}
			else
			{
				buttonColor = Color.White;
				borderColor = new Color(57, 66, 73);
			}

			spriteBatch.Draw(ButtonTexture, GetDimensions().ToRectangle(), buttonColor);
			spriteBatch.Draw(BorderTexture, GetDimensions().ToRectangle(), borderColor);
		}

	}
}
