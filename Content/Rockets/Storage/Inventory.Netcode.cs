
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
		public void SyncAllItems(int toClient = -1, int ignoreClient = -1)
		{
			ModPacket packet = Macrocosm.Instance.GetPacket();

			packet.Write((byte)MessageType.SyncInventory);
			packet.Write((byte)Owner.WhoAmI);
			packet.Write((ushort)size);

			foreach (var item in items)
				ItemIO.Send(item, packet, writeStack: true, writeFavorite: false);

			packet.Send(toClient, ignoreClient);
		}

		public static void ReceiveSyncInventory(BinaryReader reader, int sender)
		{
			int rocketId = reader.ReadByte();
			Rocket owner = RocketManager.Rockets[rocketId];
			Inventory inventory = owner.Inventory;

			inventory.Size = reader.ReadUInt16();

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

			packet.Write((byte)MessageType.SyncItemInInventory);
			packet.Write((byte)Owner.WhoAmI);
			packet.Write((ushort)index);

			ItemIO.Send(items[index], packet, writeStack: true, writeFavorite: false);

			packet.Send(toClient, ignoreClient);
		}

		public static void ReceiveSyncItemInInventory(BinaryReader reader, int sender)
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
