using Macrocosm.Common.Bases.Buffs;
using Terraria;
using Terraria.ModLoader;

namespace Macrocosm.Content.Buffs.Potions;

public class ManiaBuff : ModBuff
{
    public override void SetStaticDefaults()
    {
        Main.buffNoSave[Type] = false;
        Main.buffNoTimeDisplay[Type] = false;
    }

    public override void Update(Player player, ref int buffIndex)
    {
        player.GetAttackSpeed(DamageClass.Melee) += 0.15f;
    }
}