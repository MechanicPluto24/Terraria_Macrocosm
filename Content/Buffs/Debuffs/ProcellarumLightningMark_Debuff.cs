using Terraria;
using Terraria.ModLoader;

namespace Macrocosm.Content.Buffs.Debuffs
{
    public class Procellarum_LightningMarkDebuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = true;
            Main.buffNoSave[Type] = true;
            Main.buffNoTimeDisplay[Type] = false;
            Main.pvpBuff[Type] = true;
        }
    }
}
