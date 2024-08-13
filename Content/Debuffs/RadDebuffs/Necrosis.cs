using Terraria;
using Terraria.ModLoader;

namespace Macrocosm.Content.Debuffs.RadDebuffs
{
    public class Necrosis : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = true;
            Main.buffNoSave[Type] = true;
            Main.buffNoTimeDisplay[Type] = true;
  
        }
        public override void Update(Player player, ref int buffIndex)
        {
            player.lifeRegen = -10;
            player.moveSpeed*=0.3f;
            player.statDefense*=0.3f;
            player.GetDamage<GenericDamageClass>() *=0.3f;
            player.GetCritChance<GenericDamageClass>() *=0.3f;
        }
    }
}