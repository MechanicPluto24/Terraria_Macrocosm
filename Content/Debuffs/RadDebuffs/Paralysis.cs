using Terraria;
using Terraria.ModLoader;

namespace Macrocosm.Content.Debuffs.RadDebuffs
{
    public class Paralysis : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = true;
            Main.buffNoSave[Type] = true;
            Main.buffNoTimeDisplay[Type] = true;
 
        }
        public override void Update(Player player, ref int buffIndex)
        {
            player.moveSpeed*=0.4f;//Slow
            player.bleed = true;//Make all the tier two debuffs have this.
            player.empressBrooch = false;
            player.wingTimeMax*=(int)0.5;
        }
    }
}