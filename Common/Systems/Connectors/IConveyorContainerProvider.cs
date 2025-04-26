using System.Collections.Generic;
using System.Linq;
using Terraria.DataStructures;

namespace Macrocosm.Common.Systems.Connectors
{
    public interface IConveyorContainerProvider<T> where T : class
    {
        public IEnumerable<T> EnumerateContainers();
        public bool TryGetContainer(Point16 tilePos, out T container);
        public IEnumerable<ConveyorNode> GetAllConveyorNodes(T container);
        public IEnumerable<Point16> GetConnectionPositions(T container);
        public ConveyorNode GetConveyorNode(Point16 tilePos, ConveyorPipeType type)
        {
            if (TryGetContainer(tilePos, out T container))
                return GetAllConveyorNodes(container).FirstOrDefault(n => n.Type == type && n.Position == tilePos);
 
            return null;
        }
    }

}
