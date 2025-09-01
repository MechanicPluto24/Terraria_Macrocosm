using Macrocosm.Common.Bases.Buffs;
using Macrocosm.Common.Enums;
using Macrocosm.Common.Sets;
using Terraria;
using Terraria.ModLoader;

namespace Macrocosm.Content.Buffs.Radiation
{
    public class Fatigue : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = true;
            Main.buffNoSave[Type] = true;
            Main.buffNoTimeDisplay[Type] = false;

            BuffSets.RadiationBuffSeverity[Type] = RadiationSeverity.Mild;
            BuffSets.TypicalDuration[Type] = 60 * 20;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            // Reduced mana regen
            player.manaRegen = (int)(player.manaRegen * 0.6f);

            // No infinite flight
            player.empressBrooch = false;

            // Slow
            player.moveSpeed *= 0.75f;

            // Reduced flight time
            player.wingTimeMax = (int)(player.wingTimeMax * 0.75f);
        }
    }
}