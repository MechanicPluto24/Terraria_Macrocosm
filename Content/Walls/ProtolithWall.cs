using Macrocosm.Common.Bases.Walls;
using Macrocosm.Common.Enums;
using Macrocosm.Content.Dusts;
using Microsoft.Xna.Framework;
using Terraria.ModLoader;

namespace Macrocosm.Content.Walls
{
    public class ProtolithWall : VariantWall
    {
        public override void SetVariantStaticDefaults(WallSafetyType variant)
        {
            AddMapEntry(new Color(25, 25, 25));
            DustType = ModContent.DustType<ProtolithDust>();

            if (variant == WallSafetyType.Unsafe)
                RegisterItemDrop(ModContent.ItemType<Items.Walls.ProtolithWallUnsafe>());
            else
                RegisterItemDrop(ModContent.ItemType<Items.Walls.ProtolithWall>());
        }
    }
}