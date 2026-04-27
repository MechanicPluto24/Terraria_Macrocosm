using Macrocosm.Common.Bases.Walls;
using Macrocosm.Common.Enums;
using Macrocosm.Content.Dusts;
using Microsoft.Xna.Framework;
using Terraria.ModLoader;

namespace Macrocosm.Content.Walls;

public class AstrolithWall : VariantWall
{
    public override void SetVariantStaticDefaults(WallSafetyType variant)
    {
        AddMapEntry(new Color(60, 55, 58));
        DustType = ModContent.DustType<AstrolithDust>();

        if (variant == WallSafetyType.Unsafe)
            RegisterItemDrop(ModContent.ItemType<Items.Walls.AstrolithWallUnsafe>());
        else
            RegisterItemDrop(ModContent.ItemType<Items.Walls.AstrolithWall>());
    }
}
