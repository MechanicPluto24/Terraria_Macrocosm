using Macrocosm.Content.Subworlds.Moon;
using Macrocosm.Content.Tiles;
using Microsoft.Xna.Framework.Graphics;
using SubworldLibrary;
using System;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace Macrocosm.Content {
    class MacrocosmWorld : ModSystem {

        public static int moonBiome = 0;

        public static bool IsDusk => lastDayTime && !Main.dayTime;
        public static bool IsDawn => !lastDayTime && Main.dayTime;

        private static bool lastDayTime = false;

        public override void TileCountsAvailable(ReadOnlySpan<int> tileCounts) {
            moonBiome = tileCounts[ModContent.TileType<Regolith>()];
        }
        public override void ResetNearbyTileEffects() {
            moonBiome = 0;
        }

        public override void PreUpdateEntities() 
        {
            lastDayTime = Main.dayTime;
            if (SubworldSystem.IsActive<Moon>()) {
                Main.time += 0.125; // TODO: maybe replace this with the ModifyTimeRate Hook?
            }
        }
        
        public override void SaveWorldData(TagCompound downed) {
            /*if (Main.gameMenu && !Main.world) {
                Main.sunTexture = Main.instance.OurLoad<Texture2D>("Terraria/Sun");
            }*/
            // FIXME: like now
        }
    }
}
