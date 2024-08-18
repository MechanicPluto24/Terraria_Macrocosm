using System;

namespace Macrocosm.Common.DataStructures
{
    public enum FuelPotency
    {
        None = 0,
        VeryLow = 1,
        Low = 2,
        Medium = 3,
        High = 4,
        VeryHigh = 5
    }

    /// <summary>
    /// Fuel data of an item
    /// </summary>
    /// <param name="potency"> The <see cref="FuelPotency"/> </param>
    /// <param name="consumtionRate"> The consumtion rate, in ticks </param>
    public readonly struct FuelData
    {
        private readonly FuelPotency potency;
        private readonly int consumtionRate;

        private readonly Func<FuelPotency> getPotency;

        public FuelData() { }

        public FuelData(FuelPotency potency, int consumtionRate)
        {
            this.potency = potency;
            this.consumtionRate = consumtionRate;
        }

        public FuelData(Func<FuelPotency> getPotency, int consumtionRate) : this()
        {
            this.getPotency = getPotency;
            this.consumtionRate = consumtionRate;
        }

        public FuelPotency Potency => getPotency is not null ? getPotency() : potency;
        public int ConsumtionRate => consumtionRate;
    }
}
