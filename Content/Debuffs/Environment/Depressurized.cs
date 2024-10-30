using Macrocosm.Common.Utils;
using System.Collections.Generic;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Macrocosm.Content.Debuffs.Environment
{
    public class Depressurized : ModBuff
    {
        public static List<LocalizedText> DeathMessages { get; } = [];

        public override void Load()
        {
            for (int i = 0; i < Utility.FindAllThatStartWith("Mods.Macrocosm.DeathMessages.Depressurized").Length; i++)
                DeathMessages.Add(Language.GetOrRegister($"Mods.Macrocosm.DeathMessages.Depressurized.Message{i}"));
        }

        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = true;
            Main.buffNoSave[Type] = true;
            Main.buffNoTimeDisplay[Type] = false;
            Main.pvpBuff[Type] = true;
        }
        public override void Update(Player player, ref int buffIndex)
        {
            player.lifeRegen -= (int)(0.2f * player.statLifeMax2);
        }
    }
}