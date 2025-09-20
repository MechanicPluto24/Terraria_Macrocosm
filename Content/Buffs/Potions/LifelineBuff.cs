using Macrocosm.Common.Bases.Buffs;
using Macrocosm.Common.Players;
using Terraria;
using Terraria.ModLoader;

namespace Macrocosm.Content.Buffs.Potions;

public class LifelineBuff : ModBuff
{
    public override void SetStaticDefaults()
    {
        Main.buffNoSave[Type] = false;
        Main.buffNoTimeDisplay[Type] = false;
    }

    public override void Update(Player player, ref int buffIndex)
    {
        player.GetModPlayer<LifelinePlayer>().Lifeline = true;
    }
}