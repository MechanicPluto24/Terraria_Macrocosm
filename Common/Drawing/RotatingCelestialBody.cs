using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.GameContent;

namespace Macrocosm.Common.Drawing
{
	public static class RotatingCelestialBody
	{
		public static Vector2 pos;

		public static void Draw(Texture2D texture, bool dayTime)
		{
			int sunWidth = TextureAssets.Sun.Width();
			double duration = Main.dayTime ? Main.dayLength : Main.nightLength;

			int angle;
			float scale;
			int timeX = (int)(Main.time / duration * (Main.screenWidth + sunWidth * 2)) - sunWidth;
			double timeY = Main.time < duration / 2 ? //Gets the Y axis for the angle depending on the time
					Math.Pow((Main.time / duration - 0.5) * 2.0, 2.0) : //AM
					Math.Pow(1.0 - Main.time / duration * 2.0, 2.0); //PM
			float rotation = (float)(Main.time / duration) * 2f - 7.3f;
			double bgTop = -Main.screenPosition.Y / (Main.worldSurface * 16.0 - 600.0) * 200.0;

			//PLEASE DON'T NAME YOUR VARS LIKE "NUM474" EVER AGAIN OR I WILL FCKING RIP YOUR GUTS OUT AND EAT NACHOS FROM YOUR RIBCAGE lol
			float clouldAlphaMult = 1f - Main.cloudAlpha * 1.5f;
			if (clouldAlphaMult < 0f)
			{
				clouldAlphaMult = 0f;
			}

			angle = (int)(bgTop + timeY * 250.0 + 180.0);

			scale = (float)(1.2 - timeY * 0.4);

			Color color = new Color((byte)(255f * clouldAlphaMult), (byte)(Color.White.G * clouldAlphaMult), (byte)(Color.White.B * clouldAlphaMult), (byte)(255f * clouldAlphaMult));

			Vector2 position = new Vector2(timeX, angle + Main.sunModY);
			pos = position;


			if (dayTime == Main.dayTime)
			{
				Main.spriteBatch.Draw(texture, position, new Microsoft.Xna.Framework.Rectangle?(new Rectangle(0, 0, texture.Width, texture.Height)),
					color, rotation, new Vector2(texture.Width / 2, texture.Height / 2), scale, SpriteEffects.None, 0f);
			}
		}
	}
}