using Macrocosm.Common.Enums;
using Macrocosm.Common.Subworlds;
using Macrocosm.Common.Utils;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria.Map;
using Terraria.ModLoader;

namespace Macrocosm.Common.Systems
{
    public class MapTileSystem : ModSystem
    {
        private static Dictionary<string, Color[]> mapTileColorLookupBySubworld;
        private static Color[] defaultColorLookup = null;

        public override void Load()
        {
            mapTileColorLookupBySubworld = new();
        }

        public override void PostSetupContent()
        {
        }

        public override void Unload()
        {
            /*
            MapHelper.Initialize();
            mapTileColorLookupBySubworld = null;
            */

            if(defaultColorLookup is not null)
                typeof(MapHelper).SetFieldValue("colorLookup", defaultColorLookup);

            mapTileColorLookupBySubworld = null;
        }

        // Currently not working, do we even want this? 
        /*
        // Apply map color modifications to vanilla tiles. 
        // At PostSetupContent, the map colors for any modded tiles are not yet populated!
        public override void PostSetupContent()
        {
            Color[] vanillaColorLookup = (Color[])typeof(MapHelper).GetFieldValue("colorLookup");

            vanillaColorLookup[TileID.LunarOre] = new Color(104, 202, 163);

            typeof(MapHelper).SetFieldValue("colorLookup", vanillaColorLookup);
        }
        */

        public static Color[] GetMapColorLookup()
        {
            MacrocosmSubworld subworld = MacrocosmSubworld.Current;
            if (subworld is not null && mapTileColorLookupBySubworld.TryGetValue(subworld.ID, out Color[] subworldMapTileColorLookup))
            {
                return subworldMapTileColorLookup;
            }

            defaultColorLookup ??= (Color[])typeof(MapHelper).GetFieldValue("colorLookup");
            return defaultColorLookup;
        }

        /// <summary> 
        /// Applies the subworld-specific map tile colors specified in the current <see cref="MacrocosmSubworld.MapColors"/>. 
        /// Should be called in <see cref="MacrocosmSubworld.OnEnter"/>.
        /// </summary>
        public static void ApplyMapTileColors()
        {
            // Cache the default map color lookup, including all the vanilla modifications from ApplyVanillaMapTileModifications
            // When first applying the background colors (Subworld.OnEnter), modded map tiles ARE safely populated
            defaultColorLookup ??= (Color[])typeof(MapHelper).GetFieldValue("colorLookup");

            MacrocosmSubworld subworld = MacrocosmSubworld.Current;
            if (subworld is not null && subworld.MapColors is not null)
            {
                if (!mapTileColorLookupBySubworld.TryGetValue(subworld.ID, out Color[] subworldMapTileColorLookup))
                {
                    subworldMapTileColorLookup = ComputeSubworldMapTileColorLookup(subworld, defaultColorLookup);
                    mapTileColorLookupBySubworld[subworld.ID] = subworldMapTileColorLookup;
                }

                typeof(MapHelper).SetFieldValue("colorLookup", subworldMapTileColorLookup);
            }
        }

        /// <summary> 
        /// Restores the default (Earth) color lookup, including common modifications.
        /// Should only be called in <see cref="MacrocosmSubworld.OnExit"/>.
        /// </summary>
        public static void RestoreMapTileColors()
        {
            if (defaultColorLookup is not null)
                typeof(MapHelper).SetFieldValue("colorLookup", defaultColorLookup);
        }

        // Compute the subworld-specific background map tile colors 
        private static Color[] ComputeSubworldMapTileColorLookup(MacrocosmSubworld subworld, Color[] defaultColorLookup)
        {
            Color[] subworldMapTileColorLookup = new Color[defaultColorLookup.Length];
            Array.Copy(defaultColorLookup, subworldMapTileColorLookup, defaultColorLookup.Length);

            ushort skyPosition = (ushort)typeof(MapHelper).GetFieldValue("skyPosition");
            ushort dirtPosition = (ushort)typeof(MapHelper).GetFieldValue("dirtPosition");
            ushort rockPosition = (ushort)typeof(MapHelper).GetFieldValue("rockPosition");
            ushort hellPosition = (ushort)typeof(MapHelper).GetFieldValue("hellPosition");

            for (int i = 0; i < 256; i++)
            {
                subworldMapTileColorLookup[skyPosition + i] = Color.Lerp(subworld.MapColors[MapColorType.SkyUpper], subworld.MapColors[MapColorType.SkyLower], i / 256f);
                subworldMapTileColorLookup[dirtPosition + i] = Color.Lerp(subworld.MapColors[MapColorType.UndergroundUpper], subworld.MapColors[MapColorType.UndergroundLower], i / 256f);
                subworldMapTileColorLookup[rockPosition + i] = Color.Lerp(subworld.MapColors[MapColorType.CavernUpper], subworld.MapColors[MapColorType.CavernLower], i / 256f);
            }

            subworldMapTileColorLookup[hellPosition] = subworld.MapColors[MapColorType.Underworld];

            return subworldMapTileColorLookup;
        }
    }
}
