using Macrocosm.Common.Utility;
using Macrocosm.Content.Buffs.Debuffs;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Vanity.BossMasks
{
	[AutoloadEquip(EquipType.Head)]
	public class CraterDemonMask : ModItem
	{
		public override void SetStaticDefaults()
		{
 		}

		public override void SetDefaults()
		{
			Item.width = 24;
			Item.height = 20;
			Item.value = 10000;
			Item.rare = ItemRarityID.Green;
			Item.defense = 26;
		}
	}
}