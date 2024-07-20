using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace Macrocosm.Content.Tiles.Walls
{
    public class IndustrialTrimmingWall : ModWall
    {
        public override void SetStaticDefaults()
        {
            Main.wallHouse[Type] = true;
            AddMapEntry(new Color(71, 71, 74));
        }
    }

    public class IndustrialTrimmingWallUnsafe : IndustrialTrimmingWall
    {
        public override string Texture => base.Texture.Replace("Unsafe", "");

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            Main.wallHouse[Type] = false;
        }
    }
}