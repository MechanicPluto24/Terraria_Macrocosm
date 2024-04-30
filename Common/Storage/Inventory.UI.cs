
using Macrocosm.Common.UI;
using Macrocosm.Common.UI.Themes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.UI.Elements;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.UI;

namespace Macrocosm.Common.Storage
{
    public partial class Inventory
    {
        public UIPanel ProvideUI
        (
            int start = 0,
            int? end = null,
            int iconsPerRow = 10,
            int rowsWithoutScrollbar = 5,
            float iconSize = 48f,
            float buttonMenuTopPercent = 0.83f
        )
        {
            return ProvideUI(out _, out _, out _, out _, out _, out _, start, end, iconsPerRow, rowsWithoutScrollbar, iconSize, buttonMenuTopPercent);
        }

        public UIPanel ProvideUI
        (
            out UIListScrollablePanel inventorySlots,
            out UIPanelIconButton lootAllButton,
            out UIPanelIconButton depositAllButton,
            out UIPanelIconButton quickStackButton,
            out UIPanelIconButton restockInventoryButton,
            out UIPanelIconButton sortInventoryButton,
            int start = 0,
            int? end = null,
            int iconsPerRow = 1,
            int rowsWithoutScrollbar = 5,
            float iconSize = 48f,
            float buttonMenuTopPercent = 0.83f
        )
        {
            UIPanel inventoryPanel = new()
            {
                Width = new(0, 1f),
                Height = new(0, 1f),
                Left = new(0, 0),
                Top = new(0, 0),
                HAlign = 0f,
                BackgroundColor = UITheme.Current.PanelStyle.BackgroundColor,
                BorderColor = UITheme.Current.PanelStyle.BorderColor,
            };
            inventoryPanel.SetPadding(0f);

            inventorySlots = CreateInventorySlotsList(start, end, iconsPerRow, rowsWithoutScrollbar, iconSize);
            inventoryPanel.Append(inventorySlots);

            inventoryPanel.Append(new UIHorizontalSeparator()
            {
                Top = new(0, buttonMenuTopPercent),
                Width = new(0, 0.99f),
                Left = new(0, 0.003f),
                Color = UITheme.Current.SeparatorColor
            });

            lootAllButton = new
            (
                ModContent.Request<Texture2D>("Macrocosm/Assets/Textures/UI/Inventory/LootAll", AssetRequestMode.ImmediateLoad),
                ModContent.Request<Texture2D>("Macrocosm/Assets/Textures/UI/LargePanel", AssetRequestMode.ImmediateLoad),
                ModContent.Request<Texture2D>("Macrocosm/Assets/Textures/UI/LargePanelBorder", AssetRequestMode.ImmediateLoad),
                ModContent.Request<Texture2D>("Macrocosm/Assets/Textures/UI/LargePanelHoverBorder", AssetRequestMode.ImmediateLoad)
            )
            {
                Top = new(0, buttonMenuTopPercent + 0.02f),
                Left = new(0, 0.25f),
                BackPanelColor = new Color(45, 62, 115),
                HoverText = Language.GetText("LegacyInterface.29"),
                CheckInteractible = () => CanInteract,
                GrayscaleIconIfNotInteractible = true
            };
            lootAllButton.OnLeftClick += (_, _) => LootAll();
            inventoryPanel.Append(lootAllButton);

            depositAllButton = new
            (
                ModContent.Request<Texture2D>("Macrocosm/Assets/Textures/UI/Inventory/DepositAll", AssetRequestMode.ImmediateLoad),
                ModContent.Request<Texture2D>("Macrocosm/Assets/Textures/UI/LargePanel", AssetRequestMode.ImmediateLoad),
                ModContent.Request<Texture2D>("Macrocosm/Assets/Textures/UI/LargePanelBorder", AssetRequestMode.ImmediateLoad),
                ModContent.Request<Texture2D>("Macrocosm/Assets/Textures/UI/LargePanelHoverBorder", AssetRequestMode.ImmediateLoad)
            )
            {
                Top = new(0, buttonMenuTopPercent + 0.02f),
                Left = new(0, 0.35f),
                BackPanelColor = new Color(45, 62, 115),
                HoverText = Language.GetText("LegacyInterface.30"),
                CheckInteractible = () => CanInteract,
                GrayscaleIconIfNotInteractible = true
            };
            depositAllButton.OnLeftClick += (_, _) => DepositAll(ContainerTransferContext.FromUnknown(Main.LocalPlayer));
            inventoryPanel.Append(depositAllButton);

            quickStackButton = new
            (
                ModContent.Request<Texture2D>("Macrocosm/Assets/Textures/UI/Inventory/QuickStack", AssetRequestMode.ImmediateLoad),
                ModContent.Request<Texture2D>("Macrocosm/Assets/Textures/UI/LargePanel", AssetRequestMode.ImmediateLoad),
                ModContent.Request<Texture2D>("Macrocosm/Assets/Textures/UI/LargePanelBorder", AssetRequestMode.ImmediateLoad),
                ModContent.Request<Texture2D>("Macrocosm/Assets/Textures/UI/LargePanelHoverBorder", AssetRequestMode.ImmediateLoad)
            )
            {
                Top = new(0, buttonMenuTopPercent + 0.02f),
                Left = new(0, 0.45f),
                BackPanelColor = new Color(45, 62, 115),
                HoverText = Language.GetText("LegacyInterface.31"),
                CheckInteractible = () => CanInteract,
                GrayscaleIconIfNotInteractible = true
            };
            quickStackButton.OnLeftClick += (_, _) => QuickStack(ContainerTransferContext.FromUnknown(Main.LocalPlayer));
            inventoryPanel.Append(quickStackButton);

            restockInventoryButton = new
            (
                ModContent.Request<Texture2D>("Macrocosm/Assets/Textures/UI/Inventory/Restock", AssetRequestMode.ImmediateLoad),
                ModContent.Request<Texture2D>("Macrocosm/Assets/Textures/UI/LargePanel", AssetRequestMode.ImmediateLoad),
                ModContent.Request<Texture2D>("Macrocosm/Assets/Textures/UI/LargePanelBorder", AssetRequestMode.ImmediateLoad),
                ModContent.Request<Texture2D>("Macrocosm/Assets/Textures/UI/LargePanelHoverBorder", AssetRequestMode.ImmediateLoad)
            )
            {
                Top = new(0, buttonMenuTopPercent + 0.02f),
                Left = new(0, 0.55f),
                BackPanelColor = new Color(45, 62, 115),
                HoverText = Language.GetText("LegacyInterface.82"),
                CheckInteractible = () => CanInteract,
                GrayscaleIconIfNotInteractible = true
            };
            restockInventoryButton.OnLeftClick += (_, _) => Restock();
            inventoryPanel.Append(restockInventoryButton);

            sortInventoryButton = new
            (
                ModContent.Request<Texture2D>("Macrocosm/Assets/Textures/UI/Inventory/SortContainer", AssetRequestMode.ImmediateLoad),
                ModContent.Request<Texture2D>("Macrocosm/Assets/Textures/UI/LargePanel", AssetRequestMode.ImmediateLoad),
                ModContent.Request<Texture2D>("Macrocosm/Assets/Textures/UI/LargePanelBorder", AssetRequestMode.ImmediateLoad),
                ModContent.Request<Texture2D>("Macrocosm/Assets/Textures/UI/LargePanelHoverBorder", AssetRequestMode.ImmediateLoad)
            )
            {
                Top = new(0, buttonMenuTopPercent + 0.02f),
                Left = new(0, 0.65f),
                BackPanelColor = new Color(45, 62, 115),
                HoverText = Language.GetText("LegacyInterface.122"),
                CheckInteractible = () => CanInteract,
                GrayscaleIconIfNotInteractible = true
            };
            sortInventoryButton.OnLeftClick += (_, _) => Sort();
            inventoryPanel.Append(sortInventoryButton);

            return inventoryPanel;
        }

        private UIListScrollablePanel CreateInventorySlotsList(int start = 0, int? end = null, int iconsPerRow = 10, int rowsWithoutScrollbar = 5, float iconSize = 48f)
        {
            UIListScrollablePanel inventorySlots = new()
            {
                Width = new(0, 1f),
                Height = new(0, 0.7f),
                Top = new(0, 0.05f),
                HAlign = 0.5f,
                BackgroundColor = Color.Transparent,
                BorderColor = Color.Transparent,
                ListPadding = 0f,
                ListOuterPadding = 0f,
                ScrollbarHeight = new(0f, 0.75f),
                ScrollbarHAlign = 1f,
                ScrollbarVAlign = 0.5f,
                ListWidthWithScrollbar = new(0, 1f),
                ListWidthWithoutScrollbar = new(0, 1f),
                ShiftTitleIfHasScrollbar = false
            };
            inventorySlots.SetPadding(0f);

            float iconOffsetTop = 0f;
            float iconOffsetLeft;

            int slotLastIndex = end ?? Size;
            int slotCount = slotLastIndex - start;

            if (slotCount <= iconsPerRow * rowsWithoutScrollbar)
                iconOffsetLeft = 12f;
            else
                iconOffsetLeft = 2f;

            UIElement itemSlotContainer = new()
            {
                Width = new(0f, 1f),
                Height = new(iconSize * (slotCount / iconsPerRow + (slotCount % iconsPerRow != 0 ? 1 : 0)), 0f),
            };

            inventorySlots.Add(itemSlotContainer);
            itemSlotContainer.SetPadding(0f);

            for (int i = start; i < slotLastIndex; i++)
            {
                UIInventorySlot uiItemSlot = ProvideItemSlot(i, ItemSlot.Context.ChestItem);

                int slotIndex = i - start;
                uiItemSlot.Left = new(slotIndex % iconsPerRow * iconSize + iconOffsetLeft, 0f);
                uiItemSlot.Top = new(slotIndex / iconsPerRow * iconSize + iconOffsetTop, 0f);
                uiItemSlot.SetPadding(0f);

                itemSlotContainer.Append(uiItemSlot);
            }

            return inventorySlots;
        }

        public UIInventorySlot ProvideItemSlot(int index, int itemSlotContext = ItemSlot.Context.ChestItem, float scale = default)
        {
            UIInventorySlot slot = new(this, index, itemSlotContext, scale);
            uiItemSlots[index] = slot;
            return slot;
        }
    }
}
