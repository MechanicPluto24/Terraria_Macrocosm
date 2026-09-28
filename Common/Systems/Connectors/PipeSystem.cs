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
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameContent.UI;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace Macrocosm.Common.Systems.Connectors;

[LegacyName("ConveyorSystem")]
public partial class PipeSystem : ModSystem, IOnPlayerJoining
{

    private static Asset<Texture2D> pipeTexture;

    private static Dictionary<Point16, PipeNode> nodeLookup = new();
    private static readonly List<PipeCircuit> circuits = new();
    private static int buildTimer = 0;
    private static int solveTimer = 0;
    private const int solveMax = 60;

    public override void Load()
    {
        pipeTexture = ModContent.Request<Texture2D>(Macrocosm.TexturesPath + "Pipes");
        attachmentTexture = ModContent.Request<Texture2D>(Macrocosm.TexturesPath + "PipeAttachments");
        On_Main.DrawWires += On_Main_DrawWires;
    }

    public override void Unload()
    {
        On_Main.DrawWires -= On_Main_DrawWires;
        attachmentTexture = null;
        attachmentStates.Clear();
    }

    public override void ClearWorld()
    {
        ClearAttachments();
    }

    public static bool ShouldDraw => WiresUI.Settings.DrawWires;
    public static PipeVisibility[] Visibility { get; set; } = new PipeVisibility[(int)PipeType.Count];
    public static PipeVisibility InletOutletVisibility { get; set; } = PipeVisibility.Normal;

    public static bool PlacePipe(Point targetCoords, PipeType type, bool sync = true) => PlacePipe(targetCoords.X, targetCoords.Y, type, sync);
    public static bool PlacePipe(Point16 targetCoords, PipeType type, bool sync = true) => PlacePipe(targetCoords.X, targetCoords.Y, type, sync);
    public static bool PlacePipe(int x, int y, PipeType type, bool sync = true)
    {
        ref var data = ref Main.tile[x, y].Get<PipeData>();
        if (data.HasPipe(type))
            return false;

        data.SetPipe(type);
        PlayPlaceSound(x, y);

        if (sync && Main.netMode != NetmodeID.SinglePlayer)
            SyncPipe(x, y);

        return true;
    }

    public static bool PlaceInlet(Point targetCoords, bool sync = true) => PlaceInlet(targetCoords.X, targetCoords.Y, sync);
    public static bool PlaceInlet(Point16 targetCoords, bool sync = true) => PlaceInlet(targetCoords.X, targetCoords.Y, sync);
    public static bool PlaceInlet(int x, int y, bool sync = true)
    {
        ref var data = ref Main.tile[x, y].Get<PipeData>();
        if (data.Inlet)
            return false;

        bool dust = false;
        if (data.Outlet)
        {
            data.Outlet = false;

            dust = true;
            DustEffects(x, y);

            if (Main.netMode != NetmodeID.MultiplayerClient)
                Item.NewItem(new EntitySource_TileBreak(x, x, "Pipe"), new Vector2(x * 16 + 8, y * 16 + 8), ModContent.ItemType<PipeOutlet>());
        }

        data.Inlet = true;
        PlayPlaceSound(x, y);

        if (sync && Main.netMode != NetmodeID.SinglePlayer)
            SyncPipe(x, y, dust);

        return true;
    }

    public static bool PlaceOutlet(Point targetCoords, bool sync = true) => PlaceOutlet(targetCoords.X, targetCoords.Y, sync);
    public static bool PlaceOutlet(Point16 targetCoords, bool sync = true) => PlaceOutlet(targetCoords.X, targetCoords.Y, sync);
    public static bool PlaceOutlet(int x, int y, bool sync = true)
    {
        ref var data = ref Main.tile[x, y].Get<PipeData>();
        if (data.Outlet)
            return false;

        bool dust = false;
        if (data.Inlet)
        {
            data.Inlet = false;

            dust = true;
            DustEffects(x, y);

            if (Main.netMode != NetmodeID.MultiplayerClient)
                Item.NewItem(new EntitySource_TileBreak(x, x, "Pipe"), new Vector2(x * 16 + 8, y * 16 + 8), ModContent.ItemType<PipeInlet>());
        }

        data.Outlet = true;
        PlayPlaceSound(x, y);

        if (sync && Main.netMode != NetmodeID.SinglePlayer)
            SyncPipe(x, y, dust);

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
        ref var data = ref Main.tile[x, y].Get<PipeData>();

        // TODO: multiple type removal for Grand Design
        if (data.Inlet)
        {
            itemDrop = ModContent.ItemType<PipeInlet>();
            data.Inlet = false;
            removed = true;
        }
        else if (data.Outlet)
        {
            itemDrop = ModContent.ItemType<PipeOutlet>();
            data.Outlet = false;
            removed = true;
        }
        else if (data.AnyPipe)
        {
            for (int c = 0; c < (int)PipeType.Count; c++)
            {
                var type = (PipeType)c;
                if (data.HasPipe(type))
                {
                    data.ClearPipe(type);
                    itemDrop = ModContent.ItemType<Pipe>();
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
            PlayRemoveSound(x, y);

            if (itemDrop > 0 && Main.netMode != NetmodeID.MultiplayerClient)
                Item.NewItem(new EntitySource_TileBreak(x, x, "Pipe"), new Vector2(x * 16 + 8, y * 16 + 8), itemDrop);

            if (sync && Main.netMode != NetmodeID.SinglePlayer)
                SyncPipe(x, y, dustEffects: true);
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

    private static void PlayPlaceSound(int x, int y)
    {
        SoundEngine.PlaySound(SoundID.Mech with { Volume = 0.7f, Pitch = 0.1f }, new Vector2(x * 16 + 8, y * 16 + 8));
    }

    private static void PlayRemoveSound(int x, int y)
    {
        SoundEngine.PlaySound(SoundID.Dig with { Volume = 0.6f, Pitch = 0.2f }, new Vector2(x * 16 + 8, y * 16 + 8));
    }

    public override void PostUpdateWorld()
    {
        UpdatePipes();
    }

    private static void UpdatePipes()
    {
        if (++buildTimer >= (int)ServerConfig.Instance.CircuitSolveUpdateRate)
        {
            BuildPipeCircuits();
            buildTimer = 0;
        }

        if (++solveTimer >= solveMax)
        {
            SolvePipeCircuits();
            solveTimer = 0;
        }

        UpdateAttachments();
    }

    private static void BuildPipeCircuits()
    {
        nodeLookup.Clear();
        circuits.Clear();

        GetNodesFor(new ChestPipeContainerProvider());
        GetNodesFor(new TileEntityInventoryOwnerPipeContainerProvider());

        foreach (var node in nodeLookup.Values)
        {
            if (node.Circuit != null)
                continue;

            var search = new ConnectionSearch<PipeNode>(
                connectionCheck: p => Main.tile[p.X, p.Y].Get<PipeData>().HasPipe(node.Type),
                retrieveNode: p => nodeLookup.TryGetValue(p, out var foundNode) && foundNode.Type == node.Type ? foundNode : null
            );

            HashSet<PipeNode> connectedNodes = search.FindConnectedNodes(node.ConnectionPositions);
            if (connectedNodes.Count == 0)
                continue;

            HashSet<PipeCircuit> existingCircuits = circuits
                .Where(circuit => connectedNodes.Any(node => circuit.Contains(node)))
                .ToHashSet();

            PipeCircuit circuit;
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
                circuit = new PipeCircuit(node.Type);
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

    private static void GetNodesFor<T>(IPipeContainerProvider<T> provider) where T : class
    {
        foreach (var container in provider.EnumerateContainers())
        {
            foreach (var node in GetAllPipeNodes(provider, container))
            {
                if (!nodeLookup.ContainsKey(node.Position))
                    nodeLookup[node.Position] = node;
            }
        }
    }

    private static bool TryGetPipeNode(Point16 pos, out PipeNode node) => nodeLookup.TryGetValue(pos, out node);

    private static IEnumerable<PipeNode> GetAllPipeNodes<T>(IPipeContainerProvider<T> provider, T container) where T : class
    {
        foreach (var pos in provider.GetConnectionPositions(container))
        {
            for (PipeType type = 0; type < PipeType.Count; type++)
            {
                var node = provider.GetPipeNode(pos, type);
                if (node is not null)
                    yield return node;
            }
        }
    }

    private static void SolvePipeCircuits()
    {
        foreach (var circuit in circuits)
            circuit.Solve(solveMax);
    }

    private void On_Main_DrawWires(On_Main.orig_DrawWires orig, Main self)
    {
        orig(self);
        DrawPipes(Main.spriteBatch);
    }

    private static void DrawPipes(SpriteBatch spriteBatch)
    {
        if (!ShouldDraw)
            return;

        Vector2 zero = Vector2.Zero;
        Vector2 zero2 = Vector2.Zero;
        if (Main.drawToScreen)
            zero2 = Vector2.Zero;

        Point screenOverdrawOffset = Main.GetScreenOverdrawOffset();

        int startX = (int)((Main.screenPosition.X - zero2.X) / 16f - 1f);
        int endX = (int)((Main.screenPosition.X + Main.screenWidth + zero2.X) / 16f) + 2;
        int startY = (int)((Main.screenPosition.Y - zero2.Y) / 16f - 1f);
        int endY = (int)((Main.screenPosition.Y + Main.screenHeight + zero2.Y) / 16f) + 5;

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
                ref var data = ref Main.tile[i, j].Get<PipeData>();

                Vector2 position = new Vector2(i * 16 - (int)Main.screenPosition.X, j * 16 - (int)Main.screenPosition.Y) + zero2;
                float visiblePipeCount = 0f;
                for (int t = 0; t < (int)PipeType.Count; t++)
                {
                    Rectangle frame = new(0, 0, 16, 16);
                    PipeType type = (PipeType)t;
                    Color color = GetColor(i, j, Visibility[t]);
                    if (data.HasPipe(type) && color != Color.Transparent)
                    {
                        visiblePipeCount += 1f;
                        float alphaFactor = 1f / visiblePipeCount;
                        color *= alphaFactor;

                        // Type frame
                        frame.Y = 18 * t;

                        // Connection frame
                        if (Main.tile[i, j - 1].Get<PipeData>().HasPipe(type))
                            frame.X += 18;

                        if (Main.tile[i + 1, j].Get<PipeData>().HasPipe(type))
                            frame.X += 36;

                        if (Main.tile[i, j + 1].Get<PipeData>().HasPipe(type))
                            frame.X += 72;

                        if (Main.tile[i - 1, j].Get<PipeData>().HasPipe(type))
                            frame.X += 144;

                        spriteBatch.Draw(pipeTexture.Value, position, frame, color, 0f, zero, 1f, SpriteEffects.None, 0f);

                        if (InletOutletVisibility != PipeVisibility.Hidden && visiblePipeCount > 0)
                        {
                            int blinkFrame = (int)((Main.timeForVisualEffects % 60.0) / 30.0);
                            if (data.Inlet)
                            {
                                frame.Y = ((int)PipeType.Count + blinkFrame) * 18;
                                spriteBatch.Draw(pipeTexture.Value, position, frame, GetColor(i, j, InletOutletVisibility), 0f, zero, 1f, SpriteEffects.None, 0f);
                            }
                            else if (data.Outlet)
                            {
                                frame.Y = ((int)PipeType.Count + 2 + blinkFrame) * 18;
                                spriteBatch.Draw(pipeTexture.Value, position, frame, GetColor(i, j, InletOutletVisibility), 0f, zero, 1f, SpriteEffects.None, 0f);
                            }
                        }
                    }
                }

                if (data.Attachment)
                    DrawAttachment(spriteBatch, new Point16(i, j), zero2);
            }
        }
    }

    private static Color GetColor(int i, int j, PipeVisibility visibilty)
    {
        Color color = Lighting.GetColor(i, j);
        return visibilty switch
        {
            PipeVisibility.Bright => Color.White,
            PipeVisibility.Subtle => color * 0.5f,
            PipeVisibility.Hidden => Color.Transparent,
            _ => color
        };
    }

    public static void SyncPipe(int x, int y, bool dustEffects = false, int toClient = -1, int ignoreClient = -1)
    {
        ModPacket packet = Macrocosm.Instance.GetPacket();

        packet.Write((byte)MessageType.SyncPipe);
        packet.Write((ushort)x);
        packet.Write((ushort)y);
        packet.Write(Main.tile[x, y].Get<PipeData>().Packed);
        packet.Write(dustEffects);
        packet.Send(toClient, ignoreClient);
    }

    public static void ReceiveSyncPipe(BinaryReader reader, int sender)
    {
        int x = reader.ReadUInt16();
        int y = reader.ReadUInt16();
        ushort data = reader.ReadUInt16();
        bool dustEffects = reader.ReadBoolean();

        ref var localData = ref Main.tile[x, y].Get<PipeData>();
        localData = new(data);
        RefreshAttachmentState(new Point16(x, y), localData);

        if (dustEffects)
            DustEffects(x, y);

        if (Main.netMode == NetmodeID.Server)
            SyncPipe(x, y, dustEffects, ignoreClient: sender);
    }

    public static void SyncPipeRectangle(int startX, int startY, int width, int height, int toClient = -1, int ignoreClient = -1)
    {
        ModPacket packet = Macrocosm.Instance.GetPacket();

        packet.Write((byte)MessageType.SyncPipeRectangle);
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
                        writer.Write(Main.tile[x, y].Get<PipeData>().Packed);
                    }
                }
            }

            byte[] compressedData = memoryStream.ToArray();
            packet.Write(compressedData.Length);
            packet.Write(compressedData);
        }

        packet.Send(toClient, ignoreClient);
    }

    public static void ReceiveSyncPipeRectangle(BinaryReader reader, int sender)
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
                    PipeData newData = new(data);
                    Main.tile[x, y].Get<PipeData>() = newData;
                    RefreshAttachmentState(new Point16(x, y), newData);
                }
            }
        }

        if (Main.netMode == NetmodeID.Server)
            SyncPipeRectangle(startX, startY, width, height, ignoreClient: sender);
    }

    public void OnPlayerJoining(int playerIndex)
    {
        SyncPipeRectangle(0, 0, Main.maxTilesX, Main.maxTilesY, toClient: playerIndex);
    }

    private const string TileDataSaveKey = nameof(PipeData);
    private const string LegacyTileDataSaveKey = "ConveyorData"; // TODO: remove legacy save key after the next save-breaking update

    public override void SaveWorldData(TagCompound tag)
    {
        PipeData[] data = Main.tile.GetData<PipeData>();
        ReadOnlySpan<PipeData> span = data;
        byte[] bytes = MemoryMarshal.AsBytes(span).ToArray();
        tag.Add(TileDataSaveKey, bytes);
    }

    public override void LoadWorldData(TagCompound tag)
    {
        PipeData[] data = Main.tile.GetData<PipeData>();
        // NOTE: Must keep legacy load until the new save-breaking update
        string key = tag.ContainsKey(TileDataSaveKey) ? TileDataSaveKey : LegacyTileDataSaveKey; 
        byte[] bytes = tag.GetByteArray(key);
        int expectedSize = data.Length * Marshal.SizeOf<PipeData>();
        if (bytes.Length != expectedSize)
        {
            Mod.Logger.Error($"Failed to load pipe tile data: expected {expectedSize}, got {bytes.Length}.", new SerializationException());
            return;
        }
        bytes.CopyTo(MemoryMarshal.AsBytes(data.AsSpan()));
        RebuildAttachmentStateCache();
    }
}
