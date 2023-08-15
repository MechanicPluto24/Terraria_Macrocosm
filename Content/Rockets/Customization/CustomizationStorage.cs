using System;
using System.Collections.Generic;
using System.Linq;
using Macrocosm.Common.Drawing;
using Macrocosm.Common.Subworlds;
using Macrocosm.Common.UI;
using Macrocosm.Common.Utils;
using Macrocosm.Content.Rockets.Navigation.NavigationInfo;
using Macrocosm.Content.Rockets.Navigation;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.GameContent.UI.Elements;
using Terraria.UI;

namespace Macrocosm.Content.Rockets.Customization
{
	public class CustomizationStorage : ModSystem
	{
		private static Dictionary<(string moduleName, string patternName), Pattern> patterns;
		private static Dictionary<(string moduleName, string detailName), Detail> details;
		private static Dictionary<string, PatternColorFunction> specialFunctions;

		private static IEnumerable<IUnlockable> Unlockables => Utility.Concatenate<IUnlockable>(patterns.Values, details.Values, specialFunctions.Values);
			
		public override void Load()
		{
			patterns = new Dictionary<(string,string), Pattern>();
			details = new Dictionary<(string, string), Detail>();
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

		public static void Reset()
		{
			ModContent.GetInstance<CustomizationStorage>().Unload();
			ModContent.GetInstance<CustomizationStorage>().Load();
		}

		/// <summary>
		/// Gets a pattern <b> reference </b> from the pattern storage. Don't use this if you're modifying pattern data.
		/// </summary>
		/// <param name="moduleName"> The rocket module this pattern belongs to </param>
		/// <param name="patternName"> The pattern name </param>
		public static Pattern GetPatternReference(string moduleName, string patternName)
			=> patterns[(moduleName,patternName)];

		/// <summary>
		/// Gets a pattern <b> clone </b> from the pattern storage. As it is a clone, it can be modified freely.
		/// </summary>
		/// <param name="moduleName"> The rocket module this pattern belongs to </param>
		/// <param name="patternName"> The pattern name </param>
		public static Pattern GetPattern(string moduleName, string patternName)
			=> patterns[(moduleName, patternName)].Clone();

		public static List<Pattern> GetUnlockedPatterns(string moduleName, bool asClones = true) => GetPatternsWhere(moduleName, (pattern) => pattern.Unlocked, asClones);

		public static List<Pattern> GetPatternsWhere(string moduleName, Predicate<Pattern> match, bool asClones = true)
		{
			var patternsForModule = patterns
				.Where(kvp => kvp.Key.moduleName == moduleName && match(kvp.Value))
				.Select(kvp => kvp.Value).ToList();

			if(asClones)
				patternsForModule.ForEach((pattern) => pattern.Clone());

			return patternsForModule;
		}

		/// <summary>
		/// Attempts to get a pattern <b> clone </b> from the pattern storage. As it is a clone, it can be modified freely.
		/// </summary>
		/// <param name="moduleName"> The rocket module this pattern belongs to </param>
		/// <param name="patternName"> The pattern name </param>
		/// <param name="pattern"> The pattern clone, null if not found </param>
		/// <returns> Whether the specified pattern has been found </returns>
		public static bool TryGetPattern(string moduleName, string patternName, out Pattern pattern)
		{
			bool foundPattern = patterns.TryGetValue((moduleName, patternName), out pattern);

			if (foundPattern)
 				pattern.Clone();
  			else
 				pattern = null;

			return foundPattern;
		}

		/// <summary>
		/// Sets the unlocked status on a pattern. This affects all players, in all subworlds.
		/// </summary>
		/// <param name="moduleName"> The rocket module this pattern belongs to </param>
		/// <param name="patternName"> The pattern name </param>
		/// <param name="unlockedState"> The unlocked state to set </param>
		public static void SetPatternUnlockedStatus(string moduleName, string patternName, bool unlockedState = true)
			 => patterns[(moduleName, patternName)].Unlocked = unlockedState;

		/// <summary>
		/// Gets the detail reference from the detail storage.
		/// </summary>
		/// <param name="moduleName"> The rocket module this detail belongs to </param>
		/// <param name="detailName"> The detail name </param>
		public static Detail GetDetail(string moduleName, string detailName)
			=> details[(moduleName, detailName)];

		/// <summary>
		/// Attempts to get a detail reference from the detail storage.
		/// </summary>
		/// <param name="moduleName"> The rocket module this detail belongs to </param>
		/// <param name="detailName"> The detail name </param>
		/// <param name="detail"> The detail, null if not found </param>
		/// <returns> Whether the specified detail has been found </returns>
		public static bool TryGetDetail(string moduleName, string detailName, out Detail detail)
			=> details.TryGetValue((moduleName, detailName), out detail);


		/// <summary>
		/// Sets the unlocked status on a detail. This affects all players, in all subworlds.
		/// </summary>
		/// <param name="moduleName"> The rocket module this detail belongs to </param>
		/// <param name="detailName"> The detail name </param>
		/// <param name="unlockedState"> The unlocked state to set </param>
		public static void SetDetailUnlockedStatus(string moduleName, string detailName, bool unlockedState = true)
			 => details[(moduleName, detailName)].Unlocked = unlockedState;


		public static PatternColorFunction GetFunction(string functionName)
			=> specialFunctions[functionName];

		public static bool TryGetFunction(string functionName, out PatternColorFunction function)
			=> specialFunctions.TryGetValue(functionName, out function);

		/// <summary>
		/// Sets the unlocked status on a dynamic color. This affects all players, in all subworlds.
		/// </summary>
		/// <param name="functionName"> The function name </param>
		/// <param name="unlockedState"> The unlocked state to set </param>
		public static void SetFunctionUnlockedStatus(string functionName, bool unlockedState = true)
			 => specialFunctions[functionName].Unlocked = unlockedState;

		public override void ClearWorld()
		{
			foreach (var unlockable in Unlockables)
 				unlockable.Unlocked = unlockable.UnlockedByDefault;
 		}

		public override void SaveWorldData(TagCompound tag) => SaveUnlockedStatus(tag);

		public override void LoadWorldData(TagCompound tag) => LoadUnlockedStatus(tag);	


		public static void SaveUnlockedStatus(TagCompound tag)
		{
			foreach (var unlockable in Unlockables)
 				if (unlockable.Unlocked && !unlockable.UnlockedByDefault)
 					tag[unlockable.GetKey() + "_Unlocked"] = true;
 		}

		public static void LoadUnlockedStatus(TagCompound tag)
		{
			foreach (var unlockable in Unlockables)
 				if (tag.ContainsKey(unlockable.GetKey() + "_Unlocked"))
 					unlockable.Unlocked = true;
 		}

		/// <summary>
		/// Adds a rocket module pattern to the pattern storage
		/// </summary>
		/// <param name="moduleName"> The rocket module this pattern belongs to </param>
		/// <param name="patternName"> The pattern name </param>
		/// <param name="unlockedByDefault"> Whether this pattern is unlocked by default
		/// <param name="colorData"> The color data (default colors, whether they are user changeable, dynamic color function) </param>
		private static void AddPattern(string moduleName, string patternName, bool unlockedByDefault, params PatternColorData[] colorData)
		{
			Pattern pattern = new Pattern(moduleName, patternName, unlockedByDefault, colorData);
			patterns.Add((moduleName, patternName), pattern);
		}

		/// <summary>
		/// Adds a detail to the detail storage
		/// </summary>
		/// <param name="moduleName"> The rocket module this detail belongs to </param>
		/// <param name="detailName"> The detail name </param>
		/// <param name="unlockedByDefault"> Whether this detail is unlocked by default </param>
		private static void AddDetail(string moduleName, string detailName, bool unlockedByDefault = false)
		{
			Detail detail = new(moduleName, detailName, unlockedByDefault);
			details.Add((moduleName, detailName), detail);
		}

		/// <summary>
		/// Adds a dynamic color function to the function storage
		/// The function has an array of 8 <see cref="Color"/>s as parameter, representing the current pattern colors:
		/// <code> (colors) => expressionHere </code> 
		/// </summary>
		/// <param name="functionName"> The function identifier name </param>
		/// <param name="function"> The function expression </param>
		/// <param name="unlockedbyDefault"> Whether  </param>
		private static void AddSpecialFunction(string functionName, Func<Color[], Color> function, bool unlockedbyDefault = false)
		{
			PatternColorFunction specialFunction = new(function, functionName, unlockedbyDefault);
			specialFunctions.Add(specialFunction.GetKey(), specialFunction);
		}

		private const string localizationPath = "Mods.Macrocosm.Subworlds.";
		public static UIListScrollablePanel ProvidePatternUI(string moduleName)
		{
			UIListScrollablePanel listPanel = new()
			{
				Width = new(0, 0.99f),
				Height = new(0, 0.4f),
				HAlign = 0.5f,
				Top = new(0f, 0.595f),
				BackgroundColor = new Color(53, 72, 135),
				BorderColor = new Color(89, 116, 213, 255)
			};
			listPanel.SetPadding(0f);
 
			var patterns = GetUnlockedPatterns(moduleName);
			int count = patterns.Count;

			UIElement patternIconRow = new()
			{
				Width = StyleDimension.FromPixelsAndPercent(0f, 1f),
				Height = StyleDimension.FromPixelsAndPercent(48 * (count / 10 + ((count % 10 != 0) ? 1 : 0)), 0f)
			};

			listPanel.Add(patternIconRow);
			patternIconRow.SetPadding(0f);

			for (int i = 0; i < count; i++)
			{
				Pattern pattern = patterns[i];
				UIPatternIcon icon = pattern.ProvideUI();

				icon.Left = StyleDimension.FromPixels((float)(i % 10) * 48f + 1f);
				icon.Top = StyleDimension.FromPixels((float)(i / 10) * 48f + 1f);

				patternIconRow.Append(icon);
			}

			return listPanel;
		}

		private static void LoadPatterns()
		{
			AddPattern("CommandPod", "Basic", true, new(Color.White), new(Color.White));
			AddPattern("CommandPod", "Binary", true, new(Color.White), new(Color.White), new(new Color(80, 80, 80)));
			AddPattern("CommandPod", "Astra", true, new(Color.Transparent), new(Color.DarkBlue), new(Color.White), new((colors) => Color.Lerp(colors[1], colors[2], 0.5f)));

			AddPattern("ServiceModule", "Basic", true, new(Color.Transparent), new(Color.White));
			AddPattern("ServiceModule", "Binary", true, new(Color.White), new(Color.White), new(new Color(80, 80, 80))); 
			AddPattern("ServiceModule", "Astra", true, new(Color.White), new(Color.DarkBlue), new(Color.White), new((colors) => Color.Lerp(colors[1], colors[2], 0.5f)));

			AddPattern("ReactorModule", "Basic", true, new(Color.Transparent), new(Color.White));
			AddPattern("ReactorModule", "Binary", true, new(Color.White), new(Color.White), new(new Color(50, 50, 50)));
			AddPattern("ReactorModule", "Astra", true, new(Color.White), new(Color.DarkBlue), new(Color.White), new((colors) => Color.Lerp(colors[1], colors[2], 0.5f)));

			AddPattern("EngineModule", "Basic", true, new(Color.Transparent), new(Color.White));
			AddPattern("EngineModule", "Binary", true, new(Color.White), new(Color.White), new(new Color(50, 50, 50)));
			AddPattern("EngineModule", "Saturn", true, new(Color.White), new(Color.White), new(new Color(40, 40, 40)));
			AddPattern("EngineModule", "Delta", true, new(Color.White), new(Color.White), new(new Color(40, 40, 40)));
			AddPattern("EngineModule", "Astra", true, new(Color.White), new(Color.DarkBlue), new(Color.White), new((colors) => Color.Lerp(colors[1], colors[2], 0.5f)));
			AddPattern("EngineModule", "Rainbow", false, new(Color.White), new(Color.Red), new(Color.Orange), new(Color.Yellow), new(Color.Green), new(Color.Blue), new(Color.Indigo), new(Color.Violet));

			AddPattern("BoosterLeft", "Basic", true, new(Color.White), new(Color.White));
			AddPattern("BoosterRight", "Basic", true, new(Color.White), new(Color.White));
		}

		private static void LoadDetails()
		{
			foreach (var country in Utility.CountryCodesAlpha3)
				AddDetail("EngineModule", "Flag_" + country, true);
		}

		private static void LoadSpecialFunctions()
		{
			AddSpecialFunction("Disco", (colors) => Main.DiscoColor);
			AddSpecialFunction("Celestial", (colors) => GlobalVFX.CelestialColor);
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
