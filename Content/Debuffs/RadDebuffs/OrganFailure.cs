using Terraria;
using Terraria.ModLoader;

namespace Macrocosm.Content.Debuffs.RadDebuffs
{
    public class OrganFailure : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = true;
            Main.buffNoSave[Type] = true;
            Main.buffNoTimeDisplay[Type] = true;
         
        }
        public override void Update(Player player, ref int buffIndex)
        {
            player.lifeRegen = -70;
            player.empressBrooch = false;
        }
    }
}