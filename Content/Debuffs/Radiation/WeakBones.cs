using Macrocosm.Common.Enums;
using Macrocosm.Common.Players;
using Macrocosm.Common.Sets;
using Terraria;
using Terraria.ModLoader;
namespace Macrocosm.Content.Debuffs.Radiation
{
    public class WeakBones : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = true;
            Main.buffNoSave[Type] = true;
            Main.buffNoTimeDisplay[Type] = false;

            BuffSets.RadiationBuffSeverity[Type] = RadiationSeverity.Mild;
            BuffSets.TypicalDuration[Type] = 60 * 4;
        }
        public override void Update(Player player, ref int buffIndex)
        {
            // Reduced defense
            player.statDefense *= 1f * (1 / (player.GetModPlayer<IrradiationPlayer>().IrradiationLevel + 1f));
        }
    }
}