using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Tools
{
	public class DianiteWaraxe : ModItem
	{
		public override void SetStaticDefaults()
		{
			
		}

		public override void SetDefaults()
		{
			Item.damage = 70;
			Item.melee = true;
			Item.width = 42;
			Item.height = 42;
			Item.useTime = 5;
			Item.useAnimation = 12;
			Item.axe = 35;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.knockBack = 6;
			Item.value = 10000;
			Item.rare = ItemRarityID.Green;
			Item.UseSound = SoundID.Item1;
			Item.autoReuse = true;
			Item.tileBoost = 5;
		}

		public override void AddRecipes()
		{
			Recipe recipe = Mod.CreateRecipe(Type);
			recipe.AddIngredient(mod, "LuminiteCrystal", 1);
			recipe.AddIngredient(mod, "DianiteBar", 12);
			recipe.AddTile(TileID.WorkBenches);
			recipe.SetResult(this);
			recipe.Register();
		}
	}
}
