using Macrocosm.Common.Drawing.Particles;
using Macrocosm.Content.Players;
using Macrocosm.Content.Rockets;
using Macrocosm.Content.Rockets.LaunchPads;
using System.IO;
using Terraria;

namespace Macrocosm.Common.Netcode
{
	public enum MessageType : byte
	{
		SyncParticle,
        SyncRocketData,
		SyncLaunchPadData,
        SyncPlayerRocketStatus,
		SyncPlayerDashDirection,
        SyncModProjectile
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

                case MessageType.SyncRocketData:
                    Rocket.SyncRocketData(reader, whoAmI);
                    break;

				case MessageType.SyncLaunchPadData:
					LaunchPad.SyncLaunchPadData(reader, whoAmI);
					break;

                case MessageType.SyncPlayerRocketStatus:
                    RocketPlayer.ReceiveSyncPlayer(reader, whoAmI);
                    break;

				case MessageType.SyncPlayerDashDirection:
					DashPlayer.ReceiveSyncPlayer(reader, whoAmI);
					break;

                case MessageType.SyncModProjectile:
					SyncModProjectile(reader);
					break;

				default:
					Macrocosm.Instance.Logger.WarnFormat("Macrocosm: Unknown Message type: {messageType}", messageType);
 					break;
			}
		}

		public static void SyncModProjectile(BinaryReader reader)
		{
			ushort whoAmI = reader.ReadUInt16();
			Projectile projectile;
			if (
				Main.projectile.Length < whoAmI 
				&& (projectile = Main.projectile[whoAmI]) is not null 
				&& projectile.ModProjectile is not null
				)
			{
				projectile.ModProjectile.NetReadFields(reader);
			}
		}
	}
}
