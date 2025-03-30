using Macrocosm.Common.Bases.Walls;
using Macrocosm.Common.Enums;
using Macrocosm.Content.Dusts;
using Microsoft.Xna.Framework;
using Terraria.ModLoader;

namespace Macrocosm.Content.Walls
{
    public class IrradiatedRockWall : VariantWall
    {
        public override void SetVariantStaticDefaults(WallSafetyType variant)
        {
            AddMapEntry(new Color(64, 64, 58));
            DustType = ModContent.DustType<IrradiatedRockDust>();

            if (variant == WallSafetyType.Unsafe)
                RegisterItemDrop(ModContent.ItemType<Items.Walls.IrradiatedRockWallUnsafe>());
            else
                RegisterItemDrop(ModContent.ItemType<Items.Walls.IrradiatedRockWall>());
        }
    }
}