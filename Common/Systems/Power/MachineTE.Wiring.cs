using Macrocosm.Common.Config;
using Macrocosm.Common.DataStructures;
using Macrocosm.Common.Enums;
using Macrocosm.Common.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.DataStructures;

namespace Macrocosm.Common.Systems.Power;

public partial class MachineTE
{
    public WireCircuit Circuit { get; set; } = null;
    protected static List<WireType> WireTypes => Enum.GetValues<WireType>().ToList();


    private static int buildTimer = 0;
    private void BuildCircuits()
    {
        if (buildTimer++ >= (int)ServerConfig.Instance.CircuitSearchUpdateRate)
        {
            foreach (var te in ByID.Values)
                if (te is MachineTE machine)
                    machine.Circuit = null;

            foreach (WireType wireType in WireTypes)
            {
                foreach (var te in ByID.Values)
                {
                    if (te is MachineTE machine)
                    {
                        if (machine.Circuit != null)
                            continue;

                        ConnectionSearch<MachineTE> connectionSearch = new(
                            connectionCheck: position => Main.tile[position].HasWire(wireType),
                            retrieveNode: position => TileEntity.TryGet<MachineTE>(position.X, position.Y, out var m) ? m : null
                        );

                        HashSet<MachineTE> connectedNodes = connectionSearch.FindConnectedNodes(machine.GetConnectionPositions());
                        if (connectedNodes.Count == 0)
                            continue;

                        HashSet<WireCircuit> existingCircuits = new();
                        foreach (var node in connectedNodes)
                        {
                            if (node is MachineTE m && m.Circuit != null)
                            {
                                existingCircuits.Add(m.Circuit);
                            }
                        }

                        WireCircuit newCircuit;
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
                            newCircuit = new WireCircuit(wireType);
                        }

                        foreach (var node in connectedNodes)
                        {
                            if (node is MachineTE m && m.Circuit == null)
                            {
                                newCircuit.Add(m);
                                m.Circuit = newCircuit;
                            }
                        }

                        if (machine.Circuit == null)
                        {
                            newCircuit.Add(machine);
                            machine.Circuit = newCircuit;
                        }
                    }
                }
            }

            buildTimer = 0;
        }
    }

    private static int solveTimer = 0;
    private static void SolveCircuits()
    {
        if (solveTimer++ >= (int)ServerConfig.Instance.CircuitSolveUpdateRate)
        {
            HashSet<WireCircuit> processedWireCircuits = new();
            foreach (WireType wireType in WireTypes)
            {
                foreach (var te in ByID.Values)
                {
                    if (te is MachineTE machine)
                    {
                        if (machine.Circuit != null)
                        {
                            if (!processedWireCircuits.Contains(machine.Circuit))
                            {
                                machine.Circuit.Solve((int)ServerConfig.Instance.CircuitSolveUpdateRate);
                                processedWireCircuits.Add(machine.Circuit);
                            }
                        }
                        else
                        {
                            machine.OnPowerDisconnected();
                        }
                    }
                }
            }

            solveTimer = 0;
        }
    }

    protected virtual IEnumerable<Point16> GetConnectionPositions()
    {
        if (CanCluster && Cluster != null)
        {
            foreach (var position in Cluster)
                yield return position;
        }
        else
        {
            for (int x = Position.X; x < Position.X + MachineTile.Width; x++)
            {
                for (int y = Position.Y; y < Position.Y + MachineTile.Height; y++)
                {
                    yield return new Point16(x, y);
                }
            }
        }
    }
}
