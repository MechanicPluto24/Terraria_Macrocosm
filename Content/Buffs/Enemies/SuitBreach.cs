using Macrocosm.Common.Bases.Buffs;
using Terraria;
using Terraria.ModLoader;

namespace Macrocosm.Content.Buffs.Enemies
{
    public class SuitBreach : ComplexBuff
    {
        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = true;
            Main.buffNoSave[Type] = true;
            Main.buffNoTimeDisplay[Type] = false;
            Main.pvpBuff[Type] = true;
        }

        public override void UpdateBadLifeRegen(Player player)
        {
            player.lifeRegen -= (int)(0.05f * player.statLifeMax2);
        }

        public override void Update(Player player, ref int buffIndex)
        {
        }
    }
}