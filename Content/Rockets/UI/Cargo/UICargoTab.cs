using Macrocosm.Common.Bases.Items;
using Macrocosm.Common.Storage;
using Macrocosm.Common.Systems.UI;
using Macrocosm.Common.UI;
using Macrocosm.Common.UI.Themes;
using Macrocosm.Common.Utils;
using Macrocosm.Content.Items.Materials.Tech;
using Macrocosm.Content.Players;
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

namespace Macrocosm.Content.Rockets.UI.Cargo
{
    public class UICargoTab : UIPanel, ITabUIElement, IRocketUIDataConsumer
    {
        public Rocket Rocket { get; set; } = new();

        private int InventorySize => Rocket is not null && Rocket.HasInventory ? Rocket.Inventory.Size - Rocket.SpecialInventorySlot_Count : 0;
        private int cacheSize = Rocket.DefaultGeneralInventorySize;

        private UIPanel inventoryPanel;
        private UIPanelIconButton requestAccessButton;

        private UIListScrollablePanel crewPanel;

        private UIPanel fuelPanel;
        private UIPanelIconButton dumpFuelButton;
        private UILiquidTank fuelTank;
        private Item ItemInFuelTankSlot => Rocket.Inventory[Rocket.SpecialInventorySlot_FuelTank];
        private UIInventorySlot fuelCanisterItemSlot;

        private Player commander = Main.LocalPlayer;
        private Player prevCommander = Main.LocalPlayer;
        private List<Player> crew;
        private List<Player> prevCrew;

        private bool fuelDumpAnimationActive;
        private float fuelDumpAnimationProgress;

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

            fuelPanel = CreateFuelPanel();
            Append(fuelPanel);

            inventoryPanel = CreateInventoryPanel();
            inventoryPanel.Activate();
            Append(inventoryPanel);

            crewPanel = CreateCrewPanel();
            Append(crewPanel);
        }

        public void OnRocketChanged()
        {
            cacheSize = InventorySize;
            this.ReplaceChildWith(inventoryPanel, inventoryPanel = CreateInventoryPanel());
            fuelPanel.ReplaceChildWith(fuelCanisterItemSlot, fuelCanisterItemSlot = CreateFuelCanisterItemSlot());
        }

        public void OnTabOpen()
        {
            cacheSize = InventorySize;
            this.ReplaceChildWith(inventoryPanel, inventoryPanel = CreateInventoryPanel());
            fuelPanel.ReplaceChildWith(fuelCanisterItemSlot, fuelCanisterItemSlot = CreateFuelCanisterItemSlot());
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            UpdateCrewPanel();
            UpdateInventory();
            UpdateFuelTank();

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
                    else
                        crew.Add(player);
                }
            }

            // TODO: check why this doesn't get updated when a remote client leaves the rocket 
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
            if (UISystem.DebugModeActive)
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
                    requestAccessButton.SetImage(ModContent.Request<Texture2D>("Macrocosm/Assets/Textures/UI/Inventory/InventoryOpen"));
                else
                    requestAccessButton.SetImage(ModContent.Request<Texture2D>("Macrocosm/Assets/Textures/UI/Inventory/InventoryClosed"));
            }
        }

        private void UpdateFuelTank(GameTime gameTime)
        {
           
        }

        private void UpdateFuelTank()
        {
            if (fuelDumpAnimationActive)
            {
                float rocketFuelPercent = Rocket.Fuel / Rocket.FuelCapacity;
                fuelTank.LiquidLevel = MathHelper.Lerp(fuelTank.LiquidLevel, rocketFuelPercent, Utility.QuadraticEaseInOut(fuelDumpAnimationProgress));
                fuelDumpAnimationProgress += 0.005f;

                if (fuelDumpAnimationProgress >= 1f)
                {
                    fuelDumpAnimationProgress = 0f;
                    fuelDumpAnimationActive = false;
                }
            }
            else
            {
                fuelTank.LiquidLevel = Rocket.Fuel / Rocket.FuelCapacity;
            }
        }

        private void DumpFuel()
        {
            if (fuelCanisterItemSlot.Item.ModItem is FuelCanister fuelCanister && !fuelCanister.Empty)
            {
                float availableFuel = fuelCanister.CurrentFuel * fuelCanister.Item.stack;
                float neededFuel = Rocket.FuelCapacity - Rocket.Fuel;
                float addedFuel = Math.Min(availableFuel, neededFuel);

                Rocket.Fuel += addedFuel;
                fuelCanister.CurrentFuel -= addedFuel / fuelCanister.Item.stack;

                Rocket.NetSync();
                Rocket.Inventory.SyncItem(Rocket.SpecialInventorySlot_FuelTank);
            }

            fuelDumpAnimationActive = true;
        }

        private bool CheckDumpFuelInteractible()
        {
            if (fuelDumpAnimationActive)
                return false;

            if (fuelCanisterItemSlot.Item.ModItem is FuelCanister fuelCanister)
            {
                float availableFuel = fuelCanister.CurrentFuel * fuelCanister.Item.stack;
                float neededFuel = Rocket.FuelCapacity - Rocket.Fuel;

                if (availableFuel <= 0)
                    return false;

                if(neededFuel <= 0f)
                    return false;

                // Disable dump if more than one canister is placed,
                // to avoid fuel overflow and item stack issues
                if (availableFuel > neededFuel && fuelCanister.Item.stack > 1)
                    return false;
            }

            if(fuelCanisterItemSlot.Item.type == ItemID.None)
                return false;

            return true;
        }

        private UIPanel CreateFuelPanel()
        {
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

            fuelTank = new(WaterStyleID.Lava)
            {
                Width = new(0, 0.42f),
                Height = new(0, 0.8f),
                HAlign = 0.2f,
                VAlign = 0.5f,
                WaveAmplitude = 2f,
                WaveFrequency = 5f
            };
            fuelPanel.Append(fuelTank);

            fuelCanisterItemSlot = CreateFuelCanisterItemSlot();
            fuelPanel.Append(fuelCanisterItemSlot);

            dumpFuelButton = new
            (
               //ModContent.Request<Texture2D>(ContentSamples.ItemsByType[ModContent.ItemType<FuelCanister>()].ModItem.Texture),
               Macrocosm.EmptyTex,
               ModContent.Request<Texture2D>(Macrocosm.TexturesPath + "UI/LargePanel", AssetRequestMode.ImmediateLoad),
               ModContent.Request<Texture2D>(Macrocosm.TexturesPath + "UI/LargePanelBorder", AssetRequestMode.ImmediateLoad),
               ModContent.Request<Texture2D>(Macrocosm.TexturesPath + "UI/LargePanelHoverBorder", AssetRequestMode.ImmediateLoad)
            )
            {
                Top = new(0, 0.465f),
                Left = new(0, 0.705f),
                CheckInteractible = CheckDumpFuelInteractible,
                GrayscaleIconIfNotInteractible = true,
                HoverText = Language.GetText("Mods.Macrocosm.UI.Rocket.Cargo.DumpFuel"),
                GetIconPosition = (dimensions) => dimensions.Position() + new Vector2(dimensions.Width * 0.25f, dimensions.Height * 0.5f)
            };
            dumpFuelButton.OnLeftClick += (_, _) => DumpFuel();
            fuelPanel.Append(dumpFuelButton);

            return fuelPanel;
        }

        private UIInventorySlot CreateFuelCanisterItemSlot()
        {
            fuelCanisterItemSlot = Rocket.Inventory.ProvideItemSlot(Rocket.SpecialInventorySlot_FuelTank);
            fuelCanisterItemSlot.Top = new(0, 0.54f);
            fuelCanisterItemSlot.Left = new(0, 0.70f);
            fuelCanisterItemSlot.AddReserved(
                (item) => item.ModItem is FuelCanister,
                Lang.GetItemName(ModContent.ItemType<FuelCanister>()),
                ModContent.Request<Texture2D>(ContentSamples.ItemsByType[ModContent.ItemType<FuelCanister>()].ModItem.Texture + "_Blueprint")
            );
            return fuelCanisterItemSlot;
        }

        private UIPanel CreateInventoryPanel()
        {
            if (Rocket.HasInventory)
            {
                inventoryPanel = Rocket.Inventory.ProvideUI
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
                    Width = new(0, 0.99f),
                    Top = new(0, 0.12f),
                    HAlign = 0.5f,
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
            }
            else
            {
                inventoryPanel = new();
            }

            return inventoryPanel;
        }

        private UIListScrollablePanel CreateCrewPanel()
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
                crewPanel.Add(new UIPlayerInfoElement(Main.LocalPlayer));

            return crewPanel;
        }
    }
}
