using Macrocosm.Common.DataStructures;
using Macrocosm.Common.Sets;
using Macrocosm.Common.UI;
using Macrocosm.Common.UI.Themes;
using Macrocosm.Common.Utils;
using Macrocosm.Content.Items.LiquidContainers;
using Macrocosm.Content.Items.Tech;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.UI;

namespace Macrocosm.Content.Rockets.UI.Cargo
{
    public class UIFuelPanel : UIPanel, IRocketUIDataConsumer, IFixedUpdateable
    {
        public Rocket Rocket { get; set; } = new();
        private Item ItemInFuelTankSlot => Rocket.Inventory[Rocket.SpecialInventorySlot_FuelTank];

        private bool overflowWarningVisible = false;
        private bool dumpButtonInteractible = true;

        private UIText title;
        private UIHorizontalSeparator titleSeparator;

        private UIPanel fuelPlacementPanel;
        private UILiquidTank canisterBufferTank;
        private UIHoverImageButton overflowWarningIcon;
        private UIInventorySlot liquidContainerItemSlot;
        private UIHoverImageButton dumpFuelButton;

        private UILiquidTank rocketFuelTank;

        public override void OnInitialize()
        {
            base.OnInitialize();

            Width = new(0, 0.4f);
            Height = new(0, 1f);
            HAlign = 0f;
            BackgroundColor = UITheme.Current.PanelStyle.BackgroundColor;
            BorderColor = UITheme.Current.PanelStyle.BorderColor;
            SetPadding(2f);

            title = new(Language.GetText("Mods.Macrocosm.UI.Rocket.Common.Fuel"), textScale: 0.6f, large: true)
            {
                HAlign = 0.5f,
                Top = new(0, 0.02f)
            };
            Append(title);

            titleSeparator = new()
            {
                Width = new(0, 0.995f),
                Top = new(0, 0.061f),
                Left = new(0, 0),
                HAlign = 0f,
                Color = UITheme.Current.SeparatorColor,
            };

            Append(titleSeparator);

            rocketFuelTank = new(Liquids.LiquidType.RocketFuel)
            {
                Width = new(0, 0.421f),
                Height = new(0, 0.8f),
                Left = new(0, 0.05f),
                VAlign = 0.5f,
                WaveAmplitude = 2f,
                WaveFrequency = 5f
            };
            Append(rocketFuelTank);

            fuelPlacementPanel = new()
            {
                Width = new(0, 0.4f),
                Height = new(0, 0.35f),
                Left = new(0, 0.55f),
                VAlign = 0.5f,
                BackgroundColor = UITheme.Current.PanelStyle.BackgroundColor,
                BorderColor = UITheme.Current.PanelStyle.BorderColor
            };
            fuelPlacementPanel.SetPadding(4f);
            Append(fuelPlacementPanel);

            overflowWarningIcon = new
            (
                ModContent.Request<Texture2D>(Macrocosm.TexturesPath + "UI/Symbols/WarningYellow"),
                hoverText: Language.GetText("Fuel overflow!")
            )
            {
                Left = new(0, 0.1f),
                Top = new(0, 0f),
                CheckInteractible = () => overflowWarningVisible,
            };
            overflowWarningIcon.SetVisibility(1f, 0f, 1f);
            fuelPlacementPanel.Append(overflowWarningIcon);

            canisterBufferTank = new(Liquids.LiquidType.RocketFuel)
            {
                Width = new(0, 0.2f),
                Height = new(0, 0.8f),
                Left = new(0, 0.1f),
                VAlign = 0.5f
            };
            fuelPlacementPanel.Append(canisterBufferTank);

            liquidContainerItemSlot = CreateFuelCanisterItemSlot();
            fuelPlacementPanel.Append(liquidContainerItemSlot);

            dumpFuelButton = new
            (
               //ModContent.Request<Texture2D>(ContentSamples.ItemsByType[ModContent.ItemType<FuelCanister>()].ModItem.Texture),
               ModContent.Request<Texture2D>(Macrocosm.TexturesPath + "UI/Buttons/LongArrow", AssetRequestMode.ImmediateLoad),
               ModContent.Request<Texture2D>(Macrocosm.TexturesPath + "UI/Buttons/LongArrowBorder", AssetRequestMode.ImmediateLoad),
               Language.GetText("Mods.Macrocosm.UI.Rocket.Cargo.DumpFuel")
            )
            {
                VAlign = 1f,
                HAlign = 0.5f,
                Rotation = MathHelper.PiOver2,
                CheckInteractible = () => dumpButtonInteractible,
                HoverTextOnButonNotInteractible = true,
            };
            dumpFuelButton.SetVisibility(1f, 0.5f, 1f);
            dumpFuelButton.OnLeftClick += (_, _) => DumpFuel();
            fuelPlacementPanel.Append(dumpFuelButton);
        }

        private UIInventorySlot CreateFuelCanisterItemSlot()
        {
            liquidContainerItemSlot = Rocket.Inventory.ProvideItemSlot(Rocket.SpecialInventorySlot_FuelTank);
            liquidContainerItemSlot.Top = new(0, 0.1f);
            liquidContainerItemSlot.Left = new(0, 0.5f);
            liquidContainerItemSlot.AddReserved(
                (item) => ItemSets.LiquidContainerData[item.type].Valid,
                Lang.GetItemName(ModContent.ItemType<Canister>()),
                ModContent.Request<Texture2D>(ContentSamples.ItemsByType[ModContent.ItemType<Canister>()].ModItem.Texture + "_Blueprint")
            );
            return liquidContainerItemSlot;
        }

        public void FixedUpdate()
        {
            

            // Animate canister buffer

            // Animate rocket tank
            
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            overflowWarningVisible = false;
            dumpButtonInteractible = true;
            liquidContainerItemSlot.CanInteract = true;

            float neededFuel = Rocket.FuelCapacity - Rocket.Fuel;
            float canisterBufferLevel = 0f;

            if (liquidContainerItemSlot.Item.type == ItemID.None)
            {
                dumpButtonInteractible = false;

                if (neededFuel <= 0f)
                    liquidContainerItemSlot.CanInteract = false;
            }

            if (neededFuel <= 0f)
                dumpButtonInteractible = false;

            LiquidContainerData data = ItemSets.LiquidContainerData[liquidContainerItemSlot.Item.type];
            if (data.Valid&& !data.Empty)
            {
                float availableFuel = data.Capacity * liquidContainerItemSlot.Item.stack;

                if (availableFuel <= 0)
                    dumpButtonInteractible = false;

                if (availableFuel > neededFuel && liquidContainerItemSlot.Item.stack > 1)
                {
                    dumpButtonInteractible = false;
                    overflowWarningVisible = true;
                }

                if (neededFuel > 0f)
                {
                    canisterBufferLevel = (data.Capacity * liquidContainerItemSlot.Item.stack) / (Rocket.FuelCapacity - Rocket.Fuel);
                    if (canisterBufferLevel > 1f)
                        canisterBufferLevel = 1f;
                }
            }

            canisterBufferTank.LiquidLevel = MathHelper.Lerp(canisterBufferTank.LiquidLevel, canisterBufferLevel, 0.1f);

            float rocketFuelPercent = Rocket.Fuel / Rocket.FuelCapacity;
            if (rocketFuelPercent > 1f)
                rocketFuelPercent = 1f;

            rocketFuelTank.LiquidLevel = MathHelper.Lerp(rocketFuelTank.LiquidLevel, rocketFuelPercent, 0.1f);
        }

        public void RefreshItemSlot()
        {
            fuelPlacementPanel.ReplaceChildWith(liquidContainerItemSlot, liquidContainerItemSlot = CreateFuelCanisterItemSlot());
        }

        private void DumpFuel()
        {
            LiquidContainerData data = ItemSets.LiquidContainerData[liquidContainerItemSlot.Item.type];
            if (data.Valid && !data.Empty)
            {
                float availableFuel = data.Capacity * liquidContainerItemSlot.Item.stack;
                float neededFuel = Rocket.FuelCapacity - Rocket.Fuel;
                float addedFuel = Math.Min(availableFuel, neededFuel);

                Rocket.Fuel += addedFuel;
                liquidContainerItemSlot.Item.type = LiquidContainerData.GetEmptyType(ItemSets.LiquidContainerData, liquidContainerItemSlot.Item.type);

                Rocket.NetSync();
                Rocket.Inventory.SyncItem(Rocket.SpecialInventorySlot_FuelTank);
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            CalculatedStyle dimensions = GetInnerDimensions();
            CalculatedStyle fuelTankDimensions = rocketFuelTank.GetOuterDimensions();
            CalculatedStyle fuelPlacementDimensions = fuelPlacementPanel.GetOuterDimensions();

            base.Draw(spriteBatch);

            UIConnectors.DrawConnectorVertical(spriteBatch,
                new Rectangle
                (
                    x: (int)(fuelPlacementDimensions.Center().X - 10f),
                    y: (int)(fuelPlacementDimensions.Center().Y + fuelPlacementDimensions.Height / 2f),
                    width: 24,
                    height: 178
                ),
                UITheme.Current.PanelStyle.BackgroundColor,
                UITheme.Current.PanelStyle.BorderColor,
                out Rectangle topEndpointFuelPlacement,
                out Rectangle bottomEndpointFuelPlacement
            );

            UIConnectors.DrawConnectorVertical(spriteBatch,
                new Rectangle
                (
                    x: (int)(fuelTankDimensions.Center().X - 12f),
                    y: (int)(fuelTankDimensions.Center().Y + fuelTankDimensions.Height / 2f),
                    width: 24,
                    height: 30
                ),
                UITheme.Current.PanelStyle.BackgroundColor,
                UITheme.Current.PanelStyle.BorderColor,
                out Rectangle topEndpointFuelTank,
                out Rectangle bottomEndpointFuelTank
            );

            UIConnectors.DrawConnectorHorizontal(spriteBatch,
                new Rectangle
                (
                    x: (int)(fuelTankDimensions.Center().X),
                    y: (int)(dimensions.Y + dimensions.Height * 0.925f),
                    width: (int)(fuelPlacementDimensions.Center().X - fuelTankDimensions.Center().X),
                    height: 24
                ),
                UITheme.Current.PanelStyle.BackgroundColor,
                UITheme.Current.PanelStyle.BorderColor,
                out Rectangle leftEndpoint,
                out Rectangle rightEndpoint
            );

            UIConnectors.DrawConnectorLCorner(spriteBatch,
                bottomEndpointFuelTank,
                UITheme.Current.PanelStyle.BackgroundColor,
                UITheme.Current.PanelStyle.BorderColor
            );

            UIConnectors.DrawConnectorLCorner(spriteBatch,
              bottomEndpointFuelPlacement,
              UITheme.Current.PanelStyle.BackgroundColor,
              UITheme.Current.PanelStyle.BorderColor,
              SpriteEffects.FlipHorizontally
            );
        }
    }
}
