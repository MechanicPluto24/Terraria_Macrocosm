using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace Macrocosm.Content.Tiles.Blocks
{
    public class LexanGlass : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileSolid[Type] = true;
            Main.tileBlockLight[Type] = false;
            Main.tileLighted[Type] = true;
            Main.tileMergeDirt[Type] = false;

            MinPick = 100;
            MineResist = 2f;

            AddMapEntry(new Color(50, 50, 50), CreateMapEntryName());
        }

        public override bool CanExplode(int i, int j)
        {
            return false;
        }
    }
}