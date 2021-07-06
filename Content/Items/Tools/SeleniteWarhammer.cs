using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Tools
{
	public class SeleniteWarhammer : ModItem
	{
		public override void SetStaticDefaults()
		{
			
		}

		public override void SetDefaults()
		{
			item.damage = 70;
			item.melee = true;
			item.width = 44;
			item.height = 38;
			item.useTime = 5;
			item.useAnimation = 12;
			item.hammer = 125;
			item.useStyle = ItemUseStyleID.SwingThrow;
			item.knockBack = 6;
			item.value = 10000;
			item.rare = ItemRarityID.Green;
			item.UseSound = SoundID.Item1;
			item.autoReuse = true;
			item.tileBoost = 5;
		}

		public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(mod, "LuminiteCrystal", 1);
			recipe.AddIngredient(mod, "SeleniteBar", 12);
			recipe.AddTile(TileID.WorkBenches);
			recipe.SetResult(this);
			recipe.AddRecipe();
		}
	}
}