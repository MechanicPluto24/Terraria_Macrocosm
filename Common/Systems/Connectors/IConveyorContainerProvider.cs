using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.DataStructures;
using static Macrocosm.Common.Utils.Utility.AStar;

namespace Macrocosm.Common.Systems.Connectors
{
    public interface IConveyorContainerProvider<T> where T : class
    {
        public IEnumerable<T> EnumerateContainers();
        public bool TryGetContainer(Point16 tilePos, out T container);
        public IEnumerable<ConveyorNode> GetAllConveyorNodes(T container);
        public ConveyorNode GetConveyorNode(Point16 tilePos, ConveyorPipeType type)
            => TryGetContainer(tilePos, out T container) ? GetAllConveyorNodes(container).FirstOrDefault(n => n.Position == tilePos && n.Type == type) : null;
    }

}
