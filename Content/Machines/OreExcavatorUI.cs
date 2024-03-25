using Macrocosm.Common.Bases.Machines;
using Macrocosm.Common.Storage;
using Macrocosm.Common.Systems;
using Macrocosm.Common.UI.Themes;
using Macrocosm.Common.UI;
using Macrocosm.Common.Utils;
using Macrocosm.Content.Players;
using Macrocosm.Content.Rockets.UI;
using Macrocosm.Content.Rockets;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using ReLogic.Content;
using System.Collections.Generic;
using Terraria.GameContent.UI.Elements;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.UI;
using Terraria;
using Macrocosm.Content.Machines;
using Terraria.DataStructures;

namespace Macrocosm.Content.Machines
{
    internal class OreExcavatorUI : MachineUI
    {
        public OreExcavatorTE OreExcavator => MachineTE as OreExcavatorTE;

        private UIPanel inventoryPanel;
        private UIListScrollablePanel inventorySlots;

        private UIPanelIconButton quickStackButton;
        private UIPanelIconButton lootAllButton;
        private UIPanelIconButton depositAllButton;
        private UIPanelIconButton sortInventoryButton;
        private UIPanelIconButton restockInventoryButton;

        public OreExcavatorUI()
        {
        }

        public override void OnInitialize()
        {
            Width.Set(0, 1f);
            Height.Set(0, 1f);
            HAlign = 0.5f;
            VAlign = 0.5f;

            SetPadding(6f);

            inventoryPanel = CreateInventoryPanel();
            inventoryPanel.Activate();
            Append(inventoryPanel);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }


        private UIPanel CreateInventoryPanel()
        {
            inventoryPanel = new()
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

            inventorySlots = CreateInventorySlotsList();
            inventoryPanel.Append(inventorySlots);

            if (OreExcavator.Inventory is not null)
            {
                inventoryPanel.Append(new UIHorizontalSeparator()
                {
                    Top = new(0, 0.83f),
                    Width = new(0, 0.99f),
                    Left = new(0, 0.003f),
                    Color = UITheme.Current.SeparatorColor
                });

                bool interactible() => OreExcavator.Inventory is not null && OreExcavator.Inventory.CanInteract;

                lootAllButton = new
                (
                    ModContent.Request<Texture2D>("Macrocosm/Assets/Textures/UI/Inventory/LootAll", AssetRequestMode.ImmediateLoad),
                    ModContent.Request<Texture2D>("Macrocosm/Assets/Textures/UI/LargePanel", AssetRequestMode.ImmediateLoad),
                    ModContent.Request<Texture2D>("Macrocosm/Assets/Textures/UI/LargePanelBorder", AssetRequestMode.ImmediateLoad),
                    ModContent.Request<Texture2D>("Macrocosm/Assets/Textures/UI/LargePanelHoverBorder", AssetRequestMode.ImmediateLoad)
                )
                {
                    Top = new(0, 0.85f),
                    Left = new(0, 0.25f),
                    BackPanelColor = new Color(45, 62, 115),
                    HoverText = Language.GetText("LegacyInterface.29"),
                    CheckInteractible = interactible,
                    GrayscaleIconIfNotInteractible = true
                };
                lootAllButton.OnLeftClick += (_, _) => OreExcavator.Inventory.LootAll();
                inventoryPanel.Append(lootAllButton);

                depositAllButton = new
                (
                    ModContent.Request<Texture2D>("Macrocosm/Assets/Textures/UI/Inventory/DepositAll", AssetRequestMode.ImmediateLoad),
                    ModContent.Request<Texture2D>("Macrocosm/Assets/Textures/UI/LargePanel", AssetRequestMode.ImmediateLoad),
                    ModContent.Request<Texture2D>("Macrocosm/Assets/Textures/UI/LargePanelBorder", AssetRequestMode.ImmediateLoad),
                    ModContent.Request<Texture2D>("Macrocosm/Assets/Textures/UI/LargePanelHoverBorder", AssetRequestMode.ImmediateLoad)
                )
                {
                    Top = new(0, 0.85f),
                    Left = new(0, 0.35f),
                    BackPanelColor = new Color(45, 62, 115),
                    HoverText = Language.GetText("LegacyInterface.30"),
                    CheckInteractible = interactible,
                    GrayscaleIconIfNotInteractible = true
                };
                depositAllButton.OnLeftClick += (_, _) => OreExcavator.Inventory.DepositAll(ContainerTransferContext.FromUnknown(Main.LocalPlayer));
                inventoryPanel.Append(depositAllButton);

                quickStackButton = new
                (
                    ModContent.Request<Texture2D>("Macrocosm/Assets/Textures/UI/Inventory/QuickStack", AssetRequestMode.ImmediateLoad),
                    ModContent.Request<Texture2D>("Macrocosm/Assets/Textures/UI/LargePanel", AssetRequestMode.ImmediateLoad),
                    ModContent.Request<Texture2D>("Macrocosm/Assets/Textures/UI/LargePanelBorder", AssetRequestMode.ImmediateLoad),
                    ModContent.Request<Texture2D>("Macrocosm/Assets/Textures/UI/LargePanelHoverBorder", AssetRequestMode.ImmediateLoad)
                )
                {
                    Top = new(0, 0.85f),
                    Left = new(0, 0.45f),
                    BackPanelColor = new Color(45, 62, 115),
                    HoverText = Language.GetText("LegacyInterface.31"),
                    CheckInteractible = interactible,
                    GrayscaleIconIfNotInteractible = true
                };
                quickStackButton.OnLeftClick += (_, _) => OreExcavator.Inventory.QuickStack(ContainerTransferContext.FromUnknown(Main.LocalPlayer));
                inventoryPanel.Append(quickStackButton);

                restockInventoryButton = new
                (
                    ModContent.Request<Texture2D>("Macrocosm/Assets/Textures/UI/Inventory/Restock", AssetRequestMode.ImmediateLoad),
                    ModContent.Request<Texture2D>("Macrocosm/Assets/Textures/UI/LargePanel", AssetRequestMode.ImmediateLoad),
                    ModContent.Request<Texture2D>("Macrocosm/Assets/Textures/UI/LargePanelBorder", AssetRequestMode.ImmediateLoad),
                    ModContent.Request<Texture2D>("Macrocosm/Assets/Textures/UI/LargePanelHoverBorder", AssetRequestMode.ImmediateLoad)
                )
                {
                    Top = new(0, 0.85f),
                    Left = new(0, 0.55f),
                    BackPanelColor = new Color(45, 62, 115),
                    HoverText = Language.GetText("LegacyInterface.82"),
                    CheckInteractible = interactible,
                    GrayscaleIconIfNotInteractible = true
                };
                restockInventoryButton.OnLeftClick += (_, _) => OreExcavator.Inventory.Restock();
                inventoryPanel.Append(restockInventoryButton);

                sortInventoryButton = new
                (
                    ModContent.Request<Texture2D>("Macrocosm/Assets/Textures/UI/Inventory/SortContainer", AssetRequestMode.ImmediateLoad),
                    ModContent.Request<Texture2D>("Macrocosm/Assets/Textures/UI/LargePanel", AssetRequestMode.ImmediateLoad),
                    ModContent.Request<Texture2D>("Macrocosm/Assets/Textures/UI/LargePanelBorder", AssetRequestMode.ImmediateLoad),
                    ModContent.Request<Texture2D>("Macrocosm/Assets/Textures/UI/LargePanelHoverBorder", AssetRequestMode.ImmediateLoad)
                )
                {
                    Top = new(0, 0.85f),
                    Left = new(0, 0.65f),
                    BackPanelColor = new Color(45, 62, 115),
                    HoverText = Language.GetText("LegacyInterface.122"),
                    CheckInteractible = interactible,
                    GrayscaleIconIfNotInteractible = true
                };
                sortInventoryButton.OnLeftClick += (_, _) => OreExcavator.Inventory.Sort();
                inventoryPanel.Append(sortInventoryButton);
            }

            return inventoryPanel;
        }

        private UIListScrollablePanel CreateInventorySlotsList()
        {
            inventorySlots = new()
            {
                Width = new(0, 1f),
                Height = new(0, 0.9f),
                Top = new(0,0.05f),
                HAlign = 0.5f,
                BackgroundColor = Color.Transparent,
                BorderColor = Color.Transparent,
                ListPadding = 0f,
                ListOuterPadding = 0f,
                ScrollbarHeight = new(0f, 0.75f),
                ScrollbarHAlign = 1f,
                ScrollbarVAlign = 0.5f,
                ListWidthWithScrollbar = new(0, 1f),
                ListWidthWithoutScrollbar = new(0, 1f)
            };
            inventorySlots.SetPadding(0f);
            inventorySlots.Deactivate();

            if (OreExcavator is null || OreExcavator.Inventory is null)
                return inventorySlots;

            int count = OreExcavator.Inventory.Size;

            int iconsPerRow = 10;
            int rowsWithoutScrollbar = 5;
            float iconSize = 48f;
            float iconOffsetTop = 0f;
            float iconOffsetLeft;

            if (count <= iconsPerRow * rowsWithoutScrollbar)
                iconOffsetLeft = 14f;
            else
                iconOffsetLeft = 2f;

            UIElement itemSlotContainer = new()
            {
                Width = new(0f, 1f),
                Height = new(iconSize * (count / iconsPerRow + (count % iconsPerRow != 0 ? 1 : 0)), 0f),
            };

            inventorySlots.Add(itemSlotContainer);
            itemSlotContainer.SetPadding(0f);

            for (int i = 0; i < count; i++)
            {
                UICustomItemSlot uiItemSlot = OreExcavator.Inventory.CreateItemSlot(i, ItemSlot.Context.ChestItem);
                uiItemSlot.Left = new(i % iconsPerRow * iconSize + iconOffsetLeft, 0f);
                uiItemSlot.Top = new(i / iconsPerRow * iconSize + iconOffsetTop, 0f);
                uiItemSlot.SetPadding(0f);

                uiItemSlot.Activate();
                itemSlotContainer.Append(uiItemSlot);
            }

            inventorySlots.Activate();
            return inventorySlots;
        }
    }
}
