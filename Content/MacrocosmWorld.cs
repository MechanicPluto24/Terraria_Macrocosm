using Macrocosm.Content.Subworlds.Moon;
using Macrocosm.Content.Tiles;
using Microsoft.Xna.Framework.Graphics;
using SubworldLibrary;
using System;
using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace Macrocosm.Content {
    class MacrocosmWorld : ModSystem {

        public static int moonBiome = 0;

        public override void TileCountsAvailable(ReadOnlySpan<int> tileCounts) {
            moonBiome = tileCounts[ModContent.TileType<Regolith>()];
        }
        public override void ResetNearbyTileEffects() {
            moonBiome = 0;
        }

        public override void PreUpdateEntities()
        {
            int timeRateModifier = CreativePowerManager.Instance.GetPower<CreativePowers.ModifyTimeRate>().TargetTimeRate;

            if (SubworldSystem.IsActive<Moon>()) 
            {
                Main.time += Moon.TimeRate * (Main.fastForwardTime ? 60 : timeRateModifier); 

                // time update logic (behaves weird otherwise)
                if(Main.dayTime && Main.time >= Main.dayLength)
                {
                    Main.dayTime = false;
                    Main.time = 0;
                }
                if (!Main.dayTime && Main.time >= Main.nightLength)
                {
                    Main.dayTime = true;
                    Main.bloodMoon = false; // we can actually control how long a blood moon will be 
                    Main.time = 0;
                }

            }
        }  
    }
}
