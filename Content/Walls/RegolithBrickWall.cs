using Macrocosm.Content.Dusts;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace Macrocosm.Content.Walls
{
    public class RegolithBrickWall : ModWall
    {
        public override void SetStaticDefaults()
        {
            Main.wallHouse[Type] = true;
            AddMapEntry(new Color(145, 145, 145));

            DustType = ModContent.DustType<RegolithDust>();
        }
    }

    public class RegolithBrickWallNatural : RegolithBrickWall
    {
        public override string Texture => base.Texture.Replace("Natural", "");

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            Main.wallHouse[Type] = false;
            RegisterItemDrop(ModContent.ItemType<Items.Walls.RegolithBrickWall>());
        }
    }

    public class RegolithBrickWallUnsafe : RegolithBrickWall
    {
        public override string Texture => base.Texture.Replace("Unsafe", "");

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            Main.wallHouse[Type] = false;
        }
    }
}