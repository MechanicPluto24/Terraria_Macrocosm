using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;

namespace Macrocosm.Common.Bases.Items
{
    /// <summary>
    /// Interface for ModItems that apply a custom duration of Potion Sickness. 
    /// </summary>
    public interface IPotionDelayConsumable
    {
        /// <summary> The potion delay of this <see cref="IPotionDelayConsumable"/>, in ticks. </summary>
        public int PotionDelay { get; }
    }
}
