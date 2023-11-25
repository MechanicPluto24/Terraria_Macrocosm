using Macrocosm.Common.Drawing.Particles;
using Macrocosm.Content.Players;
using Macrocosm.Content.Rockets;
using Macrocosm.Content.Rockets.LaunchPads;
using Macrocosm.Content.Rockets.Storage;
using System.IO;
using Terraria;

namespace Macrocosm.Common.Netcode
{
    public enum MessageType : byte
    {
        SyncParticle,
        SyncRocketData,
        SyncLaunchPadData,
        SyncInventory,
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
                    Particle.ReceiveSyncParticle(reader, whoAmI);
                    break;

                case MessageType.SyncRocketData:
                    Rocket.ReceiveSyncRocketData(reader, whoAmI);
                    break;

                case MessageType.SyncInventory:
                    Inventory.HandlePacket(reader, whoAmI);
                    break;

                case MessageType.SyncLaunchPadData:
                    LaunchPad.ReceiveSyncLaunchPadData(reader, whoAmI);
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
                    Macrocosm.Instance.Logger.WarnFormat("Macrocosm: Unknown Message type: {0}", messageType);
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
