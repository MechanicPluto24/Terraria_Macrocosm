using Macrocosm.Common.DataStructures;
using Macrocosm.Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Macrocosm.Common.Systems.Power;

public class WireCircuit : Circuit<MachineTE>
{
    public WireType WireType { get; }

    public WireCircuit(WireType wireType)
    {
        WireType = wireType;
    }

    public override void Merge(Circuit<MachineTE> other)
    {
        if (other is WireCircuit wireOther && wireOther.WireType == WireType)
        {
            foreach (var node in wireOther.nodes)
            {
                Add(node);
                if (node is MachineTE machine)
                {
                    machine.Circuit = this;
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
                totalConsumerDemand += consumer.MaxPower;
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
                float circuitPowerFactor = totalAvailablePower / totalConsumerDemand;

                DistributePowerToConsumers(consumers, circuitPowerFactor);
                DrainAllBatteries(batteries);
            }
        }
    }


    private void DistributePowerToConsumers(List<ConsumerTE> consumers, float circuitPowerFactor)
    {
        foreach (var consumer in consumers)
        {
            consumer.InputPower = consumer.MaxPower * circuitPowerFactor;
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
