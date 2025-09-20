using Macrocosm.Common.Subworlds;
using SubworldLibrary;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace Macrocosm.Common.Systems.Flags;

public class WorldData : ModSystem
{
    public static bool DemonSun { get; set; } = false;

    public static bool DownedCraterDemon { get; set; } = false;
    public static bool DownedMoonBeast { get; set; } = false;
    public static bool DownedDementoxin { get; set; } = false;
    public static bool DownedLunalgamate { get; set; } = false;

    public static bool LuminiteShrineUnlocked { get; set; } = false;
    public static bool HeavenforgeShrineUnlocked { get; set; } = false;
    public static bool LunarRustShrineUnlocked { get; set; } = false;
    public static bool AstraShrineUnlocked { get; set; } = false;
    public static bool DarkCelestialShrineUnlocked { get; set; } = false;
    public static bool MercuryShrineUnlocked { get; set; } = false;
    public static bool StarRoyaleShrineUnlocked { get; set; } = false;
    public static bool CryocoreShrineUnlocked { get; set; } = false;
    public static bool CosmicEmberShrineUnlocked { get; set; } = false;
    public static bool AnyShimmerShrine => new List<bool>() { HeavenforgeShrineUnlocked, LunarRustShrineUnlocked, AstraShrineUnlocked, DarkCelestialShrineUnlocked, MercuryShrineUnlocked, StarRoyaleShrineUnlocked, CryocoreShrineUnlocked, CosmicEmberShrineUnlocked }.Any(b => b);

    public static bool FoundVulcan { get; set; } = false;
    public static bool DeimosReturn { get; set; } = false;

    public record SubworldData
    {
        public bool Unlocked { get; set; }
        public bool SolarStorm { get; set; }
        public bool MeteorStorm { get; set; }
        public bool XaocRift { get; set; }

        public TagCompound Save()
        {
            TagCompound tag = new();

            if (Unlocked) tag[nameof(Unlocked)] = true;
            if (SolarStorm) tag[nameof(SolarStorm)] = true;
            if (MeteorStorm) tag[nameof(MeteorStorm)] = true;
            if (XaocRift) tag[nameof(XaocRift)] = true;

            return tag;
        }

        public static SubworldData Load(TagCompound tag)
        {
            return new SubworldData()
            {
                Unlocked = tag.ContainsKey(nameof(Unlocked)),
                SolarStorm = tag.ContainsKey(nameof(SolarStorm)),
                MeteorStorm = tag.ContainsKey(nameof(MeteorStorm)),
                XaocRift = tag.ContainsKey(nameof(XaocRift))
            };
        }

        public void NetSend(BinaryWriter writer)
        {
            writer.WriteFlags
            (
                Unlocked,
                SolarStorm,
                MeteorStorm,
                XaocRift
            );
        }

        public static SubworldData NetReceive(BinaryReader reader)
        {
            SubworldData data = new();
            (
                data.Unlocked,
                data.SolarStorm,
                data.MeteorStorm,
                data.XaocRift
            )
            = reader.ReadBitsByte();

            return data;
        }
    }

    private static readonly Dictionary<string, SubworldData> _subworldData = new();
    public static SubworldData Current => GetSubworldData(MacrocosmSubworld.CurrentID);
    public static SubworldData GetSubworldData(string subworldName)
    {
        if (!_subworldData.TryGetValue(subworldName, out var data))
        {
            data = new SubworldData();
            _subworldData[subworldName] = data;
        }
        return data;
    }

    public override void ClearWorld()
    {
        _subworldData.Clear();

        DownedCraterDemon = false;
        DownedMoonBeast = false;
        DownedDementoxin = false;
        DownedLunalgamate = false;

        LuminiteShrineUnlocked = false;
        HeavenforgeShrineUnlocked = false;
        LunarRustShrineUnlocked = false;
        AstraShrineUnlocked = false;
        DarkCelestialShrineUnlocked = false;
        MercuryShrineUnlocked = false;
        StarRoyaleShrineUnlocked = false;
        CryocoreShrineUnlocked = false;
        CosmicEmberShrineUnlocked = false;

        FoundVulcan = false;
        DeimosReturn = false;
    }

    public override void SaveWorldData(TagCompound tag) => SaveData(tag);
    public override void LoadWorldData(TagCompound tag) => LoadData(tag);

    public static void SaveData(TagCompound tag)
    {
        var subworldTags = new List<TagCompound>();
        foreach (var (key, value) in _subworldData)
        {
            var entry = value.Save();
            entry["ID"] = key;
            subworldTags.Add(entry);
        }
        tag["SubworldData"] = subworldTags;

        if (DownedCraterDemon) tag[nameof(DownedCraterDemon)] = true;
        if (DownedMoonBeast) tag[nameof(DownedMoonBeast)] = true;
        if (DownedDementoxin) tag[nameof(DownedDementoxin)] = true;
        if (DownedLunalgamate) tag[nameof(DownedLunalgamate)] = true;

        if (LuminiteShrineUnlocked) tag[nameof(DownedLunalgamate)] = true;
        if (HeavenforgeShrineUnlocked) tag[nameof(HeavenforgeShrineUnlocked)] = true;
        if (LunarRustShrineUnlocked) tag[nameof(LunarRustShrineUnlocked)] = true;
        if (AstraShrineUnlocked) tag[nameof(AstraShrineUnlocked)] = true;
        if (DarkCelestialShrineUnlocked) tag[nameof(DarkCelestialShrineUnlocked)] = true;
        if (MercuryShrineUnlocked) tag[nameof(MercuryShrineUnlocked)] = true;
        if (StarRoyaleShrineUnlocked) tag[nameof(StarRoyaleShrineUnlocked)] = true;
        if (CryocoreShrineUnlocked) tag[nameof(CryocoreShrineUnlocked)] = true;
        if (CosmicEmberShrineUnlocked) tag[nameof(CosmicEmberShrineUnlocked)] = true;

        if (FoundVulcan) tag[nameof(FoundVulcan)] = true;
        if (DeimosReturn) tag[nameof(DeimosReturn)] = true;
    }

    public static void LoadData(TagCompound tag)
    {
        if (tag.ContainsKey("SubworldFlags"))
        {
            var subworldTags = tag.GetList<TagCompound>("SubworldData");
            foreach (var entry in subworldTags)
            {
                string subworldName = entry.GetString("ID");
                _subworldData[subworldName] = SubworldData.Load(entry);
            }
        }

        DownedCraterDemon = tag.ContainsKey(nameof(DownedCraterDemon));
        DownedMoonBeast = tag.ContainsKey(nameof(DownedMoonBeast));
        DownedDementoxin = tag.ContainsKey(nameof(DownedDementoxin));
        DownedLunalgamate = tag.ContainsKey(nameof(DownedLunalgamate));

        LuminiteShrineUnlocked = tag.ContainsKey(nameof(LuminiteShrineUnlocked));
        HeavenforgeShrineUnlocked = tag.ContainsKey(nameof(HeavenforgeShrineUnlocked));
        LunarRustShrineUnlocked = tag.ContainsKey(nameof(LunarRustShrineUnlocked));
        AstraShrineUnlocked = tag.ContainsKey(nameof(AstraShrineUnlocked));
        DarkCelestialShrineUnlocked = tag.ContainsKey(nameof(DarkCelestialShrineUnlocked));
        MercuryShrineUnlocked = tag.ContainsKey(nameof(MercuryShrineUnlocked));
        StarRoyaleShrineUnlocked = tag.ContainsKey(nameof(StarRoyaleShrineUnlocked));
        CryocoreShrineUnlocked = tag.ContainsKey(nameof(CryocoreShrineUnlocked));
        CosmicEmberShrineUnlocked = tag.ContainsKey(nameof(CosmicEmberShrineUnlocked));

        FoundVulcan = tag.ContainsKey(nameof(FoundVulcan));
        DeimosReturn = tag.ContainsKey(nameof(DeimosReturn));
    }

    public override void NetSend(BinaryWriter writer)
    {
        writer.Write(_subworldData.Count);
        foreach (var (subworldName, data) in _subworldData)
        {
            int index = SubworldSystem.GetIndex(subworldName);
            writer.Write(index);
            data.NetSend(writer);
        }

        writer.WriteFlags
        (
            DownedCraterDemon,
            DownedMoonBeast,
            DownedDementoxin,
            DownedLunalgamate,

            LuminiteShrineUnlocked,
            HeavenforgeShrineUnlocked,
            LunarRustShrineUnlocked,
            AstraShrineUnlocked
        );
        writer.WriteFlags
        (
            DarkCelestialShrineUnlocked,
            MercuryShrineUnlocked,
            StarRoyaleShrineUnlocked,
            CryocoreShrineUnlocked,
            CosmicEmberShrineUnlocked,

            FoundVulcan,
            DeimosReturn
        );
    }

    public override void NetReceive(BinaryReader reader)
    {
        int count = reader.ReadInt32();
        for (int i = 0; i < count; i++)
        {
            int index = reader.ReadInt32();
            string subworldName = MacrocosmSubworld.Subworlds[index].FullName;
            _subworldData[subworldName] = SubworldData.NetReceive(reader);
        }

        // I prefer using properties, okay?! - Feldy
        (
            DownedCraterDemon,
            DownedMoonBeast,
            DownedDementoxin,
            DownedLunalgamate,

            LuminiteShrineUnlocked,
            HeavenforgeShrineUnlocked,
            LunarRustShrineUnlocked,
            AstraShrineUnlocked
        )
        = reader.ReadBitsByte();
        (
            DarkCelestialShrineUnlocked,
            MercuryShrineUnlocked,
            StarRoyaleShrineUnlocked,
            CryocoreShrineUnlocked,
            CosmicEmberShrineUnlocked,

            FoundVulcan,
            DeimosReturn
        )
        = reader.ReadBitsByte();
    }
}
