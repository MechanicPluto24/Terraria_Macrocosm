using Terraria;
using Terraria.ModLoader;

namespace Macrocosm.Content.Buffs.Debuffs
{
    public class Depressurized : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = true;
            Main.buffNoSave[Type] = true;
            Main.buffNoTimeDisplay[Type] = false;
            Main.pvpBuff[Type] = true;
        }
        public override void Update(Player player, ref int buffIndex)
        {
            player.lifeRegen -= (int)(0.1f * player.statLifeMax2);
        }
    }
}