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

public class TileEntityInventoryOwnerPipeContainerProvider : IPipeContainerProvider<TileEntity>
{
    public IEnumerable<TileEntity> EnumerateContainers() => TileEntity.ByID.Values.Where(te => te is IInventoryOwner);

    public bool TryGetContainer(Point16 tilePos, out TileEntity te) => TileEntity.TryGet(tilePos, out te);

    public PipeNode GetPipeNode(Point16 tilePos, PipeType type)
    {
        PipeData data = Main.tile[tilePos].Get<PipeData>();
        if (data.IsValidForPipeNode(type) && TryGetContainer(tilePos, out TileEntity te) && te is IInventoryOwner)
            return new PipeNode(te, data, type, tilePos, GetConnectionPositions(te));

        return null;
    }

    public IEnumerable<Point16> GetConnectionPositions(TileEntity te) => te.GetTilePositions();
}
