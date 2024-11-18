using Macrocosm.Common.Drawing.Particles;
using Macrocosm.Common.Players;
using Macrocosm.Common.Storage;
using Macrocosm.Common.Subworlds;
using Macrocosm.Common.Systems.Connectors;
using Macrocosm.Content.Rockets;
using Macrocosm.Content.Rockets.LaunchPads;
using System;
using System.IO;
using Terraria;
using Terraria.ID;

namespace Macrocosm.Common.Netcode
{
    public enum MessageType : byte
    {
        SyncParticle,
        SyncRocketData,
        SyncRocketCustomizationData,
        SyncLaunchPadData,
        SyncInventory,
        SyncRocketPlayer,
        SyncDashPlayer,
        SyncMacrocosmPlayer,
        RequestLastSubworld,
        LastSubworldCheck,
        SpawnNPCFromClient,
        SyncNPCFromClient,
        SyncTEFromClient,
        SyncPowerWire
    }

    public class PacketHandler
    {
        public static void HandlePacket(BinaryReader reader, int whoAmI)
        {
            MessageType messageType = (MessageType)reader.ReadByte();

            if(DebugModeActive)
                DebugPackets(messageType, whoAmI);

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

                case MessageType.SyncRocketPlayer:
                    RocketPlayer.ReceiveSyncPlayer(reader, whoAmI);
                    break;

                case MessageType.SyncDashPlayer:
                    DashPlayer.ReceiveSyncPlayer(reader, whoAmI);
                    break;

                case MessageType.SyncMacrocosmPlayer:
                    MacrocosmPlayer.ReceiveSyncPlayer(reader, whoAmI);
                    break;

                case MessageType.LastSubworldCheck:
                    SubworldTravelPlayer.ReceiveLastSubworldCheck(reader, whoAmI);
                    break;

                case MessageType.SpawnNPCFromClient:
                    NetHelper.ReceiveSpawnNPCFromClient(reader, whoAmI);
                    break;

                case MessageType.SyncNPCFromClient:
                    NetHelper.ReceiveSyncNPCFromClient(reader, whoAmI);
                    break;

                case MessageType.SyncTEFromClient:
                    NetHelper.ReceiveSyncTEFromClient(reader, whoAmI);
                    break;

                case MessageType.SyncPowerWire:
                    ConnectorSystem.ReceiveSyncConnector(reader, whoAmI);
                    break;

                default:
                    Macrocosm.Instance.Logger.WarnFormat("Macrocosm: Unknown Message type: {0}", messageType);
                    break;
            }
        }

        public static bool DebugModeActive { get; set; } = false;

        private static void DebugPackets(MessageType messageType, int sender)
        {
            string message = $"Received message of type {messageType} from {sender}";

            if (Main.netMode == NetmodeID.MultiplayerClient)
            {
                Main.NewText(message);
                Macrocosm.Instance.Logger.Info(message);
            }

            if (Main.netMode == NetmodeID.Server)
            {
                if (Main.dedServ) Console.WriteLine(message);
                Macrocosm.Instance.Logger.Info(message);
            }
        }
    }
}
