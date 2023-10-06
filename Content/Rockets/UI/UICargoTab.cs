using Macrocosm.Common.UI;
using Macrocosm.Common.Utils;
using Macrocosm.Content.Rockets.Customization;
using Microsoft.Xna.Framework;
using System;
using System.Security.Cryptography.X509Certificates;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.UI;

namespace Macrocosm.Content.Rockets.UI
{
    public class UICargoTab : UIPanel, ITabUIElement, IRocketUIDataConsumer
    {
        private Rocket rocket = new();
        public Rocket Rocket
        {
            get => rocket;
            set
            {
                cacheSize = rocket.Inventory.Size;
                bool newSize = cacheSize != value.Inventory.Size;
                rocket = value;

                if (newSize)
                    this.ReplaceChildWith(inventoryPanel, inventoryPanel = CreateInventory());
            }
        }
        private int cacheSize = Rocket.DefaultInventorySize;


        private UIListScrollablePanel inventoryPanel;

        private UIPanel fuelPanel;


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

            BackgroundColor = new Color(13, 23, 59, 127);
            BorderColor = new Color(15, 15, 15, 255);

            inventoryPanel = CreateInventory();
            Append(inventoryPanel);

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
            if (Main.LocalPlayer.controlQuickHeal)
                rocket.Inventory.Size += 1;

            if (Main.LocalPlayer.controlQuickMana)
                rocket.Inventory.Size -= 1;

            if (cacheSize != rocket.Inventory.Size)
            {
                this.ReplaceChildWith(inventoryPanel, inventoryPanel = CreateInventory());
                cacheSize = rocket.Inventory.Size;
            }

            base.Update(gameTime);
        }

        private UIListScrollablePanel CreateInventory()
        {
            inventoryPanel = new()
            {
                Width = new(0, 0.596f),
                Height = new(0, 0.4f),
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
                ListWidthWithoutScrollbar = new(0, 1f)
            };
            inventoryPanel.SetPadding(0f);
            inventoryPanel.Deactivate();

            int count = rocket.Inventory.Size;

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
                UICustomItemSlot uiItemSlot = new(rocket.Inventory, i, ItemSlot.Context.ChestItem)
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

    }
}
