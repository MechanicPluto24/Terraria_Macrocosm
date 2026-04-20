using Macrocosm.Common.Netcode;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Rockets.LaunchPads;

public partial class LaunchPad
{
    /// <summary>
    /// Syncs the launchpad fields with <see cref="NetSyncAttribute"/> across all clients and the server.
    /// </summary>
    public void NetSync(string subworldId, int toClient = -1, int ignoreClient = -1, int ignoreSubserver = -1)
    {
        if (Main.netMode == NetmodeID.SinglePlayer)
            return;

        ModPacket packet = Macrocosm.Instance.GetPacket();
        packet.Write((byte)MessageType.SyncLaunchPadData);
        packet.Write((short)NetHelper.GetServerIndex());
        packet.Write(subworldId);
        internalRocket.NetWrite(packet);

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
    public static void ReceiveSyncLaunchPadData(BinaryReader reader, int sender)
    {
        int senderSubserver = reader.ReadInt16();
        string subworldId = reader.ReadString();

        LaunchPad syncedLaunchPad = new();
        syncedLaunchPad.internalRocket.NetRead(reader);
        syncedLaunchPad.NetRead(reader);

        LaunchPad existingLaunchPad = LaunchPadManager.GetLaunchPadAtStartTile(subworldId, syncedLaunchPad.StartTile);
        if (existingLaunchPad is null)
        {
            LaunchPadManager.Add(subworldId, syncedLaunchPad, notify: true);
        }
        else
        {
            existingLaunchPad.internalRocket = syncedLaunchPad.internalRocket;
            existingLaunchPad.Active = syncedLaunchPad.Active;
            existingLaunchPad.StartTile = syncedLaunchPad.StartTile;
            existingLaunchPad.EndTile = syncedLaunchPad.EndTile;
            existingLaunchPad.RocketID = syncedLaunchPad.RocketID;
            existingLaunchPad.ReservedRocketID = syncedLaunchPad.ReservedRocketID;
            existingLaunchPad.CustomName = syncedLaunchPad.CustomName;
        }

        if (Main.netMode == NetmodeID.Server)
            syncedLaunchPad.NetSync(subworldId, ignoreClient: sender, ignoreSubserver: senderSubserver);
    }
}
