using Terraria;
using Terraria.ModLoader;
using SubworldLibrary;
using Macrocosm.Content.Subworlds;
using Terraria.Map;
using System;
using System.Reflection;
using Microsoft.Xna.Framework;

namespace Macrocosm.Common.Hooks
{
	public class MapBackgroundColor : ILoadable
	{
		public static ushort SkyPosition;
		public static ushort DirtPosition;
		public static ushort RockPosition;
		public static ushort HellPosition;

		private static FieldInfo SkyPositionF;  
		private static FieldInfo DirtPositionF; 
		private static FieldInfo RockPositionF; 
		private static FieldInfo HellPositionF; 

		public void Load(Mod mod)
		{
			On.Terraria.Map.MapHelper.GetMapTileXnaColor += MapHelper_GetMapTileXnaColor;
			GetLookupPositions();
		}

		private Color MapHelper_GetMapTileXnaColor(On.Terraria.Map.MapHelper.orig_GetMapTileXnaColor orig, ref MapTile tile)
		{
			if (!SubworldSystem.AnyActive<Macrocosm>() || MacrocosmSubworld.Current().MapColors is null)
				return orig(ref tile);

			if (tile.Type >= SkyPosition && tile.Type <= SkyPosition + 255) 
				return MapColorLerp(MacrocosmSubworld.Current().MapColors[MapColorType.SkyUpper], MacrocosmSubworld.Current().MapColors[MapColorType.SkyLower], (tile.Type - SkyPosition) / 255f);

			if (tile.Type >= DirtPosition && tile.Type <= DirtPosition + 255)
				return MapColorLerp(MacrocosmSubworld.Current().MapColors[MapColorType.UndergroundUpper], MacrocosmSubworld.Current().MapColors[MapColorType.UndergroundLower], (tile.Type - DirtPosition) / 255f);

			if (tile.Type >= RockPosition && tile.Type <= RockPosition + 255)
				return MapColorLerp(MacrocosmSubworld.Current().MapColors[MapColorType.CavernUpper], MacrocosmSubworld.Current().MapColors[MapColorType.CavernLower], (tile.Type - RockPosition) / 255f);

			if (tile.Type == HellPosition)
				return MacrocosmSubworld.Current().MapColors[MapColorType.Underworld];

			return orig(ref tile);
		}

		private void GetLookupPositions()
		{
			SkyPositionF = typeof(MapHelper).GetField("skyPosition", BindingFlags.NonPublic | BindingFlags.Static);
			DirtPositionF = typeof(MapHelper).GetField("dirtPosition", BindingFlags.NonPublic | BindingFlags.Static);
			RockPositionF = typeof(MapHelper).GetField("rockPosition", BindingFlags.NonPublic | BindingFlags.Static);
			HellPositionF = typeof(MapHelper).GetField("hellPosition", BindingFlags.NonPublic | BindingFlags.Static);
			SkyPosition = (ushort)SkyPositionF.GetValue(null);
			DirtPosition = (ushort)DirtPositionF.GetValue(null);
			RockPosition = (ushort)RockPositionF.GetValue(null);
			HellPosition = (ushort)HellPositionF.GetValue(null);
		}

		/// <summary> Adapted from vanilla code </summary>
		private static Color MapColorLerp(Color from, Color to, float value)
			=> new Color((byte)((float)(int)from.R * (1f - value) + (float)(int)to.R * value), (byte)((float)(int)from.G * (1f - value) + (float)(int)to.G * value), (byte)((float)(int)from.B * (1f - value) + (float)(int)to.B * value));
		 
		public void Unload() { }
	}
}