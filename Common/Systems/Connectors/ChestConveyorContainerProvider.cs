using Humanizer;
using Macrocosm.Common.Sets;
using Macrocosm.Common.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ObjectData;

namespace Macrocosm.Common.Systems.Connectors;

public class ChestConveyorContainerProvider : IConveyorContainerProvider<Chest>
{
    public IEnumerable<Chest> EnumerateContainers() => Main.chest.Where(c => c != null);

    public bool TryGetContainer(Point16 tilePos, out Chest container) => Utility.TryGetChest(tilePos, out container);

    public ConveyorNode GetConveyorNode(Point16 tilePos, ConveyorPipeType type)
    {
        ConveyorData data = Main.tile[tilePos].Get<ConveyorData>();
        if (data.IsValidForConveyorNode(type) && TryGetContainer(tilePos, out Chest chest))
            return new ConveyorNode(chest, data, type, tilePos, GetConnectionPositions(chest));

        return null;
    }

    public IEnumerable<Point16> GetConnectionPositions(Chest chest)
    {
        if (!WorldGen.InWorld(chest.x, chest.y))
            yield break;

        Tile tile = Main.tile[chest.x, chest.y];
        int sizeX = 0;
        int sizeY = 0;

        if (TileID.Sets.BasicChest[tile.TileType])
        {
            sizeX = sizeY = 2;
        }
        else if (TileID.Sets.BasicDresser[tile.TileType])
        {
            sizeX = 3;
            sizeY = 2;
        }
        else if (TileSets.CustomContainer[tile.TileType])
        {
            var data = TileObjectData.GetTileData(tile);
            sizeX = data?.Width ?? 1;
            sizeY = data?.Height ?? 1;
        }

        for (int x = 0; x < sizeX; x++)
        {
            for (int y = 0; y < sizeY; y++)
            {
                yield return new Point16(chest.x + x, chest.y + y);
            }
        }
    }
}
