using Macrocosm.Common.Bases.Buffs;
using Macrocosm.Common.Utils;
using System.Collections.Generic;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Macrocosm.Content.Buffs.Environment
{
    public class Depressurized : ComplexBuff
    {
        public static List<LocalizedText> DeathMessages { get; } = [];

        public override void Load()
        {
            for (int i = 0; i < Utility.FindAllLocalizationThatStartsWith("Mods.Macrocosm.DeathMessages.Depressurized").Length; i++)
                DeathMessages.Add(Language.GetOrRegister($"Mods.Macrocosm.DeathMessages.Depressurized.Message{i}"));
        }

        public override LocalizedText GetCustomDeathMessage(Player player) => DeathMessages.Count > 0 ? DeathMessages.GetRandom() : null;

        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = true;
            Main.buffNoSave[Type] = true;
            Main.buffNoTimeDisplay[Type] = false;
            Main.pvpBuff[Type] = true;
        }

        public override void UpdateBadLifeRegen(Player player)
        {
            player.lifeRegen -= (int)(0.2f * player.statLifeMax2);
        }
    }
}