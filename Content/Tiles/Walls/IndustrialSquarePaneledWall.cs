using Macrocosm.Content.Dusts;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace Macrocosm.Content.Tiles.Walls
{
    [LegacyName("MoonBaseSquarePaneledWall")]
    public class IndustrialSquarePaneledWall : ModWall
    {
        public override void SetStaticDefaults()
        {
            Main.wallHouse[Type] = true;
            AddMapEntry(new Color(71, 71, 74));

            DustType = ModContent.DustType<IndustrialPlatingDust>();
        }
    }

    [LegacyName("MoonBaseSquarePaneledWallNatural")]
    public class IndustrialSquarePaneledWallNatural : IndustrialSquarePaneledWall
    {
        public override string Texture => base.Texture.Replace("Natural", "");

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            Main.wallHouse[Type] = false;
            RegisterItemDrop(ModContent.ItemType<Items.Walls.IndustrialSquarePaneledWall>());
        }
    }

    [LegacyName("MoonBaseSquarePaneledWallUnsafe")]
    public class IndustrialSquarePaneledWallUnsafe : IndustrialSquarePaneledWall
    {
        public override string Texture => base.Texture.Replace("Unsafe", "");

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            Main.wallHouse[Type] = false;
        }
    }
}