using Macrocosm.Common.UI;
using Macrocosm.Common.Utils;
using Macrocosm.Content.Players;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
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

            BackgroundColor = new Color(13, 23, 59, 127);
            BorderColor = new Color(15, 15, 15, 255);

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
                BackgroundColor = new Color(53, 72, 135),
                BorderColor = new Color(89, 116, 213, 255)
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
			// Just for testing
			if (Main.LocalPlayer.controlQuickHeal)
 				Rocket.Inventory.Size += 1;
 
			if (Main.LocalPlayer.controlQuickMana)
 				Rocket.Inventory.Size -= 1;
 
			if (cacheSize != InventorySize)
			{
				cacheSize = InventorySize;
				this.ReplaceChildWith(inventoryPanel, inventoryPanel = CreateInventoryPanel());
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
                BackgroundColor = new Color(53, 72, 135),
                BorderColor = new Color(89, 116, 213, 255),
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
					Left = new(0,0.003f),
					Color = new Color(89, 116, 213, 255)
				});

				lootAllButton = new
				(
					Macrocosm.EmptyTexAsset,
					ModContent.Request<Texture2D>("Macrocosm/Assets/Textures/UI/LargePanel", AssetRequestMode.ImmediateLoad),
					ModContent.Request<Texture2D>("Macrocosm/Assets/Textures/UI/LargePanelBorder", AssetRequestMode.ImmediateLoad),
					ModContent.Request<Texture2D>("Macrocosm/Assets/Textures/UI/LargePanelHoverBorder", AssetRequestMode.ImmediateLoad)
				)
				{
					Top = new(0, 0.85f),
					Left = new(0, 0.25f),
					BackPanelColor = new Color(106, 138, 255),
					HoverText = Lang.inter[29]
				};
				lootAllButton.OnLeftClick += (_, _) => Rocket.Inventory.LootAll();
				inventoryPanel.Append(lootAllButton);

				depositAllButton = new
				(
					Macrocosm.EmptyTexAsset,
					ModContent.Request<Texture2D>("Macrocosm/Assets/Textures/UI/LargePanel", AssetRequestMode.ImmediateLoad),
					ModContent.Request<Texture2D>("Macrocosm/Assets/Textures/UI/LargePanelBorder", AssetRequestMode.ImmediateLoad),
					ModContent.Request<Texture2D>("Macrocosm/Assets/Textures/UI/LargePanelHoverBorder", AssetRequestMode.ImmediateLoad)
				)
				{
					Top = new(0, 0.85f),
					Left = new(0, 0.35f),
					HoverText = Lang.inter[30]

				};
				depositAllButton.OnLeftClick += (_, _) => Rocket.Inventory.DepositAll(ContainerTransferContext.FromUnknown(Main.LocalPlayer));
				inventoryPanel.Append(depositAllButton);

				quickStackButton = new
				(
					Main.Assets.Request<Texture2D>("Images/ChestStack_0"),
					ModContent.Request<Texture2D>("Macrocosm/Assets/Textures/UI/LargePanel", AssetRequestMode.ImmediateLoad),
					ModContent.Request<Texture2D>("Macrocosm/Assets/Textures/UI/LargePanelBorder", AssetRequestMode.ImmediateLoad),
					ModContent.Request<Texture2D>("Macrocosm/Assets/Textures/UI/LargePanelHoverBorder", AssetRequestMode.ImmediateLoad)
				)
				{
					Top = new(0, 0.85f),
					Left = new(0, 0.45f),
					HoverText = Lang.inter[31]
				};
				quickStackButton.OnLeftClick += (_, _) => Rocket.Inventory.QuickStack(ContainerTransferContext.FromUnknown(Main.LocalPlayer));
				inventoryPanel.Append(quickStackButton);

				restockInventoryButton = new
				(
					Macrocosm.EmptyTexAsset,
					ModContent.Request<Texture2D>("Macrocosm/Assets/Textures/UI/LargePanel", AssetRequestMode.ImmediateLoad),
					ModContent.Request<Texture2D>("Macrocosm/Assets/Textures/UI/LargePanelBorder", AssetRequestMode.ImmediateLoad),
					ModContent.Request<Texture2D>("Macrocosm/Assets/Textures/UI/LargePanelHoverBorder", AssetRequestMode.ImmediateLoad)
				)
				{
					Top = new(0, 0.85f),
					Left = new(0, 0.55f),
					HoverText = Lang.inter[82]
				};
				restockInventoryButton.OnLeftClick += (_, _) => Rocket.Inventory.Restock(); 	
				inventoryPanel.Append(restockInventoryButton);

				sortInventoryButton = new
				(
					Main.Assets.Request<Texture2D>("Images/UI/Sort_0"),
					ModContent.Request<Texture2D>("Macrocosm/Assets/Textures/UI/LargePanel", AssetRequestMode.ImmediateLoad),
					ModContent.Request<Texture2D>("Macrocosm/Assets/Textures/UI/LargePanelBorder", AssetRequestMode.ImmediateLoad),
					ModContent.Request<Texture2D>("Macrocosm/Assets/Textures/UI/LargePanelHoverBorder", AssetRequestMode.ImmediateLoad)
				)
				{
					Top = new(0, 0.85f),
					Left = new(0, 0.65f),
					HoverText = Lang.inter[122]
				};
				sortInventoryButton.OnLeftClick += (_,_) => Rocket.Inventory.Sort();
				inventoryPanel.Append(sortInventoryButton);


				if (Main.netMode == NetmodeID.MultiplayerClient || true)
				{
					requestAccessButton = new
                        (
                            Macrocosm.EmptyTexAsset,
							ModContent.Request<Texture2D>("Macrocosm/Assets/Textures/UI/WidePanel", AssetRequestMode.ImmediateLoad),
							ModContent.Request<Texture2D>("Macrocosm/Assets/Textures/UI/WidePanelBorder", AssetRequestMode.ImmediateLoad),
							ModContent.Request<Texture2D>("Macrocosm/Assets/Textures/UI/WidePanelHoverBorder", AssetRequestMode.ImmediateLoad)
						)
					{
						Top = new(0, 0.85f),
						Left = new(0, 0.15f),
						BackPanelColor = new Color(106, 138, 255)
					};
					requestAccessButton.OnLeftClick += (_, _) => Rocket.Inventory.InteractingPlayer = Main.myPlayer;
					requestAccessButton.CheckInteractible = () => Rocket.Inventory.InteractingPlayer != Main.myPlayer;
					inventoryPanel.Append(requestAccessButton);

					lootAllButton.Left.Percent = 0.34f;
					depositAllButton.Left.Percent = 0.44f;
					quickStackButton.Left.Percent = 0.54f;
					restockInventoryButton.Left.Percent = 0.64f;
					sortInventoryButton.Left.Percent = 0.74f;
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
				BorderColor = new Color(89, 116, 213, 255),
			    BackgroundColor = new Color(53, 72, 135),
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
