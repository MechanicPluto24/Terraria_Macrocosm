using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Weapons
{
	public class BanHammer : ModItem
	{
		public override void SetStaticDefaults()
		{
			Tooltip.SetDefault("BEGONE.");
		}

		public override void SetDefaults()
		{
			item.damage = 100000000;
			item.melee = true;
			item.width = 40;
			item.height = 40;
			item.useTime = 60;
			item.useAnimation = 60;
			item.useStyle = ItemUseStyleID.SwingThrow;
			item.knockBack = 1000;
			item.value = 10000;
			item.rare = ItemRarityID.Green;
			item.UseSound = SoundID.Item1;
			item.autoReuse = false;
		}

		public override void AddRecipes()
		{

		}
	}
}