using Microsoft.Xna.Framework;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Rarities
{
    public class MoonRarityT1 : ModRarity
    {
        public override Color RarityColor => new(203, 227, 21); // acid color 

        public override int GetPrefixedRarity(int offset, float valueMult)
        {
            if (offset < 0)
                return ItemRarityID.Purple;

            if (offset > 0)
                return ModContent.RarityType<MoonRarityT2>();

            return Type;
        }
    }
}
