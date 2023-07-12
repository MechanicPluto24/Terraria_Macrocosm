using Macrocosm.Common.Drawing;
using Microsoft.Xna.Framework;
using Terraria.ModLoader;

namespace Macrocosm.Content.Rarities
{
	public class DevRarity : ModRarity
	{
		public override Color RarityColor => GlobalVFX.CelestialColor;

		public override int GetPrefixedRarity(int offset, float valueMult)
		{
			return Type;
		}
	}

	public class DevGlobalItem : ModRarity
	{

	}
}
