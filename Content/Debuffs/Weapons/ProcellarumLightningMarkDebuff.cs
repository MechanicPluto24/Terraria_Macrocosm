using Terraria;
using Terraria.ModLoader;

namespace Macrocosm.Content.Debuffs.Weapons
{
    public class ProcellarumLightningMarkDebuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = true;
            Main.buffNoSave[Type] = true;
            Main.buffNoTimeDisplay[Type] = false;
            Main.pvpBuff[Type] = true;
        }

        public override void Update(NPC npc, ref int buffIndex)
        {
        }

        public override void Update(Player player, ref int buffIndex)
        {
        }
    }
}
