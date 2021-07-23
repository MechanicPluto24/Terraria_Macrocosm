using Macrocosm.Content.Subworlds.Moon;
using Macrocosm.Content.Tiles;
using SubworldLibrary;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace Macrocosm.Content
{
    class MacrocosmWorld : ModWorld
    {
        public static int moonBiome = 0;
        public override void TileCountsAvailable(int[] tileCounts)
        {
            moonBiome = tileCounts[ModContent.TileType<Regolith>()];
        }
        public override void ResetNearbyTileEffects()
        {
            moonBiome = 0;
        }
        public override void PreUpdate()
        {
            if (Subworld.IsActive<Moon>())
            {
                Main.time += 0.125;
            }
        }
        public override TagCompound Save()
        {
            if (Main.gameMenu)
            {
                Main.sunTexture = ModContent.GetTexture("Terraria/Sun");
            }
            return null;
        }
    }
}
