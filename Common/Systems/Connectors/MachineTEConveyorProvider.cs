using Macrocosm.Common.Storage;
using Macrocosm.Common.Systems.Power;
using Macrocosm.Common.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.DataStructures;

namespace Macrocosm.Common.Systems.Connectors
{
    public class MachineTEConveyorProvider : IConveyorContainerProvider<MachineTE>
    {
 
        public IEnumerable<ConveyorNode> GetAllConveyorNodes(MachineTE container)
        {
            if (container is not IInventoryOwner inventoryOwner)
                yield break;

            IEnumerable<Point16> coords = container.GetConnectionPositions();
            foreach (Point16 pos in coords)
            {
                ConveyorData data = Main.tile[pos.X, pos.Y].Get<ConveyorData>();
                if (!data.Inlet && !data.Outlet)
                    continue;

                for (ConveyorPipeType type = 0; type < ConveyorPipeType.Count; type++)
                {
                    if (!data.HasPipe(type))
                        continue;

                    yield return new ConveyorNode(type, data, pos, inventoryOwner);
                }
            }
        }

        public bool TryGetContainer(Point16 tilePos, out MachineTE container)
        {
            if (Utility.TryGetTileEntityAs(tilePos, out MachineTE te))
            {
                container = te;
                return true;
            }

            container = null;
            return false;
        }

        public IEnumerable<MachineTE> EnumerateContainers()
        {
            foreach (var te in TileEntity.ByID.Values.Where(te => te is MachineTE machine and IInventoryOwner))
            {
                 yield return te as MachineTE;
            }
        }
    }
}
