using Macrocosm.Content.Projectiles.Friendly.Summon;
using Terraria;
using Terraria.ModLoader;

namespace Macrocosm.Content.Buffs.GoodBuffs.MinionBuffs
{
	public class ChandriumStaffMinionBuff : ModBuff
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Crescent Ghoul");
			Description.SetDefault("This Crescent Ghoul will fight for you!");

			Main.buffNoSave[Type] = true;
			Main.buffNoTimeDisplay[Type] = true;
		}

		public override void Update(Player player, ref int buffIndex)
		{
			if (player.ownedProjectileCounts[ModContent.ProjectileType<ChandriumStaffMinion>()] > 0)
			{
				player.buffTime[buffIndex] = 18000;
			}
			else
			{
				player.DelBuff(buffIndex);
				buffIndex--;
			}
		}
	}
}
