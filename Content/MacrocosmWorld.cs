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
                Main.time += 0.125 * (Main.fastForwardTime ? 60 : timeRateModifier); // TODO: maybe replace this with the ModifyTimeRate Hook?
            }
        }

        //public override void ModifyTimeRate(ref double timeRate, ref double tileUpdateRate, ref double eventUpdateRate)
       
    }
}
