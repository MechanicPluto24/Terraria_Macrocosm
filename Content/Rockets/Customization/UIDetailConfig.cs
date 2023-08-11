using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Terraria.GameContent.UI.Elements;

namespace Macrocosm.Content.Rockets.Customization
{
	public class UIDetailConfig : UIPanel
	{
		public Rocket Rocket;

		public UIDetailConfig()
		{
		}

		public override void OnInitialize()
		{
			Width.Set(0, 0.99f);
			Height.Set(0, 0.22f);
			HAlign = 0.5f;
			Top.Set(0f, 0.36f);
			BackgroundColor = new Color(53, 72, 135);
			BorderColor = new Color(89, 116, 213, 255);
			SetPadding(0f);
		}


		public override void Update(GameTime gameTime)
		{
			base.Update(gameTime);
		}

		public override void Draw(SpriteBatch spriteBatch)
		{
			base.Draw(spriteBatch);
		}
	}
}
