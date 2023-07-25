using Macrocosm.Common.Subworlds;
using Macrocosm.Common.Utils;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace Macrocosm.Content.Rockets.Construction
{
	public class LaunchPadLocations : ModSystem
	{
		private static Dictionary<string, List<Vector2>> launchPadLocations;

		public override void Load()
		{
			launchPadLocations = new Dictionary<string, List<Vector2>>();
		}

		public override void Unload()
		{
			launchPadLocations.Clear();
			launchPadLocations = null;
		}

		public static void Add(string subworldId, Vector2 location)
		{
			if (launchPadLocations.ContainsKey(subworldId))
			{
				launchPadLocations[subworldId].Add(location);
			}
			else
			{
				List<Vector2> locationsList = new() { location };
				launchPadLocations.Add(subworldId, locationsList);
			}
		}

		public static void SetDefaultLocation(string subworldId, Vector2 location)
		{
			if (launchPadLocations.ContainsKey(subworldId))
			{
				launchPadLocations[subworldId][0] = location;
			}
			else
			{
				List<Vector2> locationsList = new() { location };
				launchPadLocations.Add(subworldId, locationsList);
			}
		}

		public static bool Any(string subworldId) => GetLocations(subworldId).Any();

		public static bool None(string subworldId) => !Any(subworldId);

		public static List<Vector2> GetLocations(string subworldId)
		{
			if(launchPadLocations.ContainsKey(subworldId))
				return launchPadLocations[subworldId];

			return new List<Vector2>();
		}

		public static List<Vector2> GetUserLocations(string subworldId) => GetLocations(subworldId).Skip(1).ToList();
 
		public static Vector2 GetDefaultLocation(string subworldId)
		{
			if(None(subworldId))
			{
				Macrocosm.Instance.Logger.Warn("No default location for subworld " + subworldId);
				return Vector2.Zero;
			}

			return GetLocations(subworldId)[0];
		}

		public override void PostWorldGen()
		{
			SetDefaultLocation("Earth", Utility.SpawnWorldPosition);
		}

		public override void OnWorldLoad()
		{
			SetDefaultLocation(MacrocosmSubworld.CurrentSubworld, Utility.SpawnWorldPosition);
		}

		public override void ClearWorld()
		{
			launchPadLocations.Clear();
		}

		public override void SaveWorldData(TagCompound tag) => SaveLocations(tag);

		public override void LoadWorldData(TagCompound tag) => LoadLocations(tag);

		public static void SaveLocations(TagCompound tag)
		{
			foreach (var location in launchPadLocations)
 				tag["LaunchPads_" + location.Key] = location.Value;
 		}

		public static void LoadLocations(TagCompound tag)
		{
			foreach (var location in launchPadLocations)
				if(tag.ContainsKey("LaunchPads_" + location.Key))
					launchPadLocations[location.Key] = (List<Vector2>)tag.GetList<Vector2>("LaunchPads_" + location.Key);
		}
	}
}
