using Macrocosm.Common.Systems;
using SubworldLibrary;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace Macrocosm.Common.Global.Tiles
{
    /// <summary>
    /// Global tile and manager for disabled flames in airless environments
    /// <br/> Also see <see cref="Hooks.TorchLightHooks"/>
    /// </summary>
    public class LightSourceGlobalTile : GlobalTile
    {
        private static readonly Dictionary<int, HashSet<int>> lightDisabledTiles = new();
        public override void SetStaticDefaults()
        {
            RegisterDisabledTile(TileID.Torches);
            RegisterDisabledTile(TileID.Campfire);
            RegisterDisabledTile(TileID.Jackolanterns);

            RegisterDisabledTile(TileID.PlatinumCandle);
            // Register disabled candle styles
            RegisterDisabledTile(TileID.Candles,
                0,  // Blue Candle
                1,  // Green Candle
                2,  // Purple Candle
                3,  // Red Candle
                4,  // Yellow Candle
                5,  // Pink Candle
                6,  // White Candle
                8,  // Platinum Candle
                9,  // Dynasty Candle
                10, // Pumpkin Candle
                11, // Cactus Candle
                13, // Frozen Candle
                14, // Rich Mahogany Candle
                15, // Pearlwood Candle
                16, // Lihzahrd Candle
                17, // Skyware Candle
                18, // Shadewood Candle
                19, // Golden Candle
                20, // Obsidian Candle
                21, // Bone Candle
                22, // Flesh Candle
                24, // Honey Candle
                25, // Steampunk Candle
                29, // Marble Candle
                31, // Spider Candle
                32, // Lesion Candle
                37  // Bamboo Candle
            );

            // Register disabled chandelier styles
            RegisterDisabledTile(TileID.Chandeliers,
                0,  // Copper Chandelier
                1,  // Silver Chandelier
                2,  // Gold Chandelier
                3,  // Tin Chandelier
                4,  // Tungsten Chandelier
                5,  // Platinum Chandelier
                6,  // Jackelier
                7,  // Cactus Chandelier
                8,  // Ebonwood Chandelier
                9,  // Flesh Chandelier
                10, // Honey Chandelier
                11, // Frozen Chandelier
                12, // Rich Mahogany Chandelier
                13, // Pearlwood Chandelier
                14, // Lihzahrd Chandelier
                16, // Spooky Chandelier
                18, // Living Wood Chandelier
                19, // Shadewood Chandelier
                20, // Golden Chandelier
                21, // Bone Chandelier
                23, // Palm Wood Chandelier
                24, // Mushroom Chandelier
                25, // Boreal Wood Chandelier
                26, // Slime Chandelier
                27, // Blue Dungeon Chandelier
                28, // Green Dungeon Chandelier
                29, // Pink Dungeon Chandelier
                30, // Steampunk Chandelier
                31, // Pumpkin Chandelier
                32, // Obsidian Chandelier
                35, // Granite Chandelier
                36, // Marble Chandelier
                38, // Spider Chandelier
                39, // Lesion Chandelier
                44, // Sandstone Chandelier
                45, // Bamboo Chandelier
                46, // Reef Chandelier
                47  // Balloon Chandelier
            );

            // Register disabled hanging lantern styles
            RegisterDisabledTile(TileID.SkullLanterns);
            RegisterDisabledTile(TileID.HangingLanterns,
                8,  // Hanging Jack O' Lantern
                10, // Cactus Lantern
                11, // Ebonwood Lantern
                12, // Flesh Lantern
                16, // Rich Mahogany Lantern
                21, // Spooky Lantern
                22, // Living Wood Lantern
                25, // Skull Lantern
                27, // Palm Wood Lantern
                29, // Boreal Wood Lantern
                32, // Obsidian Lantern
                36, // Marble Lantern
                39, // Lesion Lantern
                44, // Sandstone Lantern
                45  // Bamboo Lantern
            );

            // Register disabled lamp styles
            RegisterDisabledTile(TileID.Lamps,
                0,  // Tiki Torch
                1,  // Cactus Lamp
                6,  // Rich Mahogany Lamp
                7,  // Pearlwood Lamp
                8,  // Lihzahrd Lamp
                10, // Spooky Lamp
                13, // Living Wood Lamp
                14, // Shadewood Lamp
                15, // Golden Lamp
                16, // Bone Lamp
                18, // Palm Wood Lamp
                20, // Boreal Wood Lamp
                30, // Marble Lamp
                33, // Lesion Lamp
                38, // Sandstone Lamp
                39  // Bamboo Lamp
            );

            // Register disabled candelabra styles

            RegisterDisabledTile(TileID.PlatinumCandelabra);
            RegisterDisabledTile(TileID.Candelabras,
                  0,  // Candelabra
                  1,  // Cactus Candelabra
                  2,  // Ebonwood Candelabra
                  3,  // Flesh Candelabra
                  4,  // Honey Candelabra
                  5,  // Steampunk Candelabra
                  7,  // Rich Mahogany Candelabra
                  8,  // Pearlwood Candelabra
                  9,  // Frozen Candelabra
                  10, // Lihzahrd Candelabra
                  12, // Spooky Candelabra
                  13, // Living Wood Candelabra
                  14, // Shadewood Candelabra
                  15, // Golden Candelabra
                  16, // Bone Candelabra
                  18, // Palm Wood Candelabra
                  19, // Mushroom Candelabra
                  20, // Boreal Wood Candelabra
                  21, // Slime Candelabra
                  22, // Blue Dungeon Candelabra
                  23, // Green Dungeon Candelabra
                  24, // Pink Dungeon Candelabra
                  25, // Obsidian Candelabra
                  26, // Pumpkin Candelabra
                  30, // Marble Candelabra
                  32, // Spider Candelabra
                  33, // Lesion Candelabra
                  38, // Sandstone Candelabra
                  39, // Bamboo Candelabra
                  40, // Reef Candelabra
                  41, // Balloon Candelabra
                  42  // Ash Wood Candelabra
            );
        }

        public static void RegisterDisabledTile(int tileType)
        {
            lightDisabledTiles.Add(tileType, null);
        }

        public static void RegisterDisabledTile(int tileType, params int[] styles)
        {
            if (!lightDisabledTiles.TryGetValue(tileType, out var value) || value is null)
                lightDisabledTiles[tileType] = new();

            foreach (var style in styles)
                lightDisabledTiles[tileType].Add(style);
        }

        /// <summary>
        /// Returns true if tile[i,j] is a block with flames 
        /// </summary>
        public static bool IsTileLightSourceDisabled(int i, int j, int type)
        {
            Tile tile = Main.tile[i, j];
            int style = TileObjectData.GetTileStyle(tile);

            if (lightDisabledTiles.TryGetValue(type, out var disabledStyles))
                return disabledStyles == null || disabledStyles.Contains(style);

            return false;
        }

        /// <summary>
        /// Turns off placed torches, campfires and other blocks with fire when placed on our subworlds 
        /// </summary>
        public override bool TileFrame(int i, int j, int type, ref bool resetFrame, ref bool noBreak)
        {
            if (SubworldSystem.AnyActive<Macrocosm>()
                && IsTileLightSourceDisabled(i, j, type)
                && !RoomOxygenSystem.IsRoomPressurized(i, j))
            {
                WorldGen.TryToggleLight(i, j, false, skipWires: false);
            }
            return true;
        }

        /// <summary>
        /// Disables wiring on our subworlds for chosen light sources 
        /// </summary>
        public override bool PreHitWire(int i, int j, int type)
        {
            return !(SubworldSystem.AnyActive<Macrocosm>()
                     && IsTileLightSourceDisabled(i, j, type)
                     && !RoomOxygenSystem.IsRoomPressurized(i, j));
        }


        /// <summary>
        /// Disables right clicking on campfires on our subworlds 
        /// </summary>
        public override void RightClick(int i, int j, int type)
        {
            if (SubworldSystem.AnyActive<Macrocosm>() && type == TileID.Campfire && !RoomOxygenSystem.IsRoomPressurized(i, j))
            {
                WorldGen.TryToggleLight(i, j, false, skipWires: false);
            }
        }
    }
}