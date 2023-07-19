using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria.ModLoader;

namespace Macrocosm.Content.Rockets.Customization
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

		public static void AddPattern(string moduleName, string patternName, bool unlockedByDefault, params PatternColorData[] colorData)
			=> patternStorage.Add(moduleName + "_" + patternName, new Pattern(moduleName, patternName, unlockedByDefault, colorData));

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
			AddPattern("CommandPod", "Basic", true);

			AddPattern("ServiceModule", "Basic", true);

			AddPattern("ReactorModule", "Basic", true);

			AddPattern("EngineModule", "Basic", true);
			AddPattern("EngineModule", "Binary", true, new(Color.White), new(Color.White), new(new Color(40, 40, 40)));
			AddPattern("EngineModule", "Saturn", true, new(Color.White), new(Color.White), new(new Color(40, 40, 40)));
			AddPattern("EngineModule", "Delta" , true, new(Color.White), new(Color.White), new(new Color(40, 40, 40)));
			AddPattern("EngineModule", "Rainbow", false, new(Color.White), new(Color.Red), new(Color.Orange), new(Color.Yellow), new(Color.Green), new(Color.Blue), new(Color.Indigo), new(Color.Violet));

			AddPattern("BoosterLeft", "Basic", true);
			AddPattern("BoosterRight", "Basic", true);
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
