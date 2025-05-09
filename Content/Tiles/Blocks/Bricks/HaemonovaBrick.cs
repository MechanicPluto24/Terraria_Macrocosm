using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Tiles.Blocks.Bricks
{
    public class HaemonovaBrick : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileSolid[Type] = true;
            Main.tileBrick[Type] = true;
            Main.tileBlockLight[Type] = true;

            AddMapEntry(new Color(78, 25, 25));

            //DustType = ModContent.DustType<HaemonovaDust>();
            DustType = DustID.LunarOre;
            HitSound = SoundID.Tink;
        }
    }
}