using Macrocosm.Common.Utils;
using Microsoft.Xna.Framework;
using System;
using Terraria;

namespace Macrocosm.Common.DataStructures
{
    public enum FuelPotency
    {
        None = 0,
        VeryLow = 1,
        Low = 2,
        Medium = 3,
        High = 4,
        VeryHigh = 5,
        SolarFragmentHigh = 10
    }

    /// <summary>
    /// Fuel data of an item
    /// </summary>
    /// <param name="potency"> The <see cref="FuelPotency"/> </param>
    /// <param name="consumptionRate"> The consumption rate, in ticks </param>
    public readonly struct FuelData
    {
        private readonly FuelPotency potency;
        private readonly int consumptionRate;

        public FuelPotency Potency => potency;
        public int ConsumptionRate => consumptionRate;

        public FuelData() { }

        public FuelData(FuelPotency potency, int consumptionRate, bool critter = false)
        {
            this.potency = potency;
            this.consumptionRate = consumptionRate;

            if (critter)
                CanBurn = (item, pos) => Utility.CanHurtCritterAroundPosition(pos, 800);
        }

        public delegate bool CanBurnDelegate(Item item, Vector2 position);
        public delegate void BurnDelegate(Item item, Vector2 position);

        public CanBurnDelegate CanBurn { get; init; } = (_, _) => true;
        public BurnDelegate Burning { get; init; } = (_, _) => { };
        public BurnDelegate OnConsumed { get; init; } = (_, _) => { };

    }
}
