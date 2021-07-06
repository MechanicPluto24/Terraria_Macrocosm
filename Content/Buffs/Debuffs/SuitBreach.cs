using Terraria;
using Terraria.ModLoader;

namespace Macrocosm.Content.Buffs.Debuffs
{
	public class SuitBreach : ModBuff
    {
        public override void SetDefaults()
        {
            DisplayName.SetDefault("Suit Breach");
            Description.SetDefault("Your blood is boiling from the lack of atmospheric pressure!");
            Main.debuff[Type] = true;
            Main.buffNoSave[Type] = true;
            Main.buffNoTimeDisplay[Type] = false;
            canBeCleared = false;
            Main.pvpBuff[Type] = true;
        }
        public override void Update(Player player, ref int buffIndex)
        {
            player.lifeRegen -= 20;
        }
    }
}