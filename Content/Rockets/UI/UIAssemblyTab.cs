using Macrocosm.Common.UI;
using Macrocosm.Common.UI.Themes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent.UI.Elements;

namespace Macrocosm.Content.Rockets.UI
{
	public class UIAssemblyTab : UIPanel, ITabUIElement, IRocketUIDataConsumer
	{
		public Rocket Rocket { get; set; } = new();

		public UIAssemblyTab()
		{
		}

		public override void OnInitialize()
		{
			Width.Set(0, 1f);
			Height.Set(0, 1f);
			HAlign = 0.5f;
			VAlign = 0.5f;

			SetPadding(3f);

			BackgroundColor = UITheme.Current.TabStyle.BackgroundColor;
			BorderColor = UITheme.Current.TabStyle.BorderColor;
		}

		public override void OnDeactivate()
		{
		}

		public override void Update(GameTime gameTime)
		{
			base.Update(gameTime);
		}

		public override void Draw(SpriteBatch spriteBatch)
		{
			base.Draw(spriteBatch);

			var dims = GetDimensions();
			Rocket.Draw(Rocket.DrawMode.Blueprint, spriteBatch, dims.Center() - Rocket.Bounds.Size() / 2f);
		}
	}
}
