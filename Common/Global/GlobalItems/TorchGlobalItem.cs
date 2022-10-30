using SubworldLibrary;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Common.Global.GlobalItems
{
	public class TorchGlobalItem : GlobalItem
	{
		public static bool IsTorch(Item item) => ItemID.Sets.Torches[item.type];
		public static bool HasFlame(Item item) => flameItems.Contains(item.type);

		private static readonly List<int> flameItems = new();

		public override void Load()
		{
			for (int type = 0; type < ItemID.Count; type++) // vanilla only 
			{
				Item item = new(type);

				if (item.flame)
					flameItems.Add(type);
			}
		}

		public override void HoldItem(Item item, Player player)
		{
			if (HasFlame(item) && IsTorch(item)) // excluding NightGlow and Brand of the Inferno
				item.flame = !SubworldSystem.AnyActive<Macrocosm>();
		}
	}
}