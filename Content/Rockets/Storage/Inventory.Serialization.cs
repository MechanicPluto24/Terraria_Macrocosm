using System;
using Terraria.ModLoader.IO;

namespace Macrocosm.Content.Rockets.Storage
{
    public partial class Inventory : TagSerializable
    {
        public static readonly Func<TagCompound, Inventory> DESERIALIZER = DeserializeData;
        public TagCompound SerializeData()
        {
            TagCompound tag = new()
            {
                [nameof(Size)] = Size,
                [nameof(WhoAmI)] = WhoAmI,
            };

            for (int i = 0; i < Size; i++)
                tag.Add($"Item{i}", ItemIO.Save(items[i]));

            return tag;
        }

        public static Inventory DeserializeData(TagCompound tag)
        {
            int whoAmI = -1;
            int size = Rocket.DefaultInventorySize;

            if (tag.ContainsKey(nameof(Size)))
                size = tag.GetInt(nameof(Size));

            if (tag.ContainsKey(nameof(WhoAmI)))
                whoAmI = tag.GetInt(nameof(WhoAmI));

            Rocket owner = (whoAmI >= 0 && whoAmI < RocketManager.MaxRockets) ? RocketManager.Rockets[whoAmI] : new();
            Inventory inventory = new(size, owner);

            for (int i = 0; i < size; i++)
                inventory.Items[i] = ItemIO.Load(tag.GetCompound($"Item{i}"));

            return inventory;
        }
    }
}
