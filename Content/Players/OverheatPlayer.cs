using Macrocosm.Content.Buffs.Debuffs;
using System.Collections.Generic;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace Macrocosm.Content.Players
{
	public class OverheatPlayer : ModPlayer
	{
		private HashSet<int> weaponsThatHaveOverheated = new();

		public void AddWeaponOverheat(int weaponItemType, int duration, bool sync = false)
		{
			Player.AddBuff(BuffType<WeaponOverheat>(), duration, quiet: !sync);
			weaponsThatHaveOverheated.Add(weaponItemType);
		}

		public void RemoveWeaponOverheats(bool clearBuff = false)
		{
			if (clearBuff)
				Player.ClearBuff(BuffType<WeaponOverheat>());

			weaponsThatHaveOverheated.Clear();
		}

		public bool WeaponHasOverheated(int weaponItemType)
		{
			return weaponsThatHaveOverheated.Contains(weaponItemType);
		}

		public override void PostUpdateMiscEffects()
		{

		}
	}
}
