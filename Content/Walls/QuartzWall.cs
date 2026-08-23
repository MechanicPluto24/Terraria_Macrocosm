using Macrocosm.Common.Bases.Walls;
using Macrocosm.Common.Enums;
using Macrocosm.Content.Dusts;
using Microsoft.Xna.Framework;
using Terraria.ModLoader;

namespace Macrocosm.Content.Walls;

public class QuartzWall : VariantWall
{
    public override void SetVariantStaticDefaults(WallSafetyType variant)
    {
        AddMapEntry(new Color(255, 255, 255));
        DustType = ModContent.DustType<QuartzDust>();

        if (variant == WallSafetyType.Unsafe)
            RegisterItemDrop(ModContent.ItemType<Items.Walls.QuartzWallUnsafe>());
        else
            RegisterItemDrop(ModContent.ItemType<Items.Walls.QuartzWall>());
    }
}
