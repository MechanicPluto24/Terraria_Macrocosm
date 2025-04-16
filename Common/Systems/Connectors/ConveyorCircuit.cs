using Macrocosm.Common.DataStructures;
using Macrocosm.Common.Drawing.Particles;
using Macrocosm.Common.Storage;
using Macrocosm.Common.Utils;
using Macrocosm.Content.Particles;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Linq;
using Terraria;

namespace Macrocosm.Common.Systems.Connectors
{
    public class ConveyorCircuit : Circuit<ConveyorNode>
    {
        public ConveyorType ConveyorType { get; }

        public ConveyorCircuit(ConveyorType type)
        {
            ConveyorType = type;
        }

        public override void Add(ConveyorNode node)
        {
            if (node is ConveyorNode conveyorNode && !nodes.Contains(conveyorNode))
                base.Add(node);
        }

        public override void Merge(Circuit<ConveyorNode> other)
        {
            if (other is ConveyorCircuit conveyorOther && conveyorOther.ConveyorType == ConveyorType)
            {
                foreach (var node in conveyorOther.nodes)
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
                .Where(n => n.Data.Outlet)
                .ToList();

            var inlets = nodes
                .OfType<ConveyorNode>()
                .Where(n => n.Data.Inlet)
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
}
