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

        public void Solve()
        {
            float totalGeneratedPower = 0;
            float totalConsumedPower = 0;
            foreach (var machine in machines)
            {
                if (machine.IsGenerator)
                    totalGeneratedPower += machine.GeneratedPower;

                if (machine.IsConsumer)
                    totalConsumedPower += machine.ConsumedPower;
            }

            bool enoughPower = totalGeneratedPower >= totalConsumedPower;
            float powerShortageFactor = enoughPower ? 1 : totalGeneratedPower / totalConsumedPower;
            float totalPowerDistributedToConsumers = 0;

            foreach (var consumer in machines.Where(machine => machine.IsConsumer))
            {
                if (enoughPower && !consumer.PoweredOn)
                    consumer.PowerOn();

                if (!enoughPower && consumer.PoweredOn)
                    consumer.PowerOff();

                if (consumer.Operating)
                {
                    float powerReceived = consumer.ConsumedPower * powerShortageFactor;
                    consumer.ActivePower = powerReceived;
                    totalPowerDistributedToConsumers += powerReceived;
                }
                else
                {
                    consumer.ActivePower = 0;
                }
            }

            foreach (var generator in machines.Where(machine => machine.IsGenerator))
            {
                float remainingPower = generator.GeneratedPower - (totalPowerDistributedToConsumers / totalGeneratedPower * generator.GeneratedPower);
                generator.ActivePower = generator.PoweredOn ? remainingPower : 0;

                if ((generator.GeneratedPower > 0 && !generator.PoweredOn))
                    generator.PowerOn();

                if ((generator.GeneratedPower <= 0 && generator.PoweredOn))
                    generator.PowerOff();
            }
        }

        public IEnumerator<MachineTE> GetEnumerator()
        {
            return ((IEnumerable<MachineTE>)machines).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)machines).GetEnumerator();
        }
    }
}
