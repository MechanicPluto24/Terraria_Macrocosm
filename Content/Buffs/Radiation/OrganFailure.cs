using Macrocosm.Common.Bases.Buffs;
using Macrocosm.Common.Enums;
using Macrocosm.Common.Sets;
using Terraria;
using Terraria.ModLoader;

namespace Macrocosm.Content.Buffs.Radiation
{
    public class OrganFailure : ComplexBuff
    {
        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = true;
            Main.buffNoSave[Type] = true;
            Main.buffNoTimeDisplay[Type] = false;

            BuffSets.RadiationBuffSeverity[Type] = RadiationSeverity.Severe;
            BuffSets.TypicalDuration[Type] = 60 * 2;
        }

        public override void UpdateBadLifeRegen(Player player)
        {
            // Heavy DoT
            player.lifeRegen -= 30;
        }

        public override void Update(Player player, ref int buffIndex)
        {
        }
    }
}