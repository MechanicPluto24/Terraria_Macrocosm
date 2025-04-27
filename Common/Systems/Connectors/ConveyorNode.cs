using System;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;

namespace Macrocosm.Common.Systems.Connectors
{
    /// <summary>
    /// Represents a conveyor tile plus any associated inventory object (such as a Chest or an IInventoryOwner).
    /// </summary>
    public class ConveyorNode(object storage, ConveyorData data, ConveyorPipeType type, Point16 position, IEnumerable<Point16> connectionPositions, ConveyorCircuit circuit = null)
    {
        public object Storage = storage;
        //public Item[] Items = items;
        public ConveyorData Data = data;
        public ConveyorPipeType Type = type;
        public Point16 Position = position;
        public IEnumerable<Point16> ConnectionPositions = connectionPositions;
        public ConveyorCircuit Circuit = circuit;

        public bool Inlet => Data.Inlet && !Data.Outlet;
        public bool Outlet => Data.Outlet && !Data.Inlet;

        public override bool Equals(object obj) => obj is ConveyorNode other && Equals(Storage, other.Storage) && Type == other.Type;
        public override int GetHashCode() => HashCode.Combine(Storage ?? 0, Type, Position);
    }
}
