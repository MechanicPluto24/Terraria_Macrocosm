using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.UI;

namespace Macrocosm.Common.UI
{
    public class UIInputTextField : UIElement
    {

        private readonly string hintText;
        private string currentText = string.Empty;
        private int textBlinkerCounter;

        public float TextScale { get; set; } = 1f;

        public Color HintColor { get; set; } = Color.Gray;
        public Color TextColor { get; set; } = Color.White;

        public Func<string, string> FormatText { get; set; } = (text) => text;
        public int TextMaxLenght { get; set; } = 20;

        public bool ForceHideHintText { get; set; } = true;

        public bool CurrentlyInputtingText { get; set; } = false;

        public string Text
        {
            get => currentText;
            set
            {
                if (currentText != value)
                {
                    currentText = value;
                    OnTextChange?.Invoke(this, EventArgs.Empty);
                }
            }
        }

        public delegate void EventHandler(object sender, EventArgs e);
        public event EventHandler OnTextChange;

        public UIInputTextField(string hintText)
        {
            this.hintText = hintText;
        }
        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            string newString = currentText;

            if (CurrentlyInputtingText)
            {
                Terraria.GameInput.PlayerInput.WritingText = CurrentlyInputtingText;
                Main.instance.HandleIME();
                newString = Main.GetInputText(currentText);
            }

            if (newString != currentText)
            {
                currentText = newString;
                OnTextChange?.Invoke(this, EventArgs.Empty);
            }

            if (currentText.Length > TextMaxLenght)
                currentText = currentText[..TextMaxLenght];

            currentText = FormatText(currentText);
            string displayString = currentText;

            if (CurrentlyInputtingText)
            {
                if (++textBlinkerCounter / 20 % 2 == 0)
                    displayString += "|";
            }

            CalculatedStyle space = GetDimensions();

            if (currentText.Length == 0 && !ForceHideHintText)
                Terraria.Utils.DrawBorderString(spriteBatch, hintText, new Vector2(space.X, space.Y), HintColor, TextScale, anchory: 0.12f);
            else
                Terraria.Utils.DrawBorderString(spriteBatch, displayString, new Vector2(space.X, space.Y), TextColor, TextScale, anchory: 0.12f);
        }
    }
}
