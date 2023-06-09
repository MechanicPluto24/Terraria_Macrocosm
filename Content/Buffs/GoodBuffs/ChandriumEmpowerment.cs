using Macrocosm.Common.Utils;
using Macrocosm.Content.Projectiles.Friendly.Summon;
using Terraria;
using Terraria.ModLoader;

namespace Macrocosm.Content.Buffs.GoodBuffs
{
	public class ChandriumEmpowerment : ModBuff
	{
		public override void SetStaticDefaults()
		{
		}

		public override void Update(Player player, ref int buffIndex)
		{
			if (player.buffTime[buffIndex] <= 1)
			{
				player.Macrocosm().ChandriumEmpowermentStacks = 0;
			}

		}
	}
}
