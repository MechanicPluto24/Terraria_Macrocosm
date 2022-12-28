﻿using Macrocosm.Common.Utility;
using Macrocosm.Content.Projectiles.Friendly.Summon;
using Terraria;
using Terraria.ModLoader;

namespace Macrocosm.Content.Buffs.GoodBuffs
{
	public class ChandriumEmpowerment : ModBuff
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Chandrium Empowerment");
			Description.SetDefault("Your next hit with your Chandrium Whip will deal increased damage!");
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