using Macrocosm.Common.Utils;
using Macrocosm.Content.Rockets.Customization;
using Microsoft.Xna.Framework;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Terraria.IO;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace Macrocosm.Common.Customization
{
    public class PatternManager : ModSystem
    {
        private static readonly Dictionary<(string context, string name), Pattern> _patterns = new();
        private static readonly Dictionary<(string context, string name), bool> _unlockStatus = new();

        public static IEnumerable<Pattern> All => _patterns.Values;

        public override void Load()
        {
            _patterns.Clear();
            _unlockStatus.Clear();

            LoadPatterns();
        }

        public override void Unload()
        {
            _patterns.Clear();
            _unlockStatus.Clear();
        }

        public static Pattern Get(string context, string name) =>
            _patterns.TryGetValue((context, name), out var pattern) ? pattern : default;

        public static bool TryGet(string context, string name, out Pattern pattern)
        {
            pattern = Get(context, name);
            return pattern != default;
        }

        public static bool IsUnlocked(string context, string name) => _unlockStatus.TryGetValue((context, name), out var unlocked) && unlocked;
        public static List<Pattern> GetAllUnlocked(string context) =>
            All.Where(p => p.Context == context && _unlockStatus.TryGetValue((p.Context, p.Name), out var unlocked) && unlocked).ToList();

        public static void SetUnlocked(string context, string name, bool unlocked)
        {
            _unlockStatus[(context, name)] = unlocked;
        }

        private void LoadPatterns()
        {
            var patternFiles = Mod.GetFileNames()
                .Where(file => file.EndsWith("pattern.json"))
                .ToList();

            foreach (var file in patternFiles)
            {
                try
                {
                    var json = JObject.Parse(Utility.GetTextFromFile(file));
                    LoadPatternFromJson(file, json, Mod);
                }
                catch (System.Exception ex)
                {
                    Macrocosm.Instance.Logger.Error($"Error loading pattern from {file}: {ex}");
                }
            }

            LogLoadedPatterns();
        }

        private static void LoadPatternFromJson(string jsonFilePath, JObject json, Mod mod)
        {
            string name = json["name"]?.Value<string>();
            if (string.IsNullOrEmpty(name))
            {
                Macrocosm.Instance.Logger.Warn($"Pattern JSON '{jsonFilePath}' is missing a 'name' field.");
                return;
            }

            string iconPath = json["iconPath"]?.Value<string>();
            if (!string.IsNullOrEmpty(iconPath) && !mod.RootContentSource.HasAsset(iconPath))
            {
                Macrocosm.Instance.Logger.Warn($"Invalid or missing icon path '{iconPath}' for pattern '{name}'. Skipping...");
                return;
            }

            var colorData = json["colorData"]?
                .Select(c => new PatternColorData(
                    Utility.TryGetColorFromHex(c["color"]?.ToString(), out Color color) ? color : Color.Transparent,
                    c["userModifiable"]?.Value<bool>() ?? true))
                .ToImmutableArray() ?? ImmutableArray<PatternColorData>.Empty;

            string folderPath = System.IO.Path.GetDirectoryName(jsonFilePath) ?? string.Empty;
            var textureFiles = mod.GetFileNames()
                .Where(f => f.StartsWith(folderPath) && f != jsonFilePath && f.EndsWith(".png"))
                .ToList();

            foreach (var texturePath in textureFiles)
            {
                string context = System.IO.Path.GetFileNameWithoutExtension(texturePath);
                var pattern = new Pattern(context, name, texturePath, iconPath, colorData.ToArray());
                _patterns[(context, name)] = pattern;

                if (!_unlockStatus.ContainsKey((context, name)))
                    _unlockStatus[(context, name)] = false;
            }
        }

        public static void SaveData(TagCompound tag)
        {
            var unlockedPatterns = _unlockStatus.Where(kvp => kvp.Value).Select(kvp => $"{kvp.Key.context}|{kvp.Key.name}").ToList();
            tag["UnlockedPatterns"] = unlockedPatterns;
        }

        public static void LoadData(TagCompound tag)
        {
            if (tag.ContainsKey("UnlockedPatterns"))
            {
                var unlockedPatterns = tag.GetList<string>("UnlockedPatterns");
                foreach (var entry in unlockedPatterns)
                {
                    var parts = entry.Split('|');
                    if (parts.Length == 2)
                    {
                        var context = parts[0];
                        var name = parts[1];
                        _unlockStatus[(context, name)] = true;
                    }
                }
            }
        }

        public override void SaveWorldData(TagCompound tag) => SaveData(tag);

        public override void LoadWorldData(TagCompound tag) => LoadData(tag);

        private static void LogLoadedPatterns()
        {
            string logString = "Loaded Patterns:\n";
            foreach (var pattern in All)
                logString += $"- Name: {pattern.Name}, Context: {pattern.Context}, Unlocked: {IsUnlocked(pattern.Context, pattern.Name)}\n";

            Macrocosm.Instance.Logger.Info(logString);
        }
    }
}
