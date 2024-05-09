using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Macrocosm.Common.Bases.Items
{
    internal interface ILiquidContainer
    {
        public float Amount { get; set; }   
        public float Capacity { get; }

        public bool Full => Amount >= Capacity;
        public bool Empty => Amount <= 0f;

        public float Percent
        {
            get => MathHelper.Clamp(Amount / Capacity, 0, 1);
            set => Amount = MathHelper.Clamp(value * Capacity, 0, Capacity);
        }
    }
}
