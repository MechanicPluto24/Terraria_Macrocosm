using Macrocosm.Common.DataStructures;
using Macrocosm.Common.Enums;
using Macrocosm.Common.Utils;
using Macrocosm.Content.Items.Tools;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.PortableExecutable;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Common.Systems.Power
{
    public class CircuitSystem : ModSystem
    {
        private static readonly Dictionary<VanillaWireType, UnionFind<Point16>> wireNetworks = new()
        {
            { VanillaWireType.Red, new UnionFind<Point16>() },
            { VanillaWireType.Blue, new UnionFind<Point16>() },
            { VanillaWireType.Green, new UnionFind<Point16>() },
            { VanillaWireType.Yellow, new UnionFind<Point16>() }
        };

        private static readonly Dictionary<int, Circuit> machineToCircuit = new();
        private static readonly HashSet<Circuit> circuits = new();

        public override void Load()
        {
            On_Main.DrawWires += On_Main_DrawWires;
            On_WorldGen.PlaceWire += On_WorldGen_PlaceWire;
            On_WorldGen.PlaceWire2 += On_WorldGen_PlaceWire2;
            On_WorldGen.PlaceWire3 += On_WorldGen_PlaceWire3;
            On_WorldGen.PlaceWire4 += On_WorldGen_PlaceWire4;
            On_WorldGen.KillWire += On_WorldGen_KillWire;
            On_WorldGen.KillWire2 += On_WorldGen_KillWire2;
            On_WorldGen.KillWire3 += On_WorldGen_KillWire3;
            On_WorldGen.KillWire4 += On_WorldGen_KillWire4;
        }

        public override void Unload()
        {
            On_Main.DrawWires -= On_Main_DrawWires;
            On_WorldGen.PlaceWire -= On_WorldGen_PlaceWire;
            On_WorldGen.PlaceWire2 -= On_WorldGen_PlaceWire2;
            On_WorldGen.PlaceWire3 -= On_WorldGen_PlaceWire3;
            On_WorldGen.PlaceWire4 -= On_WorldGen_PlaceWire4;
            On_WorldGen.KillWire -= On_WorldGen_KillWire;
            On_WorldGen.KillWire2 -= On_WorldGen_KillWire2;
            On_WorldGen.KillWire3 -= On_WorldGen_KillWire3;
            On_WorldGen.KillWire4 -= On_WorldGen_KillWire4;
        }

        public override void ClearWorld()
        {
            machineToCircuit.Clear();
            circuits.Clear();
            foreach (var uf in wireNetworks.Values)
                uf.Clear();

            InitializeWireNetworks();
            BuildCircuits();
        }

        private static int updateTimer = 0;
        private static readonly int updateRate = 1;
        public override void PostUpdateEverything()
        {
            if (updateTimer++ >= updateRate)
            {
                SolveCircuits();
                updateTimer = 0;
            }
        }

        private static void InitializeWireNetworks()
        {
            for (int x = 0; x < Main.maxTilesX; x++)
            {
                for (int y = 0; y < Main.maxTilesY; y++)
                {
                    Tile tile = Main.tile[x, y];
                    Point16 current = new(x, y);

                    foreach (var wireType in wireNetworks.Keys)
                    {
                        if (Utility.HasWire(tile, wireType))
                        {
                            UnionFind<Point16> uf = wireNetworks[wireType];
                            uf.MakeSet(current);

                            foreach (Point16 neighbor in Utility.GetWireNeighbors(x, y, wireType))
                            {
                                if (uf.Contains(neighbor))
                                    uf.Union(current, neighbor);
                                else
                                    uf.MakeSet(neighbor);
                            }
                        }
                    }
                }
            }
        }

        private static void BuildCircuits()
        {
            machineToCircuit.Clear();
            circuits.Clear();

            foreach (var kvp in TileEntity.ByID)
            {
                if (kvp.Value is MachineTE machine)
                {
                    Circuit circuit = GetOrCreateCircuitForMachine(machine);
                    circuit.Add(machine);
                    machineToCircuit[machine.ID] = circuit;
                }
            }
        }

        private static Circuit GetOrCreateCircuitForMachine(MachineTE machine)
        {
            HashSet<Circuit> connectedCircuits = new();
            HashSet<Point16> connectedWireRoots = new();

            foreach (var wireType in wireNetworks.Keys)
            {
                UnionFind<Point16> uf = wireNetworks[wireType];

                foreach (Point16 position in machine.GetWirePositions())
                {
                    if (!Utility.HasWire(Main.tile[position.X, position.Y], wireType))
                        continue;

                    Point16 root = uf.Find(position);
                    connectedWireRoots.Add(root);

                    foreach (var circuit in circuits)
                    {
                        if (circuit.ContainsWireRoot(root))
                        {
                            connectedCircuits.Add(circuit);
                            break;
                        }
                    }
                }
            }

            if (connectedCircuits.Count == 0)
            {
                Circuit newCircuit = new Circuit();
                foreach (var root in connectedWireRoots)
                {
                    newCircuit.AddWireRoot(root);
                }
                circuits.Add(newCircuit);
                return newCircuit;
            }
            else if (connectedCircuits.Count == 1)
            {
                Circuit circuit = connectedCircuits.First();
                foreach (var root in connectedWireRoots)
                {
                    circuit.AddWireRoot(root);
                }
                return circuit;
            }
            else
            {
                Circuit mergedCircuit = new Circuit();
                foreach (var circuit in connectedCircuits)
                {
                    mergedCircuit.Merge(circuit);
                    circuits.Remove(circuit);
                }
                foreach (var root in connectedWireRoots)
                {
                    mergedCircuit.AddWireRoot(root);
                }
                circuits.Add(mergedCircuit);
                return mergedCircuit;
            }
        }


        private static void SolveCircuits()
        {
            foreach (Circuit circuit in circuits)
                circuit.Solve(updateRate);
        }

        public static void HandleMachinePlacement(MachineTE machine)
        {
            if (machineToCircuit.ContainsKey(machine.ID))
                return;

            Circuit circuit = GetOrCreateCircuitForMachine(machine);
            circuit.Add(machine);
            machineToCircuit[machine.ID] = circuit;
        }

        public static void HandleMachineRemoval(MachineTE machine)
        {
            if (machineToCircuit.TryGetValue(machine.ID, out Circuit oldCircuit))
            {
                oldCircuit.Remove(machine);
                if (oldCircuit.IsEmpty)
                {
                    circuits.Remove(oldCircuit);
                }
                machineToCircuit.Remove(machine.ID);

                RebuildCircuitsAfterMachineRemoval(machine, oldCircuit);
            }
        }


        private static void RebuildCircuitsAfterMachineRemoval(MachineTE removedMachine, Circuit oldCircuit)
        {
            // Perform BFS from the wire positions connected to the removed machine
            HashSet<Point16> visited = new();
            Queue<Point16> queue = new();

            foreach (var wireType in wireNetworks.Keys)
            {
                UnionFind<Point16> uf = wireNetworks[wireType];

                foreach (Point16 position in removedMachine.GetWirePositions())
                {
                    if (!Utility.HasWire(Main.tile[position.X, position.Y], wireType))
                        continue;

                    Point16 root = uf.Find(position);
                    if (!visited.Contains(root))
                    {
                        visited.Add(root);
                        queue.Enqueue(root);
                    }
                }
            }

            HashSet<int> processedMachineIDs = new();

            while (queue.Count > 0)
            {
                Point16 currentRoot = queue.Dequeue();

                Circuit newCircuit = new();

                foreach (var kvp in TileEntity.ByID)
                {
                    if (kvp.Value is MachineTE machineTE)
                    {
                        if (machineTE.ID == removedMachine.ID)
                            continue;

                        if (processedMachineIDs.Contains(machineTE.ID))
                            continue;

                        foreach (var position in machineTE.GetWirePositions())
                        {
                            foreach (var wireType in wireNetworks.Keys)
                            {
                                UnionFind<Point16> uf = wireNetworks[wireType];
                                if (uf.Contains(position) && uf.Find(position).Equals(currentRoot))
                                {
                                    newCircuit.Add(machineTE);
                                    machineToCircuit[machineTE.ID] = newCircuit;
                                    processedMachineIDs.Add(machineTE.ID);
                                    break;
                                }
                            }
                        }
                    }
                }

                if (newCircuit.NodeCount > 0)
                {
                    circuits.Add(newCircuit);
                }
            }

            circuits.Remove(oldCircuit);
        }



        private void HandleWirePlacement(int x, int y, VanillaWireType wireType)
        {
            Point16 position = new(x, y);
            UnionFind<Point16> uf = wireNetworks[wireType];
            uf.MakeSet(position);

            foreach (Point16 neighbor in Utility.GetWireNeighbors(x, y, wireType))
            {
                if (uf.Contains(neighbor))
                {
                    uf.Union(position, neighbor);
                }
                else
                {
                    uf.MakeSet(neighbor);
                }
            }

            UpdateCircuitsForWireChange(position, wireType);
        }

        private void HandleWireRemoval(int x, int y, VanillaWireType wireType)
        {
            Point16 position = new(x, y);
            UnionFind<Point16> uf = wireNetworks[wireType];

            if (!uf.Contains(position))
                return;

            uf.Remove(position);

            List<HashSet<Point16>> newComponents = GetConnectedComponentsAfterRemoval(position, wireType);

            foreach (var component in newComponents)
            {
                Point16 representative = component.First();
                uf.MakeSet(representative);

                foreach (var point in component)
                {
                    uf.Union(representative, point);
                }
            }

            UpdateCircuitsForWireChange(position, wireType);
        }

        private List<HashSet<Point16>> GetConnectedComponentsAfterRemoval(Point16 removedPosition, VanillaWireType wireType)
        {
            HashSet<Point16> visited = new();
            List<HashSet<Point16>> components = new();

            UnionFind<Point16> uf = wireNetworks[wireType];

            foreach (Point16 neighbor in Utility.GetWireNeighbors(removedPosition.X, removedPosition.Y, wireType))
            {
                if (!Utility.HasWire(Main.tile[neighbor.X, neighbor.Y], wireType))
                    continue;

                if (visited.Contains(neighbor))
                    continue;

                HashSet<Point16> component = new();
                Queue<Point16> queue = new();
                queue.Enqueue(neighbor);
                visited.Add(neighbor);

                while (queue.Count > 0)
                {
                    Point16 current = queue.Dequeue();
                    component.Add(current);

                    foreach (Point16 nextNeighbor in Utility.GetWireNeighbors(current.X, current.Y, wireType))
                    {
                        if (!Utility.HasWire(Main.tile[nextNeighbor.X, nextNeighbor.Y], wireType))
                            continue;

                        if (!visited.Contains(nextNeighbor))
                        {
                            visited.Add(nextNeighbor);
                            queue.Enqueue(nextNeighbor);
                        }
                    }
                }

                components.Add(component);
            }

            return components;
        }

        private void UpdateCircuitsForWireChange(Point16 position, VanillaWireType wireType)
        {
            HashSet<MachineTE> affectedMachines = new();

            foreach (Point16 neighbor in Utility.GetWireNeighbors(position.X, position.Y, wireType))
            {
                foreach (var machine in GetMachinesAtPosition(neighbor))
                {
                    affectedMachines.Add(machine);
                }
            }

            // Rebuild circuits for affected machines
            foreach (var machine in affectedMachines)
            {
                if (machineToCircuit.TryGetValue(machine.ID, out Circuit oldCircuit))
                {
                    oldCircuit.Remove(machine);
                    if (oldCircuit.IsEmpty)
                    {
                        circuits.Remove(oldCircuit);
                    }
                }

                Circuit newCircuit = GetOrCreateCircuitForMachine(machine);
                newCircuit.Add(machine);
                machineToCircuit[machine.ID] = newCircuit;
            }
        }

        private IEnumerable<MachineTE> GetMachinesAtPosition(Point16 position)
        {
            if (Utility.TryGetTileEntityAs(position.X, position.Y, out MachineTE machine))
            {
                yield return machine;
            }
        }

        private void On_Main_DrawWires(On_Main.orig_DrawWires orig, Main self)
        {
            orig(self);

            if (Main.LocalPlayer.CurrentItem().type != ModContent.ItemType<CircuitProbe>())
                return;

            foreach (var kvp in TileEntity.ByID)
            {
                if (kvp.Value is MachineTE machine)
                    machine.DrawMachinePowerInfo(Main.spriteBatch);
            }
        }

        private bool On_WorldGen_PlaceWire(On_WorldGen.orig_PlaceWire orig, int i, int j)
        {
            bool result = orig(i, j);
            HandleWirePlacement(i, j, VanillaWireType.Red);
            return result;
        }

        private bool On_WorldGen_PlaceWire2(On_WorldGen.orig_PlaceWire2 orig, int i, int j)
        {
            bool result = orig(i, j);
            HandleWirePlacement(i, j, VanillaWireType.Blue);
            return result;
        }

        private bool On_WorldGen_PlaceWire3(On_WorldGen.orig_PlaceWire3 orig, int i, int j)
        {
            bool result = orig(i, j);
            HandleWirePlacement(i, j, VanillaWireType.Green);
            return result;
        }

        private bool On_WorldGen_PlaceWire4(On_WorldGen.orig_PlaceWire4 orig, int i, int j)
        {
            bool result = orig(i, j);
            HandleWirePlacement(i, j, VanillaWireType.Yellow);
            return result;
        }

        private bool On_WorldGen_KillWire(On_WorldGen.orig_KillWire orig, int i, int j)
        {
            bool result = orig(i, j);
            HandleWireRemoval(i, j, VanillaWireType.Red);
            return result;
        }

        private bool On_WorldGen_KillWire2(On_WorldGen.orig_KillWire2 orig, int i, int j)
        {
            bool result = orig(i, j);
            HandleWireRemoval(i, j, VanillaWireType.Blue);
            return result;
        }

        private bool On_WorldGen_KillWire3(On_WorldGen.orig_KillWire3 orig, int i, int j)
        {
            bool result = orig(i, j);
            HandleWireRemoval(i, j, VanillaWireType.Green);
            return result;
        }

        private bool On_WorldGen_KillWire4(On_WorldGen.orig_KillWire4 orig, int i, int j)
        {
            bool result = orig(i, j);
            HandleWireRemoval(i, j, VanillaWireType.Yellow);
            return result;
        }
    }
}
