﻿using Macrocosm.Common.Netcode;
using Macrocosm.Common.Subworlds;
using Macrocosm.Common.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SubworldLibrary;
using System;
using System.IO;
using System.Net.Sockets;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace Macrocosm.Content.Rockets.LaunchPads
{
    public partial class LaunchPad
    {
        /// <summary>
        /// Syncs the launchpad fields with <see cref="NetSyncAttribute"/> across all clients and the server.
        /// </summary>
        public void NetSync(string subworldId, int toClient = -1, int ignoreClient = -1)
        {
            if (Main.netMode == NetmodeID.SinglePlayer)
                return;

            ModPacket packet = Macrocosm.Instance.GetPacket();
            packet.Write((byte)MessageType.SyncLaunchPadData);
            packet.Write(subworldId);

            if (this.NetWriteFields(packet))
                packet.Send(toClient, ignoreClient);

            packet.Dispose();
        }

        /// <summary>
        /// Syncs a rocket with data from the <see cref="BinaryReader"/>. Don't use this method outside <see cref="PacketHandler.HandlePacket(BinaryReader, int)"/>
        /// </summary>
        /// <param name="reader"></param>
        public static void ReceiveSyncLaunchPadData(BinaryReader reader, int sender)
        {
            string subworldId = reader.ReadString();

            LaunchPad launchPad = new();
            launchPad.NetReadFields(reader);

            LaunchPad existingLaunchPad = LaunchPadManager.GetLaunchPadAtStartTile(subworldId, launchPad.StartTile);
            if (existingLaunchPad is null)
                LaunchPadManager.Add(subworldId, launchPad);

            if (Main.netMode == NetmodeID.Server)
            {
                launchPad.NetSync(subworldId, ignoreClient: sender);

                var packet = new BinaryWriter(new MemoryStream(256));
                packet.Write((byte)MessageType.SyncLaunchPadData);
                packet.Write(subworldId);
                launchPad.NetWriteFields(packet);

                if(subworldId == MacrocosmSubworld.CurrentID)
                {
                    if (SubworldSystem.AnyActive())
                        SubworldSystem.SendToMainServer(Macrocosm.Instance, (packet.BaseStream as MemoryStream).GetBuffer());
                    else
                        SubworldSystem.SendToAllSubservers(Macrocosm.Instance, (packet.BaseStream as MemoryStream).GetBuffer());
                }
            }
        }
    }
}
