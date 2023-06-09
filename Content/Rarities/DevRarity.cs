using Macrocosm.Common.Drawing;
using Macrocosm.Content.Systems;
using Microsoft.Xna.Framework;
using Terraria.ID;
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
}
