using Macrocosm.Common.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Graphics;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.UI;
using Terraria.UI.Chat;

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
        public int TextMaxLength { get; set; } = 20;
        public bool ForceHideHintText { get; set; } = true;
        public bool AllowSnippets { get; set; }

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

        public override void OnInitialize()
        {
            Width = new(0, 1f);
            Height = new(0, 1f);
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

            if (currentText.Length > TextMaxLength)
                currentText = currentText[..TextMaxLength];

            currentText = FormatText(currentText);

            Color drawColor;
            string drawText;
            if (currentText.Length == 0 && !ForceHideHintText)
            {
                drawColor = HintColor;
                drawText = hintText;
            }
            else
            {
                drawColor = TextColor;
                drawText = currentText;
            }

            CalculatedStyle dimensions = GetDimensions();
            CalculatedStyle innerDimensions = GetInnerDimensions();

            DynamicSpriteFont font = FontAssets.MouseText.Value;
            Vector2 textSize = ChatManager.GetStringSize(font, drawText, new Vector2(1));
            float scale = TextScale;
            if (true && textSize.X > innerDimensions.Width)
                scale *= innerDimensions.Width / textSize.X;

            Vector2 position = innerDimensions.Position() + new Vector2(0, -4f * TextScale);
            Vector2 origin = new Vector2(0f, 0f);

            string visibleText = currentText;
            if (CurrentlyInputtingText)
            {
                if (++textBlinkerCounter / 20 % 2 == 0)
                    visibleText += "|";
            }

            TextSnippet[] snippets = ChatManager.ParseMessage(visibleText, drawColor).ToArray();

            if (!AllowSnippets)
                ChatManager.ConvertNormalSnippets(snippets);

            ChatManager.DrawColorCodedStringShadow(spriteBatch, font, snippets, position, Color.Black.WithAlpha(drawColor.A), 0f, origin, new(scale), -1f, 1.5f);
            ChatManager.DrawColorCodedString(spriteBatch, font, snippets, position, drawColor, 0f, origin, new(scale), out var _, -1f, ignoreColors: true);
        }
    }
}
