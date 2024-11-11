using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Macrocosm.Common.Systems.Power
{
    public class Circuit : IEnumerable<MachineTE>
    {
        private readonly HashSet<MachineTE> machines = new();

        public int NodeCount => machines.Count;

        public Circuit() { }

        public void Add(MachineTE machine)
        {
            machines.Add(machine);
        }

        public void Remove(MachineTE machine)
        {
            machines.Remove(machine);
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
}
