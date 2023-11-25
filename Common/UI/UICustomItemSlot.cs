using Macrocosm.Common.UI.Themes;
using Macrocosm.Common.Utils;
using Macrocosm.Content.Rockets.Storage;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.GameContent.Achievements;
using Terraria.GameContent.UI.Chat;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;
using Terraria.UI.Chat;
using static Terraria.UI.ItemSlot;

namespace Macrocosm.Common.UI
{
    public class UICustomItemSlot : UIElement
    {
        protected Inventory inventory;
        protected int itemIndex;
        protected int itemSlotContext;
        protected float scale;

        private float glowDuration;
        private float glowTime;
        private float glowHue;

        protected bool shouldNetsync = false;

        private Asset<Texture2D> slotTexture;
        private Asset<Texture2D> slotBorderTexture;
        private Color slotColor;
        private Color slotBorderColor;
        private float notInteractibleMultiplier;

        private Asset<Texture2D> slotFavoritedTexture;
        private Color slotFavoritedColor;


        public bool CanTrash { get; set; } = false;
        public bool CanFavorite { get; set; } = false;
        protected Vector2 DrawOffset { get; set; } = new Vector2(52f, 52f) * -0.5f;

        public UICustomItemSlot(Inventory inventory, int itemIndex, int itemSlotContext, float scale = default)
        {
            this.inventory = inventory;
            this.itemIndex = itemIndex;
            this.itemSlotContext = itemSlotContext;
            Width = new StyleDimension(48f, 0f);
            Height = new StyleDimension(48f, 0f);

            // Temporary thing, until we add texture support to tha PanelStyle
            bool terrariaDefault = UITheme.Current.Name is "Terraria";

            SetAppearance
            (
                terrariaDefault ? TextureAssets.InventoryBack : ModContent.Request<Texture2D>("Macrocosm/Assets/Textures/UI/InventorySlot"),
                terrariaDefault ? Macrocosm.EmptyTexAsset : ModContent.Request<Texture2D>("Macrocosm/Assets/Textures/UI/InventorySlotBorder"),
                UITheme.Current.InventorySlotStyle.BackgroundColor,
                UITheme.Current.InventorySlotStyle.BorderColor,
                terrariaDefault ? 0.2f : 0.8f
            );

            SetFavoritedAppearance
            (
                terrariaDefault ? TextureAssets.InventoryBack : ModContent.Request<Texture2D>("Macrocosm/Assets/Textures/UI/InventorySlotFavorited"),
                terrariaDefault ? default : UITheme.Current.PanelStyle.BorderColor * 0.9f
            );
        }

        public void SetAppearance(Asset<Texture2D> slotTexture, Asset<Texture2D> slotBorderTexture, Color slotColor, Color slotBorderColor, float notInteractibleMultiplier = 0.8f)
        {
            this.slotTexture = slotTexture;
            this.slotBorderTexture = slotBorderTexture;
            this.slotColor = slotColor;
            this.slotBorderColor = slotBorderColor;
            this.notInteractibleMultiplier = notInteractibleMultiplier;
        }

        public void SetFavoritedAppearance(Asset<Texture2D> slotFavoritedTexture, Color slotFavoritedColor)
        {

            this.slotFavoritedTexture = slotFavoritedTexture;
            this.slotFavoritedColor = slotFavoritedColor;
        }

        public void SetGlow(float time, float hue)
        {
            glowDuration = time;
            glowTime = time;
            glowHue = hue;
        }

        public void ClearGlow() => glowTime = 0;


        protected virtual void HandleItemSlotLogic()
        {
            if (IsMouseHovering && inventory.CanInteract)
            {
                Item inv = inventory[itemIndex];
                HandleCursor(ref inv);
                HandleLeftClick(ref inv);
                HandleRightClick(ref inv);
                HandleHover(ref inv);
                inventory[itemIndex] = inv;
            }

            if (shouldNetsync)
            {
                inventory.SyncItem(itemIndex);
                shouldNetsync = false;
            }

            Main.LocalPlayer.mouseInterface = IsMouseHovering;
        }

        private void HandleCursor(ref Item item)
        {
            if (PlayerLoader.HoverSlot(Main.player[Main.myPlayer], inventory.Items, itemSlotContext, itemIndex))
                return;

            if (item.type <= ItemID.None)
                return;

            if (Options.DisableLeftShiftTrashCan && !ShiftForcedOn && CanTrash)
            {
                if (ControlInUse && !Options.DisableQuickTrash)
                {
                    if (!item.favorited)
                        Main.cursorOverride = CursorOverrideID.TrashCan;
                }
                else if (ShiftInUse)
                {
                    if (!item.favorited)
                        Main.cursorOverride = CursorOverrideID.ChestToInventory;
                }
            }
            else if (ShiftInUse)
            {
                if (Main.player[Main.myPlayer].ItemSpace(item).CanTakeItemToPersonalInventory)
                    Main.cursorOverride = CursorOverrideID.ChestToInventory;
            }

            if (Main.keyState.IsKeyDown(Main.FavoriteKey))
            {
                if (Main.drawingPlayerChat)
                    Main.cursorOverride = CursorOverrideID.Magnifiers;
                else if (CanFavorite)
                    Main.cursorOverride = CursorOverrideID.FavoriteStar;
            }
        }
        private void HandleLeftClick(ref Item item)
        {
            Player player = Main.player[Main.myPlayer];
            bool clicked = Main.mouseLeftRelease && Main.mouseLeft;

            if (!clicked)
                return;

            if (ShiftInUse && PlayerLoader.ShiftClickSlot(Main.player[Main.myPlayer], inventory.Items, itemSlotContext, itemIndex))
                return;

            if (Main.cursorOverride == CursorOverrideID.Magnifiers)
            {
                if (ChatManager.AddChatText(FontAssets.MouseText.Value, ItemTagHandler.GenerateTag(item), Vector2.One))
                    SoundEngine.PlaySound(SoundID.MenuTick);

                return;
            }

            if (Main.cursorOverride == CursorOverrideID.FavoriteStar)
            {
                item.favorited = !item.favorited;
                SoundEngine.PlaySound(SoundID.MenuTick);
                return;
            }

            if (Main.cursorOverride == CursorOverrideID.ChestToInventory)
            {
                item = Main.player[Main.myPlayer].GetItem(Main.myPlayer, item, GetItemSettings.InventoryEntityToPlayerInventorySettings);

                if (Main.netMode == NetmodeID.MultiplayerClient)
                    shouldNetsync = true;

                return;
            }

            if (!(Main.mouseItem.maxStack > 1 && item.type == Main.mouseItem.type && item.stack != item.maxStack && Main.mouseItem.stack != Main.mouseItem.maxStack))
                Terraria.Utils.Swap(ref item, ref Main.mouseItem);

            if (item.stack > 0)
                AnnounceTransfer(new ItemTransferInfo(item, Context.MouseItem, itemSlotContext, item.stack));
            else
                AnnounceTransfer(new ItemTransferInfo(Main.mouseItem, itemSlotContext, Context.MouseItem, Main.mouseItem.stack));

            if (item.stack > 0)
                AchievementsHelper.NotifyItemPickup(player, item);

            if (item.type == ItemID.None || item.stack < 1)
                item = new Item();

            if (Main.mouseItem.type == item.type)
            {
                if (item.stack != item.maxStack && Main.mouseItem.stack != Main.mouseItem.maxStack)
                {
                    if (ItemLoader.TryStackItems(item, Main.mouseItem, out int numTransfered))
                        AnnounceTransfer(new ItemTransferInfo(item, Context.MouseItem, itemSlotContext, numTransfered));
                }
            }

            if (Main.mouseItem.type == ItemID.None || Main.mouseItem.stack < 1)
                Main.mouseItem = new Item();

            if (Main.mouseItem.type > ItemID.None || item.type > ItemID.None)
            {
                Recipe.FindRecipes();
                SoundEngine.PlaySound(SoundID.Grab);
            }

            if (Main.netMode == NetmodeID.MultiplayerClient)
                shouldNetsync = true;
        }

        private void HandleRightClick(ref Item item)
        {
            Player player = Main.player[Main.myPlayer];
            item.newAndShiny = false;
            if (player.itemAnimation > 0)
                return;

            if (!Main.mouseRight)
                return;


            if (item.maxStack == 1)
            {
                if (Main.mouseRightRelease)
                    SwapEquip(inventory.Items, itemSlotContext, itemIndex);
            }

            if (Main.stackSplit > 1)
                return;

            if (item.maxStack <= 1 && item.stack <= 1)
                return;

            int fastStack = Main.superFastStack + 1;
            for (int i = 0; i < fastStack; i++)
            {
                if ((Main.mouseItem.type == item.type && ItemLoader.CanStack(Main.mouseItem, item) || Main.mouseItem.type == ItemID.None) && (Main.mouseItem.stack < Main.mouseItem.maxStack || Main.mouseItem.type == ItemID.None))
                {
                    if (Main.mouseItem.type == ItemID.None)
                    {
                        Main.mouseItem = ItemLoader.TransferWithLimit(item, 1);
                        AnnounceTransfer(new ItemTransferInfo(item, itemSlotContext, Context.MouseItem));
                    }
                    else
                    {
                        ItemLoader.StackItems(Main.mouseItem, item, out _, infiniteSource: false, 1);
                    }

                    if (item.stack <= 0)
                        item = new Item();

                    Recipe.FindRecipes();

                    if (Main.netMode == NetmodeID.MultiplayerClient)
                        shouldNetsync = true;

                    SoundEngine.PlaySound(SoundID.MenuTick);
                    RefreshStackSplitCooldown();
                }
            }
        }

        private void HandleHover(ref Item item)
        {
            if (item.type > ItemID.None && item.stack > 0)
            {
                Main.hoverItemName = item.Name;

                if (item.stack > 1)
                    Main.hoverItemName += " (" + item.stack + ")";

                Main.HoverItem = item.Clone();
                Main.HoverItem.tooltipContext = itemSlotContext;
            }
        }

        protected sealed override void DrawSelf(SpriteBatch spriteBatch)
        {
            if (itemIndex >= inventory.Size)
                return;

            float prevScale = Main.inventoryScale;
            Main.inventoryScale = 0.85f;

            HandleItemSlotLogic();
            Item inv = inventory[itemIndex];
            Vector2 position = GetDimensions().Center() + DrawOffset * Main.inventoryScale;
            DrawItemSlot(spriteBatch, ref inv, position);
            DrawItem(spriteBatch, ref inv, position);

            Main.inventoryScale = prevScale;

        }

        protected virtual void DrawItemSlot(SpriteBatch spriteBatch, ref Item item, Vector2 position)
        {
            Color slotColor = this.slotColor;
            Color slotBorderColor = this.slotBorderColor;

            slotColor = GetSlotGlow(item, slotColor, 0.85f);
            slotBorderColor = GetSlotGlow(item, slotBorderColor, 1.25f);

            if (!inventory.CanInteract)
            {
                var hsl = slotColor.ToHSL();
                slotColor = slotColor.WithSaturation(hsl.Y * 0.85f);
                hsl = slotBorderColor.ToHSL();
                slotBorderColor = slotColor.WithLuminance(hsl.Z * 0.85f);
            }

            spriteBatch.Draw(slotTexture.Value, position, null, slotColor, 0f, default, Main.inventoryScale, SpriteEffects.None, 0f);
            spriteBatch.Draw(slotBorderTexture.Value, position, null, slotBorderColor, 0f, default, Main.inventoryScale, SpriteEffects.None, 0f);

            if (item.favorited)
                spriteBatch.Draw(slotFavoritedTexture.Value, position, null, slotFavoritedColor, 0f, default, Main.inventoryScale, SpriteEffects.None, 0f);

        }

        protected virtual Color GetSlotGlow(Item item, Color baseColor, float multiplier = 1f)
        {
            Color resultColor = baseColor;

            if (glowTime > 0 && !item.favorited && !item.IsAir)
            {
                Color huedColor = Main.hslToRgb(glowHue, 1f, 0.3f);
                float progress = glowTime / glowDuration;
                resultColor = Color.Lerp(baseColor, huedColor, progress) * MathHelper.Lerp(multiplier, 1f, 1f - progress);

                glowTime--;
                if (glowTime <= 0)
                    glowHue = 0f;
            }

            return resultColor;
        }

        protected virtual void DrawItem(SpriteBatch spriteBatch, ref Item item, Vector2 position)
        {
            Color color = Color.White;

            Vector2 vector = slotTexture.Size() * Main.inventoryScale;
            if (item.type > ItemID.None && item.stack > 0)
            {
                float _ = ItemSlot.DrawItemIcon(item, itemSlotContext, spriteBatch, position + vector / 2f, Main.inventoryScale, 32f, color);
                DrawExtras(spriteBatch, ref item, position);

                if (item.stack > 1)
                    ChatManager.DrawColorCodedStringWithShadow(spriteBatch, FontAssets.ItemStack.Value, item.stack.ToString(), position + new Vector2(10f, 26f) * Main.inventoryScale, color, 0f, Vector2.Zero, new Vector2(Main.inventoryScale), -1f, Main.inventoryScale);
            }
        }

        private void DrawExtras(SpriteBatch spriteBatch, ref Item item, Vector2 position)
        {
            Color color = Color.White;

            if (ItemID.Sets.TrapSigned[item.type])
                spriteBatch.Draw(TextureAssets.Wire.Value, position + new Vector2(40f, 40f) * Main.inventoryScale, new Rectangle(4, 58, 8, 8), color, 0f, new Vector2(4f), 1f, SpriteEffects.None, 0f);

            if (ItemID.Sets.DrawUnsafeIndicator[item.type])
            {
                Vector2 offset = new Vector2(-4f, -4f) * Main.inventoryScale;
                Texture2D texture = TextureAssets.Extra[ExtrasID.UnsafeIndicator].Value;
                Rectangle rectangle = texture.Frame();
                spriteBatch.Draw(texture, position + offset + new Vector2(40f, 40f) * Main.inventoryScale, rectangle, color, 0f, rectangle.Size() / 2f, 1f, SpriteEffects.None, 0f);
            }

            if (Utility.IsRubblemaker(item.type))
            {
                Texture2D texture = TextureAssets.Extra[ExtrasID.RubbleMakerIndicator].Value;
                Vector2 offset = new Vector2(2f, -6f) * Main.inventoryScale;
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

                spriteBatch.Draw(texture, position + offset + new Vector2(40f, 40f) * Main.inventoryScale, rectangle, color, 0f, rectangle.Size() / 2f, 1f, SpriteEffects.None, 0f);
            }
        }
    }
}

