using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.WorldBuilding;

namespace Macrocosm.Common.WorldGeneration.Structures
{
    public abstract class LunarianHouseBuilder : MacrocosmHouseBuilder
    {
        // TODO: some common configuration
        protected LunarianHouseBuilder(Point origin, StructureMap structures) : base(origin, structures)
        {
        }
    }
}
