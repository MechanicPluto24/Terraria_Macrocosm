using Macrocosm.Common.Bases.Walls;
using Macrocosm.Common.Enums;
using Macrocosm.Content.Dusts;
using Microsoft.Xna.Framework;
using Terraria.ModLoader;

namespace Macrocosm.Content.Walls
{
    public class SeleniteBrickWall : ModWall
    {
        public override void SetStaticDefaults()
        {
            AddMapEntry(new Color(170, 170, 180));
            DustType = ModContent.DustType<IrradiatedRockDust>();
        }
    }
}