using Macrocosm.Content.Rockets;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace Macrocosm.Common.Customization
{
    public class DecalManager : ModSystem
    {
        public static bool Initialized { get; private set; }
        private static bool shouldLogLoadedItems = true;

        private static Dictionary<(string context, string decalName), Decal> decals;

        private static Dictionary<(string context, string decalName), bool> decalUnlockStatus;

        public override void Load()
        {
            decals = new();
            decalUnlockStatus = new();

            LoadDecals();

            Initialized = true;
            shouldLogLoadedItems = false;
        }

        public override void Unload()
        {
            decals = null;
            decalUnlockStatus = null;

            Initialized = false;
        }

        public static void Reset()
        {
            Initialized = false;

            decals = new();
            decalUnlockStatus = new();

            LoadDecals();

            Initialized = true;
        }

        /// <summary>
        /// Gets the decal reference from the decal storage.
        /// </summary>
        /// <param name="context"> The  context this decal belongs to </param>
        /// <param name="decalName"> The decal name </param>
        public static Decal GetDecal(string context, string decalName)
            => decals[(context, decalName)];

        public static List<Decal> GetUnlockedDecals(string context)
        {
            return GetdecalsWhere(context, decal =>
            {
                var key = (context, decal.Name);
                return decalUnlockStatus.ContainsKey(key) && decalUnlockStatus[key];
            });
        }

        public static List<Decal> GetdecalsWhere(string context, Func<Decal, bool> match)
        {
            var decalsForContext = decals
                .Select(kvp => kvp.Value)
                .Where(decal => decal.Context == context && match(decal))
                .ToList();

            return decalsForContext;
        }

        /// <summary>
        /// Attempts to get a decal reference from the decal storage.
        /// </summary>
        /// <param name="context"> The context this decal belongs to </param>
        /// <param name="decalName"> The decal name </param>
        /// <param name="decal"> The decal, null if not found </param>
        /// <returns> Whether the specified decal has been found </returns>
		public static bool TryGetDecal(string context, string decalName, out Decal decal)
            => decals.TryGetValue((context, decalName), out decal);

        public override void ClearWorld() => Reset();

        public override void SaveWorldData(TagCompound tag) => SaveData(tag);

        public override void LoadWorldData(TagCompound tag) => LoadData(tag);


        public static void SaveData(TagCompound tag)
        {
            foreach (var kvp in decalUnlockStatus)
                if (kvp.Value)
                    tag["decal_" + kvp.Key + "_Unlocked"] = true;
        }

        public static void LoadData(TagCompound tag)
        {
            foreach (var kvp in decalUnlockStatus)
                if (tag.ContainsKey("decal_" + kvp.Key + "_Unlocked"))
                    decalUnlockStatus[kvp.Key] = true;
        }

        /// <summary>
        /// Adds a decal to the decal storage
        /// </summary>
        /// <param name="context"> The context this decal belongs to </param>
        /// <param name="decalName"> The decal name </param>
        /// <param name="unlockedByDefault"> Whether this decal is unlocked by default </param>
        private static void AddDecal(string context, string decalName, string texturePath, string iconPath, bool unlockedByDefault = false)
        {
            Decal decal = new(context, decalName, texturePath, iconPath);
            decals.Add((context, decalName), decal);
            decalUnlockStatus.Add((context, decalName), unlockedByDefault);
        }

        private static void LoadDecals()
        {
            foreach (string context in Rocket.ModuleNames)
                AddDecal(context, "None", Macrocosm.EmptyTexPath, "Assets/Decals/Icons/None", true);

            if (Main.dedServ)
                return;

            // Find all existing decals
            string lookupString = "Assets/Decals/";
            var decalPathsWithIcons = Macrocosm.Instance.RootContentSource.GetAllAssetsStartingWith(lookupString, true).ToList();
            var decalPaths = decalPathsWithIcons.Where(x => !x.Contains("/Icons")).ToList();

            foreach (var path in decalPaths)
            {
                string[] split = path.Replace(lookupString, "").Split('/');
                if (split.Length == 2)
                {
                    string context = split[0];
                    string decal = split[1].Replace(".rawimg", "");

                    if (!IsDecalExcludedByRegion(decal))
                        AddDecal(context, decal, path, path.Replace(context, "Icons"), true);
                }
            }

            // Log the decal list
            string logstring = "Loaded " + decals.Count.ToString() + " decal" + (decalPaths.Count == 1 ? "" : "s") + ":\n";
            foreach (string context in Rocket.ModuleNames)
            {
                logstring += $" - Context: {context}\n\t";
                foreach (var kvp in decals)
                {
                    (string decalcontext, string decalName) = kvp.Key;
                    if (decalcontext == context && !IsDecalExcludedByRegion(decalName))
                        logstring += $"{decalName} ";
                }
                logstring += "\n\n";
            }

            if (shouldLogLoadedItems)
                Macrocosm.Instance.Logger.Info(logstring);
        }

        private static bool IsDecalExcludedByRegion(string decalName)
        {
            string country = System.Globalization.RegionInfo.CurrentRegion.ThreeLetterISORegionName;

            if (country is "CHN" && decalName is "Flag_TWN" or "Flag_MAC" or "Flag_HKG")
                return true;

            if (country is "MAC" && decalName is "Flag_TWN")
                return true;

            if (country is "HKG" && decalName is "Flag_TWN")
                return true;

            if (country is "TWN" && decalName is "Flag_CHN")
                return true;

            return false;
        }
    }
}
