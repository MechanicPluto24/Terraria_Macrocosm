using Macrocosm.Common.Loot;
using Macrocosm.Common.Netcode;
using Macrocosm.Common.Storage;
using Macrocosm.Common.Systems.Power;
using Macrocosm.Common.UI;
using Macrocosm.Common.UI.Themes;
using Macrocosm.Common.Utils;
using Macrocosm.Content.Machines.Consumers.Drills;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Linq;
using Terraria.GameContent.ItemDropRules;
using Terraria.GameContent.UI.Elements;


namespace Macrocosm.Common.UI.Machines;

public class DrillUI : MachineUI
{
    public BaseDrillTE Drill => MachineTE as BaseDrillTE;

    private UIPanel inventoryPanel;
    private UIScrollableListPanel dropRateList;

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

        dropRateList = CreateDroprateList();
        Append(dropRateList);
    }

    private UIScrollableListPanel CreateDroprateList()
    {
        dropRateList = new("Loot")
        {
            Width = new(0, 0.306f),
            Height = new(0, 1f),
            HAlign = 1f,
            BorderColor = UITheme.Current.PanelStyle.BorderColor,
            BackgroundColor = UITheme.Current.PanelStyle.BackgroundColor,
            PaddingTop = 0f,
            PaddingLeft = 2f,
            PaddingRight = 2f
        };

        List<DropRateInfo> dropRates = new();
        DropRateInfoChainFeed ratesInfo = new(1f);
        foreach (var drop in Drill.LootTable.Entries)
            if (drop.CanDrop(LootTable.CommonDropAttemptInfo) || drop is IBlacklistable blacklistable && blacklistable.Blacklisted)
                drop.ReportDroprates(dropRates, ratesInfo);

        List<DropRateInfo> sortedDropRates = dropRates.OrderBy(entry => new Terraria.Item(entry.itemId).value).OrderBy(entry => entry.ComputeDropRarity()).ToList();

        foreach (DropRateInfo dropRateInfo in sortedDropRates)
        {
            UIItemDropInfo itemDropInfo = new(dropRateInfo)
            {
                Left = new(0, 0),
                Width = new(0, 1f),
                BackgroundColor = UITheme.Current.InfoElementStyle.BackgroundColor,
                BorderColor = UITheme.Current.InfoElementStyle.BorderColor
            };

            itemDropInfo.OnLeftClick += (_, element) => BlacklistItem(dropRateInfo);
            dropRateList.Add(itemDropInfo);
        }

        return dropRateList;
    }

    private void BlacklistItem(DropRateInfo dropRateInfo)
    {
        foreach (var entry in Drill.LootTable.Entries)
        {
            if (entry is IBlacklistable blacklistable)
            {
                if (dropRateInfo.itemId == blacklistable.ItemID)
                {
                    blacklistable.Blacklisted = !blacklistable.Blacklisted;

                    if (blacklistable.Blacklisted)
                        Drill.BlacklistedItems.Add(blacklistable.ItemID);
                    else
                        Drill.BlacklistedItems.Remove(blacklistable.ItemID);
                }
            }
        }

        Drill.NetSync();
    }

    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);

        foreach (var dropInfo in dropRateList.Where(child => child is UIItemDropInfo).Cast<UIItemDropInfo>())
            dropInfo.Blacklisted = Drill.BlacklistedItems.Contains(dropInfo.Item.type);

        Inventory.ActiveInventory = Drill.Inventory;
    }
}
