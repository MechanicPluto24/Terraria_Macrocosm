using Macrocosm.Common.Netcode;
using Macrocosm.Common.Utils;
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
		public void NetSync(int ignoreClient = -1)
		{
			if (Main.netMode == NetmodeID.SinglePlayer || WhoAmI < 0 || WhoAmI > RocketManager.MaxRockets)
				return;
			
			ModPacket packet = Macrocosm.Instance.GetPacket();

			if (WriteToPacket(packet))  
				 packet.Send(-1, ignoreClient);

			packet.Dispose();
		}

		public bool WriteToPacket(ModPacket packet)
		{
			packet.Write((byte)MessageType.SyncRocketData);
			packet.Write((byte)WhoAmI);

			if (this.NetWriteFields(packet)) // Check if the writer was able to write all the fields.
				return true;

			return false;
		}

		/// <summary>
		/// Syncs a rocket with data from the <see cref="BinaryReader"/>. Don't use this method outside <see cref="PacketHandler.HandlePacket(BinaryReader, int)"/>
		/// </summary>
		/// <param name="reader"></param>
		public static void SyncRocketData(BinaryReader reader, int clientWhoAmI)
		{
			// the rocket WhoAmI
			int rocketIndex = reader.ReadByte(); 

			Rocket rocket = RocketManager.Rockets[rocketIndex];
			rocket.WhoAmI = rocketIndex;
			rocket.NetReadFields(reader);
			
			if (Main.netMode == NetmodeID.Server)
			{
				// Bounce to all other clients, minus the sender
				rocket.NetSync(ignoreClient: clientWhoAmI);

				/*
				ModPacket packet = Macrocosm.Instance.GetPacket();
				rocket.WriteToPacket(packet);
 
				if (SubworldSystem.AnyActive())
 					SubworldSystem.SendToMainServer(Macrocosm.Instance, packet.GetBuffer());
				else 
					SubworldSystem.SendToAllSubservers(Macrocosm.Instance, packet.GetBuffer());
				*/
			}
		}
	}
}  
