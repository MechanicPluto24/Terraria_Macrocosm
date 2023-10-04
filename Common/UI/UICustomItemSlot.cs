using Macrocosm.Common.Utils;
using Macrocosm.Content.Rockets;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.UI;
using Terraria.UI.Chat;

namespace Macrocosm.Common.UI
{
	public class UICustomItemSlot : UIElement
	{
		protected Item[] itemArray;
		protected int itemIndex;
		protected int itemSlotContext;
		protected float scale;

		protected Vector2 DrawOffset { get; set; } = new Vector2(52f, 52f) * -0.5f;

		public UICustomItemSlot(Item[] itemArray, int itemIndex, int itemSlotContext, float scale = default)
		{
			this.itemArray = itemArray;
			this.itemIndex = itemIndex;
			this.itemSlotContext = itemSlotContext;
			Width = new StyleDimension(48f, 0f);
			Height = new StyleDimension(48f, 0f);
		}

		protected virtual void HandleItemSlotLogic()
		{
			if (base.IsMouseHovering)
			{
				Item inv = itemArray[itemIndex];
				ItemSlot.OverrideHover(ref inv, itemSlotContext);
				ItemSlot.LeftClick(ref inv, itemSlotContext);
				ItemSlot.RightClick(ref inv, itemSlotContext);
				ItemSlot.MouseHover(ref inv, itemSlotContext);
				itemArray[itemIndex] = inv;
			}

			Main.LocalPlayer.mouseInterface = base.IsMouseHovering;
		}

		protected sealed override void DrawSelf(SpriteBatch spriteBatch)
		{
			scale = 0.85f;
			HandleItemSlotLogic();
			Item inv = itemArray[itemIndex];
			Vector2 position = GetDimensions().Center() + DrawOffset * scale;
			DrawItemSlot(spriteBatch, ref inv, position);
			DrawItem(spriteBatch, ref inv, position);
		}

		protected virtual void DrawItemSlot(SpriteBatch spriteBatch, ref Item item, Vector2 position)
		{
			Texture2D texture = TextureAssets.InventoryBack.Value;
			Color color = Main.inventoryBack;

			spriteBatch.Draw(texture, position, null, color, 0f, default, scale, SpriteEffects.None, 0f);
		}

		protected virtual void DrawItem(SpriteBatch spriteBatch, ref Item item, Vector2 position)
		{
			Color color = Color.White;
			Texture2D value = TextureAssets.InventoryBack.Value;

			Vector2 vector = value.Size() * scale;
			if (item.type > ItemID.None && item.stack > 0)
			{
				float _ = ItemSlot.DrawItemIcon(item, itemSlotContext, spriteBatch, position + vector / 2f, scale, 32f, color);

				DrawExtras(spriteBatch, ref item, position);

				if (item.stack > 1)
					ChatManager.DrawColorCodedStringWithShadow(spriteBatch, FontAssets.ItemStack.Value, item.stack.ToString(), position + new Vector2(10f, 26f) * scale, color, 0f, Vector2.Zero, new Vector2(scale), -1f, scale);
			}
		}

		private void DrawExtras(SpriteBatch spriteBatch, ref Item item, Vector2 position)
		{
			Color color = Color.White;

			if (ItemID.Sets.TrapSigned[item.type])
				spriteBatch.Draw(TextureAssets.Wire.Value, position + new Vector2(40f, 40f) * scale, new Rectangle(4, 58, 8, 8), color, 0f, new Vector2(4f), 1f, SpriteEffects.None, 0f);

			if (ItemID.Sets.DrawUnsafeIndicator[item.type])
			{
				Vector2 offset = new Vector2(-4f, -4f) * scale;
				Texture2D texture = TextureAssets.Extra[ExtrasID.UnsafeIndicator].Value;
				Rectangle rectangle = texture.Frame();
				spriteBatch.Draw(texture, position + offset + new Vector2(40f, 40f) * scale, rectangle, color, 0f, rectangle.Size() / 2f, 1f, SpriteEffects.None, 0f);
			}

			if (Utility.IsRubblemaker(item.type))
			{
				Texture2D texture = TextureAssets.Extra[ExtrasID.RubbleMakerIndicator].Value;
				Vector2 offset = new Vector2(2f, -6f) * scale;
				Rectangle rectangle = new();

				switch (item.type)
				{
					case ItemID.RubblemakerSmall:
							rectangle = texture.Frame(3, 1, 2);
							break;

					case ItemID.RubblemakerMedium:
							rectangle = texture.Frame(3, 1, 1);
							break;

					case ItemID.RubblemakerLarge:
							rectangle = texture.Frame(3);
							break;
				}

				spriteBatch.Draw(texture, position + offset + new Vector2(40f, 40f) * scale, rectangle, color, 0f, rectangle.Size() / 2f, 1f, SpriteEffects.None, 0f);
			}
		}
	}
}

