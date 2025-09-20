using Macrocosm.Common.Config;
using Macrocosm.Common.DataStructures;
using Macrocosm.Common.Netcode;
using Macrocosm.Common.Utils;
using Macrocosm.Content.Items.Connectors;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameContent.UI;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace Macrocosm.Common.Systems.Connectors;

public partial class ConveyorSystem : ModSystem, IOnPlayerJoining
{
    private static Asset<Texture2D> conveyorTexture;

    private static Dictionary<Point16, ConveyorNode> nodeLookup = new();
    private static readonly List<ConveyorCircuit> circuits = new();
    private static int buildTimer = 0;
    private static int solveTimer = 0;
    private const int solveMax = 60;

    public override void Load()
    {
        conveyorTexture = ModContent.Request<Texture2D>(Macrocosm.TexturesPath + "Conveyors");
        dropperTexture = ModContent.Request<Texture2D>(Macrocosm.TexturesPath + "Dropper");
        hopperTexture = ModContent.Request<Texture2D>(Macrocosm.TexturesPath + "Hopper");
    }

    public override void Unload()
    {
        dropperTexture = null;
        hopperTexture = null;
        attachmentStates.Clear();
    }

    public override void ClearWorld()
    {
        ClearAttachments();
    }

    public static bool ShouldDraw => WiresUI.Settings.DrawWires;
    public static ConveyorVisibility[] Visibility { get; set; } = new ConveyorVisibility[(int)ConveyorPipeType.Count];
    public static ConveyorVisibility InletOutletVisibility { get; set; } = ConveyorVisibility.Normal;

    public static bool PlacePipe(Point targetCoords, ConveyorPipeType type, bool sync = true) => PlacePipe(targetCoords.X, targetCoords.Y, type, sync);
    public static bool PlacePipe(Point16 targetCoords, ConveyorPipeType type, bool sync = true) => PlacePipe(targetCoords.X, targetCoords.Y, type, sync);
    public static bool PlacePipe(int x, int y, ConveyorPipeType type, bool sync = true)
    {
        ref var data = ref Main.tile[x, y].Get<ConveyorData>();
        if (data.HasPipe(type))
            return false;

        data.SetPipe(type);
        if (sync && Main.netMode != NetmodeID.SinglePlayer)
            SyncConveyor(x, y);

        return true;
    }

    public static bool PlaceInlet(Point targetCoords, bool sync = true) => PlaceInlet(targetCoords.X, targetCoords.Y, sync);
    public static bool PlaceInlet(Point16 targetCoords, bool sync = true) => PlaceInlet(targetCoords.X, targetCoords.Y, sync);
    public static bool PlaceInlet(int x, int y, bool sync = true)
    {
        ref var data = ref Main.tile[x, y].Get<ConveyorData>();
        if (data.Inlet)
            return false;

        bool dust = false;
        if (data.Outlet)
        {
            data.Outlet = false;

            dust = true;
            DustEffects(x, y);

            if (Main.netMode != NetmodeID.MultiplayerClient)
                Item.NewItem(new EntitySource_TileBreak(x, x, "Conveyor"), new Vector2(x * 16 + 8, y * 16 + 8), ModContent.ItemType<ConveyorOutlet>());
        }

        data.Inlet = true;
        if (sync && Main.netMode != NetmodeID.SinglePlayer)
            SyncConveyor(x, y, dust);

        return true;
    }

    public static bool PlaceOutlet(Point targetCoords, bool sync = true) => PlaceOutlet(targetCoords.X, targetCoords.Y, sync);
    public static bool PlaceOutlet(Point16 targetCoords, bool sync = true) => PlaceOutlet(targetCoords.X, targetCoords.Y, sync);
    public static bool PlaceOutlet(int x, int y, bool sync = true)
    {
        ref var data = ref Main.tile[x, y].Get<ConveyorData>();
        if (data.Outlet)
            return false;

        bool dust = false;
        if (data.Inlet)
        {
            data.Inlet = false;

            dust = true;
            DustEffects(x, y);

            if (Main.netMode != NetmodeID.MultiplayerClient)
                Item.NewItem(new EntitySource_TileBreak(x, x, "Conveyor"), new Vector2(x * 16 + 8, y * 16 + 8), ModContent.ItemType<ConveyorInlet>());
        }

        data.Outlet = true;
        if (sync && Main.netMode != NetmodeID.SinglePlayer)
            SyncConveyor(x, y, dust);

        return true;
    }

    public static bool Remove(Point targetCoords, bool sync = true) => Remove(targetCoords.X, targetCoords.Y, sync);
    public static bool Remove(Point16 targetCoords, bool sync = true) => Remove(targetCoords.X, targetCoords.Y, sync);
    public static bool Remove(int x, int y, bool sync = true)
    {
        if (TryRemoveDropper(x, y, sync) || TryRemoveHopper(x, y, sync))
            return true;

        bool removed = false;
        int itemDrop = 0;
        ref var data = ref Main.tile[x, y].Get<ConveyorData>();

        // TODO: multiple type removal for Grand Design
        if (data.Inlet)
        {
            itemDrop = ModContent.ItemType<ConveyorInlet>();
            data.Inlet = false;
            removed = true;
        }
        else if (data.Outlet)
        {
            itemDrop = ModContent.ItemType<ConveyorOutlet>();
            data.Outlet = false;
            removed = true;
        }
        else if (data.AnyPipe)
        {
            for (int c = 0; c < (int)ConveyorPipeType.Count; c++)
            {
                var type = (ConveyorPipeType)c;
                if (data.HasPipe(type))
                {
                    data.ClearPipe(type);
                    itemDrop = ModContent.ItemType<Conveyor>();
                    removed = true;
                    break;
                }
            }
        }

        if (removed)
        {
            // Sanity check
            if (!data.AnyPipe)
            {
                data.Inlet = false;
                data.Outlet = false;
            }

            DustEffects(x, y);

            if (itemDrop > 0 && Main.netMode != NetmodeID.MultiplayerClient)
                Item.NewItem(new EntitySource_TileBreak(x, x, "Conveyor"), new Vector2(x * 16 + 8, y * 16 + 8), itemDrop);

            if (sync && Main.netMode != NetmodeID.SinglePlayer)
                SyncConveyor(x, y, dustEffects: true);
        }

        return removed;
    }

    private static void DustEffects(int x, int y)
    {
        int dustType = DustID.Copper;
        if (dustType >= 0)
            for (int i = 0; i < 5; i++)
                Dust.NewDustDirect(new Vector2(x * 16 + 8, y * 16 + 8), 1, 1, dustType);
    }

    public override void PostUpdateWorld()
    {
        UpdateConveyors();
    }

    private static void UpdateConveyors()
    {
        if (++buildTimer >= (int)ServerConfig.Instance.CircuitSolveUpdateRate)
        {
            BuildConveyorCircuits();
            buildTimer = 0;
        }

        if (++solveTimer >= solveMax)
        {
            SolveConveyorCircuits();
            solveTimer = 0;
        }

        UpdateAttachments();
    }

    private static void BuildConveyorCircuits()
    {
        nodeLookup.Clear();
        circuits.Clear();

        GetNodesFor(new ChestConveyorContainerProvider());
        GetNodesFor(new TileEntityInventoryOwnerConveyorContainerProvider());

        foreach (var node in nodeLookup.Values)
        {
            if (node.Circuit != null)
                continue;

            var search = new ConnectionSearch<ConveyorNode>(
                connectionCheck: p => Main.tile[p.X, p.Y].Get<ConveyorData>().HasPipe(node.Type),
                retrieveNode: p => nodeLookup.TryGetValue(p, out var foundNode) && foundNode.Type == node.Type ? foundNode : null
            );

            HashSet<ConveyorNode> connectedNodes = search.FindConnectedNodes(node.ConnectionPositions);
            if (connectedNodes.Count == 0)
                continue;

            HashSet<ConveyorCircuit> existingCircuits = circuits
                .Where(circuit => connectedNodes.Any(node => circuit.Contains(node)))
                .ToHashSet();

            ConveyorCircuit circuit;
            if (existingCircuits.Count > 0)
            {
                circuit = existingCircuits.First();
                foreach (var otherCircuit in existingCircuits.Skip(1))
                {
                    circuit.Merge(otherCircuit);
                }
            }
            else
            {
                circuit = new ConveyorCircuit(node.Type);
            }

            foreach (var n in connectedNodes)
            {
                if (n.Circuit == null)
                {
                    circuit.Add(n);
                    n.Circuit = circuit;
                }
            }

            if (node.Circuit == null)
            {
                circuit.Add(node);
                node.Circuit = circuit;
            }

            if (!circuits.Contains(circuit))
                circuits.Add(circuit);
        }
    }

    private static void GetNodesFor<T>(IConveyorContainerProvider<T> provider) where T : class
    {
        foreach (var container in provider.EnumerateContainers())
        {
            foreach (var node in GetAllConveyorNodes(provider, container))
            {
                if (!nodeLookup.ContainsKey(node.Position))
                    nodeLookup[node.Position] = node;
            }
        }
    }

    private static bool TryGetConveyorNode(Point16 pos, out ConveyorNode node) => nodeLookup.TryGetValue(pos, out node);

    private static IEnumerable<ConveyorNode> GetAllConveyorNodes<T>(IConveyorContainerProvider<T> provider, T container) where T : class
    {
        foreach (var pos in provider.GetConnectionPositions(container))
        {
            for (ConveyorPipeType type = 0; type < ConveyorPipeType.Count; type++)
            {
                var node = provider.GetConveyorNode(pos, type);
                if (node is not null)
                    yield return node;
            }
        }
    }

    private static void SolveConveyorCircuits()
    {
        foreach (var circuit in circuits)
            circuit.Solve(solveMax);
    }

    public override void PostDrawTiles()
    {
        if (!ShouldDraw)
            return;

        SpriteBatch spriteBatch = Main.spriteBatch;
        spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, default, default, null, Main.GameViewMatrix.ZoomMatrix);

        Vector2 zero = Vector2.Zero;
        Point screenOverdrawOffset = Main.GetScreenOverdrawOffset();

        int startX = (int)((Main.screenPosition.X - zero.X) / 16f - 1f);
        int endX = (int)((Main.screenPosition.X + Main.screenWidth + zero.X) / 16f) + 2;
        int startY = (int)((Main.screenPosition.Y - zero.Y) / 16f - 1f);
        int endY = (int)((Main.screenPosition.Y + Main.screenHeight + zero.Y) / 16f) + 5;

        if (startX < 0)
            startX = 0;

        if (endX > Main.maxTilesX)
            endX = Main.maxTilesX;

        if (startY < 0)
            startY = 0;

        if (endY > Main.maxTilesY)
            endY = Main.maxTilesY;

        for (int i = startX + screenOverdrawOffset.X; i < endX - screenOverdrawOffset.X; i++)
        {
            for (int j = startY + screenOverdrawOffset.Y; j < endY - screenOverdrawOffset.Y; j++)
            {
                ref var data = ref Main.tile[i, j].Get<ConveyorData>();

                float visiblePipeCount = 0f;
                for (int t = 0; t < (int)ConveyorPipeType.Count; t++)
                {
                    Rectangle frame = new(0, 0, 16, 16);
                    ConveyorPipeType type = (ConveyorPipeType)t;
                    Color color = GetColor(i, j, Visibility[t]);
                    if (data.HasPipe(type) && color != Color.Transparent)
                    {
                        visiblePipeCount += 1f;
                        float alphaFactor = 1f / visiblePipeCount;
                        color *= alphaFactor;

                        // Type frame
                        frame.Y = 18 * t;

                        // Connection frame
                        if (Main.tile[i, j - 1].Get<ConveyorData>().HasPipe(type))
                            frame.X += 18;

                        if (Main.tile[i + 1, j].Get<ConveyorData>().HasPipe(type))
                            frame.X += 36;

                        if (Main.tile[i, j + 1].Get<ConveyorData>().HasPipe(type))
                            frame.X += 72;

                        if (Main.tile[i - 1, j].Get<ConveyorData>().HasPipe(type))
                            frame.X += 144;

                        spriteBatch.Draw(conveyorTexture.Value, new Vector2(i * 16 - (int)Main.screenPosition.X, j * 16 - (int)Main.screenPosition.Y) + zero, frame, color, 0f, zero, 1f, SpriteEffects.None, 0f);

                        if (InletOutletVisibility != ConveyorVisibility.Hidden && visiblePipeCount > 0)
                        {
                            int blinkFrame = (int)((Main.timeForVisualEffects % 60.0) / 30.0);
                            if (data.Inlet)
                            {
                                frame.Y = ((int)ConveyorPipeType.Count + blinkFrame) * 18;
                                spriteBatch.Draw(conveyorTexture.Value, new Vector2(i * 16 - (int)Main.screenPosition.X, j * 16 - (int)Main.screenPosition.Y) + zero, frame, GetColor(i, j, InletOutletVisibility), 0f, zero, 1f, SpriteEffects.None, 0f);
                            }
                            else if (data.Outlet)
                            {
                                frame.Y = ((int)ConveyorPipeType.Count + 2 + blinkFrame) * 18;
                                spriteBatch.Draw(conveyorTexture.Value, new Vector2(i * 16 - (int)Main.screenPosition.X, j * 16 - (int)Main.screenPosition.Y) + zero, frame, GetColor(i, j, InletOutletVisibility), 0f, zero, 1f, SpriteEffects.None, 0f);
                            }
                        }
                    }
                }

                if (data.Attachment)
                    DrawAttachment(spriteBatch, new Point16(i, j), zero);
            }
        }

        spriteBatch.End();
    }

    private static Color GetColor(int i, int j, ConveyorVisibility visibilty)
    {
        Color color = Lighting.GetColor(i, j);
        return visibilty switch
        {
            ConveyorVisibility.Bright => Color.White,
            ConveyorVisibility.Subtle => color * 0.5f,
            ConveyorVisibility.Hidden => Color.Transparent,
            _ => color
        };
    }

    public static void SyncConveyor(int x, int y, bool dustEffects = false, int toClient = -1, int ignoreClient = -1)
    {
        ModPacket packet = Macrocosm.Instance.GetPacket();

        packet.Write((byte)MessageType.SyncConveyor);
        packet.Write((ushort)x);
        packet.Write((ushort)y);
        packet.Write(Main.tile[x, y].Get<ConveyorData>().Packed);
        packet.Write(dustEffects);
        packet.Send(toClient, ignoreClient);
    }

    public static void ReceiveSyncConveyor(BinaryReader reader, int sender)
    {
        int x = reader.ReadUInt16();
        int y = reader.ReadUInt16();
        ushort data = reader.ReadUInt16();
        bool dustEffects = reader.ReadBoolean();

        ref var localData = ref Main.tile[x, y].Get<ConveyorData>();
        localData = new(data);
        RefreshAttachmentState(new Point16(x, y), localData);

        if (dustEffects)
            DustEffects(x, y);

        if (Main.netMode == NetmodeID.Server)
            SyncConveyor(x, y, dustEffects, ignoreClient: sender);
    }

    public static void SyncConveyorRectangle(int startX, int startY, int width, int height, int toClient = -1, int ignoreClient = -1)
    {
        ModPacket packet = Macrocosm.Instance.GetPacket();

        packet.Write((byte)MessageType.SyncConveyorRectangle);
        packet.Write((ushort)startX);
        packet.Write((ushort)startY);
        packet.Write((ushort)width);
        packet.Write((ushort)height);

        using (MemoryStream memoryStream = new())
        {
            using (BinaryWriter writer = new(new Ionic.Zlib.DeflateStream(memoryStream, Ionic.Zlib.CompressionMode.Compress, true)))
            {
                for (int x = startX; x < startX + width; x++)
                {
                    for (int y = startY; y < startY + height; y++)
                    {
                        writer.Write(Main.tile[x, y].Get<ConveyorData>().Packed);
                    }
                }
            }

            byte[] compressedData = memoryStream.ToArray();
            packet.Write(compressedData.Length);
            packet.Write(compressedData);
        }

        packet.Send(toClient, ignoreClient);
    }

    public static void ReceiveSyncConveyorRectangle(BinaryReader reader, int sender)
    {
        int startX = reader.ReadUInt16();
        int startY = reader.ReadUInt16();
        int width = reader.ReadUInt16();
        int height = reader.ReadUInt16();
        int compressedLength = reader.ReadInt32();
        byte[] compressedData = reader.ReadBytes(compressedLength);

        using (MemoryStream memoryStream = new(compressedData))
        using (BinaryReader compressedReader = new(new Ionic.Zlib.DeflateStream(memoryStream, Ionic.Zlib.CompressionMode.Decompress)))
        {
            for (int x = startX; x < startX + width; x++)
            {
                for (int y = startY; y < startY + height; y++)
                {
                    ushort data = compressedReader.ReadUInt16();
                    ConveyorData newData = new(data);
                    Main.tile[x, y].Get<ConveyorData>() = newData;
                    RefreshAttachmentState(new Point16(x, y), newData);
                }
            }
        }

        if (Main.netMode == NetmodeID.Server)
            SyncConveyorRectangle(startX, startY, width, height, ignoreClient: sender);
    }

    public void OnPlayerJoining(int playerIndex)
    {
        SyncConveyorRectangle(0, 0, Main.maxTilesX, Main.maxTilesY, toClient: playerIndex);
    }

    public override void SaveWorldData(TagCompound tag)
    {
        ConveyorData[] data = Main.tile.GetData<ConveyorData>();
        ReadOnlySpan<ConveyorData> span = data;
        byte[] bytes = MemoryMarshal.AsBytes(span).ToArray();
        tag.Add(nameof(ConveyorData), bytes);
    }

    public override void LoadWorldData(TagCompound tag)
    {
        ConveyorData[] data = Main.tile.GetData<ConveyorData>();
        byte[] bytes = tag.GetByteArray(nameof(ConveyorData));
        int expectedSize = data.Length * Marshal.SizeOf<ConveyorData>();
        if (bytes.Length != expectedSize)
        {
            Mod.Logger.Error($"Failed to load conveyor tile data: expected {expectedSize}, got {bytes.Length}.", new SerializationException());
            return;
        }
        bytes.CopyTo(MemoryMarshal.AsBytes(data.AsSpan()));
        RebuildAttachmentStateCache();
    }
}
