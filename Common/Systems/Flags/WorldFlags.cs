using Macrocosm.Common.Subworlds;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace Macrocosm.Common.Systems.Flags
{
    public class WorldFlags : ModSystem
    {
        private static List<BitsByte> globalFlags = new();
        private static Dictionary<string, List<BitsByte>> localFlags = new();

        public static GlobalFlag DemonSun { get; set; } = false;

        public static LocalFlag SubworldUnlocked { get; set; } = true;

        public static LocalFlag SolarStorm { get; set; } = false;
        public static LocalFlag MoonMeteorStorm { get; set; } = false;
        public static LocalFlag XaocRift { get; set; } = false;

        public static DownedFlag DownedCraterDemon { get; set; } = false;
        public static DownedFlag DownedMoonBeast { get; set; } = false;
        public static DownedFlag DownedDementoxin { get; set; } = false;
        public static DownedFlag DownedLunalgamate { get; set; } = false;

        public static DownedFlag LuminiteShrineUnlocked { get; set; } = false;
        public static DownedFlag HeavenforgeShrineUnlocked { get; set; } = false;
        public static DownedFlag LunarRustShrineUnlocked { get; set; } = false;
        public static DownedFlag AstraShrineUnlocked { get; set; } = false;
        public static DownedFlag DarkCelestialShrineUnlocked { get; set; } = false;
        public static DownedFlag MercuryShrineUnlocked { get; set; } = false;
        public static DownedFlag StarRoyaleShrineUnlocked { get; set; } = false;
        public static DownedFlag CryocoreShrineUnlocked { get; set; } = false;
        public static DownedFlag CosmicEmberShrineUnlocked { get; set; } = false;
        public static bool AnyShimmerShrine => new List<bool>() { HeavenforgeShrineUnlocked, LunarRustShrineUnlocked, AstraShrineUnlocked, DarkCelestialShrineUnlocked, MercuryShrineUnlocked, StarRoyaleShrineUnlocked, CryocoreShrineUnlocked, CosmicEmberShrineUnlocked }.Any(b => b);

        public static DownedFlag FoundVulcan { get; set; } = false;
        public static DownedFlag DeimosReturn { get; set; } = false;

        /// <summary> Flags that are typically only set once (e.g. boss downed) </summary>
        public readonly struct DownedFlag
        {
            private readonly int index;
            public DownedFlag(int index) => this.index = index;

            public bool Value
            {
                get => GetGlobalFlag(index);
                set { if (value) SetGlobalFlag(index, true); }
            }

            public void Reset() => SetGlobalFlag(index, false);

            public static implicit operator bool(DownedFlag flag) => flag.Value;

            public static implicit operator DownedFlag(bool value) => new(0) { Value = value };
        }

        /// <summary> Flags that share a value cross-subworld </summary>
        public readonly struct GlobalFlag
        {
            private readonly int index;
            public GlobalFlag(int index) => this.index = index;

            public bool Value
            {
                get => GetGlobalFlag(index);
                set => SetGlobalFlag(index, value);
            }

            public static implicit operator bool(GlobalFlag flag) => flag.Value;

            public static implicit operator GlobalFlag(bool value) => new(0) { Value = value };
        }

        /// <summary> 
        /// <br/> Flags that are local to a subworld. 
        /// <br/> Directly accessing the value will reflect the value associated with the current subworld.
        /// <br/> Use <see cref="GetValue(string)"/> and <see cref="SetValue(string, bool)"/> for other subworlds.
        /// </summary>
        public readonly struct LocalFlag
        {
            private readonly int index;
            public LocalFlag(int index) => this.index = index;

            public bool Value
            {
                get
                {
                    string subworld = MacrocosmSubworld.CurrentID;
                    return GetLocalFlag(subworld, index);
                }
                set
                {
                    string subworld = MacrocosmSubworld.CurrentID;
                    SetLocalFlag(subworld, index, value);
                }
            }

            public bool GetValue(string subworld) => GetLocalFlag(subworld, index);
            public void SetValue(string subworld, bool value) => SetLocalFlag(subworld, index, value);


            public static implicit operator bool(LocalFlag flag) => flag.Value;

            public static implicit operator LocalFlag(bool value) => new(0) { Value = value };
        }

        public override void Load()
        {
            RegisterFlags();
        }

        private static void RegisterFlags()
        {
            int globalIndex = 0;
            int localIndex = 0;

            foreach (var field in typeof(WorldFlags).GetFields())
            {
                if (field.FieldType == typeof(GlobalFlag))
                {
                    field.SetValue(null, new GlobalFlag(globalIndex++));
                }
                else if (field.FieldType == typeof(DownedFlag))
                {
                    field.SetValue(null, new DownedFlag(globalIndex++));
                }
                else if (field.FieldType == typeof(LocalFlag))
                {
                    field.SetValue(null, new LocalFlag(localIndex++));
                }
            }

            foreach (var property in typeof(WorldFlags).GetProperties())
            {
                if (property.PropertyType == typeof(GlobalFlag))
                {
                    property.SetValue(null, new GlobalFlag(globalIndex++));
                }
                else if (property.PropertyType == typeof(DownedFlag))
                {
                    property.SetValue(null, new DownedFlag(globalIndex++));
                }
                else if (property.PropertyType == typeof(LocalFlag))
                {
                    property.SetValue(null, new LocalFlag(localIndex++));
                }
            }

            AllocateBits(globalIndex, ref globalFlags);
        }

        private static void AllocateBits(int count, ref List<BitsByte> flags)
        {
            int requiredBitsBytes = (count + 7) / 8;
            while (flags.Count < requiredBitsBytes)
                flags.Add(new BitsByte());
        }

        private static bool GetFlag(List<BitsByte> flags, int index)
        {
            AllocateBits(index + 1, ref flags);
            int arrayIndex = index / 8;
            int bitIndex = index % 8;
            return flags[arrayIndex][bitIndex];
        }

        private static void SetFlag(List<BitsByte> flags, int index, bool value)
        {
            AllocateBits(index + 1, ref flags);
            int arrayIndex = index / 8;
            int bitIndex = index % 8;
            var bb = flags[arrayIndex];
            bb[bitIndex] = value;
            flags[arrayIndex] = bb;
        }

        public static bool GetGlobalFlag(int index) => GetFlag(globalFlags, index);

        public static void SetGlobalFlag(int index, bool value) => SetFlag(globalFlags, index, value);

        public static bool GetLocalFlag(string subworld, int index)
        {
            if (!localFlags.TryGetValue(subworld, out var flags))
            {
                flags = new List<BitsByte>();
                localFlags[subworld] = flags;
            }

            AllocateBits(index + 1, ref flags);
            return GetFlag(flags, index);
        }

        public static void SetLocalFlag(string subworld, int index, bool value)
        {
            if (!localFlags.TryGetValue(subworld, out var flags))
            {
                flags = new List<BitsByte>();
                localFlags[subworld] = flags;
            }

            AllocateBits(index + 1, ref flags);
            SetFlag(flags, index, value);
        }

        public override void ClearWorld()
        {
            globalFlags = new();
            localFlags = new();
        }

        public override void SaveWorldData(TagCompound tag) => SaveData(tag);
        public override void LoadWorldData(TagCompound tag) => LoadData(tag);

        public static void SaveData(TagCompound tag)
        {
            tag["GlobalFlags"] = globalFlags.Select(bb => (byte)bb).ToList();

            var localData = new TagCompound();
            foreach (var (key, flags) in localFlags)
                localData[key] = flags.Select(bb => (byte)bb).ToList();

            tag["LocalFlags"] = localData;
        }

        public static void LoadData(TagCompound tag)
        {
            globalFlags = tag.Get<List<byte>>("GlobalFlags").Select(b => (BitsByte)b).ToList();

            if (tag.ContainsKey("LocalFlags"))
            {
                var localData = tag.GetCompound("LocalFlags");
                foreach (var kvp in localData)
                {
                    localFlags[kvp.Key] = localData.Get<List<byte>>(kvp.Key).Select(b => (BitsByte)b).ToList();
                }
            }
        }

        public override void NetSend(BinaryWriter writer)
        {
            writer.Write(globalFlags.Count);
            foreach (var flag in globalFlags)
                writer.Write((byte)flag);

            writer.Write(localFlags.Count);
            foreach (var (key, flags) in localFlags)
            {
                writer.Write(key);
                writer.Write(flags.Count);
                foreach (var flag in flags)
                    writer.Write((byte)flag);
            }
        }

        public override void NetReceive(BinaryReader reader)
        {
            globalFlags.Clear();
            int globalCount = reader.ReadInt32();
            for (int i = 0; i < globalCount; i++)
                globalFlags.Add((BitsByte)reader.ReadByte());

            localFlags.Clear();
            int localCount = reader.ReadInt32();
            for (int i = 0; i < localCount; i++)
            {
                string key = reader.ReadString();
                int flagCount = reader.ReadInt32();
                var flags = new List<BitsByte>(flagCount);
                for (int j = 0; j < flagCount; j++)
                    flags.Add((BitsByte)reader.ReadByte());

                localFlags[key] = flags;
            }
        }
    }
}
