using Macrocosm.Content.Rockets;
using System;
using Terraria;
using Terraria.ModLoader.IO;

namespace Macrocosm.Common.Storage
{
    public partial class Inventory : TagSerializable
    {
        public static readonly Func<TagCompound, Inventory> DESERIALIZER = DeserializeData;
        public TagCompound SerializeData()
        {
            TagCompound tag = new() { [nameof(Size)] = Size };
            for (int i = 0; i < Size; i++)
                tag.Add($"Item{i}", ItemIO.Save(items[i]));

            if(Owner is not null)
            {
                tag[nameof(Owner.InventoryOwnerType)] = Owner.InventoryOwnerType;
                tag[nameof(Owner.InventoryIndex)] = Owner.InventoryIndex;
            }

            return tag;
        }

        public static Inventory DeserializeData(TagCompound tag)
        {
            int size = 50;
            InventoryOwnerType ownerType = InventoryOwnerType.None;
            int ownerSerializationIndex = -1;

            if (tag.ContainsKey(nameof(Size))) 
                size = tag.GetInt(nameof(Size));

            Item[] items = new Item[size];
            Array.Fill(items, new Item());
            for (int i = 0; i < size; i++) 
                items[i] = ItemIO.Load(tag.GetCompound($"Item{i}"));

            // Legacy stuff
            string legacyOwnerType = "";
            if (tag.ContainsKey("OwnerType")) legacyOwnerType = tag.GetString("OwnerType");
            if (tag.ContainsKey("OwnerSerializationIndex")) ownerSerializationIndex = tag.GetInt("OwnerSerializationIndex");
            if (!string.IsNullOrEmpty(legacyOwnerType) && Enum.TryParse(legacyOwnerType, ignoreCase: true, out InventoryOwnerType result)) ownerType = result;

            if (tag.ContainsKey(nameof(Owner.InventoryOwnerType))) ownerType = (InventoryOwnerType)tag.GetInt(nameof(Owner.InventoryOwnerType));
            if (tag.ContainsKey(nameof(Owner.InventoryIndex))) ownerSerializationIndex = tag.GetInt(nameof(Owner.InventoryIndex));

            IInventoryOwner owner = IInventoryOwner.GetInventoryOwnerInstance(ownerType, ownerSerializationIndex);
            Inventory inventory = new(size, owner) { items = items };

            return inventory;
        }
    }
}
