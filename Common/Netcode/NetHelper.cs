using Macrocosm.Common.Subworlds;
using Microsoft.Xna.Framework;
using SubworldLibrary;
using System.IO;
using Terraria;
using Terraria.Chat;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Macrocosm.Common.Netcode
{
    public static class NetHelper
    {
        /// <summary>
        /// Gets a network identifier of the current server (main server and subservers)
        /// <br/> Applies to both the server itself and clients currently on this server
        /// </summary>
        /// <returns>
        /// <br/> -1 for the main server
        /// <br/> 0+ for subservers
        /// </returns>
        public static int GetServerIndex()
        {
            if (!SubworldSystem.AnyActive())
                return -1;

            return SubworldSystem.GetIndex(SubworldSystem.Current.FullName);
        }

        /// <summary>
        /// Send a <see cref="ModPacket"/> to other servers running.
        /// <br/> If on the main server (not a subworld), the packet is sent to all the SubworldLibrary subservers, except <paramref name="ignoreSubserver"/>
        /// <br/> If on a subserver, the packet is sent to the main server. The main server has the responsibility to relay it to the subservers, except the original sender.
        /// </summary>
        /// <param name="mod"> The mod the packet belongs to </param>
        /// <param name="packet"> The packet to relay </param>
        /// <param name="ignoreSubserver"> The subserver to ignore, if applicable </param>
        public static void RelayToServers(this ModPacket packet, Mod mod, int ignoreSubserver = -1)
        {
            if (Main.netMode != NetmodeID.Server)
                return;

            byte[] data = packet.GetPayload();

            if (SubworldSystem.AnyActive())
                SubworldSystem.SendToMainServer(mod, data);
            else if (ignoreSubserver == -1)
                SubworldSystem.SendToAllSubservers(mod, data);
            else
                MacrocosmSubworld.Hacks.SubworldSystem_SendToAllSubserversExcept(mod, data, ignoreSubserver);
        }

        /// <summary>
        /// This method converts a mod packet to a Subworld-style payload (byte[]), trimming the entire ModPacket header.
        /// <br/> SubworldLibrary handles the packet header on its own, requires only the custom payload as a byte[].
        /// <br/> ModPacket structure:
        ///  <br/> - Length (ushort, 2 bytes, before <see cref="ModPacket.Send(int, int)"/>, it's a placeholder (ushort)0)
        ///  <br/> - <see cref="MessageID.ModPacket"/> (250, 1 byte)
        ///  <br/> - <see cref="Mod.NetID"/> (short, 2 bytes, if <see cref="ModNet.NetModCount"/> > 255, else 1 byte)
        ///  <br/> - <b>The payload</b> 
        /// </summary>
        /// <returns> The payload as a byte array </returns>
        public static byte[] GetPayload(this ModPacket packet)
        {
            // Length + MessageID + Mod.NetID
            int headerSize = sizeof(ushort) + sizeof(byte) + (ModNet.NetModCount > 255 ? sizeof(short) : sizeof(byte));

            int payloadSize = (int)packet.BaseStream.Length - headerSize;

            if (payloadSize <= 0)
                return [];

            byte[] trimmedBuffer = new byte[payloadSize];
            packet.BaseStream.Seek(headerSize, SeekOrigin.Begin);
            packet.BaseStream.Read(trimmedBuffer, 0, payloadSize);

            return trimmedBuffer;
        }

        public static void SpawnNPCFromClient(int netID, Vector2 position, float ai0 = 0f, float ai1 = 0f, float ai2 = 0f, float ai3 = 0f, Vector2 velocity = default, int target = 0, bool announce = false)
        {
            var packet = Macrocosm.Instance.GetPacket();
            packet.Write((byte)MessageType.SyncNPCFromClient);

            packet.WriteVector2(position);
            packet.WriteVector2(velocity);

            packet.Write(ai0);
            packet.Write(ai1);
            packet.Write(ai2);
            packet.Write(ai3);

            packet.Write((ushort)target);

            packet.Write(announce);

            packet.Send();
        }

        public static void ReceiveSpawnNPCFromClient(BinaryReader reader, int sender)
        {
            if (Main.netMode == NetmodeID.Server)
            {
                int netID = reader.ReadInt16();
                float positionX = reader.ReadSingle();
                float positionY = reader.ReadSingle();
                float ai0 = reader.ReadSingle();
                float ai1 = reader.ReadSingle();
                float ai2 = reader.ReadSingle();
                float ai3 = reader.ReadSingle();
                Vector2 velocity = reader.ReadVector2();
                int target = reader.ReadUInt16();
                bool announce = reader.ReadBoolean();

                int whoAmI = Terraria.NPC.NewNPC(Entity.GetSource_NaturalSpawn(), (int)positionX, (int)positionY, netID, 0, ai0, ai1, ai2, ai3, target);
                Main.npc[whoAmI].velocity = velocity;

                if (announce)
                    ChatHelper.BroadcastChatMessage(NetworkText.FromKey("Announcement.HasAwoken", Main.npc[whoAmI].GetTypeNetName()), new Color(175, 75, 255));

                // Includes sender
                NetMessage.SendData(MessageID.SyncNPC, number: whoAmI);
            }
        }

        public static void SyncNPCFromClient(int whoAmI)
        {
            var packet = Macrocosm.Instance.GetPacket();
            packet.Write((byte)MessageType.SyncNPCFromClient);

            Terraria.NPC npc = Main.npc[whoAmI];
            packet.Write((short)whoAmI);
            packet.WriteVector2(npc.position);
            packet.WriteVector2(npc.velocity);
            packet.Write((ushort)npc.target);
            int life = npc.life;
            if (!npc.active)
                life = 0;

            if (!npc.active || npc.life <= 0)
                npc.netSkip = 0;

            short netID = (short)npc.netID;
            bool[] array = new bool[4];
            BitsByte bb1 = new();
            bb1[0] = npc.direction > 0;
            bb1[1] = npc.directionY > 0;
            bb1[2] = (array[0] = npc.ai[0] != 0f);
            bb1[3] = (array[1] = npc.ai[1] != 0f);
            bb1[4] = (array[2] = npc.ai[2] != 0f);
            bb1[5] = (array[3] = npc.ai[3] != 0f);
            bb1[6] = npc.spriteDirection > 0;
            bb1[7] = life == npc.lifeMax;
            packet.Write(bb1);
            BitsByte bb2 = new();
            bb2[0] = npc.statsAreScaledForThisManyPlayers > 1;
            bb2[1] = npc.SpawnedFromStatue;
            bb2[2] = npc.strengthMultiplier != 1f;
            packet.Write(bb2);
            for (int i = 0; i < Terraria.NPC.maxAI; i++)
            {
                if (array[i])
                    packet.Write(npc.ai[i]);
            }

            packet.Write(netID);
            if (bb2[0])
                packet.Write((byte)npc.statsAreScaledForThisManyPlayers);

            if (bb2[2])
                packet.Write(npc.strengthMultiplier);

            if (!bb1[7])
            {
                byte b2 = 1;
                if (npc.lifeMax > 32767)
                    b2 = 4;
                else if (npc.lifeMax > 127)
                    b2 = 2;

                packet.Write(b2);
                switch (b2)
                {
                    case 2:
                        packet.Write((short)life);
                        break;
                    case 4:
                        packet.Write(life);
                        break;
                    default:
                        packet.Write((sbyte)life);
                        break;
                }
            }

            if (npc.type >= NPCID.None && Main.npcCatchable[npc.type])
                packet.Write((byte)npc.releaseOwner);

            NPCLoader.SendExtraAI(packet, NPCLoader.WriteExtraAI(npc));

            packet.Send();
        }
        public static void ReceiveSyncNPCFromClient(BinaryReader reader, int sender)
        {
            if (Main.netMode == NetmodeID.Server)
            {
                int npcWhoAmI = reader.ReadInt16();
                Vector2 position = reader.ReadVector2();
                Vector2 velocity = reader.ReadVector2();
                int target = reader.ReadUInt16();
                if (target == ushort.MaxValue)
                    target = 0;

                BitsByte bb1 = reader.ReadByte();
                BitsByte bb2 = reader.ReadByte();
                float[] ai = new float[Terraria.NPC.maxAI];
                for (int i = 0; i < Terraria.NPC.maxAI; i++)
                {
                    if (bb1[i + 2])
                        ai[i] = reader.ReadSingle();
                    else
                        ai[i] = 0f;
                }

                int netID = reader.ReadInt16();
                int? playerCountForMultiplayerDifficultyOverride = 1;
                if (bb2[0])
                    playerCountForMultiplayerDifficultyOverride = reader.ReadByte();

                float strengthMultiplierOverride = 1f;
                if (bb2[2])
                    strengthMultiplierOverride = reader.ReadSingle();

                int life = 0;
                if (!bb1[7])
                {
                    life = reader.ReadByte() switch
                    {
                        2 => reader.ReadInt16(),
                        4 => reader.ReadInt32(),
                        _ => reader.ReadSByte(),
                    };
                }

                int type = -1;
                Terraria.NPC npc = Main.npc[npcWhoAmI];
                if (npc.active && Main.multiplayerNPCSmoothingRange > 0 && Vector2.DistanceSquared(npc.position, position) < 640000f)
                    npc.netOffset += npc.position - position;

                if (!npc.active || npc.netID != netID)
                {
                    npc.netOffset *= 0f;
                    if (npc.active)
                        type = npc.type;

                    npc.active = true;
                    npc.SetDefaults(netID, new NPCSpawnParams
                    {
                        playerCountForMultiplayerDifficultyOverride = playerCountForMultiplayerDifficultyOverride,
                        strengthMultiplierOverride = strengthMultiplierOverride
                    });
                }

                npc.position = position;
                npc.velocity = velocity;
                npc.target = target;
                npc.direction = (bb1[0] ? 1 : (-1));
                npc.directionY = (bb1[1] ? 1 : (-1));
                npc.spriteDirection = (bb1[6] ? 1 : (-1));
                if (bb1[7])
                    life = (npc.life = npc.lifeMax);
                else
                    npc.life = life;

                if (life <= 0)
                    npc.active = false;

                npc.SpawnedFromStatue = bb2[1];
                if (npc.SpawnedFromStatue)
                    npc.value = 0f;

                for (int i = 0; i < Terraria.NPC.maxAI; i++)
                    npc.ai[i] = ai[i];

                if (type > -1 && type != npc.type)
                    npc.TransformVisuals(type, npc.type);

                if (netID == NPCID.Plantera)
                    Terraria.NPC.plantBoss = npcWhoAmI;

                if (netID == NPCID.Golem)
                    Terraria.NPC.golemBoss = npcWhoAmI;

                if (netID == NPCID.Deerclops)
                    Terraria.NPC.deerclopsBoss = npcWhoAmI;

                if (npc.type >= NPCID.None && Main.npcCatchable[npc.type])
                    npc.releaseOwner = reader.ReadByte();

                NPCLoader.ReceiveExtraAI(npc, NPCLoader.ReadExtraAI(reader));

                NetMessage.SendData(MessageID.SyncNPC, ignoreClient: sender, number: npc.whoAmI);
            }
        }

        public static void SyncTEFromClient(int id)
        {
            if (Main.netMode != NetmodeID.MultiplayerClient)
                return;

            ModPacket packet = Macrocosm.Instance.GetPacket();
            packet.Write((byte)MessageType.SyncTEFromClient);
            packet.Write(id);
            bool found = TileEntity.ByID.ContainsKey(id);
            packet.Write(found);
            if (found)
                TileEntity.Write(packet, TileEntity.ByID[id], networkSend: true, lightSend: true);

            packet.Send();
        }

        public static void ReceiveSyncTEFromClient(BinaryReader reader, int sender)
        {
            if (Main.netMode != NetmodeID.Server)
                return;

            int id = reader.ReadInt32();
            if (!reader.ReadBoolean())
            {
                if (TileEntity.ByID.TryGetValue(id, out var tileEntity))
                {
                    TileEntity.ByID.Remove(id);
                    Point16 position = tileEntity.Position;
                    TileEntity.ByPosition.Remove(position);

                    // Sync removal to the other clients
                    NetMessage.SendData(MessageID.TileEntitySharing, ignoreClient: sender, number: id, number2: position.X, number3: position.Y);
                }
            }
            else
            {
                TileEntity tileEntity = TileEntity.Read(reader, networkSend: true, lightSend: true);
                tileEntity.ID = id;
                TileEntity.ByID[tileEntity.ID] = tileEntity;
                TileEntity.ByPosition[tileEntity.Position] = tileEntity;

                // Sync state to the other clients
                NetMessage.SendData(MessageID.TileEntitySharing, ignoreClient: sender, number: tileEntity.ID, number2: tileEntity.Position.X, number3: tileEntity.Position.Y);
            }
        }
    }
}
