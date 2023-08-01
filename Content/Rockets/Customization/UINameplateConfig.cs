using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Terraria.GameContent.UI.Elements;
using Macrocosm.Common.Utils;
using Terraria.Localization;
using Terraria;
using Terraria.GameInput;
using Terraria.Audio;
using Terraria.GameContent.UI.States;
using Terraria.UI;
using Terraria.ID;

namespace Macrocosm.Content.Rockets.Customization
{
	public class UINameplateConfig : UIPanel
	{
		public Rocket Rocket;


		private UICharacterNameButton uITextBox;

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

			uITextBox = new(LocalizedText.Empty, Language.GetText("Mods.Macrocosm.Common.Rocket"))
			{
				Width = new(0f, 0.4f),
				HAlign = 0.01f,
				VAlign = 0.5f,
				//BackgroundColor = new Color(53, 72, 135),
				//BorderColor = new Color(89, 116, 213, 255),
				//TextColor = Color.White,
			};

			uITextBox.OnLeftMouseDown += Click_Naming;

			Append(uITextBox);
		}

		public override void Update(GameTime gameTime)
		{
			base.Update(gameTime);

			//var display = Utility.GetLocalizedTextOrEmpty("Mods.Macrocosm.Common." + Rocket.DisplayName);
			//
			//if(display == LocalizedText.Empty)
			//	uITextBox.SetText(Rocket.DisplayName);
			//else
			//	uITextBox.SetText(display.Value);

		}

		private void Click_Naming(UIMouseEvent evt, UIElement listeningElement)
		{
			Main.clrInput();
			UIVirtualKeyboard uIVirtualKeyboard = new UIVirtualKeyboard(Lang.menu[45].Value, "", (_) => { }, () => { }, 0, allowEmpty: true);
			uIVirtualKeyboard.SetMaxInputLength(20);
			Main.MenuUI.SetState(uIVirtualKeyboard);
		}

		public override void Draw(SpriteBatch spriteBatch)
		{
			base.Draw(spriteBatch);
		}
	}
}
