using System;
using Terraria;
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
				[nameof(Size)] = size,
				[nameof(WhoAmI)] = WhoAmI,
				[nameof(InteractingPlayer)] = interactingPlayer  
			};

			for (int i = 0; i < size; i++)
 				tag.Add($"Item{i}", ItemIO.Save(items[i]));

			return tag;
 		}

		public static Inventory DeserializeData(TagCompound tag)
		{
			int whoAmI = -1;
			int size = Rocket.DefaultInventorySize;
			int interactingPlayer = Main.maxPlayers;

			if (tag.ContainsKey(nameof(Size)))
				size = tag.GetInt(nameof(Size));

			if (tag.ContainsKey(nameof(WhoAmI)))
				whoAmI = tag.GetInt(nameof(WhoAmI));

			if (tag.ContainsKey(nameof(InteractingPlayer)))
				interactingPlayer = tag.GetInt(nameof(InteractingPlayer));

			Rocket owner = (whoAmI >= 0 && whoAmI < RocketManager.MaxRockets) ? RocketManager.Rockets[whoAmI] : new();
			Inventory inventory = new(size, owner);
			inventory.InteractingPlayer = interactingPlayer;

			for (int i = 0; i < size; i++)
				inventory.Items[i] = ItemIO.Load(tag.GetCompound($"Item{i}"));
				
			return inventory;
		}
	}
}
