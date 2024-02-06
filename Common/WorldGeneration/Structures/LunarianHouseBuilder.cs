using Macrocosm.Common.DataStructures;
using Macrocosm.Content.Tiles.Ambient;
using Macrocosm.Content.Tiles.Blocks;
using Macrocosm.Content.Tiles.Furniture.MoonBase;
using Macrocosm.Content.Tiles.Walls;
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

        protected override ushort TileType => (ushort)ModContent.TileType<RegolithBrick>();
        protected override ushort WallType => (ushort)ModContent.WallType<RegolithBrickWall>();
        protected override TileTypeStylePair PlatformEntry => new(TileID.Platforms, 39);
        protected override TileTypeStylePair DoorEntry => new(TileID.ClosedDoor, 40);
        protected override TileTypeStylePair ChandelierEntry => new(ModContent.TileType<MoonBaseNeonTube>());
        protected override TileTypeStylePair SmallPileEntry => new(ModContent.TileType<RegolithRockSmallNatural>(), 0..9);
        protected override (TileTypeStylePair data, double chance) ChestEntry => (new(ModContent.TileType<MoonBaseChest>(), (int)MoonBaseChest.State.Normal), 1.0);

        protected override bool StylizeRoomInnerCorners => true;
        protected override bool StylizeRoomOuterCorners => true;
    }
}
