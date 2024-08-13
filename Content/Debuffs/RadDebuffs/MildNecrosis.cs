using Terraria;
using Terraria.ModLoader;
using  Macrocosm.Content.Players;
namespace Macrocosm.Content.Debuffs.RadDebuffs
{
    public class MildNecrosis : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = true;
            Main.buffNoSave[Type] = true;
            Main.buffNoTimeDisplay[Type] = true;
        }
        public override void Update(Player player, ref int buffIndex)
        {
           player.GetDamage<GenericDamageClass>() *=1f*(1/(player.GetModPlayer<IrradiationPlayer>().IrradiationLevels+1f));
        }
    }
}
