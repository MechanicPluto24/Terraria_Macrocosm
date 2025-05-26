using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

namespace Macrocosm.Content.Buffs.Potions
{
    public class SafeguardCooldown : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = true;
            Main.buffNoSave[Type] = true;
            BuffID.Sets.NurseCannotRemoveDebuff[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
        }
    }
}