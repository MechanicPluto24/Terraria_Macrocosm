using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Creative;

namespace Macrocosm.Content.Items.Placeable.Relics
{
	public class CraterDemonRelic : ModItem
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
			Item.maxStack = 9999;
			Item.rare = ItemRarityID.Master;
			Item.master = true; 
			Item.value = Item.buyPrice(0, 5);
		}
	}
}
