using Terraria;
using Terraria.ModLoader;

namespace Macrocosm.Content.Debuffs.RadDebuffs
{
    public class Blindness : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = true;
            Main.buffNoSave[Type] = true;
            Main.buffNoTimeDisplay[Type] = true;
  
        }
        public override void Update(Player player, ref int buffIndex)
        {
            player.blackout=true;
            player.blind = true;
            player.bleed = true;//Make all the tier two debuffs have this.
        }
    }
}