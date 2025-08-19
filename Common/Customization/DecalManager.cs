using Macrocosm.Content.Rockets.Modules;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace Macrocosm.Common.Customization;

public class DecalManager : ModSystem
{
    public static bool Initialized { get; private set; }
    private static bool shouldLogLoadedItems = true;

    private static Dictionary<(string decalName, string context), Decal> decals;

    private static Dictionary<(string decalName, string context), bool> decalUnlockStatus;

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
    public static Decal GetDecal(string decalName, string context)
        => decals[(decalName, context)];

    public static List<Decal> GetUnlockedDecals(string context)
        => GetDecalsWhere(context, decal => decalUnlockStatus.TryGetValue((decal.Name, context), out bool value) && value);

    public static List<Decal> GetDecalsWhere(string context, Func<Decal, bool> match)
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
		public static bool TryGetDecal(string decalName, string context, out Decal decal)
        => decals.TryGetValue((decalName, context), out decal);

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
    private static void AddDecal(string decalName, string context, string texturePath, string iconPath, bool unlockedByDefault = false)
    {
        Decal decal = new(decalName, context, texturePath, iconPath);
        decals.Add((decalName, context), decal);
        decalUnlockStatus.Add((decalName, context), unlockedByDefault);
    }

    private static void LoadDecals()
    {
        foreach (string context in RocketModule.Templates.Select(m => m.Name))
            AddDecal("None", context, Macrocosm.EmptyTexPath, "Macrocosm/Assets/Decals/Icons/None", true);

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
                string name = split[1].Replace(".rawimg", "");
                string texture = nameof(Macrocosm) + "/" + path.Replace(".rawimg", "");
                string icon = texture.Replace(context, "Icons");

                if (!IsDecalExcludedByRegion(name))
                    AddDecal(name, context, texture, icon, true);
            }
        }

        // Log the decal list
        string logstring = "Loaded " + decals.Count.ToString() + " decal" + (decalPaths.Count == 1 ? "" : "s") + ":\n";
        foreach (string context in RocketModule.Templates.Select(m => m.Name))
        {
            logstring += $" - Context: {context}\n\t";
            foreach (var kvp in decals)
            {
                (string decalName, string decalcontext) = kvp.Key;
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
