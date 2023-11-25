using Macrocosm.Common.Netcode;
using Macrocosm.Common.Subworlds;
using Macrocosm.Common.Utils;
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
        /// <summary>
        /// Syncs the rocket fields with <see cref="NetSyncAttribute"/> across all clients and the server.
        /// </summary>
        public void NetSync(int toClient = -1, int ignoreClient = -1)
        {
            if (Main.netMode == NetmodeID.SinglePlayer || WhoAmI < 0 || WhoAmI > RocketManager.MaxRockets)
                return;

            ModPacket packet = Macrocosm.Instance.GetPacket();
            packet.Write((byte)MessageType.SyncRocketData);
            packet.Write((byte)WhoAmI);

            if (this.NetWriteFields(packet))
                packet.Send(toClient, ignoreClient);

            packet.Dispose();
        }

        /// <summary>
        /// Syncs a rocket with data from the <see cref="BinaryReader"/>. Don't use this method outside <see cref="PacketHandler.HandlePacket(BinaryReader, int)"/>
        /// </summary>
        /// <param name="reader"></param>
        public static void ReceiveSyncRocketData(BinaryReader reader, int sender)
        {
            int rocketIndex = reader.ReadByte();
            Rocket rocket = RocketManager.Rockets[rocketIndex];
            rocket.WhoAmI = rocketIndex;
            rocket.NetReadFields(reader);

            if (Main.netMode == NetmodeID.Server)
            {
                // Bounce to all other clients, minus the sender
                rocket.NetSync(ignoreClient: sender);
              
                var packet = new BinaryWriter(new MemoryStream(256));
                packet.Write((byte)MessageType.SyncRocketData);
                packet.Write((byte)rocket.WhoAmI);
                rocket.NetWriteFields(packet);

                if (rocket.ActiveInCurrentWorld)
                {
                    if (SubworldSystem.AnyActive())
                        SubworldSystem.SendToMainServer(Macrocosm.Instance, (packet.BaseStream as MemoryStream).GetBuffer());
                    else
                        SubworldSystem.SendToSubserver(SubworldSystem.GetIndex<Moon>(), Macrocosm.Instance, (packet.BaseStream as MemoryStream).GetBuffer());
                }

                packet.Dispose();
            }
        }

		public void SendCustomizationData(int toClient = -1, int ignoreClient = -1)
		{
			if (Main.netMode == NetmodeID.SinglePlayer || WhoAmI < 0 || WhoAmI > RocketManager.MaxRockets)
				return;

			ModPacket packet = Macrocosm.Instance.GetPacket();

			packet.Write((byte)MessageType.SyncRocketCustomizationData);
			packet.Write((byte)WhoAmI);

			packet.Write(GetCustomizationDataToJSON()); // Cringe
			packet.Send(toClient, ignoreClient);
		}

		public static void SyncRocketCustomizationData(BinaryReader reader, int clientWhoAmI)
		{
			// the rocket WhoAmI
			int rocketIndex = reader.ReadByte();

			Rocket rocket = RocketManager.Rockets[rocketIndex];
			rocket.WhoAmI = rocketIndex;

			rocket.ApplyRocketCustomizationFromJSON(reader.ReadString());

			if (Main.netMode == NetmodeID.Server)
			{
				// Bounce to all other clients, minus the sender
				rocket.SendCustomizationData(ignoreClient: clientWhoAmI);
			}
		}
	}
}  
