using Macrocosm.Common.Players;
using Terraria;
using Terraria.ModLoader;
using Macrocosm.Common.Players;

namespace Macrocosm.Content.Buffs.Potions
{
    public class DensityPotionBuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.buffNoSave[Type] = false;
            Main.buffNoTimeDisplay[Type] = false;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.gravity *= 2f;
        }
    }
}