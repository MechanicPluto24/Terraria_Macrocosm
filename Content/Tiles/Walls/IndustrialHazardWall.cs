using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace Macrocosm.Content.Tiles.Walls
{
    [LegacyName("MoonBaseHazardWall")]
    public class IndustrialHazardWall : ModWall
    {
        public override void SetStaticDefaults()
        {
            Main.wallHouse[Type] = true;
            AddMapEntry(new Color(159, 148, 0));
        }
    }

    [LegacyName("MoonBaseHazardWallUnsafe")]
    public class IndustrialHazardWallUnsafe : IndustrialHazardWall
    {
        public override string Texture => base.Texture.Replace("Unsafe", "");

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            Main.wallHouse[Type] = false;
        }
    }
}