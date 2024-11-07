using Macrocosm.Common.Enums;
using Macrocosm.Common.Utils;
using Macrocosm.Common.WorldGeneration;
using Macrocosm.Content.Tiles.Blocks;
using Macrocosm.Content.Tiles.Walls;
using Microsoft.Xna.Framework;
using Terraria.DataStructures;
using Terraria.ModLoader;
using Terraria.WorldBuilding;

namespace Macrocosm.Content.WorldGeneration.Structures.LunarOutposts
{
    public abstract class BaseLunarOutpost : Structure
    {
        public virtual void PreAgeRoom(Point16 origin) { }
        public virtual void PostAgeRoom(Point16 origin) { }

        public sealed override void PostPlace(Point16 origin)
        {
            PreAgeRoom(origin);

            ushort[] clearableTiles = [
                (ushort)ModContent.TileType<IndustrialPlating>()
            ];

            ushort[] clearableWalls = [
                (ushort)ModContent.WallType<IndustrialPlatingWall>(),
                (ushort)ModContent.WallType<IndustrialHazardWall>(),
                (ushort)ModContent.WallType<IndustrialSquarePaneledWall>(),
                (ushort)ModContent.WallType<IndustrialTrimmingWall>(),
            ];

            // Age room
            WorldUtils.Gen(new Point(origin.X, origin.Y), new Shapes.Rectangle(Size.X, Size.Y), Actions.Chain(new Modifiers.Dither(0.85), new Modifiers.Blotches(), new Modifiers.OnlyWalls(clearableWalls), (new Actions.PlaceWall((ushort)ModContent.WallType<ProtolithWall>()))));
            WorldUtils.Gen(new Point(origin.X, origin.Y), new Shapes.Rectangle(Size.X, Size.Y), Actions.Chain(new Modifiers.Dither(0.95), new Modifiers.OnlyTiles(clearableTiles), new Actions.ClearTile(frameNeighbors: true)));

            // Make walls unsafe
            Utility.ConvertWallSafetyInArea(origin.X, origin.Y, Size.X, Size.Y, WallSafetyType.Natural);

            PostAgeRoom(origin);
        }
    }
}
