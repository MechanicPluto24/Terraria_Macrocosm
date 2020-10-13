using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Macrocosm.Buffs.Debuffs;

namespace Macrocosm.Items.Armor
{
	[AutoloadEquip(EquipType.Head)]
	public class AstronautHelmet : ModItem
	{
		public override void SetStaticDefaults() 
		{
            DisplayName.SetDefault("Astronaut Visor");
			Tooltip.SetDefault("'Now they can't poke your eyes out!'");
		}
		public override void SetDefaults() 
		{
			item.width = 18;
			item.height = 18;
			item.value = 10000;
			item.rare = ItemRarityID.Green;
			item.defense = 26;
		}

		public override bool IsArmorSet(Item head, Item body, Item legs)
		{
			return head.type == ModContent.ItemType<AstronautHelmet>() && body.type == ModContent.ItemType<AstronautSuit>() && legs.type == ModContent.ItemType<AstronautLeggings>();
		}

		public override void UpdateArmorSet(Player player) 
		{
			player.GetModPlayer<MacrocosmPlayer>().accMoonArmor = true;
			player.setBonus = "Pressurized spacesuit allows for exploration of other celestial bodies"
							+ "\nVastly extends underwater breathing time";
			player.buffImmune[ModContent.BuffType<SpaceSuffocation>()] = true;
		}

		public override void AddRecipes() 
		{
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(ItemID.DirtBlock, 10);
			recipe.AddTile(TileID.WorkBenches);
			recipe.SetResult(this);
			recipe.AddRecipe();
		}
	}
}