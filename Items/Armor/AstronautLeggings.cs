// using Macrocosm.Tiles;
using Terraria;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;
using Terraria.ID;

namespace Macrocosm.Items.Armor
{
	[AutoloadEquip(EquipType.Legs)]
	public class AstronautLeggings : ModItem
	{
		public override void SetStaticDefaults() {
		}

		public override void SetDefaults() {
			item.width = 18;
			item.height = 18;
			item.value = 10000;
			item.rare = ItemRarityID.Green;
			item.defense = = 22;
		}

		public override void UpdateEquip(Player player) {
			player.moveSpeed += 0.05f;
		}

		public override void AddRecipes() {
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(ItemID.DirtBlock, 10);
			recipe.AddTile(TileID.WorkBenches);
			recipe.SetResult(this);
			recipe.AddRecipe();
		}
	}
}