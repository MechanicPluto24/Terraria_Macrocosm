using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace Macrocosm.Content.Tiles.Blocks
{
    public class RegolithBrick : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileSolid[Type] = true;
            Main.tileBrick[Type] = true;
            Main.tileBlockLight[Type] = true;
            Main.tileLighted[Type] = true;
            Main.tileMergeDirt[Type] = true;
            MinPick = 225;
            MineResist = 3f;
            AddMapEntry(new Color(165, 165, 165));
        }
    }
}