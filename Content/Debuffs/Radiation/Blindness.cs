using Macrocosm.Common.Enums;
using Macrocosm.Common.Sets;
using Terraria;
using Terraria.ModLoader;

namespace Macrocosm.Content.Debuffs.Radiation
{
    public class Blindness : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = true;
            Main.buffNoSave[Type] = true;
            Main.buffNoTimeDisplay[Type] = false;

            BuffSets.RadiationBuffSeverity[Type] = RadiationSeverity.Moderate;
            BuffSets.TypicalDuration[Type] = 60 * 15;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            // No hp regen
            player.bleed = true;

            // Blacken the screen
            player.blackout = true;
            player.blind = true;
        }
    }
}