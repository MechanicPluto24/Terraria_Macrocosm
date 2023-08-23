using Macrocosm.Common.UI;
using Microsoft.Xna.Framework;
using Terraria.GameContent.UI.Elements;
using Terraria.UI;

namespace Macrocosm.Content.Rockets.Payload
{
    public class UIPayloadTab : UIPanel, ITabUIElement, IRocketDataConsumer
    {
        public Rocket Rocket { get; set; }  

		public UIPayloadTab()
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

			Append(new UIText("Payload")
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
	}
}
