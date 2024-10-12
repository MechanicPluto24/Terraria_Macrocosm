using Macrocosm.Common.Systems.UI;
using System.IO;
using System.Runtime.CompilerServices;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

#pragma warning disable CA2211 // Non-constant fields should not be visible

namespace Macrocosm.Common.Systems
{
    public class WorldFlags : ModSystem
    {
        public static bool DownedCraterDemon = false;
        public static bool DownedMoonBeast = false;
        public static bool DownedDementoxin = false;
        public static bool DownedLunalgamate = false;

        public static bool FoundVulcan = false;
        public static bool DeimosReturn = false;

        /// <summary>
        /// Used to set the value of a flag. 
        /// <br/> Typically you only have to pass the flag, ommiting the optional arguments.
        /// </summary>
        /// <param name="flag"> The flag. </param>
        /// <param name="value"> The value to set, usually true. </param>
        /// <param name="flagName"> The flag name for special logic. Set automatically </param>
        public static void SetFlag(ref bool flag, bool value = true, [CallerArgumentExpression(nameof(flag))] string flagName = null)
        {
            bool previousValue = flag;
            flag = value;

            OnFlagChanged(flagName);

            if (previousValue != flag)
                OnFlagClearedForTheFirstTime(flagName);
        }

        private static void OnFlagChanged(string flagName)
        {
            // Reflect flags change in UI
            UISystem.Instance.ResetUI();
        }

        private static void OnFlagClearedForTheFirstTime(string flagName)
        {
        }

        public override void ClearWorld()
        {
            // Moon flags
            DownedCraterDemon = false;
            DownedMoonBeast = false;
            DownedDementoxin = false;
            DownedLunalgamate = false;

            // Global flags
            FoundVulcan = false;
            DeimosReturn = false;
        }

        public override void SaveWorldData(TagCompound tag) => SaveData(tag);
        public override void LoadWorldData(TagCompound tag) => LoadData(tag);

        public static void SaveData(TagCompound tag)
        {
            // Moon flags
            if (DownedCraterDemon) tag[nameof(DownedCraterDemon)] = true;
            if (DownedMoonBeast) tag[nameof(DownedMoonBeast)] = true;
            if (DownedDementoxin) tag[nameof(DownedDementoxin)] = true;
            if (DownedLunalgamate) tag[nameof(DownedLunalgamate)] = true;

            // Global flags
            if (FoundVulcan) tag[nameof(FoundVulcan)] = true;
            if (DeimosReturn) tag[nameof(DeimosReturn)] = true;
        }

        public static void LoadData(TagCompound tag)
        {
            // Moon flags 
            DownedCraterDemon = tag.ContainsKey(nameof(DownedCraterDemon));
            DownedMoonBeast = tag.ContainsKey(nameof(DownedMoonBeast));
            DownedDementoxin = tag.ContainsKey(nameof(DownedDementoxin));
            DownedLunalgamate = tag.ContainsKey(nameof(DownedLunalgamate));

            // Global flags
            FoundVulcan = tag.ContainsKey(nameof(FoundVulcan));
            DeimosReturn = tag.ContainsKey(nameof(DeimosReturn));
        }

        public override void NetSend(BinaryWriter writer)
        {
            var flags = new BitsByte();

            // Moon flags
            flags[0] = DownedCraterDemon;
            flags[1] = DownedMoonBeast;
            flags[2] = DownedDementoxin;
            flags[3] = DownedLunalgamate;

            // Global flags 
            flags[4] = FoundVulcan;
            flags[5] = DeimosReturn;
            writer.Write(flags);
        }

        public override void NetReceive(BinaryReader reader)
        {
            BitsByte flags = reader.ReadByte();

            // Moon flags
            DownedCraterDemon = flags[0];
            DownedMoonBeast = flags[1];
            DownedDementoxin = flags[2];
            DownedLunalgamate = flags[3];

            // Global flags
            FoundVulcan = flags[4];
            DeimosReturn = flags[5];
        }
    }
}
