
using Macrocosm.Content.Dusts;
using Macrocosm.Content.Items.Materials;
using Microsoft.Xna.Framework;
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
			Tooltip.SetDefault("Consumable/nRight click to smash open!");
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

		public override void Update(ref float gravity, ref float maxFallSpeed)
		{
			if (Main.rand.NextBool(8))
			{
				Dust dust = Dust.NewDustDirect(Item.position, Item.width, Item.height/2, ModContent.DustType<SmokeDust>(), newColor: new Color(16, 16, 16, 128));
				dust.velocity.X = Main.rand.NextFloat(-0.2f, 0.2f);
				dust.velocity.Y = -0.4f;
				dust.noGravity = true;
			}
		}
	}
}
