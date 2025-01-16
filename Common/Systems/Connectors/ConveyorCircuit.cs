using Macrocosm.Common.DataStructures;
using Macrocosm.Common.Drawing.Particles;
using Macrocosm.Common.Storage;
using Macrocosm.Common.Systems.Power;
using Macrocosm.Content.Particles;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.DataStructures;

namespace Macrocosm.Common.Systems.Connectors
{
    /// <summary>
    /// Represents a conveyor tile plus any associated inventory object
    /// (like a Chest or an IInventoryOwner).
    /// </summary>
    public class ConveyorNode
    {
        public Point16 Position;
        public ConnectorData Data;
        public object Storage;
    }

    public class ConveyorCircuit : Circuit<object>
    {
        public override void Merge(Circuit<object> other)
        {
            if (other is ConveyorCircuit conveyorOther)
            {
                foreach (var node in conveyorOther.nodes)
                    Add(node);

                other.Clear();
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

            for (int slot = 0; slot < sourceChest.item.Length; slot++)
            {
                Item sourceItem = sourceChest.item[slot];
                if (sourceItem == null || sourceItem.IsAir)
                    continue;

                foreach (var inletNode in inlets)
                {
                    Vector2 inletPosition = inletNode.Position.ToWorldCoordinates();

                    if (inletNode.Storage is Chest inletChest)
                    {
                        Chest.AskForChestToEatItem(inletPosition, 60);
                        AttemptItemTransfer(ref sourceItem, inletChest.item, sourcePosition, inletPosition);
                    }
                    else if (inletNode.Storage is IInventoryOwner inletOwner)
                    {
                        AttemptItemTransfer(ref sourceItem, inletOwner.Inventory.Items, sourcePosition, inletPosition);
                    }

                    if (sourceItem is null || sourceItem.IsAir)
                        break;
                }

                sourceChest.item[slot] = sourceItem;
                Chest.AskForChestToEatItem(sourcePosition, 60);
            }
        }


        private void TransferFromTE(IInventoryOwner sourceOwner, List<ConveyorNode> inlets)
        {
            Vector2 sourcePosition = sourceOwner.InventoryPosition.ToWorldCoordinates(); 
            for (int slot = 0; slot < sourceOwner.Inventory.Size; slot++)
            {
                Item sourceItem = sourceOwner.Inventory[slot];
                if (sourceItem == null || sourceItem.IsAir)
                    continue;

                foreach (var inletNode in inlets)
                {
                    Vector2 inletPosition = inletNode.Position.ToWorldCoordinates();

                    if (inletNode.Storage is Chest inletChest)
                    {
                        AttemptItemTransfer(ref sourceItem, inletChest.item, sourcePosition, inletPosition);
                        Chest.AskForChestToEatItem(inletPosition, 60); 
                    }
                    else if (inletNode.Storage is IInventoryOwner inletOwner)
                    {
                        AttemptItemTransfer(ref sourceItem, inletOwner.Inventory.Items, sourcePosition, inletPosition);
                    }

                    if (sourceItem is null || sourceItem.IsAir)
                        break;
                }

                sourceOwner.Inventory[slot] = sourceItem;
            }
        }


        private void AttemptItemTransfer(ref Item sourceItem, Item[] target, Vector2 startPosition, Vector2 endPosition)
        {
            if (sourceItem is null || sourceItem.IsAir)
                return;

            for (int i = 0; i < target.Length && !sourceItem.IsAir; i++)
            {
                if (target[i] is null || target[i].IsAir)
                {
                    CreateItemTransferParticle(sourceItem.type, sourceItem.stack, startPosition, endPosition);

                    target[i] = sourceItem.Clone();
                    sourceItem.TurnToAir();
                }
                else if (target[i].type == sourceItem.type && target[i].stack < target[i].maxStack)
                {
                    int space = target[i].maxStack - target[i].stack;
                    int toMove = Math.Min(space, sourceItem.stack);

                    target[i].stack += toMove;
                    sourceItem.stack -= toMove;

                    CreateItemTransferParticle(sourceItem.type, toMove, startPosition, endPosition);
                }
            }
        }

        private void CreateItemTransferParticle(int itemId, int quantity, Vector2 startPosition, Vector2 endPosition)
        {
            for (int i = 0; i < quantity; i++)
            {
                var p = Particle.Create<ItemTransferParticle>((p) =>
                {
                    p.StartPosition = startPosition + Main.rand.NextVector2Circular(32, 16);
                    p.EndPosition = endPosition + Main.rand.NextVector2Circular(16, 16);
                    p.ItemType = itemId;
                    p.TimeToLive = Main.rand.Next(60, 80);
                });
            }
        }
    }
}
