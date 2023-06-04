using Macrocosm.Common.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.ModLoader;

namespace Macrocosm.Content.Rocket.Customization
{
    public class Nameplate
    {
        public string Text = "";

        public Color[] charColors;
        public Color TextColor;

        public const int MaxChars = 13;

        public void Draw(SpriteBatch spriteBatch, Vector2 position, Color ambientColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>("Macrocosm/Assets/Textures/RocketLettersSmall").Value;

            // testing
            Text = "Feldy 1337";
            TextColor = Color.Yellow;

            int numChars = (int)MathHelper.Clamp(Text.Length, 0 , MaxChars);

            for(int i = 0; i < numChars; i++)
            {
				spriteBatch.Draw(texture, new Vector2(position.X, position.Y + (i * characterHeight)), GetCharacterRectangle(Text[i]), TextColor * ambientColor.GetLuminance(), 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
			}
		}

		public static bool SupportsChar(char c) => supportedCharacters.IndexOf(char.ToUpper(c)) != -1;

		private static readonly string supportedCharacters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-+!?";

        private static int charactersPerRow = 13;
        private static int characterWidth   = 2 + 6; // with padding
        private static int characterHeight  = 2 + 8; // with padding

		private static Rectangle GetCharacterRectangle(char c)
        {
			int index = supportedCharacters.IndexOf(char.ToUpper(c));

 			// Character not found in the sheet
			if (index == -1)
				return Rectangle.Empty;

			int row = index / charactersPerRow;
			int column = index % charactersPerRow;

			int x = column * characterWidth;
			int y = row * characterHeight;

			return new Rectangle(x, y, characterWidth, characterHeight);
		}
    }
}
