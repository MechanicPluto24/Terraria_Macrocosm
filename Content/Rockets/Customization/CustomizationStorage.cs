using Macrocosm.Common.UI;
using Macrocosm.Common.Utils;
using Microsoft.Xna.Framework;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace Macrocosm.Content.Rockets.Customization
{
	public class CustomizationStorage : ModSystem
	{
		public static bool Initialized { get; private set; }
		private static bool initialLoad;

		private static Dictionary<(string moduleName, string patternName), Pattern> patterns;
		private static Dictionary<(string moduleName, string detailName), Detail> details;

		private static Dictionary<(string moduleName, string patternName), bool> patternUnlockStatus;
		private static Dictionary<(string moduleName, string detailName), bool> detailUnlockStatus;

		public override void Load()
		{
            initialLoad = true;

			patterns = new();
			details = new();
			patternUnlockStatus = new();
			detailUnlockStatus = new();

			LoadPatterns();
			LoadDetails();

			Initialized = true;
        }

		public override void Unload()
		{
            initialLoad = false;

			patterns = null;
			details = null;
			patternUnlockStatus = null;
			detailUnlockStatus = null;

            Initialized = false;
        }

        public static void Reset()
		{
            Initialized = false;

            patterns = new();
            details = new();
            patternUnlockStatus = new();
            detailUnlockStatus = new();

            LoadPatterns();
            LoadDetails();

			Initialized = true;
        }

		/// <summary>
		/// Gets a pattern from the pattern storage.
		/// </summary>
		/// <param name="moduleName"> The rocket module this pattern belongs to </param>
		/// <param name="patternName"> The pattern name </param>
		public static Pattern GetPattern(string moduleName, string patternName)
			=> patterns[(moduleName, patternName)];

		public static Pattern GetDefaultPattern(string moduleName)
			=> patterns[(moduleName, "Basic")];

		public static List<Pattern> GetUnlockedPatterns(string moduleName)
		{
			return GetPatternsWhere(moduleName, pattern =>
			{
				var key = (moduleName, pattern.Name);
				return patternUnlockStatus.ContainsKey(key) && patternUnlockStatus[key];
			});
		}

		public static List<Pattern> GetPatternsWhere(string moduleName, Func<Pattern, bool> match)
		{
			var patternsForModule = patterns
				.Select(kvp => kvp.Value)
				.Where(pattern => pattern.ModuleName == moduleName && match(pattern))
				.ToList();

			return patternsForModule;
		}

		/// <summary>
		/// Attempts to get a pattern from the pattern storage.
		/// </summary>
		/// <param name="moduleName"> The rocket module this pattern belongs to </param>
		/// <param name="patternName"> The pattern name </param>
		/// <param name="pattern"> The pattern null if not found </param>
		/// <returns> Whether the specified pattern has been found </returns>
		public static bool TryGetPattern(string moduleName, string patternName, out Pattern pattern)
		{
			if (patterns.TryGetValue((moduleName, patternName), out Pattern defaultPattern))
			{
				pattern = defaultPattern;
				return true;
			}
			else
			{
				pattern = default;
				return false;
			}
		}

		/// <summary>
		/// Sets the unlocked status on a pattern. This affects all players, in all subworlds.
		/// </summary>
		/// <param name="moduleName"> The rocket module this pattern belongs to </param>
		/// <param name="patternName"> The pattern name </param>
		/// <param name="unlockedState"> The unlocked state to set </param>
		public static void SetPatternUnlockedStatus(string moduleName, string patternName, bool unlockedState)
			 => patternUnlockStatus[(moduleName, patternName)] = unlockedState;

		/// <summary>
		/// Gets the detail reference from the detail storage.
		/// </summary>
		/// <param name="moduleName"> The rocket module this detail belongs to </param>
		/// <param name="detailName"> The detail name </param>
		public static Detail GetDetail(string moduleName, string detailName)
			=> details[(moduleName, detailName)];

        public static Detail GetDefaultDetail(string moduleName)
			=> details[(moduleName, "None")];

        public static List<Detail> GetUnlockedDetails(string moduleName)
        {
            return GetDetailsWhere(moduleName, detail =>
            {
                var key = (moduleName, detail.Name);
                return detailUnlockStatus.ContainsKey(key) && detailUnlockStatus[key];
            });
        }

        public static List<Detail> GetDetailsWhere(string moduleName, Func<Detail, bool> match)
        {
            var detailsForModule = details
                .Select(kvp => kvp.Value)
                .Where(detail => detail.ModuleName == moduleName && match(detail))
                .ToList();

            return detailsForModule;
        }

        /// <summary>
        /// Attempts to get a detail reference from the detail storage.
        /// </summary>
        /// <param name="moduleName"> The rocket module this detail belongs to </param>
        /// <param name="detailName"> The detail name </param>
        /// <param name="detail"> The detail, null if not found </param>
        /// <returns> Whether the specified detail has been found </returns>
		public static bool TryGetDetail(string moduleName, string detailName, out Detail detail)
			=> details.TryGetValue((moduleName, detailName), out detail);

        public override void ClearWorld() => Reset();

		public override void SaveWorldData(TagCompound tag) => SaveData(tag);

		public override void LoadWorldData(TagCompound tag) => LoadData(tag);


		public static void SaveData(TagCompound tag)
		{
			foreach (var kvp in patternUnlockStatus)
				if (kvp.Value)
					tag["Pattern_" + kvp.Key + "_Unlocked"] = true;

			foreach (var kvp in detailUnlockStatus)
				if (kvp.Value)
					tag["Detail_" + kvp.Key + "_Unlocked"] = true;
		}

		public static void LoadData(TagCompound tag)
		{
			foreach (var kvp in patternUnlockStatus)
				if (tag.ContainsKey("Pattern_" + kvp.Key + "_Unlocked"))
					patternUnlockStatus[kvp.Key] = true;

			foreach (var kvp in detailUnlockStatus)
				if (tag.ContainsKey("Detail_" + kvp.Key + "_Unlocked"))
					detailUnlockStatus[kvp.Key] = true;
		}

		/// <summary>
		/// Adds a rocket module pattern to the pattern storage
		/// </summary>
		/// <param name="moduleName"> The rocket module this pattern belongs to </param>
		/// <param name="patternName"> The pattern name </param>
		/// <param name="unlockedByDefault"> Whether this pattern is unlocked by default
		/// <param name="colorData"> The color data (default colors, whether they are user changeable, dynamic color function) </param>
		private static void AddPattern(string moduleName, string patternName, bool unlockedByDefault = false, params PatternColorData[] colorData)
		{
			Pattern pattern = new(moduleName, patternName, colorData);
			patterns.Add((moduleName, patternName), pattern);
			patternUnlockStatus.Add((moduleName, patternName), unlockedByDefault);
		}

		private static void AddPattern(Pattern pattern, bool unlockedByDefault = false)
		{
			patterns.Add((pattern.ModuleName, pattern.Name), pattern);
			patternUnlockStatus.Add((pattern.ModuleName, pattern.Name), unlockedByDefault);
		}

		/// <summary>
		/// Adds a detail to the detail storage
		/// </summary>
		/// <param name="moduleName"> The rocket module this detail belongs to </param>
		/// <param name="detailName"> The detail name </param>
		/// <param name="unlockedByDefault"> Whether this detail is unlocked by default </param>
		private static void AddDetail(string moduleName, string detailName, bool unlockedByDefault = false)
		{
			Detail detail = new(moduleName, detailName);
			details.Add((moduleName, detailName), detail);
			detailUnlockStatus.Add((moduleName, detailName), unlockedByDefault);
		}

		private static void LoadPatterns()
		{
			try
			{
				JArray patternsArray = Utility.ParseJSONFromFile("Content/Rockets/Customization/Patterns/patterns.json");

				foreach (JObject patternObject in patternsArray.Cast<JObject>())
				{
					bool unlockedByDefault = false;

					if (patternObject.ContainsKey("unlockedByDefault"))
						unlockedByDefault = patternObject.Value<bool>("unlockedByDefault");

					AddPattern(Pattern.FromJObject(patternObject), unlockedByDefault);
				}
			}
			catch (Exception ex)
			{
				Macrocosm.Instance.Logger.Error(ex.Message);
			}

			for(int i = 0; i < 60; i++)
			{
				AddPattern("EngineModule", $"Test{i}", true);
			}

            string logstring = "Loaded " + patterns.Count.ToString() + " pattern" + (patterns.Count == 1 ? "" : "s") + ":\n";

            foreach (string moduleName in Rocket.DefaultModuleNames)
			{
				logstring += $" - Module: {moduleName}\n\t";
                foreach (var kvp in patterns)
				{
					(string patternModuleName, string patternName) = kvp.Key;
                    if(patternModuleName == moduleName)
                        logstring += $"{patternName} ";
                }
                logstring += "\n\n";
            }

			if(initialLoad)
				Macrocosm.Instance.Logger.Info(logstring);
        }

        private static void LoadDetails()
		{
            foreach (string moduleName in Rocket.DefaultModuleNames)
                 AddDetail(moduleName, "None", true);
 
            // Find all existing patters for this module
            string lookupString = "Content/Rockets/Customization/Details/";
            var detailPathsWithIcons = Macrocosm.Instance.RootContentSource.GetAllAssetsStartingWith(lookupString).ToList();
			var detailPaths = detailPathsWithIcons.Where(x => !x.Contains("/Icons")).ToList();

            // Log the pattern list

            foreach (var detailWithModule in detailPaths)
			{
				string[] split = detailWithModule.Replace(lookupString, "").Split('/');

				if(split.Length == 2) 
				{
					string module = split[0];
					string detail = split[1].Replace(".rawimg", "");

					AddDetail(module, detail, true);
                }
            }

            string logstring = "Loaded " + details.Count.ToString() + " detail" + (detailPaths.Count == 1 ? "" : "s") + ":\n";
            foreach (string moduleName in Rocket.DefaultModuleNames)
            {
                logstring += $" - Module: {moduleName}\n\t";
                foreach (var kvp in details)
                {
                    (string DetailModuleName, string detailName) = kvp.Key;
                    if (DetailModuleName == moduleName)
                        logstring += $"{detailName} ";
                }
                logstring += "\n\n";
            }

            if (initialLoad)
                Macrocosm.Instance.Logger.Info(logstring);
        }
	}
}
