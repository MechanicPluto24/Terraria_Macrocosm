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
            float inputPower = 0;
            float consumedPower = 0;
            foreach (var machine in machines)
            {
                if (machine.MachineType is MachineType.Generator)
                    inputPower += machine.Power;

                if (machine.MachineType is MachineType.Consumer)
                    consumedPower += machine.Power;
            }

            bool enoughPower = inputPower >= consumedPower;
            float powerShortageFactor = enoughPower ? 1 : inputPower / consumedPower;
            float totalPowerDistributedToConsumers = 0;

            foreach (var consumer in machines.Where(machine => machine.MachineType is MachineType.Consumer))
            {
                if (enoughPower && !consumer.PoweredOn && consumer.CanAutoPowerOn)
                    consumer.PowerOn();

                if (!enoughPower && consumer.PoweredOn && consumer.CanAutoPowerOff)
                    consumer.PowerOff();

                if (consumer.Operating)
                {
                    float powerReceived = consumer.Power * powerShortageFactor;
                    consumer.ActivePower = powerReceived;
                    totalPowerDistributedToConsumers += powerReceived;
                }
                else
                {
                    consumer.ActivePower = 0;
                }
            }

            foreach (var generator in machines.Where(machine => machine.MachineType is MachineType.Generator))
            {
                float remainingPower = generator.Power - (totalPowerDistributedToConsumers / inputPower * generator.Power);
                generator.ActivePower = generator.PoweredOn ? remainingPower : 0;

                if (generator.Power > 0 && !generator.PoweredOn && generator.CanAutoPowerOn)
                    generator.PowerOn();

                if (generator.Power <= 0 && generator.PoweredOn && generator.CanAutoPowerOff)
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
