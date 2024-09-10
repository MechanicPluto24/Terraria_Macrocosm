using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Macrocosm.Common.Bases.Items
{
    // TODO: add properties for slot placement (AllowedInOilRefinery, AllowedInRocket, etc.) 
    internal interface IFillableContainer
    {
        public float Fill { get; set; }

        public bool Full { get; }
        public bool Empty { get; }
    }
}
