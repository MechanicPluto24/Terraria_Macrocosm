using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Macrocosm.Common.Systems.Power
{
    public interface IOxygenSource
    {
        public bool IsProvidingOxygen { get; }
    }
}
