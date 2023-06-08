using Macrocosm.Content.Rarities;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Materials
{
	public class CosmicDust : ModItem
	{
		//This shit is fucking ugly - OmegA
		//This is a github test btw, I bloody hope this works.
		public override void SetStaticDefaults()
		{

		}

		public override void SetDefaults()
		{
			Item.width = 20;
			Item.height = 20;
			Item.maxStack = 9999;
			Item.value = 100;
			Item.rare = ModContent.RarityType<MoonRarityT1>();
			Item.material = true;

			// Set other Item.X values here
		}

		public override void AddRecipes()
		{
			// Recipes here. See Basic Recipe Guide
		}
	}
}