using Macrocosm.Common.DataStructures;
using Macrocosm.Common.Enums;
using Macrocosm.Common.Utils;
using Macrocosm.Content.Items.Tools;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Common.Systems.Power
{
    public class CircuitSystem : ModSystem
    {
        private static readonly Dictionary<VanillaWireType, UnionFind> wireNetworks = new()
        {
            { VanillaWireType.Red, new UnionFind() },
            { VanillaWireType.Blue, new UnionFind() },
            { VanillaWireType.Green, new UnionFind() },
            { VanillaWireType.Yellow, new UnionFind() }
        };

        private static readonly Dictionary<int, Circuit> machineToCircuit = new();
        private static readonly List<Circuit> circuits = new();

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
            SearchAllCircuits();
        }

        private static int updateTimer = 0;
        private const int updateRate = 1;
        public override void PostUpdateEverything()
        {
            if (updateTimer++ >= updateRate)
            {
                SolveCircuits();
                updateTimer = 0;
            }
        }

        public static void SearchAllCircuits()
        {
            machineToCircuit.Clear();
            circuits.Clear();

            foreach (var uf in wireNetworks.Values)
                uf.Clear();

            BuildWireNetworks();
            MapMachinesToCircuits();
            SolveCircuits();
        }


        /*
        public static void SearchCircuits()
        {
            machineToCircuit.Clear();
            circuits.Clear();

            foreach (var uf in wireNetworks.Values)
                uf.Clear();

            BuildWireNetworks();
            MapMachinesToCircuits();
            SolveCircuits();
        }
        */

        private static void BuildWireNetworks()
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
                            UnionFind uf = wireNetworks[wireType];
                            uf.Find(current); 

                            foreach (Point16 neighbor in Utility.GetWireNeighbors(x, y, wireType))
                            {
                                uf.Union(current, neighbor);
                            }
                        }
                    }
                }
            }
        }

        private static void MapMachinesToCircuits()
        {
            Dictionary<Point16, Circuit> rootToCircuit = new();

            foreach (var kvp in TileEntity.ByID)
            {
                if (kvp.Value is MachineTE machine)
                {
                    HashSet<Point16> roots = new();

                    for (int i = machine.Position.X; i < machine.Position.X + machine.MachineTile.Width; i++)
                    {
                        for (int j = machine.Position.Y; j < machine.Position.Y + machine.MachineTile.Height; j++)
                        {
                            Tile tile = Main.tile[i, j];
                            if (tile == null) continue;

                            foreach (var wireType in wireNetworks.Keys)
                            {
                                if (Utility.HasWire(tile, wireType))
                                {
                                    UnionFind uf = wireNetworks[wireType];
                                    Point16 root = uf.Find(new Point16(i, j));
                                    roots.Add(root);
                                }
                            }
                        }
                    }

                    if (roots.Count == 0)
                    {
                        Circuit circuit = [machine];
                        circuits.Add(circuit);
                        machineToCircuit[machine.ID] = circuit;
                    }
                    else
                    {
                        foreach (var root in roots)
                        {
                            if (!rootToCircuit.TryGetValue(root, out Circuit circuit))
                            {
                                circuit = new Circuit();
                                rootToCircuit[root] = circuit;
                                circuits.Add(circuit);
                            }

                            circuit.Add(machine);
                            machineToCircuit[machine.ID] = circuit;
                        }
                    }
                }
            }
        }

        public static void SearchCircuitsAround(int x, int y, VanillaWireType? wireType = null)
        {
            int area = 50; 
            int startX = Math.Max(0, x - area);
            int endX = Math.Min(Main.maxTilesX - 1, x + area);
            int startY = Math.Max(0, y - area);
            int endY = Math.Min(Main.maxTilesY - 1, y + area);

            HashSet<MachineTE> affectedMachines = new();
            HashSet<Point16> affectedWires = new();

            for (int i = startX; i <= endX; i++)
            {
                for (int j = startY; j <= endY; j++)
                {
                    Tile tile = Main.tile[i, j];

                    if (wireType.HasValue)
                    {
                        if (tile.HasWire(wireType.Value))
                            affectedWires.Add(new Point16(i, j));
                    }
                    else
                    {
                        if (tile.HasWire())
                            affectedWires.Add(new Point16(i, j));
                    }
                    
                    if (Utility.TryGetTileEntityAs(i, j, out MachineTE machine))
                    {
                        affectedMachines.Add(machine);
                    }
                }
            }

            foreach (var machine in affectedMachines)
            {
                if (machineToCircuit.TryGetValue(machine.ID, out Circuit oldCircuit))
                {
                    oldCircuit.Remove(machine);
                    if (oldCircuit.NodeCount == 0)
                    {
                        circuits.Remove(oldCircuit);
                    }
                }
                machineToCircuit.Remove(machine.ID);
            }

            BuildCircuits(affectedMachines, affectedWires);
            SolveCircuits();
        }

        private static void BuildCircuits(HashSet<MachineTE> machines, HashSet<Point16> wires)
        {
            HashSet<MachineTE> visitedMachines = new();
            Queue<MachineTE> queue = new();

            foreach (var machine in machines)
            {
                if (visitedMachines.Contains(machine)) continue;

                Circuit circuit = new();
                queue.Enqueue(machine);

                while (queue.Count > 0)
                {
                    MachineTE currentMachine = queue.Dequeue();
                    if (!visitedMachines.Add(currentMachine)) continue;

                    circuit.Add(currentMachine);
                    machineToCircuit[currentMachine.ID] = circuit;

                    foreach (var neighbor in GetConnectedMachines(currentMachine, wires))
                    {
                        if (!visitedMachines.Contains(neighbor))
                        {
                            queue.Enqueue(neighbor);
                        }
                    }
                }

                circuits.Add(circuit);
            }
        }

        private static IEnumerable<MachineTE> GetConnectedMachines(MachineTE machine, HashSet<Point16> wires)
        {
            HashSet<Point16> visited = new();
            Queue<Point16> queue = new();

            for (int i = machine.Position.X; i < machine.Position.X + machine.MachineTile.Width; i++)
            {
                for (int j = machine.Position.Y; j < machine.Position.Y + machine.MachineTile.Height; j++)
                {
                    Point16 point = new(i, j);
                    if (wires.Contains(point))
                    {
                        queue.Enqueue(point);
                        visited.Add(point);
                    }
                }
            }

            while (queue.Count > 0)
            {
                Point16 current = queue.Dequeue();

                foreach (var neighbor in Utility.GetWireNeighbors(current.X, current.Y))
                {
                    if (!visited.Add(neighbor)) continue;

                    if (wires.Contains(neighbor))
                    {
                        queue.Enqueue(neighbor);
                    }

                    if (Utility.TryGetTileEntityAs(neighbor.X, neighbor.Y, out MachineTE neighborMachine))
                    {
                        yield return neighborMachine;
                    }
                }
            }
        }


        private static void SolveCircuits()
        {
            foreach (Circuit circuit in circuits)
                circuit.Solve(updateRate);
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
            SearchCircuitsAround(i, j, VanillaWireType.Red);
            return orig(i, j);
        }

        private bool On_WorldGen_PlaceWire2(On_WorldGen.orig_PlaceWire2 orig, int i, int j)
        {
            SearchCircuitsAround(i ,j, VanillaWireType.Blue);
            return orig(i, j);
        }

        private bool On_WorldGen_PlaceWire3(On_WorldGen.orig_PlaceWire3 orig, int i, int j)
        {
            SearchCircuitsAround(i, j, VanillaWireType.Green);
            return orig(i, j);
        }

        private bool On_WorldGen_PlaceWire4(On_WorldGen.orig_PlaceWire4 orig, int i, int j)
        {
            SearchCircuitsAround(i, j, VanillaWireType.Yellow);
            return orig(i, j);
        }

        private bool On_WorldGen_KillWire(On_WorldGen.orig_KillWire orig, int i, int j)
        {
            SearchCircuitsAround(i, j, VanillaWireType.Red);
            return orig(i, j);
        }

        private bool On_WorldGen_KillWire2(On_WorldGen.orig_KillWire2 orig, int i, int j)
        {
            SearchCircuitsAround(i, j, VanillaWireType.Blue);
            return orig(i, j);
        }

        private bool On_WorldGen_KillWire3(On_WorldGen.orig_KillWire3 orig, int i, int j)
        {
            SearchCircuitsAround(i, j, VanillaWireType.Green);
            return orig(i, j);
        }

        private bool On_WorldGen_KillWire4(On_WorldGen.orig_KillWire4 orig, int i, int j)
        {
            SearchCircuitsAround(i, j, VanillaWireType.Yellow);
            return orig(i, j);
        }
    }
}
