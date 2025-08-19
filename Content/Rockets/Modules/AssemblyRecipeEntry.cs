using Macrocosm.Common.Utils;
using System;
using Terraria;
using Terraria.Localization;

namespace Macrocosm.Content.Rockets.Modules;

public record AssemblyRecipeEntry
{
    public Func<Item, bool> ItemCheck { get; }
    public int RequiredAmount { get; }
    public int? ItemType { get; }

    public LocalizedText Description { get; }

    public AssemblyRecipeEntry(Func<Item, bool> itemCheck, LocalizedText description, int requiredAmount = 1)
    {
        ItemCheck = itemCheck;
        RequiredAmount = requiredAmount;
        Description = description;
    }

    public AssemblyRecipeEntry(int itemType, int requiredAmount = 1) : this
    (
        itemCheck: (inputItem) => inputItem.type == itemType,
        description: Lang.GetItemName(itemType),
        requiredAmount: requiredAmount
    )
    {
        ItemType = itemType;
    }

    public bool Check(Item inputItem, bool consume = false)
    {
        if (ItemCheck(inputItem) && inputItem.stack >= RequiredAmount)
        {
            if (consume)
                inputItem.DecreaseStack(RequiredAmount);

            return true;
        }

        return false;
    }
}
