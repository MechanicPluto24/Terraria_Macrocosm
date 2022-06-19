using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Macrocosm.Content.Buffs.Debuffs;
using Macrocosm.Content.Items.Materials;

namespace Macrocosm.Content.Items.Armor
{
	[AutoloadEquip(EquipType.Head)]
	public class SeleniteHelmet : ModItem
	{
		public override void SetStaticDefaults() 
		{
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
			return head.type == ModContent.ItemType<SeleniteHelmet>() && body.type == ModContent.ItemType<SeleniteBreastplate>() && legs.type == ModContent.ItemType<SeleniteLeggings>();
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
			recipe.AddIngredient(ModContent.ItemType<SeleniteBar>(), 8);
			recipe.AddTile(TileID.WorkBenches);
			recipe.Register();
		}
	}
}