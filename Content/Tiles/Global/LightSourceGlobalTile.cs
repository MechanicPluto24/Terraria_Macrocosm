using SubworldLibrary;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace Macrocosm.Content.Tiles.Global
{
    public class LightSourceGlobalTile : GlobalTile
    {
        /// <summary>
        /// Turns off placed torches, campfires and other blocks with fire when placed on our subworlds 
        /// </summary>
        public override bool TileFrame(int i, int j, int type, ref bool resetFrame, ref bool noBreak)
        {
            if (SubworldSystem.AnyActive<Macrocosm>() && IsTileWithFlame(i, j, type))
            {
                WorldGen.TryToggleLight(i, j, false, skipWires: false);
            }
            return true;
        }

        /// <summary>
        /// Disables wiring on our subworlds for chosen light sources 
        /// </summary>
        public override bool PreHitWire(int i, int j, int type) => !(SubworldSystem.AnyActive<Macrocosm>() && IsTileWithFlame(i, j, type));


        /// <summary>
        /// Disables right clicking on campfires on our subworlds 
        /// </summary>
        public override void RightClick(int i, int j, int type)
        {
            if (SubworldSystem.AnyActive<Macrocosm>() && type == TileID.Campfire)
            {
                WorldGen.TryToggleLight(i, j, false, skipWires: false);
            }
        }

        /// <summary>
        /// Returns true if tile[i,j] is a block with flames 
        /// </summary>
        public static bool IsTileWithFlame(int i, int j, int type)
        {
            Tile tile = Main.tile[i, j];
            int style = TileObjectData.GetTileStyle(tile);

            return type == TileID.Torches ||
                   type == TileID.Campfire ||
                   type == TileID.SkullLanterns ||
                   type == TileID.Candles && !enabledCandleStyles.Contains(style) ||
                   type == TileID.Chandeliers && !enabledChandelierStyles.Contains(style) ||
                   type == TileID.HangingLanterns && disabledLanternStyles.Contains(style) ||
                   type == TileID.Lamps && disabledLampStyles.Contains(style) ||
                   type == TileID.Candelabras && !enabledCandelabras.Contains(style);
        }


        // No style IDs unfotunately
        private static readonly List<int> enabledCandleStyles = new()
        {
             7,  // Glass
             12, // Skyware
             23, // Steampunk
             26, // Martian
             27, // Meteorite
             28, // Granite
             30, // Crystal
             33, // Solar
             34, // Vortex
             35, // Nebula
             36  // Stardust
        };

        private static readonly List<int> enabledChandelierStyles = new()
        {
             15, // Skyware
             17, // Glass
             33, // Martian
             34, // Meteorite 
             37, // Crystal
             40, // Solar
             41, // Vortex
             42, // Nebula
             43  // Stardust
        };

        private static readonly List<int> disabledLanternStyles = new()
        {
             8,  // Hanging Jack O' Lantern 
             10, // Cactus
             11, // Ebonwood
             12, // Flesh 
             16, // Rich mahogany
             21, // Spooky
             22, // Living wood
             25, // Skull
             27, // Palm wood
             29, // Boreal wood
             32, // Obsidian
             36, // Marble 
             39, // Lesion
             44, // Sandstone
             45  // Bamboo
        };


        private static readonly List<int> disabledLampStyles = new()
        {
            0  ,  // Tiki torch 
            1  ,  // Cactus 
            6  ,  // Rich mahogany 
            7  ,  // Pearlwood
            8  ,  // Lihzahrd
            10 ,  // Spooky
            13 ,  // Living wood
            14 ,  // Shadewood
            15 ,  // Golden
            16 ,  // Bone
            18 ,  // Palm wood
            20 ,  // Boreal wood
            30 ,  // Marble
            33 ,  // Lesion
            38 ,  // Sandstone
            39    // Bamboo
        };

        private static readonly List<int> enabledCandelabras = new()
        {
            6, // Glass
            11, // Skyware
            27, // Martian
            28, // Meteorite
            29, // Granite
            31, // Crystal
            34, // Solar
            35, // Vortex
            36, // Nebula
            37  // Stardust
        };

    }
}