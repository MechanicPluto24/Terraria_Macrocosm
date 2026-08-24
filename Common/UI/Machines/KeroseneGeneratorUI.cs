using Macrocosm.Common.Storage;
using Macrocosm.Common.Systems.Power;
using Macrocosm.Common.UI;
using Macrocosm.Common.UI.Themes;
using Macrocosm.Content.Liquids;
using Macrocosm.Content.Machines.Generators.Fuel;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ModLiquidLib.ModLoader;
using ReLogic.Content;
using System.Collections.Generic;
using Terraria.GameContent.UI.Elements;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.UI;

namespace Macrocosm.Common.UI.Machines;

public class KeroseneGeneratorUI : MachineUI
{
    public KeroseneGeneratorTE KeroseneGenerator => MachineTE as KeroseneGeneratorTE;

    private UIPanel backgroundPanel;
    private UIPanel fuelPanel;

    private UIText rpmText;
    private UIPanelProgressBar rpmProgressBar;
    private UITextPanel<string> powerStatusText;
    private UITextureProgressBar inputArrowProgressBar;
    private UIHoverImageButton outputArrow;

    private List<UILiquidTankPiston> pistons = new();

    private float enginePhase = 0f;

    private static readonly float[] V8CrankPhaseOffsets =
    {
        0f / 4f, 3f / 4f, 1f / 4f, 2f / 4f,
        3f / 4f, 2f / 4f, 0f / 4f, 1f / 4f,
    };

    public KeroseneGeneratorUI()
    {
    }

    public override void OnInitialize()
    {
        base.OnInitialize();

        Width.Set(590f, 0f);
        Height.Set(330f, 0f);

        title.Top.Set(-36, 0);

        backgroundPanel = new()
        {
            Width = new(0, 1),
            Height = new(0, 1),
            BorderColor = UITheme.Current.PanelStyle.BorderColor,
            BackgroundColor = UITheme.Current.PanelStyle.BackgroundColor
        };
        Append(backgroundPanel);

        powerStatusText = new("", textScale: 1f, large: false)
        {
            HAlign = 1f,
            VAlign = 0.04f,
            Width = new(0, 0.6f - 0.01f),
            BorderColor = UITheme.Current.PanelStyle.BorderColor,
            BackgroundColor = UITheme.Current.PanelStyle.BackgroundColor
        };
        backgroundPanel.Append(powerStatusText);

        rpmProgressBar = new()
        {
            HAlign = 0f,
            VAlign = 0.04f,
            Width = new(0, 0.4f - 0.01f),
            Height = new(0, 0.22f),
            FillColor = new Color(0, 255, 0),
            FillColorEnd = new Color(255, 0, 0),
            IsVertical = false,
            BorderColor = UITheme.Current.PanelStyle.BorderColor,
            BackgroundColor = UITheme.Current.PanelStyle.BackgroundColor
        };
        backgroundPanel.Append(rpmProgressBar);

        rpmText = new("", textScale: 1f, large: false)
        {
            HAlign = 0.5f,
            VAlign = 0.5f,
        };
        rpmProgressBar.Append(rpmText);

        fuelPanel = new()
        {
            HAlign = 0.5f,
            Top = new(0, 0.27f),
            Height = new(0, 0.72f),
            Width = new(0, 1f),
            BorderColor = UITheme.Current.PanelStyle.BorderColor,
            BackgroundColor = UITheme.Current.PanelStyle.BackgroundColor
        };
        fuelPanel.SetPadding(0f);
        backgroundPanel.Append(fuelPanel);

        Asset<Texture2D> arrow = ModContent.Request<Texture2D>(Macrocosm.UIButtonsPath + "LongArrow", AssetRequestMode.ImmediateLoad);

        const float slotSize = 48f;
        const float slotGap = 8f;
        const float slotLeft = 24f;
        const float pistonLeft = 248f;
        const float pistonGridTop = 36f;
        const float pistonWidth = 62f;
        const float pistonHeight = 60f;
        const float pistonColumnGap = 18f;
        const float pistonRowGap = 18f;

        float pistonGridWidth = 4f * pistonWidth + 3f * pistonColumnGap;
        float pistonGridHeight = 2f * pistonHeight + pistonRowGap;
        float inputRowCenter = pistonGridTop + pistonHeight / 2f;
        float outputRowCenter = inputRowCenter + pistonHeight + pistonRowGap;
        float inputSlotTop = inputRowCenter - slotSize / 2f;
        float outputSlotTop = outputRowCenter - slotSize / 2f;
        float slotGroupRight = slotLeft + 2f * slotSize + slotGap;
        float arrowLeft = (slotGroupRight + pistonLeft - arrow.Value.Width) / 2f;

        inputArrowProgressBar = new(
            ModContent.Request<Texture2D>(Macrocosm.UIButtonsPath + "LongArrowBorder", AssetRequestMode.ImmediateLoad),
            ModContent.Request<Texture2D>(Macrocosm.UIButtonsPath + "LongArrowPlain", AssetRequestMode.ImmediateLoad),
            ModContent.Request<Texture2D>(Macrocosm.UIButtonsPath + "LongArrowPlain", AssetRequestMode.ImmediateLoad)
        )
        {
            Left = new(arrowLeft, 0f),
            Top = new(inputSlotTop + (slotSize - arrow.Value.Height) / 2f, 0f),
            BorderColor = UITheme.Current.PanelStyle.BorderColor,
            BackgroundColor = UITheme.Current.PanelStyle.BackgroundColor,
            FillColors = [Color.Black, new Color(157, 60, 0)],
            FillFromRight = true
        };
        fuelPanel.Append(inputArrowProgressBar);

        outputArrow = new(arrow, useThemeColors: true)
        {
            Left = new(arrowLeft, 0f),
            Top = new(outputSlotTop + (slotSize - arrow.Value.Height) / 2f, 0f),
            SpriteEffects = SpriteEffects.FlipHorizontally,
            CheckInteractible = () => false
        };
        outputArrow.SetVisibility(1f);
        fuelPanel.Append(outputArrow);

        UIElement pistonGrid = new()
        {
            Left = new(pistonLeft, 0f),
            Top = new(pistonGridTop, 0f),
            Width = new(pistonGridWidth, 0f),
            Height = new(pistonGridHeight, 0f)
        };
        pistonGrid.SetPadding(0f);
        fuelPanel.Append(pistonGrid);

        pistons = new();
        for (int row = 0; row < 2; row++)
        {
            for (int col = 0; col < 4; col++)
            {
                var piston = new UILiquidTankPiston(LiquidLoader.LiquidType<RocketFuel>())
                {
                    Left = new(col * (pistonWidth + pistonColumnGap), 0),
                    Top = new(row * (pistonHeight + pistonRowGap), 0),
                    Width = new(pistonWidth, 0f),
                    Height = new(pistonHeight, 0f),
                    BorderColor = UITheme.Current.PanelStyle.BorderColor,
                    BackgroundColor = UITheme.Current.PanelStyle.BackgroundColor
                };
                pistons.Add(piston);
                pistonGrid.Append(piston);
            }
        }

        if (KeroseneGenerator.Inventory is not null)
        {
            for (int i = 0; i < KeroseneGenerator.Inventory.Size; i++)
            {
                var inputSlot = KeroseneGenerator.Inventory.ProvideItemSlot(i, ItemSlot.Context.ChestItem);

                bool outputSlot = i >= 2;
                int column = i % 2;

                inputSlot.Left = new(slotLeft + column * (slotSize + slotGap), 0f);
                inputSlot.Top = new(outputSlot ? outputSlotTop : inputSlotTop, 0f);
                inputSlot.SetPadding(0f);
                fuelPanel.Append(inputSlot);
            }
        }
    }

    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);
        Inventory.ActiveInventories.Add(KeroseneGenerator.Inventory);

        string power = $"{KeroseneGenerator.GeneratedPower:F2}";
        powerStatusText.SetText(Language.GetText("Mods.Macrocosm.Machines.Common.GeneratedPower").Format(power));

        float rpmProgress = KeroseneGenerator.RPMProgress;
        rpmProgressBar.Progress = rpmProgress;
        inputArrowProgressBar.Progress = KeroseneGenerator.ConsumedItem.IsAir ? 0f : 1f - KeroseneGenerator.BurnProgress;

        float rpm = KeroseneGenerator.RPM;
        rpmText.SetText(Language.GetText("Mods.Macrocosm.Machines.KeroseneGenerator.RPM").Format((int)rpm));

        bool running = rpmProgress > 0.01f;
        if (running)
            enginePhase = (enginePhase + MathHelper.Lerp(0.008f, 0.033f, rpmProgress)) % 1f;

        for (int i = 0; i < pistons.Count; i++)
        {
            pistons[i].Running = running;
            pistons[i].Phase = (enginePhase + V8CrankPhaseOffsets[i]) % 1f;
        }
    }
}
