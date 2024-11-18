using Macrocosm.Common.Enums;
using Macrocosm.Common.Utils;
using Microsoft.Xna.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.IO;

namespace Macrocosm.Common.Systems.Power
{
    public partial class MachineTE
    {
        private Dictionary<WireType, Circuit> circuits = new();

        private Circuit GetCircuit(WireType wireType)
        {
            circuits.TryGetValue(wireType, out var circuit);
            return circuit;
        }

        private void SetCircuit(WireType wireType, Circuit circuit)
        {
            circuits[wireType] = circuit;
        }

        private class Circuit : IEnumerable<MachineTE>
        {
            private readonly HashSet<MachineTE> machines = new();
            public WireType WireType { get; }
            public Circuit(WireType wireType)
            {
                WireType = wireType;
            }

            public int NodeCount => machines.Count;

            public void Add(MachineTE machine)
            {
                machines.Add(machine);
            }

            public void Remove(MachineTE machine)
            {
                machines.Remove(machine);
            }

            public bool Contains(MachineTE machine)
            {
                return machines.Contains(machine);
            }

            public bool IsEmpty => machines.Count == 0;

            public void Merge(Circuit other, WireType wireType)
            {
                foreach (var machine in other.machines)
                {
                    Add(machine);
                    machine.SetCircuit(wireType, this);
                }
                other.machines.Clear();
            }

            public void Solve(int updateRate)
            {
                float totalGeneratorOutput = 0f;
                float totalConsumerDemand = 0f;
                float totalBatteryStoredEnergy = 0f;
                float totalBatteryCapacity = 0f;

                var generators = new List<GeneratorTE>();
                var consumers = new List<ConsumerTE>();
                var batteries = new List<BatteryTE>();

                foreach (var machine in machines)
                {
                    switch (machine)
                    {
                        case GeneratorTE generator:
                            generators.Add(generator);
                            totalGeneratorOutput += generator.GeneratedPower;
                            break;
                        case ConsumerTE consumer:
                            consumers.Add(consumer);
                            totalConsumerDemand += consumer.RequiredPower;
                            break;
                        case BatteryTE battery:
                            batteries.Add(battery);
                            totalBatteryStoredEnergy += battery.StoredEnergy;
                            totalBatteryCapacity += battery.EnergyCapacity;
                            break;
                    }
                }

                float netPower = totalGeneratorOutput - totalConsumerDemand;

                if (netPower >= 0f)
                {
                    DistributePowerToConsumers(consumers, 1f);

                    float excessPower = netPower;
                    StoreExcessPowerInBatteries(batteries, excessPower, updateRate);
                }
                else
                {
                    float powerNeeded = -netPower;

                    if (totalBatteryStoredEnergy >= powerNeeded)
                    {
                        DistributePowerToConsumers(consumers, 1f);
                        DrawPowerFromBatteries(batteries, powerNeeded, updateRate);
                    }
                    else
                    {
                        float totalAvailablePower = totalGeneratorOutput + totalBatteryStoredEnergy;
                        float powerFactor = totalAvailablePower / totalConsumerDemand;

                        DistributePowerToConsumers(consumers, powerFactor);
                        DrainAllBatteries(batteries);
                    }
                }
            }

            private void DistributePowerToConsumers(List<ConsumerTE> consumers, float powerFactor)
            {
                foreach (var consumer in consumers)
                {
                    consumer.InputPower = consumer.RequiredPower * powerFactor;
                }
            }

            private void StoreExcessPowerInBatteries(List<BatteryTE> batteries, float excessPower, int updateRate)
            {
                float deltaTime = updateRate / 60f;
                float energyToStore = excessPower * deltaTime; // kW * s = kJ

                foreach (var battery in batteries)
                {
                    float capacityLeft = battery.EnergyCapacity - battery.StoredEnergy;
                    float energyStored = Math.Min(energyToStore, capacityLeft);

                    battery.StoredEnergy += energyStored;
                    energyToStore -= energyStored;

                    if (energyToStore <= 0f)
                        break;
                }
            }

            private void DrawPowerFromBatteries(List<BatteryTE> batteries, float powerNeeded, int updateRate)
            {
                float deltaTime = updateRate / 60f;
                float energyNeeded = powerNeeded * deltaTime; // kW * s = kJ

                foreach (var battery in batteries)
                {
                    float energyAvailable = battery.StoredEnergy;
                    float energyDrawn = Math.Min(energyNeeded, energyAvailable);

                    battery.StoredEnergy -= energyDrawn;
                    energyNeeded -= energyDrawn;

                    if (energyNeeded <= 0f)
                        break;
                }
            }

            private void DrainAllBatteries(List<BatteryTE> batteries)
            {
                foreach (var battery in batteries)
                {
                    battery.StoredEnergy = 0f;
                }
            }

            public IEnumerator<MachineTE> GetEnumerator()
            {
                return machines.GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
        }
        private class WireSearch
        {
            private readonly HashSet<Point16> visited;
            private readonly Queue<Point16> toProcess;
            private readonly HashSet<MachineTE> connectedMachines;
            private readonly WireType wireType;

            public WireSearch(WireType wireType)
            {
                this.wireType = wireType;
                visited = new HashSet<Point16>();
                toProcess = new Queue<Point16>();
                connectedMachines = new HashSet<MachineTE>();
            }

            public HashSet<MachineTE> FindConnectedMachines(IEnumerable<Point16> startingPositions)
            {
                foreach (Point16 position in startingPositions)
                {
                    if (Utility.HasWire(Main.tile[position.X, position.Y], wireType))
                    {
                        toProcess.Enqueue(position);
                        visited.Add(position);
                    }
                }

                while (toProcess.Count > 0)
                {
                    Point16 current = toProcess.Dequeue();

                    // Check for connected machines at this position
                    if (TileEntity.ByPosition.TryGetValue(current, out TileEntity te))
                    {
                        if (te is MachineTE machine)
                        {
                            connectedMachines.Add(machine);
                        }
                    }

                    // Process neighboring tiles
                    ProcessWire(current.X, current.Y - 1); // Up
                    ProcessWire(current.X, current.Y + 1); // Down
                    ProcessWire(current.X - 1, current.Y); // Left
                    ProcessWire(current.X + 1, current.Y); // Right
                }

                return connectedMachines;
            }

            private void ProcessWire(int x, int y)
            {
                if (!WorldGen.InWorld(x, y))
                    return;

                Point16 position = new(x, y);

                if (visited.Contains(position))
                    return;

                Tile tile = Main.tile[x, y];

                if (tile != null && Utility.HasWire(tile, wireType))
                {
                    visited.Add(position);
                    toProcess.Enqueue(position);

                    //if (Main.rand.NextBool(10))
                    //    Dust.NewDustPerfect(position.ToWorldCoordinates() + new Vector2(0, 8f), DustID.Electric, Velocity: null, Scale: 0.3f);
                }
            }
        }

        private void BuildCircuit()
        {
            //foreach (WireType wireType in Enum.GetValues(typeof(WireType)))
            WireType wireType = WireType.Red;
            {
                WireSearch wireSearch = new(wireType);

                HashSet<MachineTE> connectedMachines = wireSearch.FindConnectedMachines(GetWirePositions());

                HashSet<Circuit> connectedCircuits = new();
                foreach (var machine in connectedMachines)
                {
                    var machineCircuit = machine.GetCircuit(wireType);
                    if (machineCircuit != null)
                    {
                        connectedCircuits.Add(machineCircuit);
                    }
                }

                // Remove this machine from any previous circuit for this wire type
                var currentCircuit = GetCircuit(wireType);
                if (currentCircuit != null)
                {
                    currentCircuit.Remove(this);
                    connectedCircuits.Remove(currentCircuit);
                }

                // If there are existing circuits, merge them
                Circuit circuit;
                if (connectedCircuits.Count > 0)
                {
                    circuit = connectedCircuits.First();
                    foreach (var otherCircuit in connectedCircuits.Skip(1))
                    {
                        if (otherCircuit != circuit)
                            circuit.Merge(otherCircuit, wireType);
                    }
                }
                else
                {
                    circuit = new Circuit(wireType);
                }

                // Add this machine and connected machines to the circuit
                circuit.Add(this);
                SetCircuit(wireType, circuit);

                foreach (var machine in connectedMachines)
                {
                    if (machine != this)
                    {
                        circuit.Add(machine);
                        machine.SetCircuit(wireType, circuit);
                    }
                }
            }
        }

        private static int updateTimer = 0;
        private static int updateRate = 1;
        private static void SolveAllCircuits()
        {
            if (updateTimer++ >= updateRate)
            {
                var processedCircuits = new HashSet<Circuit>();

                foreach (var te in ByID.Values)
                {
                    if (te is MachineTE machine)
                    {
                        //foreach (WireType wireType in Enum.GetValues(typeof(WireType)))
                        WireType wireType = WireType.Red;
                        {
                            var circuit = machine.GetCircuit(wireType);
                            if (circuit != null && !processedCircuits.Contains(circuit))
                            {
                                circuit.Solve(updateRate);
                                processedCircuits.Add(circuit);
                            }
                        }
                    }
                }
                updateTimer = 0;
            }
        }

        protected virtual IEnumerable<Point16> GetWirePositions()
        {
            int startX = Position.X;
            int startY = Position.Y;
            int width = MachineTile.Width;
            int height = MachineTile.Height;

            for (int x = startX; x < startX + width; x++)
            {
                for (int y = startY; y < startY + height; y++)
                {
                    yield return new Point16(x, y);
                }
            }
        }
    }
}
