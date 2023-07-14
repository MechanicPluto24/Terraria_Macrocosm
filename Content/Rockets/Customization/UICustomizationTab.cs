using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent.UI.Elements;
 
namespace Macrocosm.Content.Rockets.Navigation
{
    public class UICustomizationTab : UIPanel
    {
        public Rocket Rocket;

		public UICustomizationTab()
		{
		}

		public override void OnInitialize()
        {
			Width.Set(0, 1f);
			Height.Set(0, 1f);
			HAlign = 0.5f;
			VAlign = 0.5f;

			SetPadding(2f);

			BackgroundColor = new Color(13, 23, 59, 127);
			BorderColor = new Color(15, 15, 15, 255);

			Append(new UIText("test")
			{
				Top = new(0, 0.5f),
				Left = new(0, 0.5f),
				Width = new(0, 0.2f),
				Height = new(0, 0.2f)
			});

		}

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
		}

		public override void Draw(SpriteBatch spriteBatch)
		{
			base.Draw(spriteBatch);
			Rocket.DrawDummy(spriteBatch, -GetDimensions().Position(), Color.White);
		}
	}
}
