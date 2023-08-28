using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.UI;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Macrocosm.Common.UI;
using Macrocosm.Common.Utils;
using Macrocosm.Common.Drawing;
using Hjson;
using Terraria.IO;
using Newtonsoft.Json.Linq;
using System.IO;

namespace Macrocosm.Content.Rockets.Customization
{
	internal class CustomizationStorage : ModSystem
	{
		public static bool Initialized { get; private set; }

		private static Dictionary<(string moduleName, string patternName), Pattern> patterns;
		private static Dictionary<(string moduleName, string detailName), Detail> details;

		private static Dictionary<string, ColorFunction> functions;

		private static IEnumerable<IUnlockable> Unlockables => Utility.Concatenate<IUnlockable>(patterns.Values, details.Values, functions.Values);
			
		public override void Load()
		{
			patterns = new Dictionary<(string,string), Pattern>();
			details = new Dictionary<(string, string), Detail>();
 			functions = new Dictionary<string, ColorFunction>();

			LoadFunctions(); // Load functions first, as they are used in the pattern loading
			LoadPatterns();
			LoadDetails();

			Initialized = true;
		}

		public override void Unload()
		{
			patterns.Clear();
			details.Clear();
			functions.Clear();
			patterns = null;
			details = null;
			functions = null;

			Initialized = false;
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

		public static Pattern GetDefaultPattern(string moduleName)
			=> patterns[(moduleName, "Basic")].Clone();

		public static List<Pattern> GetUnlockedPatterns(string moduleName, bool asClones = true) => GetPatternsWhere(moduleName, (pattern) => pattern.Unlocked, asClones);

		public static List<Pattern> GetPatternsWhere(string moduleName, Func<Pattern, bool> match, bool asClones = true)
		{
			var patternsForModule = patterns
				.Select(kvp => kvp.Value)
				.Where(pattern => pattern.ModuleName == moduleName && match(pattern))
				.ToList();

			if (asClones)
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
			if (patterns.TryGetValue((moduleName, patternName), out Pattern defaultPattern))
			{
				pattern = defaultPattern.Clone();
				return true;
			}
			else
			{
 				pattern = null;
				return false;
			}
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


		public static ColorFunction GetFunction(string functionName)
			=> functions[functionName];

		public static bool TryGetFunction(string functionName, out ColorFunction function)
			=> functions.TryGetValue(functionName, out function);

		/// <summary>
		/// Sets the unlocked status on a dynamic color. This affects all players, in all subworlds.
		/// </summary>
		/// <param name="functionName"> The function name </param>
		/// <param name="unlockedState"> The unlocked state to set </param>
		public static void SetFunctionUnlockedStatus(string functionName, bool unlockedState = true)
			 => functions[functionName].Unlocked = unlockedState;

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
			Pattern pattern = new(moduleName, patternName, unlockedByDefault, colorData);
			patterns.Add((moduleName, patternName), pattern);
		}

		private static void AddPattern(Pattern pattern)
		{
			patterns.Add((pattern.ModuleName, pattern.Name), pattern);
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
		/// Adds a dynamic color function expression to the function storage
		/// The function has an array of 8 <see cref="Color"/>s as parameter, representing the current pattern colors:
		/// <code> (colors) => expressionHere </code> 
		/// </summary>
		/// <param name="functionName"> The function identifier name </param>
		/// <param name="function"> The function expression </param>
		/// <param name="unlockedbyDefault"> Whether  </param>
		private static void AddFunction(string functionName, Func<Color[], Color> function, bool unlockedbyDefault = false)
		{
			ColorFunction func = new(function, functionName, unlockedbyDefault);
			functions.Add(func.GetKey(), func);
		}

		/// <summary>
		/// Adds a dynamic color function to the function storage
		/// The function has an array of 8 <see cref="Color"/>s as parameter, representing the current pattern colors.
		/// </summary>
		/// <param name="function"> The function object </param>
		private static void AddFunction(ColorFunction function)
		{
			functions.Add(function.GetKey(), function);
		}

		private const string localizationPath = "Mods.Macrocosm.Subworlds.";
		public static UIListScrollablePanel ProvidePatternUI(string moduleName)
		{
			UIListScrollablePanel listPanel = new()
			{
				Width = new(0, 0.99f),
				Height = new(0, 0.8f),
				HAlign = 0.5f,
				Top = new(0f, 0.2f),
				BackgroundColor = new Color(53, 72, 135),
				BorderColor = new Color(89, 116, 213, 255),
				ListPadding = 0f,
				ListOuterPadding = 2f,
				ScrollbarHeight = new(0f, 0.9f),
				ScrollbarHAlign = 0.99f,
				ListWidthWithScrollbar = new(0, 1f),
				ListWidthWithoutScrollbar = new(0, 1f)
			};
			listPanel.SetPadding(0f);

			var patterns = GetUnlockedPatterns(moduleName);
			int count = patterns.Count;

			int iconsPerRow = 9;
			float iconSize;
			float iconOffsetLeft;
			float iconOffsetTop;

			if (count <= iconsPerRow)
			{
				iconSize = 44f + 7f;
				iconOffsetLeft = 8f;
				iconOffsetTop = 7f;
			} 
			else
			{
				iconSize = 44f + 5f;
				iconOffsetLeft = 7f;
				iconOffsetTop = 7f;
			}

			UIElement patternIconContainer = new()
			{
				Width = new(0f, 1f),
				Height = new(iconSize * (count / iconsPerRow + ((count % iconsPerRow != 0) ? 1 : 0)), 0f),
			};

			listPanel.Add(patternIconContainer);
			patternIconContainer.SetPadding(0f);

			for (int i = 0; i < count; i++)
			{
				Pattern pattern = patterns[i].Clone();
				UIPatternIcon icon = pattern.ProvideUI();

				icon.Left = new((i % iconsPerRow) * iconSize + iconOffsetLeft, 0f);
				icon.Top = new((i / iconsPerRow) * iconSize + iconOffsetTop, 0f);

				icon.Activate();
				patternIconContainer.Append(icon);
			}

			return listPanel;
		}

		private static void LoadFunctions()
		{
			AddFunction("Disco", (colors) => Main.DiscoColor);
			AddFunction("Celestial", (colors) => GlobalVFX.CelestialColor);
		}

		private static void LoadPatterns()
		{
			try
			{
				JArray patternsArray = Utility.ParseJSONFromFile("Content/Rockets/Customization/Patterns/patterns.json");

				foreach (JObject patternObject in patternsArray.Cast<JObject>())
 					AddPattern(Pattern.FromJSON(patternObject.ToString()));
 			}
			catch (Exception ex)
			{
				Macrocosm.Instance.Logger.Error(ex.Message);
			}

			// Just for testing the scrollbar
			for (int i = 1; i <= 7; i++)
 				AddPattern("ServiceModule", "Test" + i, true, new(Color.Transparent), new(Color.White));
 			
			for (int i = 1; i <= 8; i++)
 				AddPattern("ReactorModule", "Test" + i, true, new(Color.Transparent), new(Color.White));
			
			for (int i = 1; i <= 74; i++)
 				AddPattern("EngineModule", "Test" + i, true, new(Color.White), new(Color.White));
 		}

		private static void LoadDetails()
		{
			foreach (var country in Utility.CountryCodesAlpha3)
				AddDetail("EngineModule", "Flag_" + country, true);
		}
	}
}
