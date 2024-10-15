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

        public static bool LuminiteShrineUnlocked = false;
        public static bool HeavenforgeShrineUnlocked = false;
        public static bool LunarRustShrineUnlocked = false;
        public static bool AstraShrineUnlocked = false;
        public static bool DarkCelestialShrineUnlocked = false;
        public static bool MercuryShrineUnlocked = false;
        public static bool StarRoyaleShrineUnlocked = false;
        public static bool CryocoreShrineUnlocked = false;
        public static bool CosmicEmberShrineUnlocked = false;

        public static bool FoundVulcan = false;
        public static bool DeimosReturn = false;

        /// <summary>
        /// Used to set the value of a flag. 
        /// <br/> Typically you only have to pass the flag, ommiting the optional arguments.
        /// </summary>
        /// <param name="flag"> The flag. </param>
        /// <param name="value"> The value to set, usually true. </param>
        /// <param name="flagName"> The flag name, for special logic. Set automatically </param>
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
            DownedCraterDemon = false;
            DownedMoonBeast = false;
            DownedDementoxin = false;
            DownedLunalgamate = false;
            LuminiteShrineUnlocked = false;
            FoundVulcan = false;
            DeimosReturn = false;

            HeavenforgeShrineUnlocked = false;
            LunarRustShrineUnlocked = false;
            AstraShrineUnlocked = false;
            DarkCelestialShrineUnlocked = false;
            MercuryShrineUnlocked = false;
            StarRoyaleShrineUnlocked = false;
            CryocoreShrineUnlocked = false;
            CosmicEmberShrineUnlocked = false;
        }

        public override void SaveWorldData(TagCompound tag) => SaveData(tag);
        public override void LoadWorldData(TagCompound tag) => LoadData(tag);

        public static void SaveData(TagCompound tag)
        {
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
            BitsByte bb = new
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

            BitsByte bb2 = new
            (
                DarkCelestialShrineUnlocked,
                MercuryShrineUnlocked,
                StarRoyaleShrineUnlocked,
                CryocoreShrineUnlocked,
                CosmicEmberShrineUnlocked,

                FoundVulcan,
                DeimosReturn
            );

            writer.Write(bb);
            writer.Write(bb2);
        }

        public override void NetReceive(BinaryReader reader)
        {
            BitsByte bb = reader.ReadByte();
            BitsByte bb2 = reader.ReadByte();

            DownedCraterDemon = bb[0];
            DownedMoonBeast = bb[1];
            DownedDementoxin = bb[2];
            DownedLunalgamate = bb[3];
            LuminiteShrineUnlocked = bb[4];
            HeavenforgeShrineUnlocked = bb[5];
            LunarRustShrineUnlocked = bb[6];
            AstraShrineUnlocked = bb[7];

            DarkCelestialShrineUnlocked = bb2[0];
            MercuryShrineUnlocked = bb2[1];
            StarRoyaleShrineUnlocked = bb2[2];
            CryocoreShrineUnlocked = bb2[3]; 
            CosmicEmberShrineUnlocked = bb2[4]; 
            FoundVulcan = bb2[5];
            DeimosReturn = bb2[6];
        }
    }
}
