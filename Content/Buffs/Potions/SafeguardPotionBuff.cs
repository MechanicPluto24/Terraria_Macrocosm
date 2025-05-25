using Macrocosm.Common.Players;
using Terraria;
using Terraria.ModLoader;
using Macrocosm.Common.Players;
using Terraria.ID;

namespace Macrocosm.Content.Buffs.Potions
{
    public class SafeguardPotionBuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.buffNoSave[Type] = false;
            Main.buffNoTimeDisplay[Type] = false;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.GetModPlayer<SafeguardPotionPlayer>().Safeguard = true;
        }
    }
    public class SafeguardPotionCooldown : ModBuff
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