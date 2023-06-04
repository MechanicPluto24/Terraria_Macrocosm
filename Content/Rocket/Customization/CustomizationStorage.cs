using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria.ModLoader;

namespace Macrocosm.Content.Rocket.Customization
{
	public class CustomizationStorage : ILoadable
	{
		private static Dictionary<string, Pattern> patternStorage;
		private static Dictionary<string, Detail> detailStorage;

		public void Load(Mod mod)
		{
			patternStorage = new Dictionary<string, Pattern>();
			detailStorage = new Dictionary<string, Detail>();
			LoadPatterns();
			LoadDetails();
		}

		public void Unload()
		{
			patternStorage = null;
			detailStorage = null;
		}

		public static void AddPattern(string moduleName, string patternName, params Color[] defaultColors)
			=> patternStorage.Add(moduleName + "_" + patternName, new Pattern(moduleName, patternName, defaultColors));

		public static Pattern GetPattern(string moduleName, string patternName)
			=> patternStorage[moduleName + "_" + patternName];

		public static bool TryGetPattern(string moduleName, string patternName, out Pattern pattern)
			=> patternStorage.TryGetValue(moduleName + "_" + patternName, out pattern);


		public static void AddDetail(string moduleName, string detailName)
			=> detailStorage.Add(moduleName + "_" + detailName, new Detail(moduleName, detailName));

		public static Detail GetDetail(string moduleName, string detailName)
			=> detailStorage[moduleName + "_" + detailName];

		public static bool TryGetDetail(string moduleName, string detailName, out Detail detail)
			=> detailStorage.TryGetValue(moduleName + "_" + detailName, out detail);


		private static void LoadPatterns()
		{
			AddPattern("CommandPod", "Basic");

			AddPattern("ServiceModule", "Basic");

			AddPattern("ReactorModule", "Basic");

			AddPattern("EngineModule", "Basic");
			AddPattern("EngineModule", "Binary", Color.White, Color.White, new Color(40, 40, 40));
			AddPattern("EngineModule", "Saturn", Color.White, Color.White, new Color(40, 40, 40));
			AddPattern("EngineModule", "Delta" , Color.White, Color.White, new Color(40, 40, 40));
			AddPattern("EngineModule", "Rainbow", Color.White, Color.Red, Color.Orange, Color.Yellow, Color.Green, Color.Blue, Color.Indigo, Color.Violet);

			AddPattern("Boosters", "Basic");
		}

		private static void LoadDetails()
		{
			AddDetail("EngineModule", "FlagRO");
		}


		/*
		public void AutoloadPatterns()
		{
			// Find all existing patters for this module
			string lookupString = (HERE + MODULES[n] + "_Pattern_").Replace("Macrocosm/", "");
			PatternPaths = Macrocosm.Instance.RootContentSource.GetAllAssetsStartingWith(lookupString).ToList();

			// Log the pattern list
			string logstring = "Found " + PatternPaths.Count.ToString() + " pattern" + (PatternPaths.Count == 1 ? "" : "s") + " for rocket module " + MODULES[n] + ": ";
			foreach (var pattern in PatternPaths)
				logstring += pattern.Replace(lookupString, "").Replace(".rawimg", "") + " ";
			Macrocosm.Instance.Logger.Info(logstring);
		}
		*/
	}
}
