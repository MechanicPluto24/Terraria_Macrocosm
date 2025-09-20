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

namespace Macrocosm.Common.Systems.Connectors;

public class TileEntityInventoryOwnerConveyorContainerProvider : IConveyorContainerProvider<TileEntity>
{
    public IEnumerable<TileEntity> EnumerateContainers() => TileEntity.ByID.Values.Where(te => te is IInventoryOwner);

    public bool TryGetContainer(Point16 tilePos, out TileEntity te) => TileEntity.TryGet(tilePos, out te);

    public ConveyorNode GetConveyorNode(Point16 tilePos, ConveyorPipeType type)
    {
        ConveyorData data = Main.tile[tilePos].Get<ConveyorData>();
        if (data.IsValidForConveyorNode(type) && TryGetContainer(tilePos, out TileEntity te) && te is IInventoryOwner)
            return new ConveyorNode(te, data, type, tilePos, GetConnectionPositions(te));

        return null;
    }

    public IEnumerable<Point16> GetConnectionPositions(TileEntity te) => te.GetTilePositions();
}
