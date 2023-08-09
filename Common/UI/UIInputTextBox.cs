using Terraria.UI;
using Terraria.GameContent.UI.Elements;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameInput;
using Terraria;
using System.Linq;
using Microsoft.Xna.Framework.Input;
using System;
using Terraria.Localization;

namespace Macrocosm.Common.UI
{
	public class UIInputTextBox : UIPanel
	{
		private UIInputTextField textField;

		public string Text => textField.Text;

		public void SetText(string text) => textField.Text = text;

		public float TextScale { get; set; } = 1f;

		public int TextMaxLenght { get; set; } = 20;

		public bool HasFocus { get; set; }

		public Action OnFocusGain { get; set; }

		public Action OnTextChange { get; set; }

		public Func<string, string> FormatText { get; set; } = (text) => text;

 		public Color? HoverBorderColor { get; set; }
		private Color normalBorderColor;


		public UIInputTextBox(string defaultText)
		{
			textField = new(defaultText);
		}

		public override void OnInitialize()
		{
			base.OnInitialize();

			PaddingLeft = 4f;
			PaddingRight = 4f;

			Append(textField);
			textField.OnTextChange += (_, _) => { OnTextChange?.Invoke(); };
			textField.FormatText = FormatText;
			textField.TextMaxLenght = TextMaxLenght;
			OnLeftClick += UIInputTextBox_OnLeftClick;

			normalBorderColor = BorderColor;
			HoverBorderColor ??= BorderColor;
		}

		public override void Update(GameTime gameTime)
		{
			base.Update(gameTime);

			if (HasFocus)
			{
				if (Main.keyState.IsKeyDown(Keys.Escape) && !Main.oldKeyState.IsKeyDown(Keys.Escape))
					HasFocus = false;
			}

			Main.blockInput = HasFocus;
			textField.CurrentlyInputtingText = HasFocus;
			textField.ForceHideHintText = HasFocus;
			textField.TextScale = TextScale;

			if (IsMouseHovering || HasFocus)
				BorderColor = HoverBorderColor.Value;
			else
				BorderColor = normalBorderColor;
		}

		private void UIInputTextBox_OnLeftClick(UIMouseEvent evt, UIElement listeningElement)
		{
			if(!HasFocus)
				OnFocusGain?.Invoke();

			HasFocus = true;
		}

		public override void Draw(SpriteBatch spriteBatch)
		{
			base.Draw(spriteBatch);
		}
	}
}
