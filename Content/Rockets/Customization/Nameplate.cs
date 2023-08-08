using Macrocosm.Common.Utils;
using Macrocosm.Content.Rockets.Modules;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace Macrocosm.Content.Rockets.Customization
{
    public class Nameplate : TagSerializable
    {
        public string Text { get; set; } = "";

        public Color TextColor { get; set; } = Color.White;

        public enum AlignmentModeHorizontal { Left, Center, Right };
        public enum AlignmentModeVertical { Top, Center, Bottom };

        public AlignmentModeHorizontal HorizontalAlignment { get; set; } = AlignmentModeHorizontal.Right;
        public AlignmentModeVertical VerticalAlignment { get; set; } = AlignmentModeVertical.Top;


        public const int MaxChars = 13;

        public void Draw(SpriteBatch spriteBatch, Vector2 position, Color ambientColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>(Macrocosm.TextureAssetsPath + "Fonts/RocketLettersSmall").Value;
            int numChars = (int)MathHelper.Clamp(Text.Length, 0 , MaxChars);

            float offsetX = 6;
            float offsetY = 61;
			int totalTextHeight = numChars * characterHeight;

            switch (HorizontalAlignment)
            {
                case AlignmentModeHorizontal.Left: offsetX = -15; break;
                case AlignmentModeHorizontal.Center: offsetX = -4; break;
                case AlignmentModeHorizontal.Right: offsetX = 7f; break;
            }

			switch (VerticalAlignment)
			{
				case AlignmentModeVertical.Top:
					offsetY = 61; 
					break;
				case AlignmentModeVertical.Center:
					offsetY = 126 - (totalTextHeight / 2); 
					break;
				case AlignmentModeVertical.Bottom:
					offsetY = 191 - totalTextHeight; 
					break;
			}

			for (int i = 0; i < numChars; i++)
			{
				spriteBatch.Draw(texture, new Vector2(position.X + offsetX, position.Y + offsetY + (i * characterHeight)), GetCharacterRectangle(Text[i]), TextColor * ambientColor.GetLuminance(), 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
			}
		}

        public bool HasNoSupportedChars()
        {
            foreach(char c in Text)
                 if (SupportsChar(c))
                    return true;
 
            return false;
        }

		public static bool SupportsChar(char c) => SupportedCharacters.IndexOf(char.ToUpper(c)) != -1;

		public static readonly string SupportedCharacters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-+!? ";

        private const int charactersPerRow = 13;
        private const int characterWidth   = 2 + 6; // with padding
        private const int characterHeight  = 2 + 8; // with padding

		private static Rectangle GetCharacterRectangle(char c)
        {
			int index = SupportedCharacters.IndexOf(char.ToUpper(c));

 			// Character not found in the sheet
			if (index == -1)
				return Rectangle.Empty;

			int row = index / charactersPerRow;
			int column = index % charactersPerRow;

            int x = column * characterWidth + 1;
			int y = row * characterHeight + 1;

			return new Rectangle(x, y, characterWidth, characterHeight);
		}

		public static readonly Func<TagCompound, Nameplate> DESERIALIZER = DeserializeData;

		public TagCompound SerializeData()
		{
			return new()
			{
				[nameof(Text)] = Text,
				[nameof(TextColor)] = TextColor,
				[nameof(HorizontalAlignment)] = (int)HorizontalAlignment,
				[nameof(VerticalAlignment)] = (int)VerticalAlignment,

			};
		}

        public static Nameplate DeserializeData(TagCompound tag)
        {
            Nameplate nameplate = new();

			if (tag.ContainsKey(nameof(Text)))
				nameplate.Text = tag.GetString(nameof(Text));

			if (tag.ContainsKey(nameof(TextColor)))
				nameplate.TextColor = tag.Get<Color>(nameof(TextColor));

			if (tag.ContainsKey(nameof(HorizontalAlignment)))
				nameplate.HorizontalAlignment = (AlignmentModeHorizontal)tag.GetInt(nameof(HorizontalAlignment));

			if (tag.ContainsKey(nameof(VerticalAlignment)))
				nameplate.VerticalAlignment = (AlignmentModeVertical)tag.GetInt(nameof(VerticalAlignment));

            return nameplate;
		}
	}
}
