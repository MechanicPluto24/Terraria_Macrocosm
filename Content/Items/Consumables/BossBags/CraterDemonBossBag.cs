using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Creative;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;
using Terraria.GameContent.ItemDropRules;
using Macrocosm.Content.Items.Currency;
using Macrocosm.Content.Items.Weapons.Summon;
using Macrocosm.Content.Items.Weapons.Ranged;
using Macrocosm.Content.Items.Materials;
using Macrocosm.Common.Utility;

namespace Macrocosm.Content.Items.Consumables.BossBags
{
 	public class CraterDemonBossBag : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Treasure Bag (Crater Demon)");
			Tooltip.SetDefault("{$CommonItemTooltip.RightClickToOpen}"); // References a language key that says "Right Click To Open" in the language of the game

			ItemID.Sets.BossBag[Type] = true; // This set is one that every boss bag should have, it, for example, lets our boss bag drop dev armor..
 
			CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 3;
		}

		public override void SetDefaults()
		{
			Item.maxStack = 9999;
			Item.consumable = true;
			Item.width = 24;
			Item.height = 24;
			Item.rare = ItemRarityID.Purple;
			Item.expert = true; // This makes sure that "Expert" displays in the tooltip and the item name color changes
		}

		public override bool CanRightClick() => true;
 
		public override void ModifyItemLoot(ItemLoot itemLoot)
		{
			//itemLoot.Add(ItemDropRule.NotScalingWithLuck(ModContent.ItemType<CraterDemonMask>(), 7));
			itemLoot.Add(ItemDropRule.Common(ModContent.ItemType<MoonCoin>(), 1, 30, 60));
			itemLoot.Add(ItemDropRule.Common(ModContent.ItemType<DeliriumPlating>(), 1, 30, 90));

			itemLoot.Add(ItemDropRule.Common(ModContent.ItemType<BrokenHeroShield>()));

			itemLoot.Add(ItemDropRule.OneFromOptions(1,
				ModContent.ItemType<CalcicCane>(),
				ModContent.ItemType<Cruithne3753>()
				/*, ModContent.ItemType<JewelOfShowers>() */
				/*, ModContent.ItemType<ChampionBlade>() */
				));
		}

		// Below is code for the visuals
		public override Color? GetAlpha(Color lightColor)
			=> Color.Lerp(lightColor, Color.White, 0.4f);
 
		public override void PostUpdate()
		{
 			Lighting.AddLight(Item.Center, Color.White.ToVector3() * 0.4f);
		}

		public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
		{
			Color colorFront = new Color(31, 255, 106, 15);
			Color colorBack = new Color(158, 255, 157, 20);

			Item.DrawBossBagEffect(spriteBatch, colorFront, colorBack, rotation, scale);

			return true;
		}
	}
}
