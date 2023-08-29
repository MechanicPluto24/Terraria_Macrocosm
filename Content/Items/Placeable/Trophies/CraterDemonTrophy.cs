using Macrocosm.Content.Rarities;
using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Placeable.Trophies
{
	internal class CraterDemonTrophy : ModItem
	{
		public override void SetStaticDefaults()
		{
			CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
		}

		public override void SetDefaults() 
		{
			Item.DefaultToPlaceableTile(ModContent.TileType<Tiles.Trophies.CraterDemonTrophy>());

			Item.width = 32;
			Item.height = 32;
			Item.maxStack = Item.CommonMaxStack;
			Item.rare = ModContent.RarityType<MoonRarityT1>();
			Item.value = Item.buyPrice(0, 1);
		}
	}
}
