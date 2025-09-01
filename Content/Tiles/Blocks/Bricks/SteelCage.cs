using Macrocosm.Common.Sets;
using Macrocosm.Content.Dusts;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Tiles.Blocks.Bricks;

public class SteelCage : ModTile
{
    public override void SetStaticDefaults()
    {
        Main.tileSolid[Type] = true;
        Main.tileBlockLight[Type] = false;

        TileSets.AllowLiquids[Type] = true;

        AddMapEntry(new Color(103, 120, 138));

        DustType = ModContent.DustType<SteelDust>();
        HitSound = SoundID.Tink;
    }
}