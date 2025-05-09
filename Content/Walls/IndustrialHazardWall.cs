using Macrocosm.Common.Bases.Walls;
using Macrocosm.Common.Enums;
using Macrocosm.Content.Dusts;
using Microsoft.Xna.Framework;
using Terraria.ModLoader;

namespace Macrocosm.Content.Walls
{
    public class IndustrialHazardWall : VariantWall
    {
        public override void SetVariantStaticDefaults(WallSafetyType variant)
        {
            AddMapEntry(new Color(159, 148, 0));
            DustType = ModContent.DustType<IndustrialPlatingDust>();

            if (variant == WallSafetyType.Unsafe)
                RegisterItemDrop(ModContent.ItemType<Items.Walls.IndustrialHazardWallUnsafe>());
            else
                RegisterItemDrop(ModContent.ItemType<Items.Walls.IndustrialHazardWall>());
        }
    }
}