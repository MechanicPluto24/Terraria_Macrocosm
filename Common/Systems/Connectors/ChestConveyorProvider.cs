using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.DataStructures;
using Terraria;
using Macrocosm.Common.Utils;
using Terraria.ID;
using Macrocosm.Common.Sets;
using Terraria.ObjectData;
using Microsoft.Xna.Framework;
using static AssGen.Assets;

namespace Macrocosm.Common.Systems.Connectors
{
    public class ChestConveyorProvider : IConveyorContainerProvider<Chest>
    {
        public IEnumerable<Chest> EnumerateContainers() => Main.chest.Where(c => c != null);
        public IEnumerable<ConveyorNode> GetAllConveyorNodes(Chest chest)
        {
            foreach (var pos in GetConnectionPositions(chest))
            {
                var data = Main.tile[pos.X, pos.Y].Get<ConveyorData>();
                if (!data.Inlet && !data.Outlet)
                    continue;

                for (ConveyorPipeType type = 0; type < ConveyorPipeType.Count; type++)
                {
                    if (!data.HasPipe(type))
                        continue;

                    yield return new ConveyorNode(type, data, pos, chest);
                }
            }
        }

        public IEnumerable<Point16> GetConnectionPositions(Chest chest)
        {
            if (Utility.CoordinatesOutOfBounds(chest.x, chest.y))
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
                sizeX = data != null ? data.Width : 1;
                sizeY = data != null ? data.Height : 1;
            }

            for (int dx = 0; dx < sizeX; dx++)
            {
                for (int dy = 0; dy < sizeY; dy++)
                {
                    yield return new Point16(chest.x + dx, chest.y + dy);
                }
            }
        }

        public bool TryGetContainer(Point16 tilePos, out Chest container)
        {
            int index = Chest.FindChest(tilePos.X, tilePos.Y);
            if (index >= 0)
            {
                container = Main.chest[index];
                return true;
            }

            container = null;
            return false;
        }
    }

}
