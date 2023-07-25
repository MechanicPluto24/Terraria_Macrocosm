using System;
using System.Collections.Generic;
using System.Linq;
using Macrocosm.Common.Drawing;
using Macrocosm.Common.Subworlds;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace Macrocosm.Content.Rockets.Customization
{
	public class CustomizationStorage : ModSystem
	{
		private static Dictionary<string, Pattern> patterns;
		private static Dictionary<string, Detail> details;
		private static Dictionary<string, PatternColorFunction> specialFunctions;

		public override void Load()
		{
			patterns = new Dictionary<string, Pattern>();
			details = new Dictionary<string, Detail>();
			specialFunctions = new Dictionary<string, PatternColorFunction>();

			LoadSpecialFunctions(); // Load functions first, as they might be used in the pattern loading
			LoadPatterns();
			LoadDetails();
		}

		public override void Unload()
		{
			patterns.Clear();
			details.Clear();
			specialFunctions.Clear();
			patterns = null;
			details = null;
			specialFunctions = null;
		}

		public static void AddPattern(string moduleName, string patternName, bool unlockedByDefault, params PatternColorData[] colorData)
			=> patterns.Add(moduleName + "_" + patternName, new Pattern(moduleName, patternName, unlockedByDefault, colorData));

		public static Pattern GetPattern(string moduleName, string patternName)
			=> patterns[moduleName + "_" + patternName];

		public static bool TryGetPattern(string moduleName, string patternName, out Pattern pattern)
			=> patterns.TryGetValue(moduleName + "_" + patternName, out pattern);


		public static void AddDetail(string moduleName, string detailName, bool unlockedbyDefault = false)
			=> details.Add(moduleName + "_" + detailName, new Detail(moduleName, detailName, unlockedbyDefault));

		public static Detail GetDetail(string moduleName, string detailName)
			=> details[moduleName + "_" + detailName];

		public static bool TryGetDetail(string moduleName, string detailName, out Detail detail)
			=> details.TryGetValue(moduleName + "_" + detailName, out detail);


		public static void AddSpecialFunction(string functionName, Func<Color[], Color> function, bool unlockedbyDefault = false)
			=> specialFunctions.Add(functionName, new PatternColorFunction(function, unlockedbyDefault));

		public static PatternColorFunction GetFunction(string functionName)
			=> specialFunctions[functionName];

		public static bool TryGetFunction(string functionName, out PatternColorFunction function)
			=> specialFunctions.TryGetValue(functionName, out function);

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
			AddDetail("EngineModule", "FlagRO", false);
		}

		private static void LoadSpecialFunctions()
		{
			AddSpecialFunction("Disco", (colors) => Main.DiscoColor);
			AddSpecialFunction("Celestial", (colors) => GlobalVFX.CelestialColor);
		}

		public override void ClearWorld()
		{
			foreach (var pattern in patterns)
 				pattern.Value.Unlocked = pattern.Value.UnlockedByDefault;
 
			foreach (var detail in details)
				detail.Value.Unlocked = detail.Value.UnlockedByDefault;

			foreach (var function in specialFunctions)
				function.Value.Unlocked = function.Value.UnlockedByDefault;
		}

		public override void SaveWorldData(TagCompound tag) => SaveUnlockedStatus(tag);

		public override void LoadWorldData(TagCompound tag) => LoadUnlockedStatus(tag);	


		public static void SaveUnlockedStatus(TagCompound tag)
		{
			foreach(var pattern in patterns)
 				if(pattern.Value.Unlocked && !pattern.Value.UnlockedByDefault)
					tag["Pattern_" + pattern.Key + "_Unlocked"] = true;
 
			foreach (var detail in details)
 				if (detail.Value.Unlocked && !detail.Value.UnlockedByDefault)
					tag["Detail_" + detail.Key + "_Unlocked"] = true;

			foreach (var function in specialFunctions)
				if (function.Value.Unlocked && !function.Value.UnlockedByDefault)
					tag["SpecialFunction_" + function.Key + "_Unlocked"] = true;
		}

		public static void LoadUnlockedStatus(TagCompound tag)
		{
			foreach (var pattern in patterns)
				if(tag.ContainsKey("Pattern_" + pattern.Key + "_Unlocked"))
					patterns[pattern.Key].Unlocked = true;

			foreach (var detail in details)
				if (tag.ContainsKey("Detail_" + detail.Key + "_Unlocked"))
					patterns[detail.Key].Unlocked = true;

			foreach (var function in specialFunctions)
				if (tag.ContainsKey("SpecialFunction_" + function.Key + "_Unlocked"))
					patterns[function.Key].Unlocked = true;
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
