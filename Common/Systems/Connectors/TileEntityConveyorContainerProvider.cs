using Macrocosm.Common.Storage;
using Macrocosm.Common.Systems.Power;
using Macrocosm.Common.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.DataStructures;

namespace Macrocosm.Common.Systems.Connectors
{
    public class TileEntityConveyorContainerProvider : IConveyorContainerProvider<TileEntity>
    {
        public IEnumerable<TileEntity> EnumerateContainers() => TileEntity.ByID.Values.Where(te => te is IInventoryOwner); // TODO: extend to TEs with simple Item[] inventory?

        public bool TryGetContainer(Point16 tilePos, out TileEntity container) => Utility.TryGetTileEntityAs(tilePos, out container);

        public ConveyorNode GetConveyorNode(Point16 tilePos, ConveyorPipeType type)
        {
            var data = Main.tile[tilePos].Get<ConveyorData>();
            if (data.HasPipe(type) && (data.Inlet || data.Outlet) && TryGetContainer(tilePos, out TileEntity te) && te is IInventoryOwner)
                return new ConveyorNode(te, data, type, tilePos, GetConnectionPositions(te));

            return null;
        }

        public IEnumerable<Point16> GetConnectionPositions(TileEntity te) => te.GetTilePositions();
    }
}
