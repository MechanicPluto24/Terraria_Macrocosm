using Microsoft.Xna.Framework;
using Terraria.ModLoader;

namespace Macrocosm.Content.Rarities
{
    public class MoonRarityT2 : ModRarity
    {
        public override Color RarityColor => new(64, 224, 208); // turquoise color 

        public override int GetPrefixedRarity(int offset, float valueMult)
        {
            if (offset < 0)
                return ModContent.RarityType<MoonRarityT1>();

            if (offset > 0)
                return ModContent.RarityType<MoonRarityT3>();

            return Type;
        }
    }
}
