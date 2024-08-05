using Terraria;
using Terraria.ModLoader;

namespace Macrocosm.Content.Buffs.Medkit
{
    public class MedkitHigh : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.buffNoSave[Type] = true;
            Main.buffNoTimeDisplay[Type] = false;
        }

        public override void Update(Player player, ref int buffIndex)
        {
        }
    }
}