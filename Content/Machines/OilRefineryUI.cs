using Macrocosm.Common.Sets;
using Macrocosm.Common.Storage;
using Macrocosm.Common.Systems.Power;
using Macrocosm.Common.UI;
using Macrocosm.Common.UI.Themes;
using Macrocosm.Content.Items.LiquidContainers;
using Macrocosm.Content.Liquids;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria.GameContent.UI.Elements;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;


namespace Macrocosm.Content.Machines
{
    public class OilRefineryUI : MachineUI
    {
        public OilRefineryTE OilRefinery => MachineTE as OilRefineryTE;

        private UIPanel backgroundPanel;
        private UIPanel inventoryPanel;

        private UILiquidTank inputLiquidTank;
        private UILiquidTank outputLiquidTank;

        private UITextPanel<string> inputTankLiquidName;
        private UITextPanel<string> outputTankLiquidName;

        private UITextureProgressBar refineArrowProgressBar;
        private UITextureProgressBar extractArrowProgressBar;

        private UIHoverImageButton containerArrow1;
        private UIHoverImageButton containerArrow2;

        private UIInventorySlot containerSlot;
        private UIInventorySlot outputSlot;

        public OilRefineryUI()
        {
        }

        public override void OnInitialize()
        {
            base.OnInitialize();

            Width.Set(510f, 0f);
            Height.Set(394f, 0f);

            //Recalculate();

            Asset<Texture2D> arrow = ModContent.Request<Texture2D>(Macrocosm.TexturesPath + "UI/Buttons/LongArrow", AssetRequestMode.ImmediateLoad);

            backgroundPanel = new()
            {
                Width = new(0, 1f),
                Height = new(0, 1f),
                BorderColor = UITheme.Current.PanelStyle.BorderColor,
                BackgroundColor = UITheme.Current.PanelStyle.BackgroundColor,
            };
            backgroundPanel.SetPadding(0);
            Append(backgroundPanel);

            if (OilRefinery.Inventory is not null)
            {
                inventoryPanel = new()
                {
                    Height = new(0, 0.75f),
                    Width = new(48 + 12, 0f),
                    Left = new(0, 0.04f),
                    VAlign = 0.6f,
                    BorderColor = UITheme.Current.PanelStyle.BorderColor,
                    BackgroundColor = UITheme.Current.PanelStyle.BackgroundColor,
                };
                backgroundPanel.Append(inventoryPanel);

                for (int i = 2; i < OilRefinery.Inventory.Size; i++)
                {
                    var inputSlot = OilRefinery.Inventory.ProvideItemSlot(i);
                    inputSlot.Top = new(48 * (i - 2), 0f);
                    inputSlot.HAlign = 0.5f;
                    inventoryPanel.Append(inputSlot);
                }
            }

            extractArrowProgressBar = new(
                  ModContent.Request<Texture2D>(Macrocosm.TexturesPath + "UI/ProgressArrowBorder", AssetRequestMode.ImmediateLoad),
                  ModContent.Request<Texture2D>(Macrocosm.TexturesPath + "UI/ProgressArrowBackground", AssetRequestMode.ImmediateLoad),
                  ModContent.Request<Texture2D>(Macrocosm.TexturesPath + "UI/ProgressArrowBackground", AssetRequestMode.ImmediateLoad)
            )
            {
                BorderColor = UITheme.Current.PanelStyle.BorderColor,
                BackgroundColor = UITheme.Current.PanelStyle.BackgroundColor,
                FillColors = [Color.Black],
                Left = new(0, 0.17f),
                VAlign = 0.52f
            };
            backgroundPanel.Append(extractArrowProgressBar);

            inputLiquidTank = new(LiquidType.Oil)
            {
                Width = new(64, 0),
                Height = new(0, 0.7f),
                HAlign = 0.335f,
                VAlign = 0.65f
            };
            backgroundPanel.Append(inputLiquidTank);

            inputTankLiquidName = new(Language.GetTextValue($"Mods.Macrocosm.Liquids.{nameof(LiquidType.Oil)}"))
            {
                HAlign = 0.335f,
                Top = new(0, 0.08f),
                TextScale = 0.8f,
                BorderColor = UITheme.Current.PanelStyle.BorderColor,
                BackgroundColor = UITheme.Current.PanelStyle.BackgroundColor
            };
            backgroundPanel.Append(inputTankLiquidName);

            refineArrowProgressBar = new(
               ModContent.Request<Texture2D>(Macrocosm.TexturesPath + "UI/ProgressArrowBorder", AssetRequestMode.ImmediateLoad),
               ModContent.Request<Texture2D>(Macrocosm.TexturesPath + "UI/ProgressArrowBackground", AssetRequestMode.ImmediateLoad),
               ModContent.Request<Texture2D>(Macrocosm.TexturesPath + "UI/ProgressArrowBackground", AssetRequestMode.ImmediateLoad)
            )
            {
                BorderColor = UITheme.Current.PanelStyle.BorderColor,
                BackgroundColor = UITheme.Current.PanelStyle.BackgroundColor,
                FillColors = [Color.Black, new Color(157, 60, 0)],
                HAlign = 0.5f,
                VAlign = 0.52f
            };
            backgroundPanel.Append(refineArrowProgressBar);

            outputLiquidTank = new(LiquidType.RocketFuel)
            {
                Width = new(64, 0),
                Height = new(0, 0.7f),
                HAlign = 0.665f,
                VAlign = 0.65f
            };
            backgroundPanel.Append(outputLiquidTank);

            outputTankLiquidName = new(Language.GetTextValue($"Mods.Macrocosm.Liquids.{nameof(LiquidType.RocketFuel)}"))
            {
                HAlign = 0.68f,
                Top = new(0, 0.08f),
                TextScale = 0.8f,
                BorderColor = UITheme.Current.PanelStyle.BorderColor,
                BackgroundColor = UITheme.Current.PanelStyle.BackgroundColor
            };
            backgroundPanel.Append(outputTankLiquidName);

            containerArrow1 = new(arrow)
            {
                HAlign = 0.82f,
                VAlign = 0.32f
            };
            containerArrow1.SetVisibility(1f);
            backgroundPanel.Append(containerArrow1);

            if (OilRefinery.Inventory is not null)
            {
                containerSlot = OilRefinery.Inventory.ProvideItemSlot(0);
                containerSlot.VAlign = 0.32f;
                containerSlot.HAlign = 0.95f;
                backgroundPanel.Append(containerSlot);
            }

            containerArrow2 = new(arrow)
            {
                HAlign = 0.935f,
                VAlign = 0.54f,
                Rotation = MathHelper.PiOver2
            };
            containerArrow2.SetVisibility(1f);
            backgroundPanel.Append(containerArrow2);

            if (OilRefinery.Inventory is not null)
            {
                outputSlot = OilRefinery.Inventory.ProvideItemSlot(1);
                outputSlot.Top = new(0, 0.65f);
                outputSlot.HAlign = 0.95f;
                backgroundPanel.Append(outputSlot);
            }
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            Inventory.ActiveInventory = OilRefinery.Inventory;
            outputSlot.CanInteract = true;

            extractArrowProgressBar.Progress = OilRefinery.RefineProgress;
            refineArrowProgressBar.Progress = OilRefinery.RefineProgress;

            inputLiquidTank.LiquidLevel = MathHelper.Lerp(inputLiquidTank.LiquidLevel, OilRefinery.InputTankAmount / OilRefinery.SourceTankCapacity, 0.025f);
            inputLiquidTank.WaveAmplitude = 1f;
            inputLiquidTank.WaveFrequency = 1f;

            outputLiquidTank.LiquidLevel = MathHelper.Lerp(outputLiquidTank.LiquidLevel, OilRefinery.OutputTankAmount / OilRefinery.ResultTankCapacity, 0.025f);
            outputLiquidTank.WaveAmplitude = 1f;
            outputLiquidTank.WaveFrequency = 1f;
        }
    }
}
