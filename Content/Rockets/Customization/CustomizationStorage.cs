using Macrocosm.Common.UI;
using Macrocosm.Common.UI.Themes;
using Macrocosm.Common.Utils;
using Macrocosm.Content.Rockets.UI;
using Microsoft.Xna.Framework;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.UI;

namespace Macrocosm.Content.Rockets.Customization
{
    public class CustomizationStorage : ModSystem
    {
        public static bool Initialized { get; private set; }

        private static Dictionary<(string moduleName, string patternName), Pattern> patterns;
        private static Dictionary<(string moduleName, string detailName), Detail> details;

		private static Dictionary<(string moduleName, string patternName), bool> patternUnlockStatus;
		private static Dictionary<(string moduleName, string detailName), bool> detailUnlockStatus;


		public override void Load()
		{
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
			patterns.Clear();
			details.Clear();
			patternUnlockStatus.Clear();
			detailUnlockStatus.Clear();

			patterns = null;
			details = null;
			patternUnlockStatus = null;
			detailUnlockStatus = null;


			Initialized = false;
		}

        public static void Reset()
        {
            ModContent.GetInstance<CustomizationStorage>().Unload();
            ModContent.GetInstance<CustomizationStorage>().Load();
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
			return GetPatternsWhere(moduleName, pattern => {
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
		public static void SetPatternUnlockedStatus(string moduleName, string patternName, bool unlockedState = true)
			 => patternUnlockStatus[(moduleName, patternName)] = unlockedState;

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

		public override void ClearWorld()
		{
			Reset();
		}

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
			detailUnlockStatus.Add((moduleName,detailName), unlockedByDefault);
		}

        public static UIListScrollablePanel ProvidePatternUI(string moduleName)
        {
            UIListScrollablePanel listPanel = new()
            {
                Width = new(0, 0.99f),
                Height = new(0, 0.8f),
                HAlign = 0.5f,
                Top = new(0f, 0.2f),
                BackgroundColor = UITheme.Current.PanelStyle.BackgroundColor,
                BorderColor = UITheme.Current.PanelStyle.BorderColor,
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

			// TODO: fix positioning just like in inventory panel
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
				Pattern pattern = patterns[i];
				UIPatternIcon icon = pattern.ProvideUI();

                icon.Left = new((i % iconsPerRow) * iconSize + iconOffsetLeft, 0f);
                icon.Top = new((i / iconsPerRow) * iconSize + iconOffsetTop, 0f);

                icon.Activate();
                patternIconContainer.Append(icon);
            }

            return listPanel;
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
