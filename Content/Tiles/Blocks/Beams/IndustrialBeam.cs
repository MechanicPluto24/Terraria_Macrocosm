using Macrocosm.Common.DataStructures;
using Macrocosm.Common.TileFrame;
using Macrocosm.Common.Utils;
using Macrocosm.Content.Dusts;
using Microsoft.Xna.Framework;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Tiles.Blocks.Beams;

public class IndustrialBeam : ModTile
{
    public override void SetStaticDefaults()
    {
        TileID.Sets.IsBeam[Type] = true;
        DustType = ModContent.DustType<IndustrialPlatingDust>();
        AddMapEntry(new Color(70, 70, 73));
    }
}