using Macrocosm.Common.UI;
using Macrocosm.Common.Utils;
using Macrocosm.Content.Rockets.Customization;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.ID;
using Terraria.Localization;
using Terraria.UI;

namespace Macrocosm.Content.Rockets.UI
{
    public class UICargoTab : UIPanel, ITabUIElement, IRocketUIDataConsumer
    {
        public Rocket Rocket { get; set; }

        private int InventorySize => (Rocket is not null && Rocket.HasInventory) ? Rocket.Inventory.Size : cacheSize;
        private int cacheSize = Rocket.DefaultInventorySize;

        private UIListScrollablePanel inventoryPanel;
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
			this.ReplaceChildWith(inventoryPanel, inventoryPanel = CreateInventory());
		}

        public void OnTabOpen()
        {
            cacheSize = InventorySize;
			this.ReplaceChildWith(inventoryPanel, inventoryPanel = CreateInventory());
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

            inventoryPanel = CreateInventory();
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
				this.ReplaceChildWith(inventoryPanel, inventoryPanel = CreateInventory());
			}
		}

        private UIListScrollablePanel CreateInventory()
        {
            inventoryPanel = new(new LocalizedColorScaleText(Language.GetText("Mods.Macrocosm.UI.Rocket.Common.Inventory"), scale: 1.2f))
            {
                Width = new(0, 0.596f),
                Height = new(0, 0.465f),
                Left = new(0, 0.405f),
                Top = new(0, 0),
                HAlign = 0f,
                BackgroundColor = new Color(53, 72, 135),
                BorderColor = new Color(89, 116, 213, 255),
                ListPadding = 0f,
                ListOuterPadding = 2f,
                ScrollbarHeight = new(0f, 0.95f),
                ScrollbarHAlign = 0.995f,
                ListWidthWithScrollbar = new(0, 1f),
                ListWidthWithoutScrollbar = new(0, 1f),
				ShiftTitleIfHasScrollbar = false
			};
            inventoryPanel.SetPadding(0f);
            inventoryPanel.Deactivate();

            if (Rocket is null || !Rocket.HasInventory)
                return inventoryPanel;

			int count = InventorySize;

            int iconsPerRow = 10;
            int rowsWithoutScrollbar = 5;
            float iconSize;
            float iconOffsetLeft;
            float iconOffsetTop;

            if (count <= iconsPerRow * rowsWithoutScrollbar)
            {
                iconSize = 48f;
                iconOffsetLeft = 10f;
                iconOffsetTop = 6f;
            }
            else
            {
                iconSize = 48f;
                iconOffsetLeft = 2f;
                iconOffsetTop = 6f;
            }

            UIElement itemSlotContainer = new()
            {
                Width = new(0f, 1f),
                Height = new(iconSize * (count / iconsPerRow + (count % iconsPerRow != 0 ? 1 : 0)), 0f),
            };

            inventoryPanel.Add(itemSlotContainer);
            itemSlotContainer.SetPadding(0f);

            for (int i = 0; i < count; i++)
            {
                UICustomItemSlot uiItemSlot = new(Rocket.Inventory, i, ItemSlot.Context.ChestItem)
                {
                    Left = new(i % iconsPerRow * iconSize + iconOffsetLeft, 0f),
                    Top = new(i / iconsPerRow * iconSize + iconOffsetTop, 0f)
                };
                uiItemSlot.SetPadding(0f);

                uiItemSlot.Activate();
                itemSlotContainer.Append(uiItemSlot);
            }

            inventoryPanel.Activate();
            return inventoryPanel;
        }

		private UIListScrollablePanel CreateCrewPanel()
		{
			crewPanel = new(new LocalizedColorScaleText(Language.GetText("Mods.Macrocosm.UI.Rocket.Common.Crew"), scale: 1.2f))
			{
				Top = new(0f, 0.47f),
				Left = new(0, 0.405f),
				Height = new(0, 0.53f),
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
