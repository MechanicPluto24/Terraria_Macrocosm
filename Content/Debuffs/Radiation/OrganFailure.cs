using Macrocosm.Common.Enums;
using Macrocosm.Common.Sets;
using Terraria;
using Terraria.ModLoader;

namespace Macrocosm.Content.Debuffs.Radiation
{
    public class OrganFailure : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = true;
            Main.buffNoSave[Type] = true;
            Main.buffNoTimeDisplay[Type] = false;

            BuffSets.RadiationBuffSeverity[Type] = RadiationSeverity.Severe;
            BuffSets.TypicalDuration[Type] = 60 * 2;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            // Heavy DoT
            player.lifeRegen = -70; 
        }
    }
}