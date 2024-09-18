using Macrocosm.Common.Enums;
using Macrocosm.Common.Sets;
using Terraria;
using Terraria.ModLoader;

namespace Macrocosm.Content.Debuffs.Radiation
{
    public class Necrosis : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = true;
            Main.buffNoSave[Type] = true;
            Main.buffNoTimeDisplay[Type] = false;

            BuffSets.RadiationBuffSeverity[Type] = RadiationSeverity.Severe;
            BuffSets.TypicalDuration[Type] = 60 * 25;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            // DoT
            player.lifeRegen = -10;

            // Slow
            player.moveSpeed *= 0.3f;

            // Reduced defense
            player.statDefense *= 0.3f;

            // Reduced damage
            player.GetDamage<GenericDamageClass>() *= 0.3f;

            // Reduced crit chance
            player.GetCritChance<GenericDamageClass>() *= 0.3f;
        }
    }
}