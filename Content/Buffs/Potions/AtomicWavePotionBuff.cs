using Macrocosm.Common.Players;
using Terraria;
using Terraria.ModLoader;

namespace Macrocosm.Content.Buffs.Potions
{
    public class AtomicWavePotionBuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.buffNoSave[Type] = false;
            Main.buffNoTimeDisplay[Type] = false;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.GetModPlayer<AtomicWavePotionPlayer>().AtomicWave = true;
        }
    }
}