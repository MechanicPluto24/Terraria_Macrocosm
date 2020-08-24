// using Macrocosm.Tiles;
using Terraria;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;
using Terraria.ID;

namespace Macrocosm.Items.Armor
{
	[AutoloadEquip(EquipType.Head)]
	public class AstronautHelmet : ModItem
	{
		public override void SetStaticDefaults() {
		}

		public override void SetDefaults() {
			item.width = 18;
			item.height = 18;
			item.value = 10000;
			item.rare = ItemRarityID.Green;
			item.defense = 10;
		}

		public override bool IsArmorSet(Item head, Item body, Item legs)
		{
			return head.type == ItemType<AstronautHelmet>() && body.type == ItemType<AstronautSuit>() && legs.type == ItemType<AstronautLeggings>();
		}

		public override void UpdateArmorSet(Player player) {
			/* player.setBonus = "trollface.jpg";
			player.allDamage -= 0.2f;
			Here are the individual weapon class bonuses.
			player.meleeDamage -= 0.2f;
			player.thrownDamage -= 0.2f;
			player.rangedDamage -= 0.2f;
			player.magicDamage -= 0.2f;
			player.minionDamage -= 0.2f;
			*/
			player.setBonus = "Pressurized spacesuit allows for exploration of other celestial bodies"
							+ "\nVastly extends underwater breathing time";
			player.buffImmune[mod.BuffType("SpaceSuffocation")] = true;
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