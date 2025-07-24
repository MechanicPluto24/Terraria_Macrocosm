using Macrocosm.Common.DataStructures;
using Macrocosm.Common.Utils;
using Microsoft.Xna.Framework;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace Macrocosm.Common.Customization;

public class PatternManager : ModSystem
{
    private static readonly Dictionary<(string name, string context), Pattern> patterns = new();
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

    /// <summary> Gets a pattern by name, context. Returns a dummy object if no match is found. </summary>
    public static Pattern Get(string name, string context)
    {
        if (patterns.TryGetValue((name, context), out var specificPattern))
            return specificPattern.Clone();

        return new Pattern();
    }

    /// <summary> Tries to get a pattern by name and context. </summary>
    public static bool TryGet(string name, string context, out Pattern pattern)
    {
        pattern = Get(name, context);
        return !string.IsNullOrEmpty(pattern.Name);
    }

    /// <summary>
    /// Gets all patterns based on the specified filters. Any filter set to null or default is ignored.
    /// </summary>
    /// <param name="name">The name of the pattern to filter by. Null to ignore.</param>
    /// <param name="context">The context of the pattern to filter by. Null to ignore.</param>
    /// <param name="unlocked">
    /// If true, only unlocked patterns will be returned. 
    /// If false, only locked patterns will be returned. 
    /// If null, unlocked status is ignored.
    /// </param>
    /// <returns>An enumerable of filtered patterns.</returns>
    public static IEnumerable<Pattern> GetAll(string name = null, string context = null, bool? unlocked = null)
    {
        var query = patterns.AsEnumerable();
        if (!string.IsNullOrEmpty(name))
            query = query.Where(kvp => kvp.Key.name == name);

        if (!string.IsNullOrEmpty(context))
            query = query.Where(kvp => kvp.Key.context == context);

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

                // "unlockedByDefault" determines whether the pattern is readily available, or must be unlocked by in-game means
                bool unlockedByDefault = json["unlockedByDefault"]?.Value<bool>() ?? true;
                if (unlockedByDefault)
                    SetUnlocked(name, true);

                // Look for other image next to the .mpattern
                var imageFiles = Mod.GetFileNames().Where(file => file.StartsWith(folderPath) && file.EndsWith(".rawimg")).ToList();
                foreach (var imageFile in imageFiles)
                {
                    string textureAssetPath = $"{Mod.Name}/{imageFile}".Replace(".rawimg", "");
                    var rawTexture = RawTexture.FromStream(Macrocosm.Instance.GetFileStream(imageFile));

                    // The "context" is determined by the image file name
                    string context = Path.GetFileNameWithoutExtension(imageFile);

                    Dictionary<Color, PatternColorData> colorData = new();

                    if (json["colorData"] is JObject colorDataJson)
                    {
                        foreach (var entry in colorDataJson.Properties())
                        {
                            if (Utility.TryGetColorFromHex(entry.Name, out Color key))
                            {
                                if (entry.Value.Type == JTokenType.String) // Default to user color when it's a shorthand ("#XXXXXX" : "#YYYYYY")
                                {
                                    string colorHex = entry.Value.Value<string>();
                                    if (Utility.TryGetColorFromHex(colorHex, out Color color))
                                        colorData[key] = new PatternColorData(color, isUserModifiable: true);
                                    else
                                        throw new ArgumentException($"Invalid color shorthand: {colorHex}");
                                }
                                else if (entry.Value is JObject value && value != null) // Full object ({"color": "#YYYYYY"})
                                {
                                    colorData[key] = PatternColorData.FromJObject(value);
                                }
                            }
                        }
                    }

                    // Keys and default from the first MaxColors unique fully opaque colors 
                    foreach (var color in rawTexture.GetUniqueColors(Pattern.MaxColors, maxDistance: Pattern.ColorDistance, alphaSensitive: false, validColor: c => c.A > 0))
                    {
                        colorData.TryAdd(color, new PatternColorData(color));
                    }

                    patterns[(name, context)] = new Pattern(name, context, textureAssetPath, colorData);
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
        foreach (var ((name, context), pattern) in patterns)
        {
            bool unlocked = unlockedPatterns.Contains(pattern.Name);
            logString += $"- Name: {name}, Context: {context}, Unlocked: {unlocked}\n";
        }
        Macrocosm.Instance.Logger.Info(logString);
    }
}
