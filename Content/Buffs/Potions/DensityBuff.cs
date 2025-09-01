using Macrocosm.Common.Bases.Buffs;
using Terraria;
using Terraria.ModLoader;

namespace Macrocosm.Content.Buffs.Potions
{
    public class DensityBuff : ModBuff
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