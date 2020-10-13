using Terraria;
using Terraria.ModLoader;

namespace Macrocosm.Buffs.Debuffs
{
    public class SpaceSuffocation : ModBuff
    {
        public override void SetDefaults()
        {
            DisplayName.SetDefault("Osphyxiation");
            Description.SetDefault("You are suffocating");
            Main.debuff[Type] = true;
            Main.buffNoSave[Type] = true;
            Main.buffNoTimeDisplay[Type] = false;
            canBeCleared = false;
        }
        public override void Update(Player player, ref int buffIndex)
        {
            player.lifeRegen -= 120; // Oh no, double trouble
        }
    }
}