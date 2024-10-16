﻿using System;
using Terraria;
using Terraria.ID;
using Terraria.Localization;

namespace Macrocosm.Content.Rockets.Modules
{
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
                    Consume(inputItem);

                return true;
            }

            return false;
        }

        private void Consume(Item inputItem)
        {
            inputItem.stack -= RequiredAmount;

            if (inputItem.type == ItemID.None || inputItem.stack < 1)
            {
                inputItem.TurnToAir(fullReset: true);
            }
        }
    }
}
