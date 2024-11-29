using Macrocosm.Common.DataStructures;
using Macrocosm.Common.Sets;
using Macrocosm.Common.UI;
using Macrocosm.Common.UI.Themes;
using Macrocosm.Common.Utils;
using Macrocosm.Content.Items.LiquidContainers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using Terraria;
using Terraria.DataStructures;
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

        private UIDynamicTextPanel textPanel;

        private UITextPanel<string> textPanel1;
        private UITextPanel<string> currentFuelTextPanel;
        private UIText separator;

        private UIPanel fuelPlacementPanel;

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
                Height = new(0, 0.45f),
                Left = new(0, 0.55f),
                Top = new(0, 0.1f),
                BackgroundColor = UITheme.Current.PanelStyle.BackgroundColor,
                BorderColor = UITheme.Current.PanelStyle.BorderColor
            };
            fuelPlacementPanel.SetPadding(4f);
            Append(fuelPlacementPanel);

            currentFuelTextPanel = new("", textScale: 1.1f)
            {
                Width = new(0, 0.8f),
                Height = new(0, 0.225f),
                HAlign = 0.5f,
                VAlign = 0.085f,
                BackgroundColor = UITheme.Current.PanelStyle.BackgroundColor,
                BorderColor = UITheme.Current.PanelStyle.BorderColor
            };
            currentFuelTextPanel.SetPadding(8f);
            fuelPlacementPanel.Append(currentFuelTextPanel);

            textPanel1 = new("", textScale: 1.1f)
            {
                Width = new(0, 0.8f),
                Height = new(0, 0.1f),
                HAlign = 0.5f,
                VAlign = 0.4f,
                BackgroundColor = UITheme.Current.PanelStyle.BackgroundColor,
                BorderColor = UITheme.Current.PanelStyle.BorderColor
            };
            textPanel1.SetPadding(8f);
            fuelPlacementPanel.Append(textPanel1);

            separator = new("___")
            {
                HAlign = 0.5f,
                VAlign = 0.15f,
            };
            currentFuelTextPanel.Append(separator);

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
                VAlign = 0.955f,
                HAlign = 0.5f,
                Rotation = MathHelper.PiOver2,
                CheckInteractible = () => dumpButtonInteractible,
                HoverTextOnButonNotInteractible = true,
            };
            dumpFuelButton.SetVisibility(1f, 0.5f, 1f);
            dumpFuelButton.OnLeftClick += (_, _) => DumpFuel();
            fuelPlacementPanel.Append(dumpFuelButton);
        }

        public void OnRocketChanged()
        {
            RefreshItemSlot();
        }

        private UIInventorySlot CreateFuelCanisterItemSlot()
        {
            liquidContainerItemSlot = Rocket.Inventory.ProvideItemSlot(Rocket.SpecialInventorySlot_FuelTank);
            liquidContainerItemSlot.Top = new(0, 0.55f);
            liquidContainerItemSlot.HAlign = 0.535f;
            return liquidContainerItemSlot;
        }

        public void FixedUpdate()
        {
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            overflowWarningVisible = false;
            dumpButtonInteractible = true;

            float neededFuel = Rocket.FuelCapacity - Rocket.Fuel;
            float previewLiquidLevel = 0f;

            if (liquidContainerItemSlot.Item.type == ItemID.None)
                dumpButtonInteractible = false;

            if (neededFuel <= 0f)
                dumpButtonInteractible = false;

            //TODO: localize
            string text = "";
            text += $"{Rocket.Fuel}\n";
            text += $"{Rocket.FuelCapacity}\n";
            currentFuelTextPanel.SetText(text);

            LiquidContainerData data = ItemSets.LiquidContainerData[liquidContainerItemSlot.Item.type];
            if (data.Valid && !data.Empty)
            {
                float availableFuel = data.Capacity * liquidContainerItemSlot.Item.stack;

                if (availableFuel <= 0)
                {
                    dumpButtonInteractible = false;
                }
                else
                {
                    textPanel1.SetText($"+{MathHelper.Clamp(availableFuel, 0, neededFuel)}");
                }

                if (neededFuel > 0f)
                {
                    previewLiquidLevel = availableFuel / neededFuel;
                    if (previewLiquidLevel > 1f)
                        previewLiquidLevel = 1f;
                }
            }
            else
            {
                textPanel1.SetText("?");
            }

            float rocketFuelPercent = Rocket.Fuel / Rocket.FuelCapacity;
            if (rocketFuelPercent > 1f)
                rocketFuelPercent = 1f;

            rocketFuelTank.LiquidLevel = MathHelper.Lerp(rocketFuelTank.LiquidLevel, rocketFuelPercent, 0.1f);
            rocketFuelTank.PreviewLiquidLevel = MathHelper.Lerp(rocketFuelTank.PreviewLiquidLevel, previewLiquidLevel, 0.1f);
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
                float fuelPerCanister = data.Capacity;
                int availableCanisters = liquidContainerItemSlot.Item.stack;
                float neededFuel = Rocket.FuelCapacity - Rocket.Fuel;

                if (neededFuel <= 0)
                    return; 

                int canistersUsed = (int)Math.Min(availableCanisters, Math.Floor(neededFuel / fuelPerCanister));

                if (canistersUsed == 0)
                    return; 

                float addedFuel = canistersUsed * fuelPerCanister;
                Rocket.Fuel += addedFuel;

                liquidContainerItemSlot.Item.stack -= canistersUsed;
                if (liquidContainerItemSlot.Item.stack <= 0)
                    liquidContainerItemSlot.Item.TurnToAir();

                int emptyType = LiquidContainerData.GetEmptyType(ItemSets.LiquidContainerData, liquidContainerItemSlot.Item.type);
                Item emptyCanisters = new(emptyType, canistersUsed);

                bool addedToInventory = Rocket.Inventory.TryPlacingItem(emptyCanisters);
                if (!addedToInventory)
                    Main.LocalPlayer.QuickSpawnItem(new EntitySource_OverfullInventory(Main.LocalPlayer), emptyType, canistersUsed);

                RefreshItemSlot();
                Rocket.NetSync();
                Rocket.Inventory.SyncEverything();
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
                    height: 261
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
                    height: 31
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
