using Macrocosm.Common.Netcode;
using Macrocosm.Common.Subworlds;
using Macrocosm.Content.Subworlds;
using SubworldLibrary;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Rockets
{
    public partial class Rocket
    {
        public void SyncEverything(int toClient = -1, int ignoreClient = -1, int ignoreSubserver = -1)
        {
            SyncCommonData(toClient, ignoreClient, ignoreSubserver);
            SyncCustomizationData(toClient, ignoreClient, ignoreSubserver);
            Inventory.SyncEverything(toClient, ignoreClient, toSubservers: true, ignoreSubserver: ignoreSubserver);
        }

        /// <summary>
        /// Syncs the rocket fields with <see cref="NetSyncAttribute"/> across all clients and the server.
        /// </summary>
        public void SyncCommonData(int toClient = -1, int ignoreClient = -1, int ignoreSubserver = -1)
        {
            if (Main.netMode == NetmodeID.SinglePlayer || WhoAmI < 0 || WhoAmI > RocketManager.MaxRockets)
                return;

            ModPacket packet = Macrocosm.Instance.GetPacket();
            packet.Write((byte)MessageType.SyncRocketCommonData);

            packet.Write((short)NetHelper.GetServerIndex()); 
            packet.Write((byte)WhoAmI);

            if (this.NetWrite(packet))
            {
                packet.RelayToServers(Macrocosm.Instance, ignoreSubserver);
                packet.Send(toClient, ignoreClient);
            }

            packet.Dispose();
        }

        /// <summary>
        /// Syncs a rocket with data from the <see cref="BinaryReader"/>. Don't use this method outside <see cref="PacketHandler.HandlePacket(BinaryReader, int)"/>
        /// </summary>
        /// <param name="reader"></param>
        public static void ReceiveSyncRocketCommonData(BinaryReader reader, int sender)
        {
            int senderSubserver = reader.ReadInt16();
            int rocketIndex = reader.ReadByte();

            Rocket rocket = RocketManager.Rockets[rocketIndex];
            rocket.WhoAmI = rocketIndex;

            rocket.NetReadFields(reader);

            if (Main.netMode == NetmodeID.Server)
                rocket.SyncCommonData(ignoreClient: sender, ignoreSubserver: senderSubserver);
        }

        public void SyncCustomizationData(int toClient = -1, int ignoreClient = -1, int ignoreSubserver = -1)
        {
            if (Main.netMode == NetmodeID.SinglePlayer || WhoAmI < 0 || WhoAmI > RocketManager.MaxRockets)
                return;

            ModPacket packet = Macrocosm.Instance.GetPacket();
            packet.Write((byte)MessageType.SyncRocketCustomizationData);

            packet.Write((short)NetHelper.GetServerIndex());
            packet.Write((byte)WhoAmI);

            packet.Write(GetCustomizationDataToJSON()); // Cringe

            packet.RelayToServers(Macrocosm.Instance, ignoreSubserver);
            packet.Send(toClient, ignoreClient);
        }

        public static void ReceiveSyncRocketCustomizationData(BinaryReader reader, int clientWhoAmI)
        {
            int senderSubserver = reader.ReadInt16();
            int rocketIndex = reader.ReadByte();

            Rocket rocket = RocketManager.Rockets[rocketIndex];
            rocket.WhoAmI = rocketIndex;

            rocket.ApplyRocketCustomizationFromJSON(reader.ReadString());

            if (Main.netMode == NetmodeID.Server)
                rocket.SyncCustomizationData(ignoreClient: clientWhoAmI, ignoreSubserver: senderSubserver);
        }
    }
}
