using Macrocosm.Common.UI;
using Macrocosm.Common.UI.Themes;
using Macrocosm.Common.Utils;
using Macrocosm.Content.Players;
using Macrocosm.Content.Rockets.Storage;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.UI.Elements;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.UI;

namespace Macrocosm.Content.Rockets.UI
{
    public class UICargoTab : UIPanel, ITabUIElement, IRocketUIDataConsumer
    {
        public Rocket Rocket { get; set; } = new();

        private int InventorySize => (Rocket is not null && Rocket.HasInventory) ? Rocket.Inventory.Size : 0;
        private int cacheSize = Rocket.DefaultInventorySize;

        private UIPanel inventoryPanel;
        private UIListScrollablePanel inventorySlots;
        private UIPanelIconButton requestAccessButton;
        private UIPanelIconButton quickStackButton;
        private UIPanelIconButton lootAllButton;
        private UIPanelIconButton depositAllButton;
        private UIPanelIconButton sortInventoryButton;
        private UIPanelIconButton restockInventoryButton;

        private UIListScrollablePanel crewPanel;
        private UIPanel fuelPanel;

        private Player commander = Main.LocalPlayer;
        private Player prevCommander = Main.LocalPlayer;
        private List<Player> crew = new();
        private List<Player> prevCrew = new();

        public UICargoTab()
        {
        }

        public void OnRocketChanged()
        {
            cacheSize = InventorySize;
            this.ReplaceChildWith(inventoryPanel, inventoryPanel = CreateInventoryPanel());
        }

        public void OnTabOpen()
        {
            cacheSize = InventorySize;
            this.ReplaceChildWith(inventoryPanel, inventoryPanel = CreateInventoryPanel());
        }

        public override void OnInitialize()
        {
            Width.Set(0, 1f);
            Height.Set(0, 1f);
            HAlign = 0.5f;
            VAlign = 0.5f;

            SetPadding(6f);

            BackgroundColor = UITheme.Current.TabStyle.BackgroundColor;
            BorderColor = UITheme.Current.TabStyle.BorderColor;

            inventoryPanel = CreateInventoryPanel();
            inventoryPanel.Activate();
            Append(inventoryPanel);

            crewPanel = CreateCrewPanel();
            Append(crewPanel);

            fuelPanel = new()
            {
                Width = new(0, 0.4f),
                Height = new(0, 1f),
                HAlign = 0f,
                BackgroundColor = UITheme.Current.PanelStyle.BackgroundColor,
                BorderColor = UITheme.Current.PanelStyle.BorderColor
            };
            fuelPanel.SetPadding(2f);
            fuelPanel.Activate();
            Append(fuelPanel);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            UpdateCrewPanel();
            UpdateInventory();
        }

        private void UpdateCrewPanel()
        {
            if (Main.netMode != NetmodeID.MultiplayerClient)
                return;

            crew.Clear();

            for (int i = 0; i < Main.maxPlayers; i++)
            {
                var player = Main.player[i];

                if (!player.active)
                    continue;

                var rocketPlayer = player.GetModPlayer<RocketPlayer>();

                if (rocketPlayer.InRocket && rocketPlayer.RocketID == Rocket.WhoAmI)
                {
                    if (rocketPlayer.IsCommander)
                        commander = player;
                    else
                        crew.Add(player);
                }
            }

            // FIXME: check why this doesn't get updated when a remote client leaves the rocket 
            if (!commander.Equals(prevCommander) || !crew.SequenceEqual(prevCrew))
            {
                crewPanel.Deactivate();
                crewPanel.ClearList();

                crewPanel.Add(new UIPlayerInfoElement(commander));
                crew.ForEach(player => crewPanel.Add(new UIPlayerInfoElement(player)));

                if (crew.Any())
                    crewPanel.OfType<UIPlayerInfoElement>().LastOrDefault().LastInList = true;

                prevCommander = commander;
                prevCrew = crew;

                Activate();
            }
        }

        private void UpdateInventory()
        {
            // Use H and J to increase/decrease inventory size, for testing
            if (RocketUISystem.DebugModeActive)
            {
                if (Main.LocalPlayer.controlQuickHeal && Rocket.Inventory.Size < Inventory.MaxInventorySize)
                    Rocket.Inventory.Size += 1;

                if (Main.LocalPlayer.controlQuickMana && Rocket.Inventory.Size > 1)
                    Rocket.Inventory.Size -= 1;
            }

            if (cacheSize != InventorySize)
            {
                cacheSize = InventorySize;
                this.ReplaceChildWith(inventoryPanel, inventoryPanel = CreateInventoryPanel());
            }

            if (Rocket is not null && Rocket.HasInventory && Main.netMode == NetmodeID.MultiplayerClient)
            {
                if (Rocket.Inventory.CanInteract)
                    requestAccessButton.SetIcon(ModContent.Request<Texture2D>("Macrocosm/Assets/Textures/UI/Inventory/InventoryOpen"));
                else
                    requestAccessButton.SetIcon(ModContent.Request<Texture2D>("Macrocosm/Assets/Textures/UI/Inventory/InventoryClosed"));
            }
        }

        private UIPanel CreateInventoryPanel()
        {
            inventoryPanel = new()
            {
                Width = new(0, 0.596f),
                Height = new(0, 0.535f),
                Left = new(0, 0.405f),
                Top = new(0, 0),
                HAlign = 0f,
                BackgroundColor = UITheme.Current.PanelStyle.BackgroundColor,
                BorderColor = UITheme.Current.PanelStyle.BorderColor,
            };
            inventoryPanel.SetPadding(0f);

            inventorySlots = CreateInventorySlotsList();
            inventoryPanel.Append(inventorySlots);

            if (Rocket.HasInventory)
            {
                inventoryPanel.Append(new UIHorizontalSeparator()
                {
                    Top = new(0, 0.83f),
                    Width = new(0, 0.99f),
                    Left = new(0, 0.003f),
                    Color = UITheme.Current.SeparatorColor
                });

                bool interactible() => Rocket.HasInventory && Rocket.Inventory.CanInteract;

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
                lootAllButton.OnLeftClick += (_, _) => Rocket.Inventory.LootAll();
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
                depositAllButton.OnLeftClick += (_, _) => Rocket.Inventory.DepositAll(ContainerTransferContext.FromUnknown(Main.LocalPlayer));
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
                quickStackButton.OnLeftClick += (_, _) => Rocket.Inventory.QuickStack(ContainerTransferContext.FromUnknown(Main.LocalPlayer));
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
                restockInventoryButton.OnLeftClick += (_, _) => Rocket.Inventory.Restock();
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
                sortInventoryButton.OnLeftClick += (_, _) => Rocket.Inventory.Sort();
                inventoryPanel.Append(sortInventoryButton);

                if (Main.netMode == NetmodeID.MultiplayerClient)
                {
                    requestAccessButton = new
                    (
                        ModContent.Request<Texture2D>("Macrocosm/Assets/Textures/UI/Inventory/SortContainer", AssetRequestMode.ImmediateLoad),
                        ModContent.Request<Texture2D>("Macrocosm/Assets/Textures/UI/LargePanel", AssetRequestMode.ImmediateLoad),
                        ModContent.Request<Texture2D>("Macrocosm/Assets/Textures/UI/LargePanelBorder", AssetRequestMode.ImmediateLoad),
                        ModContent.Request<Texture2D>("Macrocosm/Assets/Textures/UI/LargePanelHoverBorder", AssetRequestMode.ImmediateLoad)
                    )
                    {
                        Top = new(0, 0.85f),
                        Left = new(0, 0.195f),
                        BackPanelColor = new Color(45, 62, 115),
                        HoverText = Language.GetText("Mods.Macrocosm.UI.Rocket.Inventory.OpenInventory")
                    };
                    requestAccessButton.OnLeftClick += (_, _) => Rocket.Inventory.InteractingPlayer = Main.myPlayer;
                    requestAccessButton.CheckInteractible = () => Rocket.Inventory.InteractingPlayer != Main.myPlayer;
                    inventoryPanel.Append(requestAccessButton);

                    inventoryPanel.Append(new UIVerticalSeparator()
                    {
                        Top = new(0, 0.85f),
                        Left = new(0, 0.2981f),
                        Height = new(0, 0.13f),
                        Color = UITheme.Current.SeparatorColor,
                    });

                    lootAllButton.Left.Percent = 0.32f;
                    depositAllButton.Left.Percent = 0.42f;
                    quickStackButton.Left.Percent = 0.52f;
                    restockInventoryButton.Left.Percent = 0.62f;
                    sortInventoryButton.Left.Percent = 0.72f;
                }
            }

            return inventoryPanel;
        }

        private UIListScrollablePanel CreateInventorySlotsList()
        {
            inventorySlots = new(new LocalizedColorScaleText(Language.GetText("Mods.Macrocosm.UI.Rocket.Common.Inventory"), scale: 1.2f))
            {
                Width = new(0, 1f),
                Height = new(0, 0.9f),
                BackgroundColor = Color.Transparent,
                BorderColor = Color.Transparent,
                ListPadding = 0f,
                ListOuterPadding = 0f,
                ScrollbarHeight = new(0f, 0.75f),
                ScrollbarHAlign = 0.995f,
                ScrollbarVAlign = 0.9f,
                ListWidthWithScrollbar = new(0, 1f),
                ListWidthWithoutScrollbar = new(0, 1f),
                ShiftTitleIfHasScrollbar = false
            };
            inventorySlots.SetPadding(0f);
            inventorySlots.Deactivate();

            if (Rocket is null || !Rocket.HasInventory)
                return inventorySlots;

            int count = InventorySize;

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
                UICustomItemSlot uiItemSlot = Rocket.Inventory.CreateItemSlot(i, ItemSlot.Context.ChestItem);
                uiItemSlot.Left = new(i % iconsPerRow * iconSize + iconOffsetLeft, 0f);
                uiItemSlot.Top = new(i / iconsPerRow * iconSize + iconOffsetTop, 0f);
                uiItemSlot.SetPadding(0f);

                uiItemSlot.Activate();
                itemSlotContainer.Append(uiItemSlot);
            }

            inventorySlots.Activate();
            return inventorySlots;
        }

        private UIListScrollablePanel CreateCrewPanel()
        {
            crewPanel = new(new LocalizedColorScaleText(Language.GetText("Mods.Macrocosm.UI.Rocket.Common.Crew"), scale: 1.2f))
            {
                Top = new(0f, 0.54f),
                Left = new(0, 0.405f),
                Height = new(0, 0.46f),
                Width = new(0, 0.596f),
                BorderColor = UITheme.Current.PanelStyle.BorderColor,
                BackgroundColor = UITheme.Current.PanelStyle.BackgroundColor,
                ScrollbarHAlign = 1.015f,
                ListWidthWithScrollbar = new StyleDimension(0, 1f),
                ShiftTitleIfHasScrollbar = false,
                PaddingLeft = 12f,
                PaddingRight = 12f,
                ListOuterPadding = 12f,
                PaddingTop = 0f,
                PaddingBottom = 0f
            };

            if (Main.netMode == NetmodeID.SinglePlayer)
                crewPanel.Add(new UIPlayerInfoElement(Main.LocalPlayer));

            return crewPanel;
        }
    }
}
