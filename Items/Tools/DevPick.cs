using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Items.Tools
{
	public class DevPick : ModItem
	{
		public override void SetStaticDefaults()
		{
			Tooltip.SetDefault("Dev Pickaxe.");
		}

		public override void SetDefaults()
		{
			item.damage = 10000;
			item.melee = true;
			item.width = 40;
			item.height = 40;
			item.useTime = 1;
			item.useAnimation = 12;
			item.pick = 10000;
			item.useStyle = ItemUseStyleID.SwingThrow;
			item.knockBack = 6;
			item.value = 10000;
			item.rare = ItemRarityID.Green;
			item.UseSound = SoundID.Item1;
			item.autoReuse = true;
			item.tileBoost = 50;
		}
	}
}