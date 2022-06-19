// using Macrocosm.Tiles;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Macrocosm.Content.Items.Materials;

namespace Macrocosm.Content.Items.Armor
{
	[AutoloadEquip(EquipType.Body)]
	public class SeleniteBreastplate : ModItem
	{
		public override void SetStaticDefaults() 
		{
		}

		public override void SetDefaults() {
			Item.width = 18;
			Item.height = 18;
			Item.value = 10000;
			Item.rare = ItemRarityID.Green;
			Item.defense = 40;
		}

		public override void AddRecipes() {
			Recipe recipe = Mod.CreateRecipe(Type);
			recipe.AddIngredient(ModContent.ItemType<SeleniteBar>(), 16);
			recipe.AddTile(TileID.WorkBenches);
			recipe.Register();
		}
	}
}