
using Macrocosm.Content.Items.Materials;
using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Miscellaneous
{
	public class MoonGeode : ModItem
	{

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Geode");
			Tooltip.SetDefault("Right click to crack open!");
			CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
		}

		override public void SetDefaults()
		{
			Item.width = 32;
			Item.height = 32;
			Item.maxStack = 999;
			Item.value = Item.sellPrice(silver: 1);
			Item.rare = ItemRarityID.Blue;
		}

		public override bool CanRightClick() => true;

		public override void RightClick(Player player)
		{

			for (int i = 0; i < Main.rand.Next(1, 5); i++)
			{
				int itemType = Utils.SelectRandom(Main.rand, ModContent.ItemType<ArtemiteOre>(), ModContent.ItemType<ChandriumOre>(), ModContent.ItemType<SeleniteOre>(), ModContent.ItemType<DianiteOre>());
				player.QuickSpawnItem(player.GetSource_OpenItem(Type), itemType, Main.rand.Next(1, 4));
			}
		}
	}
}
