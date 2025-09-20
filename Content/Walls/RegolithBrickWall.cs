using Macrocosm.Common.Bases.Walls;
using Macrocosm.Common.Enums;
using Macrocosm.Content.Dusts;
using Microsoft.Xna.Framework;
using Terraria.ModLoader;

namespace Macrocosm.Content.Walls;

public class RegolithBrickWall : VariantWall
{
    public override void SetVariantStaticDefaults(WallSafetyType variant)
    {
        AddMapEntry(new Color(145, 145, 145));
        DustType = ModContent.DustType<RegolithDust>();

        if (variant == WallSafetyType.Unsafe)
            RegisterItemDrop(ModContent.ItemType<Items.Walls.RegolithBrickWallUnsafe>());
        else
            RegisterItemDrop(ModContent.ItemType<Items.Walls.RegolithBrickWall>());
    }
}