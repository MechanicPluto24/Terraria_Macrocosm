using Macrocosm.Content.Dusts;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace Macrocosm.Content.Tiles.Walls
{
    [LegacyName("MoonBaseTrimmingWall")]
    public class IndustrialTrimmingWall : ModWall
    {
        public override void SetStaticDefaults()
        {
            Main.wallHouse[Type] = true;
            AddMapEntry(new Color(71, 71, 74));

            DustType = ModContent.DustType<IndustrialPlatingDust>();
        }
    }

    [LegacyName("IndustrialTrimmingWallNatural")]
    public class IndustrialTrimmingWallNaturalNatural : IndustrialTrimmingWall
    {
        public override string Texture => base.Texture.Replace("Natural", "");

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            Main.wallHouse[Type] = false;
            RegisterItemDrop(ModContent.ItemType<Items.Walls.IndustrialTrimmingWall>());
        }
    }

    [LegacyName("MoonBaseTrimmingWallUnsafe")]
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