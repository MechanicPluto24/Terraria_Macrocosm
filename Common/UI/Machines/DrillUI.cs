using Macrocosm.Common.Storage;
using Macrocosm.Common.Systems.Power;
using Macrocosm.Common.UI;
using Macrocosm.Common.UI.Themes;
using Macrocosm.Content.Machines.Consumers.Drills;
using Microsoft.Xna.Framework;
using Terraria.GameContent.UI.Elements;


namespace Macrocosm.Common.UI.Machines;

public class DrillUI : MachineUI
{
    public BaseDrillTE Drill => MachineTE as BaseDrillTE;

    private UIPanel inventoryPanel;
    private UITileSampleGrid sampleGrid;

    public DrillUI()
    {
    }

    public override void OnInitialize()
    {
        base.OnInitialize();

        Width.Set(745f, 0f);
        Height.Set(154 + 48 * (MachineTE?.InventorySize ?? 0) / 10 , 0f);

        Recalculate();

        if (Drill.Inventory is not null)
        {
            inventoryPanel = Drill.Inventory.ProvideUIWithInteractionButtons(iconsPerRow: 10, rowsWithoutScrollbar: 5, buttonMenuTopPercent: 0.765f);
            inventoryPanel.Width = new(0, 0.69f);
            inventoryPanel.BorderColor = UITheme.Current.PanelStyle.BorderColor;
            inventoryPanel.BackgroundColor = UITheme.Current.PanelStyle.BackgroundColor;
            inventoryPanel.Activate();
            Append(inventoryPanel);
        }

        // Right panel: tile sample grid
        UIPanel rightPanel = new()
        {
            Width = new(0, 0.306f),
            Height = new(0, 1f),
            HAlign = 1f,
            BorderColor = UITheme.Current.PanelStyle.BorderColor,
            BackgroundColor = UITheme.Current.PanelStyle.BackgroundColor,
        };

        sampleGrid = new UITileSampleGrid(Drill)
        {
            HAlign = 0.5f,
            VAlign = 0.5f,
        };

        rightPanel.Append(sampleGrid);
        Append(rightPanel);

        // Fresh sample so the grid is populated the moment the UI opens.
        Drill?.RequestResample();
    }

    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);

        Inventory.ActiveInventories.Add(Drill.Inventory);
    }
}
