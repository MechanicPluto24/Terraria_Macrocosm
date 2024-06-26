﻿using Macrocosm.Content.Rockets;
using System;
using Terraria.ModLoader.IO;

namespace Macrocosm.Common.Storage
{
    public partial class Inventory : TagSerializable
    {
        public static readonly Func<TagCompound, Inventory> DESERIALIZER = DeserializeData;
        public TagCompound SerializeData()
        {
            TagCompound tag = new()
            {
                [nameof(Size)] = Size,
                ["OwnerType"] = Owner is not null ? Owner.InventoryOwnerType : "Uninitialized",
                ["OwnerSerializationIndex"] = Owner is not null ? Owner.InventorySerializationIndex : -1,
            };

            for (int i = 0; i < Size; i++)
                tag.Add($"Item{i}", ItemIO.Save(items[i]));

            return tag;
        }

        public static Inventory DeserializeData(TagCompound tag)
        {
            string ownerType = "";
            int ownerSerializationIndex = -1;
            int size = Rocket.DefaultTotalInventorySize;

            if (tag.ContainsKey(nameof(Size)))
                size = tag.GetInt(nameof(Size));

            if (tag.ContainsKey("OwnerType"))
                ownerType = tag.GetString("OwnerType");

            if (tag.ContainsKey("OwnerSerializationIndex"))
                ownerSerializationIndex = tag.GetInt("OwnerSerializationIndex");

            IInventoryOwner owner = IInventoryOwner.GetInventoryOwnerInstance(ownerType, ownerSerializationIndex);
            Inventory inventory = new(size, owner);

            for (int i = 0; i < size; i++)
                inventory.Items[i] = ItemIO.Load(tag.GetCompound($"Item{i}"));

            return inventory;
        }
    }
}
