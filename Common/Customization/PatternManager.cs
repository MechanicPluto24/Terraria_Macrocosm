using Macrocosm.Common.DataStructures;
using Macrocosm.Common.Utils;
using Macrocosm.Content.Rockets.Customization;
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
        private static readonly Dictionary<string, Dictionary<string, Pattern>> patterns = new();
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

        /// <summary> Gets a pattern by context and name. Profile is optional. Returns a dummy object if no match is found.  </summary>
        public static Pattern Get(string context, string name, string profile = null)
        {
            if (patterns.TryGetValue(context, out var profiles))
            {
                // Try to get a specific profile
                if (profile != null && profiles.TryGetValue(profile, out var specificPattern) && specificPattern.Name == name)
                    return specificPattern.Clone();

                // Fallback to the first profile matching the name
                var firstMatchingPattern = profiles.Values.FirstOrDefault(p => p.Name == name);
                return firstMatchingPattern?.Clone() ?? new();
            }

            return new();
        }

        /// <summary> Tries to get a pattern by context and name. Profile is optional. </summary>
        public static bool TryGet(string context, string name, out Pattern pattern, string profile = null)
        {
            if (patterns.TryGetValue(context, out var profiles))
            {
                if (profile != null && profiles.TryGetValue(profile, out var specificPattern) && specificPattern.Name == name)
                {
                    pattern = specificPattern.Clone();
                    return true;
                }

                var firstMatchingPattern = profiles.Values.FirstOrDefault(p => p.Name == name);
                if (firstMatchingPattern != null)
                {
                    pattern = firstMatchingPattern.Clone();
                    return true;
                }
            }

            pattern = new();
            return false;
        }

        /// <summary> Gets all patterns with the specified name across all contexts. </summary>
        public static IEnumerable<Pattern> GetAllByName(string name) => patterns.Values
                .SelectMany(profiles => profiles.Values)
                .Where(pattern => pattern.Name == name)
                .Select(pattern => pattern.Clone());

        /// <summary> Gets all unlocked patterns in the specified context.  </summary>
        public static IEnumerable<Pattern> GetAllUnlocked(string context) => unlockedPatterns
                .SelectMany(GetAllByName)
                .Where(pattern => pattern.Context == context)
                .Select(pattern => pattern.Clone());

        public static bool IsUnlocked(string name) => unlockedPatterns.Contains(name);

        public static void SetUnlocked(string name, bool unlocked)
        {
            if (unlocked)
                unlockedPatterns.Add(name);
            else
                unlockedPatterns.Remove(name);
        }

        private static string NormalizePath(string path) => Path.GetFullPath(path).Replace("\\", "/");

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
                    string name = json["name"]?.Value<string>() ?? throw new Exception("Missing 'name' field.");
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

                    bool unlockedByDefault = json["unlockedByDefault"]?.Value<bool>() ?? true;
                    if (unlockedByDefault)
                        SetUnlocked(name, true);

                    // Load a "default" profile if colorData is specified next to the other data, or is missing
                    JObject profilesJson = json["profiles"] as JObject ?? new JObject
                    {
                        ["default"] = new JObject { ["colorData"] = json["colorData"] ?? new JArray() }
                    };

                    var textureFiles = Mod.GetFileNames()
                        .Where(file => file.StartsWith(folderPath) && file.EndsWith(".rawimg") && file != iconAssetFile)
                        .ToList();

                    foreach (var textureFile in textureFiles)
                    {
                        string context = Path.GetFileNameWithoutExtension(textureFile);
                        var rawTexture = RawTexture.FromStream(Macrocosm.Instance.GetFileStream(textureFile));

                        if (!patterns.ContainsKey(context))
                            patterns[context] = new Dictionary<string, Pattern>();

                        foreach (var profileEntry in profilesJson.Properties())
                        {
                            string profileName = profileEntry.Name;
                            var colorDataJson = profileEntry.Value["colorData"] as JArray;
                            var patternJson = new JObject
                            {
                                ["name"] = name,
                                ["context"] = context,
                                ["profile"] = profileName,
                                ["texturePath"] = $"{Mod.Name}/{textureFile}".Replace(".rawimg", ""),
                                ["iconPath"] = iconAssetFile,
                                ["colorData"] = colorDataJson
                            };

                            var pattern = Pattern.FromJObject(patternJson);
                            patterns[context][profileName] = pattern;
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

        private Pattern CreatePattern(string name, string context, string profile, string texturePath, string iconPath, RawTexture rawTexture, JArray colorDataJson)
        {
            var defaultColorData = GenerateDefaultColorData(rawTexture);
            if (colorDataJson != null)
            {
                foreach (var entry in colorDataJson.OfType<JObject>())
                {
                    string colorHex = entry["key"]?.Value<string>();
                    if (Utility.TryGetColorFromHex(colorHex, out var color) && defaultColorData.ContainsKey(color))
                    {
                        var patternColorData = PatternColorData.FromJObject(entry);
                        defaultColorData[color] = patternColorData;
                    }
                }
            }

            return new Pattern(name, context, profile, texturePath, iconPath, defaultColorData);
        }


        private static Dictionary<Color, PatternColorData> GenerateDefaultColorData(RawTexture rawTexture)
        {
            var data = new Dictionary<Color, PatternColorData>();
            foreach (var pixel in rawTexture.Data)
            {
                if (pixel.A == 255 && !data.Keys.Any(k => Utility.ColorDistance(k, pixel) < 0.1f))
                    data[pixel] = new PatternColorData(pixel);
            }

            return data;
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
            foreach (var (context, profiles) in patterns)
            {
                logString += $"- Context: {context}\n";
                foreach (var (profile, pattern) in profiles)
                {
                    bool unlocked = unlockedPatterns.Contains(pattern.Name);
                    logString += $"\tProfile: {profile}, Name: {pattern.Name}, Unlocked: {unlocked}\n";
                }
            }

            Macrocosm.Instance.Logger.Info(logString);
        }
    }
}
