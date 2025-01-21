using Macrocosm.Common.DataStructures;
using Macrocosm.Common.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json.Linq;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace Macrocosm.Common.Customization
{
    public class PatternManager : ModSystem
    {
        private static readonly Dictionary<(string name, string context, string profile), Pattern> patterns = new();
        private static readonly HashSet<string> unlockedPatterns = new();

        public override void Load()
        {
            patterns.Clear();
            unlockedPatterns.Clear();

            LoadPatterns();
        }

        public override void Unload()
        {
            patterns.Clear();
            unlockedPatterns.Clear();
        }

        /// <summary> Gets a pattern by name, context and profile. Returns a dummy object if no match is found. </summary>
        public static Pattern Get(string name, string context, string profile = null)
        {
            if (profile != null)
            {
                if (patterns.TryGetValue((name, context, profile), out var specificPattern))
                    return specificPattern.Clone();
            }

            // Match first profile
            var matchingPattern = patterns.Where(kvp => kvp.Key.name == name && kvp.Key.context == context).Select(kvp => kvp.Value).FirstOrDefault();
            return matchingPattern?.Clone() ?? new Pattern();
        }

        /// <summary> Tries to get a pattern by name, context, and profile. </summary>
        public static bool TryGet(string name, string context, out Pattern pattern, string profile = "default")
        {
            pattern = Get(name, context, profile);
            return !string.IsNullOrEmpty(pattern.Name);
        }

        /// <summary>
        /// Gets all patterns based on the specified filters. Any filter set to null or default is ignored.
        /// </summary>
        /// <param name="name">The name of the pattern to filter by. Null to ignore.</param>
        /// <param name="context">The context of the pattern to filter by. Null to ignore.</param>
        /// <param name="profile">The profile of the pattern to filter by. Null to ignore.</param>
        /// <param name="unlocked">
        /// If true, only unlocked patterns will be returned. 
        /// If false, only locked patterns will be returned. 
        /// If null, unlocked status is ignored.
        /// </param>
        /// <returns>An enumerable of filtered patterns.</returns>
        public static IEnumerable<Pattern> GetAll(string name = null, string context = null, string profile = null, bool? unlocked = null)
        {
            var query = patterns.AsEnumerable();
            if (!string.IsNullOrEmpty(name))
                query = query.Where(kvp => kvp.Key.name == name);

            if (!string.IsNullOrEmpty(context))
                query = query.Where(kvp => kvp.Key.context == context);

            if (!string.IsNullOrEmpty(profile))
                query = query.Where(kvp => kvp.Key.profile == profile);

            if (unlocked.HasValue)
            {
                if (unlocked.Value)
                    query = query.Where(kvp => unlockedPatterns.Contains(kvp.Key.name));
                else
                    query = query.Where(kvp => !unlockedPatterns.Contains(kvp.Key.name));
            }

            return query.Select(kvp => kvp.Value.Clone());
        }

        public static bool IsUnlocked(string name) => unlockedPatterns.Contains(name);

        public static void SetUnlocked(string name, bool unlocked)
        {
            if (unlocked)
                unlockedPatterns.Add(name);
            else
                unlockedPatterns.Remove(name);
        }

        private void LoadPatterns()
        {
            var mpatternFiles = Mod.GetFileNames()
                .Where(file => file.EndsWith(".mpattern"))
                .ToList();

            foreach (var mpatternFile in mpatternFiles)
            {
                try
                {
                    string folderPath = mpatternFile[..mpatternFile.LastIndexOf('/')];

                    var json = JObject.Parse(Utility.GetTextFromFile(mpatternFile));

                    // "name" is the only data which is strictly required in the .mpattern files
                    string name = json["name"]?.Value<string>() ?? throw new Exception("Missing 'name' field.");

                    // "iconPath" can either be specified, or defaults to an image named "icon.*" (not case sensitive) in the same folder as the .mpattern
                    string iconAssetFile = json["iconPath"]?.Value<string>();
                    if (string.IsNullOrEmpty(iconAssetFile))
                    {
                        string inferredIconPath = Mod.GetFileNames()
                            .FirstOrDefault(file => file.StartsWith(folderPath) && file.ToLowerInvariant().EndsWith("icon.rawimg"));

                        if (!string.IsNullOrEmpty(inferredIconPath))
                            iconAssetFile = $"{Mod.Name}/{inferredIconPath}".Replace(".rawimg", "");
                        else
                            Macrocosm.Instance.Logger.Warn($"No icon found for pattern '{name}' in '{mpatternFile}'. Defaulting to empty texture.");
                    }

                    // "unlockedByDefault" determines whether the pattern is readily available, or must be unlocked by in-game means
                    bool unlockedByDefault = json["unlockedByDefault"]?.Value<bool>() ?? true;
                    if (unlockedByDefault)
                        SetUnlocked(name, true);

                    // Look for other image next to the .mpattern
                    var imageFiles = Mod.GetFileNames().Where(file => file.StartsWith(folderPath) && file.EndsWith(".rawimg") && file != iconAssetFile).ToList();
                    foreach (var imageFile in imageFiles)
                    {
                        string textureAssetPath = $"{Mod.Name}/{imageFile}".Replace(".rawimg", "");
                        var rawTexture = RawTexture.FromStream(Macrocosm.Instance.GetFileStream(imageFile));

                        // The "context" is determined by the image file name
                        string context = Path.GetFileNameWithoutExtension(imageFile);

                        // Assume a "profile" named "default" if colorData is specified outside of a profile, or is missing
                        JObject profiles = json["profiles"] as JObject ?? new JObject { ["default"] = new JObject { ["colorData"] = json["colorData"] ?? new JArray() }};
                        foreach (var profile in profiles.Properties())
                        {
                            Dictionary<Color, PatternColorData> colorData = new();

                            if (profile.Value["colorData"] is JObject colorDataJson)
                                foreach (var entry in colorDataJson.Properties())
                                    if (Utility.TryGetColorFromHex(entry.Name, out Color key) && entry.Value is JObject value && value != null)
                                        colorData[key] = PatternColorData.FromJObject(value);

                            // Keys and default from the first MaxColors unique fully opaque colors 
                            foreach (var color in rawTexture.GetUniqueColors(Pattern.MaxColors, maxDistance: 0.1f, c => c.A == 255))
                                colorData.TryAdd(color, new PatternColorData(color));

                            // Share missing colors across contexts for the same name and profile
                            var misingData = GetAll(name, profile: profile.Name).Select(kvp => kvp.ColorData);
                            foreach (var kvp in misingData.SelectMany(existingColorData => existingColorData.Where(kvp => !colorData.ContainsKey(kvp.Key))))
                                colorData[kvp.Key] = kvp.Value;

                            patterns[(name, context, profile.Name)] = new Pattern(name, context, profile.Name, textureAssetPath, iconAssetFile, colorData);
                        }
                    }

                }
                catch (Exception ex)
                {
                    Macrocosm.Instance.Logger.Error($"Error loading pattern from {mpatternFile}: {ex}");
                }
            }

            LogLoadedPatterns();
        }

        public override void SaveWorldData(TagCompound tag) => SaveData(tag);

        public static void SaveData(TagCompound tag)
        {
            tag["UnlockedPatterns"] = unlockedPatterns.ToList();
        }

        public override void LoadWorldData(TagCompound tag) => LoadData(tag);

        public static void LoadData(TagCompound tag)
        {
            if (tag.ContainsKey("UnlockedPatterns"))
            {
                var savedPatterns = tag.GetList<string>("UnlockedPatterns");
                foreach (var name in savedPatterns)
                    unlockedPatterns.Add(name);
            }
        }

        private static void LogLoadedPatterns()
        {
            string logString = "Loaded Patterns:\n";
            foreach (var ((name, context, profile), pattern) in patterns)
            {
                bool unlocked = unlockedPatterns.Contains(pattern.Name);
                logString += $"- Name: {name}, Context: {context}, Profile: {profile}, Unlocked: {unlocked}\n";
            }
            Macrocosm.Instance.Logger.Info(logString);
        }
    }
}
