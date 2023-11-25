using Macrocosm.Common.Subworlds;
using Microsoft.Xna.Framework;
using System.Reflection;
using Terraria.Map;
using Terraria.ModLoader;

namespace Macrocosm.Common.Hooks
{
    public class MapBackgroundColor : ILoadable
    {
        private static ushort skyPosition;
        private static ushort dirtPosition;
        private static ushort rockPosition;
        private static ushort hellPosition;

        private static FieldInfo fieldSkyPosition;
        private static FieldInfo fieldDirtPosition;
        private static FieldInfo fieldRockPosition;
        private static FieldInfo fieldHellPosition;

        public void Load(Mod mod)
        {
            On_MapHelper.GetMapTileXnaColor += MapHelper_GetMapTileXnaColor;
            GetLookupPositions();
        }
        public void Unload()
        {
            On_MapHelper.GetMapTileXnaColor -= MapHelper_GetMapTileXnaColor;
        }

        private Color MapHelper_GetMapTileXnaColor(On_MapHelper.orig_GetMapTileXnaColor orig, ref MapTile tile)
        {
            if (MacrocosmSubworld.Current is null || MacrocosmSubworld.Current.MapColors is null)
                return orig(ref tile);

            if (tile.Type >= skyPosition && tile.Type <= skyPosition + 255)
                return MapColorLerp(MacrocosmSubworld.Current.MapColors[MapColorType.SkyUpper], MacrocosmSubworld.Current.MapColors[MapColorType.SkyLower], (tile.Type - skyPosition) / 255f);

            if (tile.Type >= dirtPosition && tile.Type <= dirtPosition + 255)
                return MapColorLerp(MacrocosmSubworld.Current.MapColors[MapColorType.UndergroundUpper], MacrocosmSubworld.Current.MapColors[MapColorType.UndergroundLower], (tile.Type - dirtPosition) / 255f);

            if (tile.Type >= rockPosition && tile.Type <= rockPosition + 255)
                return MapColorLerp(MacrocosmSubworld.Current.MapColors[MapColorType.CavernUpper], MacrocosmSubworld.Current.MapColors[MapColorType.CavernLower], (tile.Type - rockPosition) / 255f);

            if (tile.Type == hellPosition)
                return MacrocosmSubworld.Current.MapColors[MapColorType.Underworld];

            return orig(ref tile);
        }

        private void GetLookupPositions()
        {
            fieldSkyPosition = typeof(MapHelper).GetField("skyPosition", BindingFlags.NonPublic | BindingFlags.Static);
            fieldDirtPosition = typeof(MapHelper).GetField("dirtPosition", BindingFlags.NonPublic | BindingFlags.Static);
            fieldRockPosition = typeof(MapHelper).GetField("rockPosition", BindingFlags.NonPublic | BindingFlags.Static);
            fieldHellPosition = typeof(MapHelper).GetField("hellPosition", BindingFlags.NonPublic | BindingFlags.Static);
            skyPosition = (ushort)fieldSkyPosition.GetValue(null);
            dirtPosition = (ushort)fieldDirtPosition.GetValue(null);
            rockPosition = (ushort)fieldRockPosition.GetValue(null);
            hellPosition = (ushort)fieldHellPosition.GetValue(null);
        }

        /// <summary> Adapted from vanilla code </summary>
        private static Color MapColorLerp(Color from, Color to, float value)
            => new Color((byte)((float)(int)from.R * (1f - value) + (float)(int)to.R * value), (byte)((float)(int)from.G * (1f - value) + (float)(int)to.G * value), (byte)((float)(int)from.B * (1f - value) + (float)(int)to.B * value));

    }
}