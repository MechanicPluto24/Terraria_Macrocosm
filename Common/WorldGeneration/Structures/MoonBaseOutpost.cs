using Macrocosm.Content.Tiles.Blocks;
using Macrocosm.Content.Tiles.Walls;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.WorldBuilding;

namespace Macrocosm.Common.WorldGeneration.Structures
{
    public class MoonBaseOutpost : MicroBiome
    {
        public override bool Place(Point origin, StructureMap structures)
        {
            Rectangle room = GetRoom(origin);

            if (!WorldGen.InWorld(room.X, room.Y))
                return false;

            if (!structures.CanPlace(room))
                return false;

            WorldUtils.Gen(new Point(room.X, room.Y), new Shapes.Rectangle(room.Width, room.Height), Actions.Chain(new Actions.SetTileKeepWall((ushort)ModContent.TileType<MoonBasePlating>()), new Actions.SetFrames(frameNeighbors: true)));
            WorldUtils.Gen(new Point(room.X, room.Y + room.Height - 1 - 3), new Shapes.Rectangle(1, 3), new Actions.ClearTile(frameNeighbors: true));
            WorldUtils.Gen(new Point(room.X + room.Width - 1, room.Y + room.Height - 1 - 3), new Shapes.Rectangle(1, 3), new Actions.ClearTile(frameNeighbors: true));
            WorldUtils.Gen(new Point(room.X + 1, room.Y + 1), new Shapes.Rectangle(room.Width - 2, room.Height - 2), Actions.Chain(new Actions.ClearTile(frameNeighbors: true), new Actions.PlaceWall((ushort)ModContent.WallType<MoonBasePlatingWall>())));
            WorldUtils.Gen(new Point(room.X + 1, room.Y + room.Height - 2), new Shapes.Rectangle(room.Width - 2, 1), Actions.Chain(new Actions.ClearWall(frameNeighbors: true), new Actions.PlaceWall((ushort)ModContent.WallType<HazardWall>())));

            structures.AddProtectedStructure(room, padding: 10);

            return true;
        }

        private Rectangle GetRoom(Point origin)
        {
            bool solidDown = WorldUtils.Find(origin, Searches.Chain(new Searches.Down(25), new Conditions.IsSolid()), out Point solidGround);

            if (!solidDown)
                return new(-1, -1, 0, 0);
 
            int maxHeight = Terraria.Utils.Clamp(solidGround.Y - origin.Y, 8, 12);
            int roomHeight = (int)(maxHeight * 0.75); 

            bool solidLeft = WorldUtils.Find(origin, Searches.Chain(new Searches.Left(5), new Conditions.IsSolid()), out Point leftBound);
            bool solidRight = WorldUtils.Find(origin, Searches.Chain(new Searches.Right(5), new Conditions.IsSolid()), out Point rightBound);
            if (!solidLeft) leftBound = new Point(origin.X - 5, origin.Y);
            if (!solidRight) rightBound = new Point(origin.X + 5, origin.Y);

            int maxWidth = Terraria.Utils.Clamp(rightBound.X - leftBound.X, 0, 10);
            Rectangle room = new Rectangle(0, 0, 9, 9);
            room.X = Math.Min(leftBound.X, rightBound.X - maxWidth);
            room.Y = solidGround.Y - roomHeight;
            return room;
        }
    }
}
