using Macrocosm.Content.Dusts;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Tiles.Blocks;

public class SteelPipe : ModTile
{
    public override void SetStaticDefaults()
    {
        Main.tileSolid[Type] = true;
        Main.tileBlockLight[Type] = true;
        Main.tileBrick[Type] = false;

        HitSound = SoundID.Tink;

        AddMapEntry(new Color(103, 120, 138), CreateMapEntryName());

        DustType = ModContent.DustType<SteelDust>();
    }
}
