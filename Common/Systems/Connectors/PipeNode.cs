using System;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;

namespace Macrocosm.Common.Systems.Connectors;

/// <summary>
/// Represents a pipe tile plus any associated inventory object (such as a Chest or an IInventoryOwner).
/// </summary>
public class PipeNode(object entity, PipeData data, PipeType type, Point16 position, IEnumerable<Point16> connectionPositions, PipeCircuit circuit = null)
{
    public object Entity = entity;
    public PipeData Data = data;
    public PipeType Type = type;
    public Point16 Position = position;
    public IEnumerable<Point16> ConnectionPositions = connectionPositions;
    public PipeCircuit Circuit = circuit;

    public bool Inlet => Data.Inlet && !Data.Outlet;
    public bool Outlet => Data.Outlet && !Data.Inlet;

    public override bool Equals(object obj) => obj is PipeNode other && Equals(Entity, other.Entity) && Type == other.Type;
    public override int GetHashCode() => HashCode.Combine(Entity ?? 0, Type, Position);
}
