using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;

namespace Macrocosm.Common.Utility
{
	public static class MiscUtils
	{
		/// <summary>
		/// Hostile projectiles deal 2x the <paramref name="damage"/> in Normal Mode and 4x the <paramref name="damage"/> in Expert Mode.
		/// This helper method remedies that.
		/// </summary>
		public static int TrueDamage(int damage)
			=> damage / (Main.expertMode ? 4 : 2);


		/// <summary> Gets the dust spritesheet of this vanilla dust type as a rectangle </summary>
		public static Rectangle VanillaDustFrame(int dustType)
		{
			int frameX = dustType * 10 % 1000;
			int frameY = dustType * 10 / 1000 * 30 + Main.rand.Next(3) * 10;
			return new Rectangle(frameX, frameY, 8, 8);
		}

		/// <summary> 
		/// Get the spritesheet of this particular dust type 
		/// Use with dust.frame as source rectangle when drawing 
		/// </summary>
		public static Texture2D GetDustTexture(int dustType)
			 => ModContent.GetModDust(dustType).Texture2D.Value;

		/// <summary> 
		/// Get the spritesheet of this dust instance  
		/// Use with the dust.frame of this instace as sourceRectangle when drawing 
		/// </summary>
		public static Texture2D GetTexture(this Dust dust)
			 => GetDustTexture(dust.type);
	}
}
