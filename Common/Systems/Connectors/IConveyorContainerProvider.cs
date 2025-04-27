using System.Collections.Generic;
using System.Linq;
using Terraria.DataStructures;

namespace Macrocosm.Common.Systems.Connectors
{
    public interface IConveyorContainerProvider<T> where T : class
    {
        /// <summary> Used to get all valid containers in the world </summary>
        public IEnumerable<T> EnumerateContainers();

        /// <summary> Used to fetch the container at the given position </summary>
        public bool TryGetContainer(Point16 tilePos, out T container);

        /// <summary>
        /// Used to get a single valid <see cref="ConveyorNode"/> at the given position
        /// <br/> Returns <see langword="null"/> if none found
        /// </summary>
        public ConveyorNode GetConveyorNode(Point16 tilePos, ConveyorPipeType type);

        /// <summary> Used to get all tile positions of this container </summary>
        public IEnumerable<Point16> GetConnectionPositions(T container);
    }
}
