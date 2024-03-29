using Macrocosm.Common.Drawing;
using Microsoft.Xna.Framework;
using Terraria.ModLoader;

namespace Macrocosm.Content.Rarities
{
    public class DevRarity : ModRarity
    {
        public override Color RarityColor => CelestialDisco.CelestialColor;

        public override int GetPrefixedRarity(int offset, float valueMult)
        {
            return Type;
        }
    }

    public class DevGlobalItem : ModRarity
    {

    }
}
