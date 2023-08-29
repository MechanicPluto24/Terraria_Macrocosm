using Macrocosm.Content.Rarities;
using Terraria;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Materials
{
	internal class DeliriumPlating : ModItem
	{
		public override void SetStaticDefaults()
		{
		}

		public override void SetDefaults()
		{
			Item.width = 20;
			Item.height = 20;
			Item.maxStack = Item.CommonMaxStack;
			Item.value = 100;
			Item.rare = ModContent.RarityType<MoonRarityT2>();
			Item.material = true;

		}
	}
}