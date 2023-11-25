using Macrocosm.Common.DataStructures;
using Macrocosm.Common.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Linq;
using Terraria.ModLoader;

namespace Macrocosm.Content.Rockets.Customization
{
	public partial class Nameplate 
    {
        // the text
        private string text = "";

        /// <summary> The display text, formatted </summary>
        public string Text
        {
            get => text;
            set => text = FormatText(value);
        }

        /// <summary> The nameplate text color on the rocket hull </summary>
        public Color TextColor { get; set; } = Color.White;

		/// <summary> The nameplate's horizontal text alignment on the rocket </summary>
		public TextHorizontalAlign HAlign { get; set; } = TextHorizontalAlign.Right;

		/// <summary> The nameplate's vertical text alignment on the rocket </summary>
		public TextVerticalAlign VAlign { get; set; } = TextVerticalAlign.Top;

        /// <summary> Max number of characters supported on the nameplate </summary>
        public const int MaxChars = 13;

        /// <summary> The characters that can bew displayed on the rocket's hull </summary>
        public static readonly string DisplayCharacters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-+!? ";

        /// <summary> The characters supported by the rocket's name. Lowercase letters are displayed as uppercase on the hull </summary>
        public static readonly string SupportedCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-+!? ";

        // Number of characters por row in the spritesheet
        private const int charactersPerRow = 13;

        // A character's dimensions in the spritesheet, with padding
        private const int characterWidth = 2 + 6;
        private const int characterHeight = 2 + 8;

        /// <summary> Formats the input text so the output can only contain the supported characters (including lowercase letters) </summary>
        public static string FormatText(string text) => new(text.Where(SupportedCharacters.Contains).ToArray());

        /// <summary> Whether the rocket's name supports this character </summary>
        public static bool SupportsChar(char c) => SupportedCharacters.IndexOf(c) != -1;

        public void Draw(SpriteBatch spriteBatch, Vector2 position, Color ambientColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>(Macrocosm.TextureAssetsPath + "Fonts/RocketLettersSmall").Value;
            int numChars = (int)MathHelper.Clamp(text.Length, 0, MaxChars);

            float offsetX = 6;
            float offsetY = 61;
            int totalTextHeight = numChars * characterHeight;

            switch (HAlign)
            {
                case TextHorizontalAlign.Left: 
					offsetX = -15;
					break;
                case TextHorizontalAlign.Center: 
					offsetX = -4;
					break;
                case TextHorizontalAlign.Right:
					offsetX = 7f;
					break;
            }

			switch (VAlign)
			{
				case TextVerticalAlign.Top:
					offsetY = 61; 
					break;
				case TextVerticalAlign.Center:
					offsetY = 126 - (totalTextHeight / 2); 
					break;
				case TextVerticalAlign.Bottom:
					offsetY = 191 - totalTextHeight; 
					break;
			}

            for (int i = 0; i < numChars; i++)
            {
                spriteBatch.Draw(texture, new Vector2(position.X + offsetX, position.Y + offsetY + (i * characterHeight)), GetCharacterRectangle(text[i]), TextColor * ambientColor.GetLuminanceNTSC(), 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
            }
        }

        /// <summary> Whether the current rocket name is valid </summary>
        public bool IsValid()
        {
            bool foundNonWhitespace = false;

            foreach (char c in text)
            {
                if (!SupportsChar(c))
                    return false;

                if (c != ' ')
                    foundNonWhitespace = true;
            }

            return foundNonWhitespace;
        }

        private static Rectangle GetCharacterRectangle(char c)
        {
            int index = DisplayCharacters.IndexOf(char.ToUpper(c));

            // Character not found in the sheet
            if (index == -1)
                return Rectangle.Empty;

            int row = index / charactersPerRow;
            int column = index % charactersPerRow;

            int x = column * characterWidth + 1;
            int y = row * characterHeight + 1;

			return new Rectangle(x, y, characterWidth, characterHeight);
		}
	}
}
