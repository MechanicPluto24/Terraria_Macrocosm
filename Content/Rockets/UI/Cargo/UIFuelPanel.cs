using Macrocosm.Common.UI;
using Macrocosm.Common.UI.Themes;
using Macrocosm.Common.Utils;
using Macrocosm.Content.Items.Materials.Tech;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.UI.Elements;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.UI;

namespace Macrocosm.Content.Rockets.UI.Cargo
{
    internal class UIFuelPanel : UIPanel, IRocketUIDataConsumer, IFixedUpdateable
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
        private UIInventorySlot fuelCanisterItemSlot;
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

            rocketFuelTank = new(WaterStyleID.Lava)
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

            canisterBufferTank = new(WaterStyleID.Lava)
            {
                Width = new(0, 0.2f),
                Height = new(0, 0.8f),
                Left = new(0, 0.1f),
                VAlign = 0.5f
            };
            fuelPlacementPanel.Append(canisterBufferTank);

            fuelCanisterItemSlot = CreateFuelCanisterItemSlot();
            fuelPlacementPanel.Append(fuelCanisterItemSlot);

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
            fuelCanisterItemSlot = Rocket.Inventory.ProvideItemSlot(Rocket.SpecialInventorySlot_FuelTank);
            fuelCanisterItemSlot.Top = new(0, 0.1f);
            fuelCanisterItemSlot.Left = new(0, 0.5f);
            fuelCanisterItemSlot.AddReserved(
                (item) => item.ModItem is FuelCanister,
                Lang.GetItemName(ModContent.ItemType<FuelCanister>()),
                ModContent.Request<Texture2D>(ContentSamples.ItemsByType[ModContent.ItemType<FuelCanister>()].ModItem.Texture + "_Blueprint")
            );
            return fuelCanisterItemSlot;
        }

        public void FixedUpdate()
        {
            overflowWarningVisible = false;
            dumpButtonInteractible = true;

            float neededFuel = Rocket.FuelCapacity - Rocket.Fuel;
            float canisterBufferLevel = 0f;

            if (fuelCanisterItemSlot.Item.type == ItemID.None)
            {
                dumpButtonInteractible = false;

                if (neededFuel <= 0f)
                    fuelCanisterItemSlot.CanInteract = false;
            }

            if (neededFuel <= 0f)
                 dumpButtonInteractible = false;
 
            if (fuelCanisterItemSlot.Item.ModItem is FuelCanister fuelCanister)
            {
                float availableFuel = fuelCanister.CurrentFuel * fuelCanister.Item.stack;

                if (availableFuel <= 0)
                    dumpButtonInteractible = false;  

                if (availableFuel > neededFuel && fuelCanister.Item.stack > 1)
                {
                    dumpButtonInteractible = false;
                    overflowWarningVisible = true;
                }

                if(neededFuel > 0f)
                {
                    canisterBufferLevel = (fuelCanister.CurrentFuel * fuelCanister.Item.stack) / (Rocket.FuelCapacity - Rocket.Fuel);
                    if (canisterBufferLevel > 1f)
                        canisterBufferLevel = 1f;
                }
            }

            // Animate canister buffer
            canisterBufferTank.LiquidLevel = MathHelper.Lerp(canisterBufferTank.LiquidLevel, canisterBufferLevel, 0.1f);

            // Animate rocket tank
            float rocketFuelPercent = Rocket.Fuel / Rocket.FuelCapacity;
            if (rocketFuelPercent > 1f) 
                rocketFuelPercent = 1f;

            rocketFuelTank.LiquidLevel = MathHelper.Lerp(rocketFuelTank.LiquidLevel, rocketFuelPercent, 0.1f);
        }

        public void RefreshItemSlot()
        {
            fuelPlacementPanel.ReplaceChildWith(fuelCanisterItemSlot, fuelCanisterItemSlot = CreateFuelCanisterItemSlot());
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
