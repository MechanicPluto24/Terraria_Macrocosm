using Microsoft.Xna.Framework;
using Terraria.ModLoader;

namespace Macrocosm.Content.Rarities
{
    public class MoonRarity2 : ModRarity
    {
        public override Color RarityColor => new(64, 224, 208); // turquoise color 

        public override int GetPrefixedRarity(int offset, float valueMult)
        {
            if (offset < 0)
                return ModContent.RarityType<MoonRarity1>();

            if (offset > 0)
                return ModContent.RarityType<MoonRarity3>();

            return Type;
        }
    }
}
