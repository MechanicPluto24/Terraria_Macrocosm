using Macrocosm.Common.Bases.Walls;
using Macrocosm.Common.Enums;
using Macrocosm.Content.Dusts;
using Microsoft.Xna.Framework;
using Terraria.ModLoader;

namespace Macrocosm.Content.Walls;

public class IndustrialSquarePaneledWall : VariantWall
{
    public override void SetVariantStaticDefaults(WallSafetyType variant)
    {
        AddMapEntry(new Color(71, 71, 74));
        DustType = ModContent.DustType<IndustrialPlatingDust>();

        if (variant == WallSafetyType.Unsafe)
            RegisterItemDrop(ModContent.ItemType<Items.Walls.IndustrialSquarePaneledWallUnsafe>());
        else
            RegisterItemDrop(ModContent.ItemType<Items.Walls.IndustrialSquarePaneledWall>());
    }
}