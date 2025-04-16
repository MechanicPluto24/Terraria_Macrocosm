using System;
using Terraria.DataStructures;

namespace Macrocosm.Common.Systems.Connectors
{
    /// <summary>
    /// Represents a conveyor tile plus any associated inventory object (such as a Chest or an IInventoryOwner).
    /// </summary>
    public class ConveyorNode(ConveyorType type, ConveyorData data, Point16 position, object storage, ConveyorCircuit circuit = null)
    {
        public ConveyorType Type = type;
        public ConveyorData Data = data;
        public Point16 Position = position;
        public object Storage = storage;
        public ConveyorCircuit Circuit = circuit;

        public override bool Equals(object obj) => obj is ConveyorNode other && Equals(Storage, other.Storage) && Type == other.Type;
        public override int GetHashCode() => HashCode.Combine(Storage ?? 0, Type);
    }
}
