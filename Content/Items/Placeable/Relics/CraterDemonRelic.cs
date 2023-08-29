using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Placeable.Relics
{
	internal class CraterDemonRelic : ModItem
	{
		public override void SetStaticDefaults()
		{
			CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
		}

		public override void SetDefaults()
		{
			Item.DefaultToPlaceableTile(ModContent.TileType<Tiles.Relics.CraterDemonRelic>(), 0);

			Item.width = 30;
			Item.height = 40;
			Item.maxStack = Item.CommonMaxStack;
			Item.rare = ItemRarityID.Master;
			Item.master = true; 
			Item.value = Item.buyPrice(0, 5);
		}
	}
}
