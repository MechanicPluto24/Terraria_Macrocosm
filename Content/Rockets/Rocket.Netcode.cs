using System;
using System.IO;
using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Macrocosm.Common.Netcode;

namespace Macrocosm.Content.Rockets
{
    public partial class Rocket 
	{
		/// <summary>
		/// Syncs the rocket fields with <see cref="NetSyncAttribute"/> across all clients and the server.
		/// </summary>
		public void NetSync(int ignoreClient = -1)
		{
			if (Main.netMode == NetmodeID.SinglePlayer || WhoAmI < 0 || WhoAmI > MaxRockets)
				return;
			
			ModPacket packet = Macrocosm.Instance.GetPacket();

			packet.Write((byte)MessageType.SyncRocketData);
			packet.Write((byte)WhoAmI);

			if (this.NetWriteFields(packet)) // Check if the writer was able to write all the fields.
				 packet.Send(-1, ignoreClient);

			packet.Dispose();
		}

		/// <summary>
		/// Syncs a rocket with data from the <see cref="BinaryReader"/>. Don't use this method outside <see cref="PacketHandler.HandlePacket(BinaryReader, int)"/>
		/// </summary>
		/// <param name="reader"></param>
		public static void SyncRocketData(BinaryReader reader, int clientWhoAmI)
		{
			int rocketIndex = reader.ReadByte(); // the rocket WhoAmI

			Rocket rocket;
			if (RocketManager.Rockets.Count <= rocketIndex)
			{
				rocket = new Rocket();
				RocketManager.Rockets.Add(rocket);
			}
			else
			{
				rocket = RocketManager.Rockets[rocketIndex];
			}

			rocket.NetReadFields(reader);
			
			if (Main.netMode == NetmodeID.Server)
				rocket.NetSync(ignoreClient: clientWhoAmI);
		}

		public void SendEmbarkedPlayer(int playerID, bool asCommander)
		{
			ModPacket packet = Macrocosm.Instance.GetPacket();
			packet.Write((byte)MessageType.SyncEmbarkInRocket);
			packet.Write((byte)playerID);
			packet.Write(asCommander);
			packet.Write((byte)WhoAmI);
			packet.Send();
		}

		public static void ReceiveEmbarkedPlayer(BinaryReader reader, int whoAmI)
		{
			int playerId = reader.ReadByte();
			bool asCommander = reader.ReadBoolean();
			int rocketId = reader.ReadByte();
			Rocket rocket = RocketManager.Rockets[rocketId];

			// Server receives embark status from client interaction 
			if (Main.netMode == NetmodeID.Server && Main.player[playerId].active)
			{
				rocket.GetRocketPlayer(playerId).InRocket = true;
				rocket.GetRocketPlayer(playerId).AsCommander = asCommander;
				rocket.GetRocketPlayer(playerId).RocketID = rocketId;
			}
		}
	}
}
