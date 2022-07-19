using Macrocosm.Backgrounds.Moon;
using Macrocosm.Content.Subworlds.Moon;
using Macrocosm.Content.Tiles;
using Microsoft.Xna.Framework.Graphics;
using SubworldLibrary;
using System;
using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;


namespace Macrocosm.Content {
    class MacrocosmWorld : ModSystem {

        public static int moonBiome = 0;

        public override void TileCountsAvailable(ReadOnlySpan<int> tileCounts) {
            moonBiome = tileCounts[ModContent.TileType<Regolith>()];
        }
        public override void ResetNearbyTileEffects() {
            moonBiome = 0;
        }

        public override void PreUpdateWorld()
        {
            if (SubworldSystem.AnyActive<Macrocosm>())
            {
                // time update logic (behaves weird otherwise)
                //if (Main.dayTime && Main.time >= Main.dayLength)
                //{
                //    Main.dayTime = false;
                //    Main.time = 0;
                //    OnDusk();
                //}
                //if (!Main.dayTime && Main.time >= Main.nightLength)
                //{
                //    Main.dayTime = true;
                //    Main.bloodMoon = false; // we can actually control how long a blood moon will be 
                //    Main.time = 0;
                //    OnDawn();
                //}

                Main.numClouds = 0;
                Main.windSpeedCurrent = 0;
                Main.weatherCounter = 0;

                Main.StopRain(); // Rain, rain, go away, come again another day
                Main.StopSlimeRain();
            }

        }

        public override void ModifyTimeRate(ref double timeRate, ref double tileUpdateRate, ref double eventUpdateRate)
        {
            int timeRateModifier = CreativePowerManager.Instance.GetPower<CreativePowers.ModifyTimeRate>().TargetTimeRate;
            bool freezeTime = CreativePowerManager.Instance.GetPower<CreativePowers.FreezeTime>().Enabled;

            if (SubworldSystem.IsActive<Moon>())
            {
                timeRate = freezeTime ? 0f : Moon.TimeRate * (Main.fastForwardTime ? 60 : timeRateModifier); 
            }
        }

    }
}
