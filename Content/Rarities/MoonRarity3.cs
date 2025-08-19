using Microsoft.Xna.Framework;
using Terraria.ModLoader;

namespace Macrocosm.Content.Rarities;

public class MoonRarity3 : ModRarity
{
    public override Color RarityColor => new(220, 20, 60); // crimson color 

    public override int GetPrefixedRarity(int offset, float valueMult)
    {
        if (offset < 0)
            return ModContent.RarityType<MoonRarity2>();

        // maybe return MarsRarity1 if (offset > 0) 

        return Type;
    }
}
