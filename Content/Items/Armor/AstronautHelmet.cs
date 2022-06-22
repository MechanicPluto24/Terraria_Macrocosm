using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Macrocosm.Content.Buffs.Debuffs;

namespace Macrocosm.Content.Items.Armor
{
	[AutoloadEquip(EquipType.Head)]
	public class AstronautHelmet : ModItem
	{
		public override void SetStaticDefaults() 
		{
            DisplayName.SetDefault("Astronaut Helmet");
		}
		public override void SetDefaults() 
		{
			Item.width = 18;
			Item.height = 18;
			Item.value = 10000;
			Item.rare = ItemRarityID.Green;
			Item.defense = 26;
		}

		public override bool IsArmorSet(Item head, Item body, Item legs)
		{
			return head.type == ModContent.ItemType<AstronautHelmet>() && body.type == ModContent.ItemType<AstronautSuit>() && legs.type == ModContent.ItemType<AstronautLeggings>();
		}

		public override void UpdateArmorSet(Player player) 
		{
			player.GetModPlayer<MacrocosmPlayer>().accMoonArmor = true;
			player.setBonus = "Pressurized spacesuit allows for safe exploration of other celestial bodies"
							+ "\nTier 1 extraterrestrial protection"
							+ "\nVastly extends underwater breathing time";
			player.buffImmune[ModContent.BuffType<SuitBreach>()] = true;
		}

		public override void AddRecipes() 
		{
			Recipe recipe = Mod.CreateRecipe(Type);
			recipe.AddIngredient(ItemID.DirtBlock, 10);
			recipe.AddTile(TileID.WorkBenches);
			recipe.Register();
		}
	}
}