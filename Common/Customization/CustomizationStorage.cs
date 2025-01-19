using Macrocosm.Common.Utils;
using Macrocosm.Content.Rockets;
using Macrocosm.Content.Rockets.Customization;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace Macrocosm.Common.Customization
{
    public class CustomizationStorage : ModSystem
    {
        public static bool Initialized { get; private set; }
        private static bool shouldLogLoadedItems = true;

        private static Dictionary<(string context, string detailName), Detail> details;

        private static Dictionary<(string context, string detailName), bool> detailUnlockStatus;

        public override void Load()
        {
            details = new();
            detailUnlockStatus = new();

            LoadDetails();

            Initialized = true;
            shouldLogLoadedItems = false;
        }

        public override void Unload()
        {
            details = null;
            detailUnlockStatus = null;

            Initialized = false;
        }

        public static void Reset()
        {
            Initialized = false;

            details = new();
            detailUnlockStatus = new();

            LoadDetails();

            Initialized = true;
        }

        /// <summary>
        /// Gets the detail reference from the detail storage.
        /// </summary>
        /// <param name="context"> The  context this detail belongs to </param>
        /// <param name="detailName"> The detail name </param>
        public static Detail GetDetail(string context, string detailName)
            => details[(context, detailName)];

        public static List<Detail> GetUnlockedDetails(string context)
        {
            return GetDetailsWhere(context, detail =>
            {
                var key = (context, detail.Name);
                return detailUnlockStatus.ContainsKey(key) && detailUnlockStatus[key];
            });
        }

        public static List<Detail> GetDetailsWhere(string context, Func<Detail, bool> match)
        {
            var detailsForContext = details
                .Select(kvp => kvp.Value)
                .Where(detail => detail.Context == context && match(detail))
                .ToList();

            return detailsForContext;
        }

        /// <summary>
        /// Attempts to get a detail reference from the detail storage.
        /// </summary>
        /// <param name="context"> The context this detail belongs to </param>
        /// <param name="detailName"> The detail name </param>
        /// <param name="detail"> The detail, null if not found </param>
        /// <returns> Whether the specified detail has been found </returns>
		public static bool TryGetDetail(string context, string detailName, out Detail detail)
            => details.TryGetValue((context, detailName), out detail);

        public override void ClearWorld() => Reset();

        public override void SaveWorldData(TagCompound tag) => SaveData(tag);

        public override void LoadWorldData(TagCompound tag) => LoadData(tag);


        public static void SaveData(TagCompound tag)
        {
            foreach (var kvp in detailUnlockStatus)
                if (kvp.Value)
                    tag["Detail_" + kvp.Key + "_Unlocked"] = true;
        }

        public static void LoadData(TagCompound tag)
        {
            foreach (var kvp in detailUnlockStatus)
                if (tag.ContainsKey("Detail_" + kvp.Key + "_Unlocked"))
                    detailUnlockStatus[kvp.Key] = true;
        }

        /// <summary>
        /// Adds a detail to the detail storage
        /// </summary>
        /// <param name="context"> The context this detail belongs to </param>
        /// <param name="detailName"> The detail name </param>
        /// <param name="unlockedByDefault"> Whether this detail is unlocked by default </param>
        private static void AddDetail(string context, string detailName, bool unlockedByDefault = false)
        {
            Detail detail = new(context, detailName);
            details.Add((context, detailName), detail);
            detailUnlockStatus.Add((context, detailName), unlockedByDefault);
        }

        private static void LoadDetails()
        {
            foreach (string context in Rocket.ModuleNames)
                AddDetail(context, "None", true);

            if (Main.dedServ)
                return;

            // Find all existing details
            string lookupString = "Content/Rockets/Customization/Details/";
            var detailPathsWithIcons = Macrocosm.Instance.RootContentSource.GetAllAssetsStartingWith(lookupString, true).ToList();
            var detailPaths = detailPathsWithIcons.Where(x => !x.Contains("/Icons")).ToList();

            // Log the detail list
            foreach (var detailWithContext in detailPaths)
            {
                string[] split = detailWithContext.Replace(lookupString, "").Split('/');

                if (split.Length == 2)
                {
                    string context = split[0];
                    string detail = split[1].Replace(".rawimg", "");

                    if (!IsDetailExcludedByRegion(detail))
                        AddDetail(context, detail, true);
                }
            }

            string logstring = "Loaded " + details.Count.ToString() + " detail" + (detailPaths.Count == 1 ? "" : "s") + ":\n";
            foreach (string context in Rocket.ModuleNames)
            {
                logstring += $" - Context: {context}\n\t";
                foreach (var kvp in details)
                {
                    (string Detailcontext, string detailName) = kvp.Key;
                    if (Detailcontext == context && !IsDetailExcludedByRegion(detailName))
                        logstring += $"{detailName} ";
                }
                logstring += "\n\n";
            }

            if (shouldLogLoadedItems)
                Macrocosm.Instance.Logger.Info(logstring);
        }

        private static bool IsDetailExcludedByRegion(string detailName)
        {
            string country = System.Globalization.RegionInfo.CurrentRegion.ThreeLetterISORegionName;

            if (country is "CHN" && detailName is "Flag_TWN" or "Flag_MAC" or "Flag_HKG")
                return true;

            if (country is "MAC" && detailName is "Flag_TWN")
                return true;

            if (country is "HKG" && detailName is "Flag_TWN")
                return true;

            if (country is "TWN" && detailName is "Flag_CHN")
                return true;

            return false;
        }
    }
}
