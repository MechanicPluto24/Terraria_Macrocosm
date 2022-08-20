// using Macrocosm.Tiles;
using Macrocosm.Content.Items.Materials;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Armor
{
	[AutoloadEquip(EquipType.Body)]
	public class ChandriumBreastplate : ModItem
	{
		public override void SetStaticDefaults()
		{
			ArmorIDs.Body.Sets.IncludedCapeBack[EquipLoader.GetEquipSlot(Mod, Name, EquipType.Body)] = capeSlot;
			ArmorIDs.Body.Sets.IncludedCapeBackFemale[EquipLoader.GetEquipSlot(Mod, Name, EquipType.Body)] = capeSlot;
		}

		private int capeSlot;

		public override void Load()
		{
			capeSlot = EquipLoader.AddEquipTexture(Mod, "Macrocosm/Content/Items/Armor/ChandriumCape", EquipType.Back, name: "ChandriumCape");
		}

		public override void SetDefaults()
		{
			Item.width = 18;
			Item.height = 18;
			Item.value = 10000;
			Item.rare = ItemRarityID.Green;
			Item.defense = 40;
		}

		public override void AddRecipes()
		{
			Recipe recipe = Recipe.Create(Type);
			recipe.AddIngredient(ModContent.ItemType<ChandriumBar>(), 16);
			recipe.AddTile(TileID.WorkBenches);
			recipe.Register();
		}
	}
}