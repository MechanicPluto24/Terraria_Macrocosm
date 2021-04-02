// using Macrocosm.Tiles;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;
using Macrocosm.Items.Materials;

namespace Macrocosm.Items.Armor
{
	[AutoloadEquip(EquipType.Body)]
	public class SeleniteBreastplate : ModItem
	{
		public override void SetStaticDefaults() 
		{
		}

		public override void SetDefaults() {
			item.width = 18;
			item.height = 18;
			item.value = 10000;
			item.rare = ItemRarityID.Green;
			item.defense = 40;
		}

		public override void AddRecipes() {
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(ModContent.ItemType<SeleniteBar>(), 16);
			recipe.AddTile(TileID.WorkBenches);
			recipe.SetResult(this);
			recipe.AddRecipe();
		}
	}
}