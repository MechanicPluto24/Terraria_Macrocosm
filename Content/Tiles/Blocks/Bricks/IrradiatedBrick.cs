using Macrocosm.Content.Dusts;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Tiles.Blocks.Bricks
{
    public class IrradiatedBrick : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileSolid[Type] = true;
            Main.tileBrick[Type] = true;
            Main.tileBlockLight[Type] = true;
            Main.tileLighted[Type] = true;

            MinPick = 225;
            MineResist = 3f;

            AddMapEntry(new Color(129, 117, 0));

            DustType = ModContent.DustType<IrradiatedRockDust>();
            HitSound = SoundID.Tink;
        }
    }
}