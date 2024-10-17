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
using Terraria;
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

        private UILiquidTank inputLiquidTank;
        private UILiquidTank outputLiquidTank;

        private UITextPanel<string> inputTankLiquidName;
        private UITextPanel<string> outputTankLiquidName;

        private UITextureProgressBar arrowProgressBar;

        private UIHoverImageButton arrow1;
        private UIHoverImageButton arrow2;
        private UIHoverImageButton arrow3;

        private UIInventorySlot inputSlot;
        private UIInventorySlot containerSlot;
        private UIInventorySlot outputSlot;

        public OilRefineryUI()
        {
        }

        public override void OnInitialize()
        {
            base.OnInitialize();

            Width.Set(615f, 0f);
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
                inputSlot = OilRefinery.Inventory.ProvideItemSlot(0);
                inputSlot.Top = new(0, 0.45f);
                inputSlot.Left = new(-48, 0.12f);
                backgroundPanel.Append(inputSlot);
            }

            arrow1 = new(arrow)
            {
                Left = new(0, 0.125f),
                VAlign = 0.52f
            };
            arrow1.SetVisibility(1f);
            backgroundPanel.Append(arrow1);

            inputLiquidTank = new(LiquidType.Oil)
            {
                Width = new(25, 0),
                Height = new(0, 0.7f),
                Left = new(0, 0.23f),
                VAlign = 0.65f
            };
            backgroundPanel.Append(inputLiquidTank);

            inputTankLiquidName = new(Language.GetTextValue($"Mods.Macrocosm.Liquids.{nameof(LiquidType.Oil)}"))
            {
                HAlign = 0.2325f,
                Top = new(0, 0.08f),
                TextScale = 0.8f,
                BorderColor = UITheme.Current.PanelStyle.BorderColor,
                BackgroundColor = UITheme.Current.PanelStyle.BackgroundColor
            };
            backgroundPanel.Append(inputTankLiquidName);

            arrowProgressBar = new(
               ModContent.Request<Texture2D>(Macrocosm.TexturesPath + "UI/ProgressArrowBorder", AssetRequestMode.ImmediateLoad),
               ModContent.Request<Texture2D>(Macrocosm.TexturesPath + "UI/ProgressArrowBackground", AssetRequestMode.ImmediateLoad),
               ModContent.Request<Texture2D>(Macrocosm.TexturesPath + "UI/ProgressArrowBackground", AssetRequestMode.ImmediateLoad)
            )
            {
                BorderColor = UITheme.Current.PanelStyle.BorderColor,
                BackgroundColor = UITheme.Current.PanelStyle.BackgroundColor,
                FillColors = [Color.Black, new Color(157, 60, 0)],
                Left = new(0, 0.28f),
                VAlign = 0.52f
            };
            backgroundPanel.Append(arrowProgressBar);

            outputLiquidTank = new(LiquidType.RocketFuel)
            {
                Width = new(25, 0),
                Height = new(0, 0.7f),
                Left = new(0, 0.575f),
                VAlign = 0.65f
            };
            backgroundPanel.Append(outputLiquidTank);

            outputTankLiquidName = new(Language.GetTextValue($"Mods.Macrocosm.Liquids.{nameof(LiquidType.RocketFuel)}"))
            {
                HAlign = 0.61f,
                Top = new(0, 0.08f),
                TextScale = 0.8f,
                BorderColor = UITheme.Current.PanelStyle.BorderColor,
                BackgroundColor = UITheme.Current.PanelStyle.BackgroundColor
            };
            backgroundPanel.Append(outputTankLiquidName);

            arrow2 = new(arrow)
            {
                Left = new(0, 0.62f),
                VAlign = 0.52f
            };
            arrow2.SetVisibility(1f);
            backgroundPanel.Append(arrow2);

            if (OilRefinery.Inventory is not null)
            {
                containerSlot = OilRefinery.Inventory.ProvideItemSlot(1);
                containerSlot.Top = new(0, 0.45f);
                containerSlot.Left = new(0, 0.72f);
                containerSlot.AddReserved(
                    (item) => item.type >= 0 && ItemSets.LiquidContainerData[item.type].Valid,
                    Lang.GetItemName(ModContent.ItemType<Canister>()),
                    ModContent.Request<Texture2D>(ContentSamples.ItemsByType[ModContent.ItemType<Canister>()].ModItem.Texture + "_Blueprint")
                );
                backgroundPanel.Append(containerSlot);
            }

            arrow3 = new(arrow)
            {
                Left = new(0, 0.8f),
                VAlign = 0.52f
            };
            arrow3.SetVisibility(1f);
            backgroundPanel.Append(arrow3);

            if (OilRefinery.Inventory is not null)
            {
                outputSlot = OilRefinery.Inventory.ProvideItemSlot(2);
                outputSlot.Top = new(0, 0.45f);
                outputSlot.Left = new(0, 0.895f);
                outputSlot.AddReserved(
                    (item) => item.type >= 0 && ItemSets.LiquidContainerData[item.type].Valid,
                    Lang.GetItemName(ModContent.ItemType<Canister>()),
                    ModContent.Request<Texture2D>(ContentSamples.ItemsByType[ModContent.ItemType<Canister>()].ModItem.Texture + "_Blueprint")
                );
                backgroundPanel.Append(outputSlot);
            }
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            Inventory.ActiveInventory = OilRefinery.Inventory;
            outputSlot.CanInteract = true;

            arrowProgressBar.Progress = OilRefinery.RefineProgress;

            inputLiquidTank.LiquidLevel = MathHelper.Lerp(inputLiquidTank.LiquidLevel, OilRefinery.InputTankAmount / OilRefinery.SourceTankCapacity, 0.025f);
            inputLiquidTank.WaveAmplitude = 1f;
            inputLiquidTank.WaveFrequency = 1f;

            outputLiquidTank.LiquidLevel = MathHelper.Lerp(outputLiquidTank.LiquidLevel, OilRefinery.OutputTankAmount / OilRefinery.ResultTankCapacity, 0.025f);
            outputLiquidTank.WaveAmplitude = 1f;
            outputLiquidTank.WaveFrequency = 1f;
        }
    }
}
