using Macrocosm.Common.Bases.Buffs;
using Macrocosm.Common.Enums;
using Macrocosm.Common.Sets;
using Terraria;
using Terraria.ModLoader;

namespace Macrocosm.Content.Buffs.Radiation
{
    public class Paralysis : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = true;
            Main.buffNoSave[Type] = true;
            Main.buffNoTimeDisplay[Type] = false;

            BuffSets.RadiationBuffSeverity[Type] = RadiationSeverity.Moderate;
            BuffSets.TypicalDuration[Type] = 60 * 2;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.webbed = true;
        }
    }
}