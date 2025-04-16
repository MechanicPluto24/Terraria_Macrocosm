using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.DataStructures;

namespace Macrocosm.Common.Systems.Connectors
{
    public interface IConveyorContainerProvider<T> where T : class
    {
        IEnumerable<T> EnumerateContainers();
        IEnumerable<ConveyorNode> GetConveyorNodes(T container);
        bool TryGetContainer(Point16 tilePos, out T container);
    }

}
