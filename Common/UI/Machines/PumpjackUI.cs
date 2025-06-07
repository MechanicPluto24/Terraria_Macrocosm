using Macrocosm.Common.Storage;
using Macrocosm.Common.Systems.Power;
using Macrocosm.Common.UI;
using Macrocosm.Common.UI.Themes;
using Macrocosm.Content.Liquids;
using Macrocosm.Content.Machines.Consumers.Oil;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria.GameContent.UI.Elements;
using Terraria.Localization;
using Terraria.ModLoader;


namespace Macrocosm.Common.UI.Machines
{
    public class PumpjackUI : MachineUI
    {
        public PumpjackTE Pumpjack => MachineTE as PumpjackTE;

        private UIPanel backgroundPanel;

        private UILiquidTank oilTank;
        private UITextPanel<string> oilTankLiquidName;

        private UITextureProgressBar fillArrowProgressBar;
        private UIHoverImageButton containerArrow;
        private UIHoverImageButton containerArrow1;

        private UIInventorySlot containerSlot;
        private UIInventorySlot outputSlot;

        public PumpjackUI()
        {
        }

        public override void OnInitialize()
        {
            base.OnInitialize();

            Width.Set(510f, 0f);
            Height.Set(394f, 0f);

            //Recalculate();
            backgroundPanel = new()
            {
                Width = new(0, 1f),
                Height = new(0, 1f),
                BorderColor = UITheme.Current.PanelStyle.BorderColor,
                BackgroundColor = UITheme.Current.PanelStyle.BackgroundColor,
            };
            backgroundPanel.SetPadding(0);
            Append(backgroundPanel);

            fillArrowProgressBar = new(
                  ModContent.Request<Texture2D>(Macrocosm.UIButtonsPath + "LongArrowBorder", AssetRequestMode.ImmediateLoad),
                  ModContent.Request<Texture2D>(Macrocosm.UIButtonsPath + "LongArrowPlain", AssetRequestMode.ImmediateLoad),
                  ModContent.Request<Texture2D>(Macrocosm.UIButtonsPath + "LongArrowPlain", AssetRequestMode.ImmediateLoad)
            )
            {
                BorderColor = UITheme.Current.PanelStyle.BorderColor,
                BackgroundColor = UITheme.Current.PanelStyle.BackgroundColor,
                FillColors = [Color.Black, new Color(157, 60, 0)],
                HAlign = 0.085f,
                VAlign = 0.52f
            };
            backgroundPanel.Append(fillArrowProgressBar);

            oilTank = new(LiquidType.Oil)
            {
                Width = new(64, 0),
                Height = new(0, 0.7f),
                HAlign = 0.265f,
                VAlign = 0.65f
            };
            backgroundPanel.Append(oilTank);

            oilTankLiquidName = new(Language.GetTextValue($"Mods.Macrocosm.Liquids.{nameof(LiquidType.Oil)}"))
            {
                HAlign = 0.28f,
                Top = new(0, 0.08f),
                TextScale = 0.8f,
                BorderColor = UITheme.Current.PanelStyle.BorderColor,
                BackgroundColor = UITheme.Current.PanelStyle.BackgroundColor
            };
            backgroundPanel.Append(oilTankLiquidName);

            containerArrow = new(ModContent.Request<Texture2D>(Macrocosm.UIButtonsPath + "LongArrow", AssetRequestMode.ImmediateLoad), useThemeColors: true)
            {
                HAlign = 0.695f,
                VAlign = 0.52f
            };
            containerArrow.SetVisibility(1f);
            backgroundPanel.Append(containerArrow);

            containerArrow1 = new(ModContent.Request<Texture2D>(Macrocosm.UIButtonsPath + "LongArrow", AssetRequestMode.ImmediateLoad), useThemeColors: true)
            {
                HAlign = 0.415f,
                VAlign = 0.52f
            };
            containerArrow1.SetVisibility(1f);
            backgroundPanel.Append(containerArrow1);

            if (Pumpjack.Inventory is not null)
            {
                containerSlot = Pumpjack.Inventory.ProvideItemSlot(0);
                containerSlot.HAlign = 0.565f;
                containerSlot.VAlign = 0.52f;
                backgroundPanel.Append(containerSlot);

                outputSlot = Pumpjack.Inventory.ProvideItemSlot(1);
                outputSlot.HAlign = 0.825f;
                outputSlot.VAlign = 0.52f;
                backgroundPanel.Append(outputSlot);
            }
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            Inventory.ActiveInventory = Pumpjack.Inventory;
            outputSlot.CanInteractWithItem = true;

            fillArrowProgressBar.Progress = Pumpjack.ExtractProgress;

            oilTank.LiquidLevel = MathHelper.Lerp(oilTank.LiquidLevel, Pumpjack.TankAmount / Pumpjack.TankCapacity, 0.1f);
            oilTank.WaveAmplitude = 1f;
            oilTank.WaveFrequency = 1f;
        }
    }
}
