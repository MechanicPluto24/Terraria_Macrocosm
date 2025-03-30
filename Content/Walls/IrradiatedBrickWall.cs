using Macrocosm.Common.Bases.Walls;
using Macrocosm.Common.Enums;
using Macrocosm.Content.Dusts;
using Microsoft.Xna.Framework;
using Terraria.ModLoader;

namespace Macrocosm.Content.Walls
{
    public class IrradiatedBrickWall : VariantWall
    {
        public override void SetVariantStaticDefaults(WallSafetyType variant)
        {
            AddMapEntry(new Color(199, 199, 184));
            DustType = ModContent.DustType<IrradiatedRockDust>();

            if (variant == WallSafetyType.Unsafe)
                RegisterItemDrop(ModContent.ItemType<Items.Walls.IrradiatedBrickWallUnsafe>());
            else
                RegisterItemDrop(ModContent.ItemType<Items.Walls.IrradiatedBrickWall>());
        }
    }
}