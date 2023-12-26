using Macrocosm.Common.DataStructures;
using Macrocosm.Content.Tiles.Ambient;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.WorldBuilding;

namespace Macrocosm.Common.WorldGeneration.Structures
{
    public class LunarianHouseBuilder : MacrocosmHouseBuilder
    {
        public LunarianHouseBuilder() : base()
        {

        }

        protected override ushort TileType => TileID.VortexBrick;
        protected override ushort WallType => WallID.VortexBrick;
        protected override TileTypeStylePair PlatformEntry => new(TileID.Platforms, 39);
        protected override TileTypeStylePair DoorEntry => new(TileID.ClosedDoor, 40);
        protected override TileTypeStylePair ChandelierEntry => new(TileID.Chandeliers, 41);
        protected override TileTypeStylePair SmallPileEntry => new(ModContent.TileType<RegolithRockSmallNatural>(), 0..9);

        protected override (TileTypeStylePair data, double chance) ChestEntry => (new(TileID.Containers2, 6), 1.0);

        protected override bool StylizeRoomInnerCorners => true;
        protected override bool StylizeRoomOuterCorners => true;
    }
}
