using Macrocosm.Content.Dusts;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Tiles.Ores
{
    public class Coal : ModTile
    {
        public override void SetStaticDefaults()
        {
            TileID.Sets.Ore[Type] = true;
            Main.tileSpelunker[Type] = true;
            Main.tileOreFinderPriority[Type] = 190;
            Main.tileSolid[Type] = true;
            Main.tileBlockLight[Type] = true;

            Main.tileMergeDirt[Type] = true;

            AddMapEntry(new Color(32, 31, 33), CreateMapEntryName());

            DustType = ModContent.DustType<CoalDust>();
            HitSound = SoundID.Tink;

            MinPick = 35;
        }
    }
}