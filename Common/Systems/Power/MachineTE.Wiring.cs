using Macrocosm.Common.Config;
using Macrocosm.Common.DataStructures;
using Macrocosm.Common.Enums;
using Macrocosm.Common.Systems.Connectors;
using Macrocosm.Common.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.DataStructures;

namespace Macrocosm.Common.Systems.Power
{
    public partial class MachineTE
    {
        private class WireCircuit : Circuit<MachineTE>
        {
            public WireType WireType { get; }

            public WireCircuit(WireType wireType)
            {
                WireType = wireType;
            }

            public override void Merge(Circuit<MachineTE> other)
            {
                if (other is WireCircuit otherWireCircuit && otherWireCircuit.WireType == WireType)
                {
                    foreach (var node in otherWireCircuit.nodes)
                    {
                        Add(node);
                        if (node is MachineTE machine)
                        {
                            machine.wireCircuit = this;
                        }
                    }
                    other.Clear();
                }
            }

            public override void Solve(int updateRate)
            {
                float totalGeneratorOutput = 0f;
                float totalConsumerDemand = 0f;
                float totalBatteryStoredEnergy = 0f;
                float totalBatteryCapacity = 0f;

                var generators = new List<GeneratorTE>();
                var consumers = new List<ConsumerTE>();
                var batteries = new List<BatteryTE>();

                foreach (var node in nodes)
                {
                    if (node is GeneratorTE generator)
                    {
                        generators.Add(generator);
                        totalGeneratorOutput += generator.GeneratedPower;
                    }
                    else if (node is ConsumerTE consumer)
                    {
                        consumers.Add(consumer);
                        totalConsumerDemand += consumer.RequiredPower;
                    }
                    else if (node is BatteryTE battery)
                    {
                        batteries.Add(battery);
                        totalBatteryStoredEnergy += battery.StoredEnergy;
                        totalBatteryCapacity += battery.EnergyCapacity;
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
                float totalEnergyToStore = excessPower * deltaTime; // kW * s = kJ

                var availableBatteries = batteries.Where(b => b.StoredEnergy < b.EnergyCapacity).ToList();

                while (totalEnergyToStore > 0f && availableBatteries.Count > 0)
                {
                    float energyPerBattery = totalEnergyToStore / availableBatteries.Count;
                    bool anyStored = false;

                    foreach (var battery in availableBatteries.ToList()) 
                    {
                        float capacityLeft = battery.EnergyCapacity - battery.StoredEnergy;
                        float energyToStore = Math.Min(energyPerBattery, capacityLeft);

                        if (energyToStore > 0f)
                        {
                            battery.PowerFlow = energyToStore;
                            battery.StoredEnergy += energyToStore;
                            totalEnergyToStore -= energyToStore;
                            anyStored = true;

                            if (battery.StoredEnergy >= battery.EnergyCapacity)
                            {
                                availableBatteries.Remove(battery);
                                battery.PowerFlow = 0;
                            }
                        }
                    }

                    if (!anyStored)
                        break;
                }
            }

            private void DrawPowerFromBatteries(List<BatteryTE> batteries, float powerNeeded, int updateRate)
            {
                float deltaTime = updateRate / 60f;
                float totalEnergyNeeded = powerNeeded * deltaTime; // kW * s = kJ

                var availableBatteries = batteries.Where(b => b.StoredEnergy > 0f).ToList();

                while (totalEnergyNeeded > 0f && availableBatteries.Count > 0)
                {
                    float energyPerBattery = totalEnergyNeeded / availableBatteries.Count;
                    bool anyDrawn = false;

                    foreach (var battery in availableBatteries.ToList()) 
                    {
                        float energyAvailable = battery.StoredEnergy;
                        float energyToDraw = Math.Min(energyPerBattery, energyAvailable);

                        if (energyToDraw > 0f)
                        {
                            battery.PowerFlow = -energyToDraw;
                            battery.StoredEnergy -= energyToDraw;
                            totalEnergyNeeded -= energyToDraw;
                            anyDrawn = true;

                            if (battery.StoredEnergy <= 0f)
                            {
                                availableBatteries.Remove(battery);
                                battery.PowerFlow = 0;
                            }
                        }
                    }

                    if (!anyDrawn)
                        break;
                }
            }


            private void DrainAllBatteries(List<BatteryTE> batteries)
            {
                foreach (var battery in batteries)
                {
                    battery.PowerFlow = 0f;
                    battery.StoredEnergy = 0f;
                }
            }
        }

        private class ConveyorCircuit : Circuit<MachineTE>
        {
            public override void Merge(Circuit<MachineTE> other)
            {
                if (other is ConveyorCircuit conveyorOther)
                {
                    foreach (var node in conveyorOther.nodes)
                    {
                        Add(node);
                        if (node is MachineTE machine)
                        {
                            machine.conveyorCircuit = this;
                        }
                    }
                    other.Clear();
                }
            }

            public override void Solve(int updateRate)
            {
                foreach (var node in nodes)
                {
                    if (node is MachineTE machine)
                    {
                        // Implement your item transfer logic here
                        // machine.TransferItems();
                    }
                }
            }
        }

        private WireCircuit wireCircuit = null;
        private ConveyorCircuit conveyorCircuit = null;

        private static List<WireType> WireTypes => Enum.GetValues<WireType>().ToList();

        private static int buildTimer = 0;
        private void BuildCircuits()
        {
            if (buildTimer++ >= ServerConfig.Instance.CircuitSearchUpdateRate)
            {
                RefreshCircuits();
                BuildWireCircuits();
                BuildConveyorCircuits();
                buildTimer = 0;
            }
        }

        private static void RefreshCircuits()
        {
            foreach (var te in ByID.Values)
            {
                if (te is MachineTE machine)
                {
                    machine.wireCircuit = null;
                    machine.conveyorCircuit = null;
                }
            }
        }

        private static void BuildWireCircuits()
        {
            foreach (WireType wireType in WireTypes)
            {
                foreach (var te in ByID.Values)
                {
                    if (te is MachineTE machine)
                    {
                        if (machine.wireCircuit != null)
                            continue;

                        ConnectionSearch<MachineTE> connectionSearch = new(
                            connectionCheck: position => Main.tile[position].HasWire(wireType),
                            retrieveNode: position => Utility.TryGetTileEntityAs<MachineTE>(position.X, position.Y, out var m) ? m : null
                        );

                        HashSet<MachineTE> connectedNodes = connectionSearch.FindConnectedNodes(machine.GetConnectionPositions());

                        if (connectedNodes.Count == 0)
                            continue;

                        HashSet<WireCircuit> existingCircuits = new();
                        foreach (var node in connectedNodes)
                        {
                            if (node is MachineTE m && m.wireCircuit != null)
                            {
                                existingCircuits.Add(m.wireCircuit);
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
                            if (node is MachineTE m && m.wireCircuit == null)
                            {
                                newCircuit.Add(m);
                                m.wireCircuit = newCircuit;
                            }
                        }

                        if (machine.wireCircuit == null)
                        {
                            newCircuit.Add(machine);
                            machine.wireCircuit = newCircuit;
                        }
                    }
                }
            }
        }

        private static void BuildConveyorCircuits()
        {
            foreach (var te in ByID.Values)
            {
                if (te is MachineTE machine)
                {
                    if (machine.conveyorCircuit != null)
                    {
                        continue;
                    }

                    ConnectionSearch<MachineTE> connectionSearch = new(
                        connectionCheck: position => ConnectorSystem.Map[position].Conveyor,
                        retrieveNode: position => Utility.TryGetTileEntityAs<MachineTE>(position.X, position.Y, out var m) ? m : null
                    );

                    HashSet<MachineTE> connectedNodes = connectionSearch.FindConnectedNodes(machine.GetConnectionPositions());

                    if (connectedNodes.Count == 0)
                        continue;

                    HashSet<ConveyorCircuit> existingCircuits = new();
                    foreach (var node in connectedNodes)
                    {
                        if (node is MachineTE m && m.conveyorCircuit != null)
                        {
                            existingCircuits.Add(m.conveyorCircuit);
                        }
                    }

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
                        if (node is MachineTE m && m.conveyorCircuit == null)
                        {
                            newCircuit.Add(m);
                            m.conveyorCircuit = newCircuit;
                        }
                    }

                    if (machine.conveyorCircuit == null)
                    {
                        newCircuit.Add(machine);
                        machine.conveyorCircuit = newCircuit;
                    }
                }
            }
        }

        private static int solveTimer = 0;
        private static void SolveCircuits()
        {
            if (solveTimer++ >= ServerConfig.Instance.CircuitSolveUpdateRate)
            {
                HashSet<WireCircuit> processedWireCircuits = new();
                foreach (WireType wireType in WireTypes)
                {
                    foreach (var te in ByID.Values)
                    {
                        if (te is MachineTE machine)
                        {
                            if (machine.wireCircuit != null)
                            {
                                if (!processedWireCircuits.Contains(machine.wireCircuit))
                                {
                                    //Main.NewText($"{machine.Name}{machine.ID}: {machine.wireCircuit.NodeCount}");
                                    machine.wireCircuit.Solve(ServerConfig.Instance.CircuitSolveUpdateRate);
                                    processedWireCircuits.Add(machine.wireCircuit);
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

            HashSet<ConveyorCircuit> processedConveyorCircuits = new();
            foreach (var te in ByID.Values)
            {
                if (te is MachineTE machine)
                {
                    var circuit = machine.conveyorCircuit;
                    if (circuit != null && !processedConveyorCircuits.Contains(circuit))
                    {
                        circuit.Solve(updateRate: 1);
                        processedConveyorCircuits.Add(circuit);
                    }
                }
            }
        }

        protected virtual IEnumerable<Point16> GetConnectionPositions()
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

        public void TransferItems()
        {
        }
    }
}
