using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent.UI.Elements;
 
namespace Macrocosm.Content.Rockets.Navigation
{
    public class UICustomizationTab : UIPanel
    {
        public Rocket Rocket;

		public UIPanel RocketPreviewBackground;
		public UIRocketPreviewLarge RocketPreview;

		public UICustomizationTab()
		{
		}

		public override void OnInitialize()
        {
			Width.Set(0, 1f);
			Height.Set(0, 1f);
			HAlign = 0.5f;
			VAlign = 0.5f;

			SetPadding(14f);

			BackgroundColor = new Color(13, 23, 59, 127);
			BorderColor = new Color(15, 15, 15, 255);

			RocketPreviewBackground = new()
			{
				Width = new(0, 0.4f),
				Height = new(0, 1f),
				Left = new (0,0.6f),
				HAlign = 0f,
				BackgroundColor = new Color(53, 72, 135),
				BorderColor = new Color(89, 116, 213, 255)
			};
			RocketPreviewBackground.SetPadding(2f);
			RocketPreviewBackground.OverflowHidden = true;
			Append(RocketPreviewBackground);

			RocketPreview = new();
			RocketPreviewBackground.Append(RocketPreview);
			RocketPreview.Activate();
		}

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

			RocketPreview.Rocket = Rocket;
		}

		public override void Draw(SpriteBatch spriteBatch)
		{
			base.Draw(spriteBatch);
		}
	}
}
