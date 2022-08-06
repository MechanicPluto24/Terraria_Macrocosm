using Microsoft.Xna.Framework;
using Terraria;

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

		public static Rectangle VanillaDustFrame(int dustId)
		{
			int frameX = dustId * 10 % 1000;
			int frameY = dustId * 10 / 1000 * 30 + Main.rand.Next(3) * 10;
			return new Rectangle(frameX, frameY, 8, 8);
		}
	}
}
