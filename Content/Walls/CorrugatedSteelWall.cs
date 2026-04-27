using Macrocosm.Common.Bases.Walls;
using Macrocosm.Common.Enums;
using Macrocosm.Content.Dusts;
using Microsoft.Xna.Framework;
using Terraria.ModLoader;

namespace Macrocosm.Content.Walls;

public class CorrugatedSteelWall : VariantWall
{
    public override void SetVariantStaticDefaults(WallSafetyType variant)
    {
        AddMapEntry(new Color(115, 132, 143));
        DustType = ModContent.DustType<SteelDust>();

        if (variant == WallSafetyType.Unsafe)
            RegisterItemDrop(ModContent.ItemType<Items.Walls.CorrugatedSteelWallUnsafe>());
        else
            RegisterItemDrop(ModContent.ItemType<Items.Walls.CorrugatedSteelWall>());
    }
}
