using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Tiles.Blocks.Woods;

public class HeveaWood : ModTile
{
    public override void SetStaticDefaults()
    {
        Main.tileSolid[Type] = true;
        Main.tileBlockLight[Type] = true;
        Main.tileLighted[Type] = true;

        Main.tileMergeDirt[Type] = true;


        // Only to avoid slope slicing, TileFrame code is different
        TileID.Sets.HasSlopeFrames[Type] = true;

        TileID.Sets.ChecksForMerge[Type] = true;



        AddMapEntry(new Color(90, 44, 30));

        HitSound = SoundID.Dig;
        DustType = 7;
    }
}