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

        private UIHoverImageButton inputSlotToInputTankArrow;
        private UIHoverImageButton inputTankToOutputTankArrow;
        private UIHoverImageButton outputTankToContainerSlotArrow;
        private UIHoverImageButton containerSlotToOutputSlotArrow;

        private UIInventorySlot inputSlot;
        private UIInventorySlot containerSlot;
        private UIInventorySlot outputSlot;

        public OilRefineryUI()
        {
        }

        public override void OnInitialize()
        {
            base.OnInitialize();

            Width.Set(515f, 0f);
            Height.Set(394f, 0f);

            Recalculate();

            backgroundPanel = new()
            {
                Width = new(0, 1f),
                Height = new(0, 1f),
                BorderColor = UITheme.Current.PanelStyle.BorderColor,
                BackgroundColor = UITheme.Current.PanelStyle.BackgroundColor,
            };
            backgroundPanel.SetPadding(0);
            Append(backgroundPanel);

            inputLiquidTank = new(LiquidType.Oil)
            {
                Width = new(25, 0),
                Height = new(0, 0.7f),
                Left = new(-25, 0.30f),
                VAlign = 0.65f
            };
            backgroundPanel.Append(inputLiquidTank);

            outputLiquidTank = new(LiquidType.RocketFuel)
            {
                Width = new(25, 0),
                Height = new(0, 0.7f),
                Left = new(0, 0.495f),
                VAlign = 0.65f
            };
            backgroundPanel.Append(outputLiquidTank);

            inputTankLiquidName = new(Language.GetTextValue($"Mods.Macrocosm.Liquids.{nameof(LiquidType.Oil)}"))
            {
                HAlign = 0.257f,
                Top = new(0, 0.08f),
                TextScale = 0.8f,
                BorderColor = UITheme.Current.PanelStyle.BorderColor,
                BackgroundColor = UITheme.Current.PanelStyle.BackgroundColor
            };
            backgroundPanel.Append(inputTankLiquidName);

            outputTankLiquidName = new(Language.GetTextValue($"Mods.Macrocosm.Liquids.{nameof(LiquidType.RocketFuel)}"))
            {
                HAlign = 0.53f,
                Top = new(0, 0.08f),
                TextScale = 0.8f,
                BorderColor = UITheme.Current.PanelStyle.BorderColor,
                BackgroundColor = UITheme.Current.PanelStyle.BackgroundColor
            };
            backgroundPanel.Append(outputTankLiquidName);

            if (OilRefinery.Inventory is not null)
            {
                inputSlot = OilRefinery.Inventory.ProvideItemSlot(0);
                inputSlot.Top = new(0, 0.45f);
                inputSlot.Left = new(-48, 0.12f);
                backgroundPanel.Append(inputSlot);

                containerSlot = OilRefinery.Inventory.ProvideItemSlot(1);
                containerSlot.Top = new(0, 0.45f);
                containerSlot.Left = new(0, 0.67f);
                containerSlot.AddReserved(
                    (item) => item.type >= 0 && ItemSets.LiquidContainerData[item.type].Valid,
                    Lang.GetItemName(ModContent.ItemType<Canister>()),
                    ModContent.Request<Texture2D>(ContentSamples.ItemsByType[ModContent.ItemType<Canister>()].ModItem.Texture + "_Blueprint")
                );
                backgroundPanel.Append(containerSlot);

                outputSlot = OilRefinery.Inventory.ProvideItemSlot(2);
                outputSlot.Top = new(0, 0.45f);
                outputSlot.Left = new(0, 0.885f);
                outputSlot.AddReserved(
                    (item) => item.type >= 0 && ItemSets.LiquidContainerData[item.type].Valid,
                    Lang.GetItemName(ModContent.ItemType<Canister>()),
                    ModContent.Request<Texture2D>(ContentSamples.ItemsByType[ModContent.ItemType<Canister>()].ModItem.Texture + "_Blueprint")
                );
                backgroundPanel.Append(outputSlot);
            }

            Asset<Texture2D> arrow = ModContent.Request<Texture2D>(Macrocosm.TexturesPath + "UI/Buttons/LongArrow", AssetRequestMode.ImmediateLoad);
            inputSlotToInputTankArrow = new(arrow)
            {
                Left = new(0, 0.125f),
                VAlign = 0.52f
            };
            inputSlotToInputTankArrow.SetVisibility(1f);
            backgroundPanel.Append(inputSlotToInputTankArrow);

            inputTankToOutputTankArrow = new(arrow)
            {
                Left = new(0, 0.341f),
                VAlign = 0.52f
            };
            inputTankToOutputTankArrow.SetVisibility(1f);
            backgroundPanel.Append(inputTankToOutputTankArrow);

            outputTankToContainerSlotArrow = new(arrow)
            {
                Left = new(0, 0.555f),
                VAlign = 0.52f
            };
            outputTankToContainerSlotArrow.SetVisibility(1f);
            backgroundPanel.Append(outputTankToContainerSlotArrow);

            containerSlotToOutputSlotArrow = new(arrow)
            {
                Left = new(0, 0.772f),
                VAlign = 0.52f
            };
            containerSlotToOutputSlotArrow.SetVisibility(1f);
            backgroundPanel.Append(containerSlotToOutputSlotArrow);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            Inventory.ActiveInventory = OilRefinery.Inventory;
            outputSlot.CanInteract = true;

            inputLiquidTank.LiquidLevel = MathHelper.Lerp(inputLiquidTank.LiquidLevel, OilRefinery.InputTankAmount / OilRefinery.SourceTankCapacity, 0.025f);
            inputLiquidTank.WaveAmplitude = 1f;
            inputLiquidTank.WaveFrequency = 1f;

            outputLiquidTank.LiquidLevel = MathHelper.Lerp(outputLiquidTank.LiquidLevel, OilRefinery.OutputTankAmount / OilRefinery.ResultTankCapacity, 0.025f);
            outputLiquidTank.WaveAmplitude = 1f;
            outputLiquidTank.WaveFrequency = 1f;
        }
    }
}
