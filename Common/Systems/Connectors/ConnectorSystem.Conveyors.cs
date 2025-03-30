using Macrocosm.Common.Config;
using Macrocosm.Common.DataStructures;
using Macrocosm.Common.Drawing.Particles;
using Macrocosm.Common.Storage;
using Macrocosm.Common.Utils;
using Macrocosm.Content.Particles;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.DataStructures;

namespace Macrocosm.Common.Systems.Connectors
{
    public partial class ConnectorSystem
    {
        /// <summary>
        /// Represents a conveyor tile plus any associated inventory object
        /// (like a Chest or an IInventoryOwner).
        /// </summary>
        private class ConveyorNode(ConnectorData data, object storage, ConveyorCircuit circuit = null)
        {
            public ConnectorData Data = data;
            public object Storage = storage;
            public ConveyorCircuit Circuit = circuit;

            public override bool Equals(object obj) => obj is ConveyorNode other && Equals(Storage, other.Storage);
            public override int GetHashCode() => Storage?.GetHashCode() ?? 0;
        }

        private class ConveyorCircuit : Circuit<object>
        {
            public override void Add(object node)
            {
                if (node is ConveyorNode conveyorNode && !nodes.Contains(conveyorNode))
                    base.Add(node);
            }

            public override void Merge(Circuit<object> other)
            {
                if (other is ConveyorCircuit conveyorOther)
                {
                    foreach (var node in conveyorOther.nodes.OfType<ConveyorNode>())
                    {
                        Add(node);
                        node.Circuit = this;
                    }

                    conveyorOther.Clear();
                }
            }

            public override void Solve(int updateRate)
            {
                var outlets = nodes
                    .OfType<ConveyorNode>()
                    .Where(n => n.Data.Type == ConnectorType.ConveyorOutlet)
                    .ToList();

                var inlets = nodes
                    .OfType<ConveyorNode>()
                    .Where(n => n.Data.Type == ConnectorType.ConveyorInlet)
                    .ToList();

                foreach (var outletNode in outlets)
                {
                    if (outletNode.Storage is Chest chestOut)
                        TransferFromChest(chestOut, inlets);
                    else if (outletNode.Storage is IInventoryOwner ownerOut)
                        TransferFromTE(ownerOut, inlets);
                }
            }

            private void TransferFromChest(Chest sourceChest, List<ConveyorNode> inlets)
            {
                Vector2 sourcePosition = new Vector2(sourceChest.x, sourceChest.y) * 16f + new Vector2(8, 8);
                foreach (var inletNode in inlets)
                {
                    int transferAmount = 1;
                    for (int slot = 0; slot < sourceChest.item.Length; slot++)
                    {
                        Item sourceItem = sourceChest.item[slot];
                        if (sourceItem == null || sourceItem.IsAir)
                            continue;

                        Item sourceClone = sourceItem.Clone();
                        sourceClone.stack = transferAmount;
                        Item visualClone = sourceClone.Clone();

                        if (inletNode.Storage is Chest inletChest)
                        {
                            if (Utility.TryPlaceItemInChest(ref sourceClone, inletChest, justCheck: false))
                            {
                                Vector2 inletPosition = new Vector2(inletChest.x, inletChest.y) * 16f + new Vector2(8, 8);
                                ItemTransferVisuals(visualClone.type, visualClone.stack, sourcePosition, inletPosition, sourceChest, inletChest);
                                sourceItem.DecreaseStack(transferAmount);
                                break;
                            }
                        }
                        else if (inletNode.Storage is IInventoryOwner inletOwner)
                        {
                            if (inletOwner.Inventory.TryPlacingItem(ref sourceClone, justCheck: false))
                            {
                                Vector2 inletPosition = inletOwner.InventoryPosition;
                                ItemTransferVisuals(visualClone.type, visualClone.stack, sourcePosition, inletPosition, sourceChest, null);
                                sourceItem.DecreaseStack(transferAmount);
                                break;
                            }
                        }
                    }
                }
            }


            private void TransferFromTE(IInventoryOwner sourceOwner, List<ConveyorNode> inlets)
            {
                Vector2 sourcePosition = sourceOwner.InventoryPosition;
                foreach (var inletNode in inlets)
                {
                    int transferAmount = 1;
                    for (int slot = 0; slot < sourceOwner.Inventory.Size; slot++)
                    {
                        Item sourceItem = sourceOwner.Inventory[slot];
                        if (sourceItem == null || sourceItem.IsAir)
                            continue;

                        Item sourceClone = sourceItem.Clone();
                        sourceClone.stack = transferAmount;
                        Item visualClone = sourceClone.Clone();

                        if (inletNode.Storage is Chest inletChest)
                        {
                            if (Utility.TryPlaceItemInChest(ref sourceClone, inletChest, justCheck: false))
                            {
                                Vector2 inletPosition = new Vector2(inletChest.x, inletChest.y) * 16f + new Vector2(8, 8);
                                ItemTransferVisuals(visualClone.type, visualClone.stack, sourcePosition, inletPosition, null, inletChest);
                                sourceItem.DecreaseStack(transferAmount);
                                break;
                            }
                        }
                        else if (inletNode.Storage is IInventoryOwner inletOwner)
                        {
                            if (inletOwner.Inventory.TryPlacingItem(ref sourceClone, justCheck: false))
                            {
                                Vector2 inletPosition = inletOwner.InventoryPosition;
                                ItemTransferVisuals(visualClone.type, visualClone.stack, sourcePosition, inletPosition, null, null);
                                sourceItem.DecreaseStack(transferAmount);
                                break;
                            }
                        }
                    }
                }
            }


            private void ItemTransferVisuals(int itemId, int quantity, Vector2 startPosition, Vector2 endPosition, Chest sourceChest = null, Chest destinationChest = null)
            {
                for (int i = 0; i < 1 + (quantity / 500); i++)
                {
                    var p = Particle.Create<ItemTransferParticle>((p) =>
                    {
                        p.StartPosition = startPosition + Main.rand.NextVector2Circular(32, 16);
                        p.EndPosition = endPosition + Main.rand.NextVector2Circular(16, 16);
                        p.ItemType = itemId;
                        p.TimeToLive = Main.rand.Next(60, 80);
                    });
                }

                if (sourceChest != null)
                {
                    Vector2 sourcePosition = new Vector2(sourceChest.x, sourceChest.y) * 16f + new Vector2(8, 8);
                    Chest.AskForChestToEatItem(sourcePosition, 60);
                }

                if (destinationChest != null)
                {
                    Vector2 destinationPosition = new Vector2(destinationChest.x, destinationChest.y) * 16f + new Vector2(8, 8);
                    Chest.AskForChestToEatItem(destinationPosition, 60);
                }
            }

            public override bool Equals(object obj) => obj is ConveyorCircuit other && nodes.SetEquals(other.nodes);
            public override int GetHashCode() => nodes.Aggregate(0, (hash, node) => hash ^ node.GetHashCode());
        }

        private static readonly List<ConveyorCircuit> conveyorCircuits = new();
        private static int buildTimer = 0;
        private static int solveTimer = 0;
        private const int solveMax = 60;
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
            conveyorCircuits.Clear();

            foreach (var (position, data) in Map)
            {
                var startNode = CreateConveyorNode(position);
                if (startNode == null || startNode.Circuit != null)
                    continue;

                var search = new ConnectionSearch<ConveyorNode>(
                    connectionCheck: point => Map[point].AnyConveyor,
                    retrieveNode: CreateConveyorNode
                );

                HashSet<ConveyorNode> connectedNodes = search.FindConnectedNodes(startingPositions: [position]);
                if (connectedNodes.Count == 0)
                    continue;

                HashSet<ConveyorCircuit> existingCircuits = conveyorCircuits
                    .Where(circuit => connectedNodes.Any(node => circuit.Contains(node)))
                    .ToHashSet();

                ConveyorCircuit newCircuit;
                if (existingCircuits.Count > 0)
                {
                    newCircuit = existingCircuits.First();
                    foreach (var otherCircuit in existingCircuits.Skip(1))
                    {
                        newCircuit.Merge(otherCircuit);
                    }
                }
                else
                {
                    newCircuit = new ConveyorCircuit();
                }

                foreach (var node in connectedNodes)
                {
                    if (node.Circuit == null)
                    {
                        newCircuit.Add(node);
                        node.Circuit = newCircuit;
                    }
                }

                if (!conveyorCircuits.Contains(newCircuit))
                    conveyorCircuits.Add(newCircuit);
            }
        }

        private static ConveyorNode CreateConveyorNode(Point16 position)
        {
            if (!Map[position].ConveyorInlet && !Map[position].ConveyorOutlet)
                return null;

            var storage = TryGetStorageObject(position.X, position.Y);
            if (storage is null)
                return null;

            return new ConveyorNode(Map[position], storage);
        }

        private static object TryGetStorageObject(int x, int y)
        {
            Point16 position = Utility.GetMultitileTopLeft(x, y);
            int chestIndex = Chest.FindChest(position.X, position.Y);
            if (chestIndex >= 0 && Main.chest[chestIndex] is Chest chest)
                return chest;

            if (Utility.TryGetTileEntityAs(new Point16(x, y), out TileEntity te) && te is IInventoryOwner inventoryOwner)
                return inventoryOwner;

            return null;
        }

        private static void SolveConveyorCircuits()
        {
            foreach (var circuit in conveyorCircuits)
                circuit.Solve(solveMax);
        }
    }
}
