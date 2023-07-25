using log4net.Repository.Hierarchy;
using Macrocosm.Common.Drawing.Particles;
using Macrocosm.Common.Utils;
using Macrocosm.Content.Players;
using Macrocosm.Content.Rockets;
using System.IO;

namespace Macrocosm.Common.Netcode
{
	public enum MessageType : byte
	{
		SyncParticle,
		SyncPlayerDashDirection,
		SyncRocketData,
		SyncPlayerRocketStatus
	}

	public class PacketHandler
	{
		public static void HandlePacket(BinaryReader reader, int whoAmI)
		{
			MessageType messageType = (MessageType)reader.ReadByte();

			switch (messageType)
			{
				case MessageType.SyncParticle:
					Particle.SyncParticle(reader, whoAmI);
					break;

				case MessageType.SyncPlayerDashDirection:
					DashPlayer.ReceiveSyncPlayer(reader, whoAmI);
					break;

				case MessageType.SyncPlayerRocketStatus:
					RocketPlayer.ReceiveSyncPlayer(reader, whoAmI);
					break;

				case MessageType.SyncRocketData:
					Rocket.SyncRocketData(reader, whoAmI);
					break;

				default:
					Macrocosm.Instance.Logger.WarnFormat("Macrocosm: Unknown Message type: {messageType}", messageType);
 					break;
			}
		}
	}
}
