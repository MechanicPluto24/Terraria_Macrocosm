using Macrocosm.Common.TileFrame;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria.WorldBuilding;

namespace Macrocosm.Common.WorldGeneration.Structures
{
    public class VortexHouseBuilder : LunarianHouseBuilder
    {
        public VortexHouseBuilder(Point origin, StructureMap structures) : base(origin, structures)
        {
        }

        protected override ushort TileType => TileID.VortexBrick;
        protected override ushort WallType => WallID.VortexBrick;
        protected override TileEntry PlatformEntry => new(TileID.Platforms, 39);
        protected override TileEntry DoorEntry => new(TileID.ClosedDoor, 40);
        protected override TileEntry ChandelierEntry => new(TileID.Chandeliers, 41);
    }
}
