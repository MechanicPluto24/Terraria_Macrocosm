using System.IO;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Common.Netcode
{
    public class TENetHelper
    {
        public static void SyncTEFromClient(int id)
        {
            if (Main.netMode != NetmodeID.MultiplayerClient)
                return;

            ModPacket packet = Macrocosm.Instance.GetPacket();
            packet.Write((byte)MessageType.SyncTEFromClient);
            packet.Write(id);
            bool found = TileEntity.ByID.ContainsKey(id);
            packet.Write(found);
            if (found)
                TileEntity.Write(packet, TileEntity.ByID[id], networkSend: true, lightSend: true);

            packet.Send();
        }

        public static void ReceiveSyncTEFromClient(BinaryReader reader, int sender)
        {
            if (Main.netMode != NetmodeID.Server)
                return;

            int id = reader.ReadInt32();
            if (!reader.ReadBoolean())
            {
                if (TileEntity.ByID.TryGetValue(id, out var tileEntity))
                {
                    TileEntity.ByID.Remove(id);
                    Point16 position = tileEntity.Position;
                    TileEntity.ByPosition.Remove(position);

                    // Sync removal to the other clients
                    NetMessage.SendData(MessageID.TileEntitySharing, ignoreClient: sender, number: id, number2: position.X, number3: position.Y);
                }
            }
            else
            {
                TileEntity tileEntity = TileEntity.Read(reader, networkSend: true, lightSend: true);
                tileEntity.ID = id;
                TileEntity.ByID[tileEntity.ID] = tileEntity;
                TileEntity.ByPosition[tileEntity.Position] = tileEntity;

                // Sync state to the other clients
                NetMessage.SendData(MessageID.TileEntitySharing, ignoreClient: sender, number: tileEntity.ID, number2: tileEntity.Position.X, number3: tileEntity.Position.Y);
            }
        }
    }
}
