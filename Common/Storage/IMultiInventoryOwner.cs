using System;
using System.Collections.Generic;
using Terraria;

namespace Macrocosm.Common.Storage;

public interface IMultiInventoryOwner : IInventoryOwner
{
    IReadOnlyList<Inventory> Inventories { get; }

    /// <summary>
    /// Identifies the current physical inventory occupying an index. Replacing an inventory must change its revision.
    /// </summary>
    int GetInventoryRevision(int inventoryIndex);
}

public interface IInventoryAutomationOwner : IInventoryOwner
{
    bool TryInsertFromAutomation(ref Item item, bool sound = false);
    IEnumerable<Inventory> GetAutomationOutputInventories();
}

public static class InventoryOwnerExtensions
{
    public static IReadOnlyList<Inventory> GetInventories(this IInventoryOwner owner)
        => owner is IMultiInventoryOwner multiOwner ? multiOwner.Inventories : owner?.Inventory is not null ? [owner.Inventory] : [];

    public static Inventory GetInventory(this IInventoryOwner owner, int inventoryIndex)
    {
        IReadOnlyList<Inventory> inventories = owner.GetInventories();
        return inventoryIndex >= 0 && inventoryIndex < inventories.Count ? inventories[inventoryIndex] : null;
    }

    public static int GetInventoryIndex(this IInventoryOwner owner, Inventory inventory)
    {
        IReadOnlyList<Inventory> inventories = owner.GetInventories();
        for (int i = 0; i < inventories.Count; i++)
            if (ReferenceEquals(inventories[i], inventory))
                return i;
        return -1;
    }

    public static int GetInventoryRevision(this IInventoryOwner owner, int inventoryIndex)
        => owner is IMultiInventoryOwner multiOwner ? multiOwner.GetInventoryRevision(inventoryIndex) : 0;

    public static bool TryInsertFromAutomation(this IInventoryOwner owner, ref Item item, bool sound = false)
        => owner is IInventoryAutomationOwner automationOwner
            ? automationOwner.TryInsertFromAutomation(ref item, sound)
            : owner?.Inventory?.TryPlacingItem(ref item, InventoryPlacementSource.Automation, sound: sound) ?? false;

    public static IEnumerable<Inventory> GetAutomationOutputInventories(this IInventoryOwner owner)
        => owner is IInventoryAutomationOwner automationOwner ? automationOwner.GetAutomationOutputInventories() : owner.GetInventories();
}
