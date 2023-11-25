using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using Terraria;
using Terraria.GameContent.UI.Elements;

namespace Macrocosm.Common.UI
{
    public class UIInputTextBox : UIPanel, IFocusable
    {
        private UIInputTextField textField;

        public string Text
        {
            get => textField.Text;
            set => textField.Text = value;
        }

        public Color TextColor
        {
            get => textField.TextColor;
            set => textField.TextColor = value;
        }

        public float TextScale { get; set; } = 1f;

        public int TextMaxLenght { get; set; } = 20;

        public bool HasFocus { get; set; }
        public string FocusContext { get; set; }

        public Action OnFocusGain { get; set; } = () => { };
        public Action OnFocusLost { get; set; } = () => { };

        public Action OnTextChange { get; set; } = () => { };

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

            PaddingLeft = 8f;
            PaddingRight = 4f;

            Append(textField);

            textField.OnTextChange += (_, _) => { OnTextChange.Invoke(); };

            textField.FormatText = FormatText;
            textField.TextMaxLenght = TextMaxLenght;

            OnLeftClick += (_, _) => { HasFocus = true; };

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
    }
}
