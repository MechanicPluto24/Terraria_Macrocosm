using Macrocosm.Common.Config;
using Macrocosm.Common.DataStructures;
using Macrocosm.Common.Enums;
using Macrocosm.Common.Netcode;
using Macrocosm.Common.Storage;
using Macrocosm.Common.Systems.Power;
using Macrocosm.Common.Utils;
using Macrocosm.Content.Items.Connectors;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using static System.Runtime.InteropServices.JavaScript.JSType;
using static Terraria.GameContent.Bestiary.IL_BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions;
using static Terraria.ID.ContentSamples.CreativeHelper;

namespace Macrocosm.Common.Systems.Connectors
{
    public partial class ConveyorSystem : ModSystem
    {
        private static Asset<Texture2D> conveyorTexture;
        private static Asset<Texture2D> inletTexture;
        private static Asset<Texture2D> outletTexture;

        private static readonly HashSet<Point16> nodeCache = new();
        private static readonly List<ConveyorCircuit> circuits = new();
        private static int buildTimer = 0;
        private static int solveTimer = 0;
        private const int solveMax = 60;

        public override void Load()
        {
            conveyorTexture = ModContent.Request<Texture2D>(Macrocosm.TexturesPath + "Conveyors");
            inletTexture = ModContent.Request<Texture2D>(Macrocosm.TexturesPath + "ConveyorInlet");
            outletTexture = ModContent.Request<Texture2D>(Macrocosm.TexturesPath + "ConveyorOutlet");
        }

        public override void Unload()
        {
        }

        public override void ClearWorld()
        {
        }

        public static bool ShouldDraw => Main.LocalPlayer.CurrentItem().mech;
        public static ConveyorVisibility[] Visibility { get; set; } = new ConveyorVisibility[(int)ConveyorPipeType.Count];

        public static void PlacePipe(Point targetCoords, ConveyorPipeType type, bool sync = true) => PlacePipe(targetCoords.X, targetCoords.Y, type, sync);
        public static void PlacePipe(Point16 targetCoords, ConveyorPipeType type, bool sync = true) => PlacePipe(targetCoords.X, targetCoords.Y, type, sync);
        public static void PlacePipe(int x, int y, ConveyorPipeType type, bool sync = true)
        {
            ref var data = ref Main.tile[x, y].Get<ConveyorData>();
            data.SetPipe(type);
            if (sync && Main.netMode != NetmodeID.SinglePlayer)
                SyncConveyor(x, y);
        }

        public static void PlaceInlet(Point targetCoords, bool sync = true) => PlaceInlet(targetCoords.X, targetCoords.Y, sync);
        public static void PlaceInlet(Point16 targetCoords, bool sync = true) => PlaceInlet(targetCoords.X, targetCoords.Y, sync);
        public static void PlaceInlet(int x, int y, bool sync = true)
        {
            bool dust = false;
            ref var data = ref Main.tile[x, y].Get<ConveyorData>();
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
        }

        public static void PlaceOutlet(Point targetCoords, bool sync = true) => PlaceOutlet(targetCoords.X, targetCoords.Y, sync);
        public static void PlaceOutlet(Point16 targetCoords, bool sync = true) => PlaceOutlet(targetCoords.X, targetCoords.Y, sync);
        public static void PlaceOutlet(int x, int y, bool sync = true)
        {
            bool dust = false;
            ref var data = ref Main.tile[x, y].Get<ConveyorData>();
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
        }

        public static bool Remove(Point targetCoords, bool sync = true) => Remove(targetCoords.X, targetCoords.Y, sync);
        public static bool Remove(Point16 targetCoords, bool sync = true) => Remove(targetCoords.X, targetCoords.Y, sync);
        public static bool Remove(int x, int y, bool sync = true)
        {
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
                itemDrop = ModContent.ItemType<ConveyorInlet>();
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
                for (int i = 0; i < 10; i++)
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
        }

        private static void BuildConveyorCircuits()
        {
            circuits.Clear();
            BuildFor(new ChestConveyorProvider());
            BuildFor(new MachineTEConveyorProvider());
        }

        private static void BuildFor<T>(IConveyorContainerProvider<T> provider) where T : class
        {
            foreach (var container in provider.EnumerateContainers())
            {
                foreach (var node in provider.GetAllConveyorNodes(container))
                {
                    if (node.Circuit != null)
                        continue;

                    var search = new ConnectionSearch<ConveyorNode>(
                        connectionCheck: p => Main.tile[p.X, p.Y].Get<ConveyorData>().HasPipe(node.Type),
                        retrieveNode: p => provider.GetConveyorNode(p, node.Type)
                    );

                    HashSet<ConveyorNode> connectedNodes = search.FindConnectedNodes(startingPositions: [node.Position]);
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
                            circuit.Add(node);
                            n.Circuit = circuit;
                        }
                    }

                    if (!circuits.Contains(circuit))
                        circuits.Add(circuit);
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

            Rectangle frame = new(0, 0, 16, 16);
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

                    float pipeCount = 0f;
                    for (int t = 0; t < (int)ConveyorPipeType.Count; t++)
                    {
                        ConveyorPipeType type = (ConveyorPipeType)t;
                        if (data.HasPipe(type) && GetColor(i, j, Visibility[t]) != Color.Transparent)
                            pipeCount++;
                    }

                    for (int t = 0; i < (int)ConveyorPipeType.Count; t++)
                    {
                        ConveyorPipeType type = (ConveyorPipeType)t;
                        Color color = GetColor(i, j, Visibility[t]);
                        if (data.HasPipe(type) && color != Color.Transparent)
                        {
                            color *= 1f / pipeCount;

                            // Type frame
                            int frameY = 18 * t;

                            // Connection frame
                            int frameX = 0;
                            if (Main.tile[i, j - 1].Get<ConveyorData>().HasPipe(type))
                                frameX += 18;

                            if (Main.tile[i + 1, j].Get<ConveyorData>().HasPipe(type))
                                frameX += 36;

                            if (Main.tile[i, j + 1].Get<ConveyorData>().HasPipe(type))
                                frameX += 72;

                            if (Main.tile[i - 1, j].Get<ConveyorData>().HasPipe(type))
                                frameX += 144;

                            frame.Y = frameY;
                            frame.X = frameX;
       
                            spriteBatch.Draw(conveyorTexture.Value, new Vector2(i * 16 - (int)Main.screenPosition.X, j * 16 - (int)Main.screenPosition.Y) + zero, frame, color, 0f, zero, 1f, SpriteEffects.None, 0f);
                        }
                    }
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

            packet.Write((byte)MessageType.SyncPowerWire);
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
            byte data = reader.ReadByte();
            bool dustEffects = reader.ReadBoolean();

            ref var localData = ref Main.tile[x, y].Get<ConveyorData>();
            localData = new(data);

            if (dustEffects)
                DustEffects(x, y);

            if (Main.netMode == NetmodeID.Server)
                SyncConveyor(x, y, dustEffects, ignoreClient: sender);
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
        }
    }
}
