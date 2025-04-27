
using Macrocosm.Common.Netcode;
using System;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace Macrocosm.Common.Storage
{
    public partial class Inventory
    {
        public enum InventoryMessageType
        {
            SyncEverything,
            SyncInteraction,
            SyncItem
        }

        public static void HandlePacket(BinaryReader reader, int sender)
        {
            var type = (InventoryMessageType)reader.ReadByte();

            switch (type)
            {
                case InventoryMessageType.SyncEverything:
                    ReceiveSyncEverything(reader, sender);
                    break;

                case InventoryMessageType.SyncInteraction:
                    ReceiveSyncInteraction(reader, sender);
                    break;

                case InventoryMessageType.SyncItem:
                    ReceiveSyncItem(reader, sender);
                    break;
            }
        }

        public void SyncEverything(int toClient = -1, int ignoreClient = -1, bool toSubservers = false, int ignoreSubserver = -1)
        {
            if (Main.netMode == NetmodeID.SinglePlayer)
                return;

            ModPacket packet = Macrocosm.Instance.GetPacket();

            packet.Write((byte)MessageType.SyncInventory);
            packet.Write((byte)InventoryMessageType.SyncEverything);

            packet.Write(toSubservers);
            packet.Write((short)NetHelper.GetServerIndex());

            packet.Write((byte)Owner.InventoryOwnerType);
            packet.Write(Owner.InventoryIndex);

            packet.Write((ushort)Size);
            packet.Write((byte)interactingPlayer);

            foreach (var item in items)
                ItemIO.Send(item, packet, writeStack: true, writeFavorite: true);

            if (toSubservers)
                packet.RelayToServers(Macrocosm.Instance, ignoreSubserver);

            packet.Send(toClient, ignoreClient);
        }

        private static void ReceiveSyncEverything(BinaryReader reader, int sender)
        {
            bool toSubservers = reader.ReadBoolean();
            int senderSubserver = reader.ReadInt16();

            InventoryOwnerType ownerType = (InventoryOwnerType)reader.ReadByte();
            int ownerSerializationIndex = reader.ReadInt32();

            int newSize = reader.ReadUInt16();
            int interactingPlayer = reader.ReadByte();

            Item[] items = new Item[newSize];
            for (int i = 0; i < newSize; i++)
                items[i] = ItemIO.Receive(reader, readStack: true, readFavorite: true);

            IInventoryOwner owner = IInventoryOwner.GetInventoryOwnerInstance(ownerType, ownerSerializationIndex);
            if (owner is not null)
            {
                Inventory inventory;
                inventory = owner.Inventory;

                if (inventory.Size != newSize)
                    inventory.OnResize(inventory.Size, newSize);

                inventory.interactingPlayer = interactingPlayer;

                for (int i = 0; i < inventory.Size; i++)
                    inventory[i] = items[i].Clone();

                if (Main.netMode == NetmodeID.Server)
                    inventory.SyncEverything(ignoreClient: sender, toSubservers: toSubservers, ignoreSubserver: senderSubserver);
            }
        }

        public void SyncItem(Item item, int toClient = -1, int ignoreClient = -1) => SyncItem(Array.IndexOf(items, item), toClient, ignoreClient);

        public void SyncItem(int index, int toClient = -1, int ignoreClient = -1)
        {
            if (Main.netMode == NetmodeID.SinglePlayer)
                return;

            if (index < 0 || index > MaxInventorySize)
                return;

            ModPacket packet = Macrocosm.Instance.GetPacket();

            packet.Write((byte)MessageType.SyncInventory);
            packet.Write((byte)InventoryMessageType.SyncItem);
            packet.Write((byte)Owner.InventoryOwnerType);
            packet.Write(Owner.InventoryIndex);
            packet.Write((ushort)index);

            ItemIO.Send(items[index], packet, writeStack: true, writeFavorite: false);

            packet.Send(toClient, ignoreClient);
        }

        private static void ReceiveSyncItem(BinaryReader reader, int sender)
        {
            InventoryOwnerType ownerType = (InventoryOwnerType)reader.ReadByte();
            int ownerSerializationIndex = reader.ReadInt32();
            int itemIndex = reader.ReadUInt16();
            Item item = ItemIO.Receive(reader, readStack: true, readFavorite: false);

            IInventoryOwner owner = IInventoryOwner.GetInventoryOwnerInstance(ownerType, ownerSerializationIndex);
            if (owner is not null)
            {
                Inventory inventory = owner.Inventory;
                inventory[itemIndex] = item;

                if (Main.netMode == NetmodeID.Server)
                    inventory.SyncItem(itemIndex, ignoreClient: sender);
            }
        }

        public void SyncInteraction(int toClient = -1, int ignoreClient = -1)
        {
            if (Main.netMode == NetmodeID.SinglePlayer)
                return;

            ModPacket packet = Macrocosm.Instance.GetPacket();

            packet.Write((byte)MessageType.SyncInventory);
            packet.Write((byte)InventoryMessageType.SyncInteraction);
            packet.Write((byte)Owner.InventoryOwnerType);
            packet.Write(Owner.InventoryIndex);
            packet.Write((byte)interactingPlayer);

            packet.Send(toClient, ignoreClient);
        }

        private static void ReceiveSyncInteraction(BinaryReader reader, int sender)
        {
            InventoryOwnerType ownerType = (InventoryOwnerType)reader.ReadByte();
            int ownerSerializationIndex = reader.ReadInt32();
            int interactingPlayer = reader.ReadByte();

            IInventoryOwner owner = IInventoryOwner.GetInventoryOwnerInstance(ownerType, ownerSerializationIndex);
            if (owner is not null)
            {
                Inventory inventory = owner.Inventory;
                inventory.interactingPlayer = interactingPlayer;

                if (Main.netMode == NetmodeID.Server)
                    inventory.SyncInteraction(ignoreClient: sender);
            }
        }
    }
}
