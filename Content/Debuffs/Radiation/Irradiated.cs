using Terraria;
using Terraria.ModLoader;

namespace Macrocosm.Content.Debuffs.Radiation
{
    // Used only for letting the player that they are in an irradiated environment
    public class Irradiated : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = true;
            Main.buffNoSave[Type] = true;
            Main.buffNoTimeDisplay[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
        }
    }
}