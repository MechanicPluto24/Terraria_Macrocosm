using Macrocosm.Common.Players;
using Macrocosm.Common.Storage;
using Macrocosm.Common.UI;
using Macrocosm.Common.UI.Themes;
using Macrocosm.Common.Utils;
using Macrocosm.Content.Rockets;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.UI;

namespace Macrocosm.Common.UI.Rockets.Cargo
{
    public class UICargoTab : UIPanel, ITabUIElement, IRocketUIDataConsumer
    {
        public Rocket Rocket { get; set; } = new();

        private int InventorySize => Rocket is not null ? Rocket.Inventory.Size - Rocket.SpecialInventorySlot_Count : 0;
        private int cacheSize = Rocket.DefaultGeneralInventorySize;

        private UIPanel inventoryPanel;
        private UIPanelIconButton requestAccessButton;

        private UIScrollableListPanel crewPanel;

        private UIFuelPanel fuelPanel;

        private Player commander = Main.LocalPlayer;
        private Player prevCommander = Main.LocalPlayer;
        private List<Player> crew;
        private List<Player> prevCrew;

        public UICargoTab()
        {
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

            fuelPanel = new();
            Append(fuelPanel);

            inventoryPanel = CreateInventoryPanel();
            inventoryPanel.Activate();
            Append(inventoryPanel);

            crewPanel = CreateCrewPanel();
            Append(crewPanel);
        }

        public void OnRocketChanged()
        {
            RefreshInventory();
        }

        public void OnTabOpen()
        {
            Main.stackSplit = 600;
            RefreshInventory();
        }

        private void RefreshInventory()
        {
            cacheSize = InventorySize;
            this.ReplaceChildWith(inventoryPanel, inventoryPanel = CreateInventoryPanel());
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            UpdateCrewPanel();
            UpdateInventory();

            Inventory.ActiveInventory = Rocket.Inventory;
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

                    crew.Add(player);
                }
            }

            // TODO: check why this doesn't get updated when a remote client leaves the rocket 
            if (!commander.Equals(prevCommander) || !crew.SequenceEqual(prevCrew))
            {
                crewPanel.Deactivate();
                crewPanel.ClearList();

                crew.ForEach(player => crewPanel.Add(new UIPlayerInfoElement(player, large: false)));

                if (crew.Count > 0)
                    crewPanel.OfType<UIPlayerInfoElement>().LastOrDefault().LastInList = true;

                prevCommander = commander;
                prevCrew = crew;

                crewPanel.Activate();
            }
        }

        private void UpdateInventory()
        {
            if (cacheSize != InventorySize)
                RefreshInventory();

            if (Rocket is not null && Main.netMode == NetmodeID.MultiplayerClient)
            {
                if (Rocket.Inventory.CanInteract)
                    requestAccessButton.SetImage(ModContent.Request<Texture2D>("Macrocosm/Assets/Textures/UI/Inventory/InventoryOpen"));
                else
                    requestAccessButton.SetImage(ModContent.Request<Texture2D>("Macrocosm/Assets/Textures/UI/Inventory/InventoryClosed"));
            }
        }

        private UIPanel CreateInventoryPanel()
        {
            inventoryPanel = Rocket.Inventory.ProvideUIWithInteractionButtons
            (
                out var slots,
                out var lootAllButton,
                out var depositAllButton,
                out var quickStackButton,
                out var restockInventoryButton,
                out var sortInventoryButton,
                start: Rocket.SpecialInventorySlot_Count,
                iconsPerRow: 10,
                rowsWithoutScrollbar: 5
            );
            inventoryPanel.Width = new(0, 0.596f);
            inventoryPanel.Height = new(0, 0.535f);
            inventoryPanel.Left = new(0, 0.405f);
            inventoryPanel.Top = new(0, 0);
            inventoryPanel.HAlign = 0f;
            inventoryPanel.SetPadding(0f);
            inventoryPanel.PaddingLeft = 2f;

            inventoryPanel.Append(new UIHorizontalSeparator()
            {
                Top = new(0, 0.12f),
                Width = new(0, 0.995f),
                Left = new(0, 0),
                HAlign = 0f,
                Color = UITheme.Current.SeparatorColor
            });

            slots.SetTitle(new LocalizedColorScaleText(Language.GetText("Mods.Macrocosm.UI.Rocket.Common.Inventory"), scale: 1.2f));
            slots.Width = new(0, 1f);
            slots.Height = new(0, 0.9f);
            slots.Top = new(0, 0);
            slots.SetPadding(0f);

            if (Main.netMode == NetmodeID.MultiplayerClient)
            {
                requestAccessButton = new
                (
                    ModContent.Request<Texture2D>("Macrocosm/Assets/Textures/UI/Inventory/InventoryOpen", AssetRequestMode.ImmediateLoad),
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

            return inventoryPanel;
        }

        private UIScrollableListPanel CreateCrewPanel()
        {
            crew = new();
            prevCrew = new();

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
                crewPanel.Add(new UIPlayerInfoElement(Main.LocalPlayer, large: false));

            return crewPanel;
        }

    }
}
