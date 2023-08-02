using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Terraria.GameContent.UI.Elements;
using Terraria.Localization;
using Macrocosm.Common.UI;
using System;
using System.Linq;
using Terraria;

namespace Macrocosm.Content.Rockets.Customization
{
	public class UINameplateConfig : UIPanel
	{
		public Rocket Rocket;

		private UIInputTextBox uITextBox;

		public string Text => uITextBox.Text;
		public bool TextBoxHasFocus => uITextBox.HasFocus;

		public void SetText(string text) => uITextBox.SetText(text);	

		public Action OnFocusSet = () => { };
		public Func<bool> CheckTextSubmit = () => false;

		public UINameplateConfig()
		{
		}

		public override void OnInitialize()
		{
			Width.Set(0, 0.99f);
			Height.Set(0, 0.08f);
			HAlign = 0.5f;
			BackgroundColor = new Color(53, 72, 135);
			BorderColor = new Color(89, 116, 213, 255);
			SetPadding(0f);

			uITextBox = new(Language.GetText("Mods.Macrocosm.Common.Rocket").Value)
			{
				Width = new(0f, 0.43f),
				Height = new(0f, 0.82f),
				HAlign = 0.02f,
				VAlign = 0.5f,
				BackgroundColor = new Color(53, 72, 135),
				BorderColor = new Color(89, 116, 213, 255),
				HoverBorderColor = Color.Gold,
				TextMaxLenght = Nameplate.MaxChars,
				OnFocusSet = OnFocusSet,
				CheckTextSubmit = CheckTextSubmit,
				TextScale = 1.2f,
				FormatText = (text) => new(text.ToUpperInvariant().Where(Nameplate.SupportedCharacters.Contains).ToArray())
			};
			Append(uITextBox);
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
