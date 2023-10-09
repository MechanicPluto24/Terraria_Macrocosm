
using Macrocosm.Common.Netcode;
using System;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace Macrocosm.Content.Rockets.Storage
{
	public partial class Inventory
	{
		public enum InventoryMessageType
		{
			SyncInteraction,
			SyncSize,
			SyncAllItems,
			SyncItem
		}

		public static void HandlePacket(BinaryReader reader, int sender)
		{
			var type = (InventoryMessageType)reader.ReadByte();

			switch (type)
			{
				case InventoryMessageType.SyncInteraction:
					ReceiveSyncInteraction(reader, sender);
					break;

				case InventoryMessageType.SyncSize:
					ReceiveSyncSize(reader, sender);
					break;

				case InventoryMessageType.SyncAllItems:
					ReceiveSyncAllItems(reader, sender);
					break;

				case InventoryMessageType.SyncItem: 
					ReceiveSyncItem(reader, sender);
					break;
			}
		}

		public void SyncInteraction(int toClient = -1, int ignoreClient = -1)
		{
			ModPacket packet = Macrocosm.Instance.GetPacket();

			packet.Write((byte)MessageType.SyncInventory);
			packet.Write((byte)InventoryMessageType.SyncInteraction);
			packet.Write((byte)Owner.WhoAmI);
			packet.Write((byte)interactingPlayer);

			packet.Send(toClient, ignoreClient);
		}

		private static void ReceiveSyncInteraction(BinaryReader reader, int sender)
		{
			int rocketId = reader.ReadByte();
			Rocket owner = RocketManager.Rockets[rocketId];
			Inventory inventory = owner.Inventory;

			inventory.interactingPlayer = reader.ReadByte();

			if (Main.netMode == NetmodeID.Server)
			{
				inventory.SyncInteraction(ignoreClient: sender);
			}
		}

		public void SyncSize(int toClient = -1, int ignoreClient = -1)
		{
			ModPacket packet = Macrocosm.Instance.GetPacket();

			packet.Write((byte)MessageType.SyncInventory);
			packet.Write((byte)InventoryMessageType.SyncSize);
			packet.Write((byte)Owner.WhoAmI);
			packet.Write((ushort)size);

			packet.Send(toClient, ignoreClient);
		}

		private static void ReceiveSyncSize(BinaryReader reader, int sender)
		{
			int rocketId = reader.ReadByte();
			Rocket owner = RocketManager.Rockets[rocketId];
			Inventory inventory = owner.Inventory;

			int oldSize = inventory.size;
			int newSize = reader.ReadUInt16();

			if (oldSize != newSize)
				inventory.OnResize(oldSize, newSize);

			inventory.size = newSize;

			if (Main.netMode == NetmodeID.Server)
			{
				inventory.SyncSize(ignoreClient: sender);
			}
		}

		public void SyncAllItems(int toClient = -1, int ignoreClient = -1)
		{
			ModPacket packet = Macrocosm.Instance.GetPacket();

			packet.Write((byte)MessageType.SyncInventory);
			packet.Write((byte)InventoryMessageType.SyncAllItems);
			packet.Write((byte)Owner.WhoAmI);
			packet.Write((ushort)size);

			foreach (var item in items)
				ItemIO.Send(item, packet, writeStack: true, writeFavorite: false);

			packet.Send(toClient, ignoreClient);
		}

		private static void ReceiveSyncAllItems(BinaryReader reader, int sender)
		{
			int rocketId = reader.ReadByte();
			Rocket owner = RocketManager.Rockets[rocketId];
			Inventory inventory = owner.Inventory;

			int oldSize = inventory.size;
			int newSize = reader.ReadUInt16();

			if(oldSize != newSize)
				inventory.OnResize(oldSize, newSize);

			inventory.size = newSize;

			for (int i = 0; i < inventory.size; i++)
				inventory[i] = ItemIO.Receive(reader, readStack: true, readFavorite: false);

			if (Main.netMode == NetmodeID.Server)
			{
				inventory.SyncAllItems(ignoreClient: sender);
			}
		}

		public void SyncItem(Item item, int toClient = -1, int ignoreClient = -1) => SyncItem(Array.IndexOf(items, item), toClient, ignoreClient);

		public void SyncItem(int index, int toClient = -1, int ignoreClient = -1)
		{
			if (index < 0 || index > MaxInventorySize)
				return;

			ModPacket packet = Macrocosm.Instance.GetPacket();

			packet.Write((byte)MessageType.SyncInventory);
			packet.Write((byte)InventoryMessageType.SyncItem);
			packet.Write((byte)Owner.WhoAmI);
			packet.Write((ushort)index);

			ItemIO.Send(items[index], packet, writeStack: true, writeFavorite: false);

			packet.Send(toClient, ignoreClient);
		}

		private static void ReceiveSyncItem(BinaryReader reader, int sender)
		{
			int rocketId = reader.ReadByte();

			Rocket owner = RocketManager.Rockets[rocketId];
			Inventory inventory = owner.Inventory;

			int index = reader.ReadUInt16();

			inventory[index] = ItemIO.Receive(reader, readStack: true, readFavorite: false);

			if (Main.netMode == NetmodeID.Server)
			{
				inventory.SyncItem(index, ignoreClient: sender);
			}
		}
	}
}
