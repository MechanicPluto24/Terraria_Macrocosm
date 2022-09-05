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
			Item.maxStack = 999;
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
			itemLoot.Add(ItemDropRule.Common(ModContent.ItemType<DeliriumPlating>(), 1, 5, 15));

			itemLoot.Add(ItemDropRule.OneFromOptions(1,
				ModContent.ItemType<CalcicCane>(),
				ModContent.ItemType<Cruithne3753>()  
				/*, ModContent.ItemType<CrystalPortalThingy>() */
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
			// Draw the periodic glow effect behind the item when dropped in the world (hence PreDrawInWorld)
			Texture2D texture = TextureAssets.Item[Item.type].Value;
			Rectangle frame = texture.Frame();
 
			Vector2 frameOrigin = frame.Size() / 2f;
			Vector2 offset = new Vector2(Item.width / 2 - frameOrigin.X, Item.height - frame.Height);
			Vector2 drawPos = Item.position - Main.screenPosition + frameOrigin + offset;

			float time = Main.GlobalTimeWrappedHourly;
			float timer = Item.timeSinceItemSpawned / 240f + time * 0.04f;

			time %= 4f;
			time /= 2f;

			if (time >= 1f)
 				time = 2f - time;
 
			time = time * 0.5f + 0.5f;

			for (float i = 0f; i < 1f; i += 0.25f)
			{
				float radians = (i + timer) * MathHelper.TwoPi;
				spriteBatch.Draw(texture, drawPos + new Vector2(0f, 8f).RotatedBy(radians) * time, frame, new Color(31, 255, 106, 15), rotation, frameOrigin, scale, SpriteEffects.None, 0);
			}

			for (float i = 0f; i < 1f; i += 0.34f)
			{
				float radians = (i + timer) * MathHelper.TwoPi;
				spriteBatch.Draw(texture, drawPos + new Vector2(0f, 4f).RotatedBy(radians) * time, frame, new Color(158, 255, 157, 20), rotation, frameOrigin, scale, SpriteEffects.None, 0);
			}

			return true;
		}
	}
}
