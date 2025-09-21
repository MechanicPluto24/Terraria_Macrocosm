using Macrocosm.Common.Storage;
using Macrocosm.Common.Systems.Power;
using Macrocosm.Common.UI;
using Macrocosm.Common.UI.Themes;
using Macrocosm.Content.Liquids;
using Macrocosm.Content.Machines.Generators.Fuel;
using Microsoft.Xna.Framework;
using ModLiquidLib.ModLoader;
using System.Collections.Generic;
using Terraria.GameContent.UI.Elements;
using Terraria.Localization;
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

    private List<UILiquidTankPiston> pistons = new();

    private int timer;

    public KeroseneGeneratorUI()
    {
    }

    public override void OnInitialize()
    {
        base.OnInitialize();

        Width.Set(495f, 0f);
        Height.Set(250f, 0f);

        title.Top.Set(-36, 0);

        //Recalculate();

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
            //Width = new(0, 0.4f - 0.01f),
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
        backgroundPanel.Append(fuelPanel);

        pistons = new();
        for (int i = 0; i < 6; i++)
        {
            var piston = new UILiquidTankPiston(LiquidLoader.LiquidType<RocketFuel>())
            {
                Left = new(42 + i * 66, 0),
                Top = new(-6, 0.5f),
                Width = new(60, 0f),
                Height = new(0, 0.4f),
                BorderColor = UITheme.Current.PanelStyle.BorderColor,
                BackgroundColor = UITheme.Current.PanelStyle.BackgroundColor
            };
            pistons.Add(piston);
            fuelPanel.Append(piston);
        }

        if (KeroseneGenerator.Inventory is not null)
        {
            for (int i = 0; i < KeroseneGenerator.Inventory.Size; i++)
            {
                var inputSlot = KeroseneGenerator.Inventory.ProvideItemSlot(i, ItemSlot.Context.ChestItem);

                inputSlot.Left = new(i * 48, 0f);
                inputSlot.Top = new(-6, 0f);
                inputSlot.SetPadding(0f);
                fuelPanel.Append(inputSlot);
            }
        }
    }

    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);
        Inventory.ActiveInventory = KeroseneGenerator.Inventory;

        string power = $"{KeroseneGenerator.GeneratedPower:F2}";
        powerStatusText.SetText(Language.GetText("Mods.Macrocosm.Machines.Common.GeneratedPower").Format(power));

        timer++;

        float rpmProgress = KeroseneGenerator.RPMProgress;
        rpmProgressBar.Progress = rpmProgress;

        float rpm = KeroseneGenerator.RPM;
        rpmText.SetText(Language.GetText("Mods.Macrocosm.Machines.KeroseneGenerator.RPM").Format((int)rpm));
        rpmText.SetText($"{(int)rpm} RPM");

        if (rpmProgress > 0)
        {
            // how do I animate this shit
            int interval = (int)MathHelper.Lerp(120, 30, rpmProgress);
            for (int i = 0; i < pistons.Count; i++)
            {
                UILiquidTankPiston piston = pistons[i];
                if (timer % (interval * 2) < interval)
                {
                    if (i % 2 == 0)
                        piston.StartAnimation(2);
                }
                else
                {
                    if (i % 2 == 1)
                        piston.StartAnimation(2);
                }
            }
        }
    }

}
