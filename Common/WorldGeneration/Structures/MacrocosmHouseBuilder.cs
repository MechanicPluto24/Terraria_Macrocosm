using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Macrocosm.Common.DataStructures;
using Macrocosm.Common.Utils;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent.Biomes.CaveHouse;
using Terraria.ID;
using Terraria.Utilities;
using Terraria.WorldBuilding;

namespace Macrocosm.Common.WorldGeneration
{
    // TODO: more abstraction, width & height configuration
    public abstract class MacrocosmHouseBuilder : MicroBiome
    {
        public bool IsValid;

        protected abstract ushort TileType { get; }
        protected abstract ushort WallType { get; }
        protected virtual ushort BeamType { get; } = ushort.MaxValue;
        protected virtual TileTypeStylePair PlatformEntry { get; }
        protected virtual TileTypeStylePair DoorEntry { get; }

        protected virtual TileTypeStylePair ChandelierEntry { get; }
        protected virtual List<TileTypeStylePair> PaintingPool { get; } = new();
        protected virtual List<TileTypeStylePair> ExtraFurniturePool { get; } = new();

        protected virtual (TileTypeStylePair data, double chance) ChestEntry { get; }

        protected virtual TileTypeStylePair SmallPileEntry { get; }
        protected virtual TileTypeStylePair MediumPileEntry { get; }
        //protected virtual TileEntry LargePileEntry { get; }

        protected virtual bool StylizeRoomOuterCorners { get; }
        protected virtual bool StylizeRoomInnerCorners { get; }

        public List<Rectangle> Rooms { get; set; }

        public Rectangle TopRoom => Rooms.First();

        public Rectangle BottomRoom => Rooms.Last();

        private UnifiedRandom Random => WorldGen.genRand;

        private Tilemap Tiles => Main.tile;

        protected MacrocosmHouseBuilder()
        {
        }

        protected virtual void AgeRoom(Rectangle room)
        {
        }

        public override bool Place(Point origin, StructureMap structures)
        {
            if(!(IsValid = Initialize(origin, structures, out List<Rectangle> rooms)))
                return false;

            Rooms = rooms;
            PlaceEmptyRooms();

            foreach (Rectangle room in Rooms)
                structures.AddProtectedStructure(room, 8);

            PlaceStairs();
            StylizeRoomCorners();
            PlaceDoors();
            PlacePlatforms();
            PlaceSupportBeams();

            FillRooms();

            foreach (Rectangle room in Rooms)
                AgeRoom(room);

            PlaceChests();

            return true;
        }

        private bool Initialize(Point origin, StructureMap structures, out List<Rectangle> rooms)
        {
            if (!WorldGen.InWorld(origin.X, origin.Y, 10))
            {
                rooms = new();
                return false;
            }

            rooms = CreateRooms(origin);

            if (rooms.Count == 0 || !AreRoomLocationsValid(rooms, structures))
            {
                rooms = new();
                return false;
            }

            rooms.Sort((Rectangle lhs, Rectangle rhs) => lhs.Top.CompareTo(rhs.Top));
            return true;
        }

        private static List<Rectangle> CreateRooms(Point origin)
        {
            if (!WorldUtils.Find(origin, Searches.Chain(new Searches.Down(200), new Conditions.IsSolid()), out var result) || result == origin)
                return new List<Rectangle>();

            Rectangle item = FindRoom(result);
            Rectangle rectangle = FindRoom(new Point(item.Center.X, item.Y + 1));
            Rectangle rectangle2 = FindRoom(new Point(item.Center.X, item.Y + item.Height + 10));
            rectangle2.Y = item.Y + item.Height - 1;
            double roomSolidPrecentage = GetRoomSolidPercentage(rectangle);
            double roomSolidPrecentage2 = GetRoomSolidPercentage(rectangle2);
            item.Y += 3;
            rectangle.Y += 3;
            rectangle2.Y += 3;
            List<Rectangle> list = new List<Rectangle>();
            if (WorldGen.genRand.NextDouble() > roomSolidPrecentage + 0.2)
                list.Add(rectangle);

            list.Add(item);
            if (WorldGen.genRand.NextDouble() > roomSolidPrecentage2 + 0.2)
                list.Add(rectangle2);

            return list;
        }

        private static Rectangle FindRoom(Point origin)
        {
            Point result;
            bool flag = WorldUtils.Find(origin, Searches.Chain(new Searches.Left(25), new Conditions.IsSolid()), out result);
            Point result2;
            bool num = WorldUtils.Find(origin, Searches.Chain(new Searches.Right(25), new Conditions.IsSolid()), out result2);
            if (!flag)
                result = new Point(origin.X - 25, origin.Y);

            if (!num)
                result2 = new Point(origin.X + 25, origin.Y);

            Rectangle result3 = new Rectangle(origin.X, origin.Y, 0, 0);
            if (origin.X - result.X > result2.X - origin.X)
            {
                result3.X = result.X;
                result3.Width = Terraria.Utils.Clamp(result2.X - result.X, 15, 30);
            }
            else
            {
                result3.Width = Terraria.Utils.Clamp(result2.X - result.X, 15, 30);
                result3.X = result2.X - result3.Width;
            }

            Point result4;
            bool flag2 = WorldUtils.Find(result, Searches.Chain(new Searches.Up(10), new Conditions.IsSolid()), out result4);
            Point result5;
            bool num2 = WorldUtils.Find(result2, Searches.Chain(new Searches.Up(10), new Conditions.IsSolid()), out result5);
            if (!flag2)
                result4 = new Point(origin.X, origin.Y - 10);

            if (!num2)
                result5 = new Point(origin.X, origin.Y - 10);

            result3.Height = Terraria.Utils.Clamp(Math.Max(origin.Y - result4.Y, origin.Y - result5.Y), 8, 12);
            result3.Y -= result3.Height;
            return result3;
        }

        private static double GetRoomSolidPercentage(Rectangle room)
        {
            double num = room.Width * room.Height;
            Ref<int> @ref = new(0);
            WorldUtils.Gen(new Point(room.X, room.Y), new Shapes.Rectangle(room.Width, room.Height), Actions.Chain(new Modifiers.IsSolid(), new Actions.Count(@ref)));
            return (double)@ref.Value / num;
        }

        private bool AreRoomLocationsValid(List<Rectangle> rooms, StructureMap structures)
        {
            foreach (Rectangle room in rooms)
            {
                if (room.Y + room.Height > Main.maxTilesY - 220)
                    return false;

                if(!structures.CanPlace(room))
                    return false;       
            }

            return true;
        }

        private void PlaceEmptyRooms()
        {
            foreach (Rectangle room in Rooms)
            {
                WorldUtils.Gen(new Point(room.X, room.Y), new Shapes.Rectangle(room.Width, room.Height), Actions.Chain(new Actions.SetTileKeepWall(TileType), new Actions.SetFrames(frameNeighbors: true)));
                WorldUtils.Gen(new Point(room.X + 1, room.Y + 1), new Shapes.Rectangle(room.Width - 2, room.Height - 2), Actions.Chain(new Actions.ClearTile(frameNeighbors: true), new Actions.PlaceWall(WallType)));
            }
        }

        private void StylizeRoomCorners()
        {
            foreach (Rectangle room in Rooms)
            {
                var tileTopLeft = Main.tile[room.X, room.Y];
                var tileTopRight = Main.tile[room.X + room.Width - 1, room.Y];
                var tileBottomLeft = Main.tile[room.X, room.Y + room.Height - 1];
                var tileBottomRight = Main.tile[room.X + room.Width - 1, room.Y + room.Height - 1];

                TileNeighbourInfo infoTopLeft = new(room.X, room.Y);
                TileNeighbourInfo infoTopRight = new(room.X + room.Width - 1, room.Y);
                TileNeighbourInfo infoBottomLeft = new(room.X, room.Y + room.Height - 1);
                TileNeighbourInfo infoBottomRight = new(room.X + room.Width - 1, room.Y + room.Height - 1);

                if (StylizeRoomOuterCorners)
                {
                    if (infoTopLeft.Solid.Count == 2)
                        tileTopLeft.Slope = SlopeType.SlopeDownRight;

                    if (infoTopRight.Solid.Count == 2)
                        tileTopRight.Slope = SlopeType.SlopeDownLeft;

                    if (infoBottomLeft.Solid.Count == 2)
                        tileBottomLeft.Slope = SlopeType.SlopeUpRight;

                    if (infoBottomRight.Solid.Count == 2)
                        tileBottomRight.Slope = SlopeType.SlopeUpLeft;
                }

                if (StylizeRoomInnerCorners)
                {
                    Tile tile = Main.tile[room.X + 1, room.Y + 1];
                    if (!tile.HasTile)
                    {
                        WorldGen.PlaceTile(room.X + 1, room.Y + 1, TileType, true);
                        tile.Slope = SlopeType.SlopeUpLeft;
                    }
                   
                    tile = Main.tile[room.X + room.Width - 2, room.Y + 1];
                    if (!tile.HasTile)
                    {
                        WorldGen.PlaceTile(room.X + room.Width - 2, room.Y + 1, TileType, true);
                        tile.Slope = SlopeType.SlopeUpRight;
                    }

                    tile = Main.tile[room.X, room.Y + room.Height - 2];
                    if (!tile.HasTile)
                    {
                        WorldGen.PlaceTile(room.X, room.Y + room.Height - 2, TileType, true);
                        tile.Slope = SlopeType.SlopeDownLeft;
                    }

                    tile = Main.tile[room.X + room.Width - 2, room.Y + room.Height - 2];
                    if (!tile.HasTile)
                    {
                        WorldGen.PlaceTile(room.X + room.Width - 2, room.Y + room.Height - 2, TileType, true);
                        tile.Slope = SlopeType.SlopeDownRight;
                    }
                }
            }
        }

        private void FillRooms()
        {
            foreach (Rectangle room in Rooms)
            {
                int num = room.Width / 8;
                int num2 = room.Width / (num + 1);
                int num3 = Random.Next(2);
                for (int i = 0; i < num; i++)
                {
                    int tileX = (i + 1) * num2 + room.X;
                    switch (i + num3 % 2)
                    {
                        case 0:
                            {
                                int tileY = room.Y + Math.Min(room.Height / 2, room.Height - 5);
                                if (PaintingPool.Any())
                                    WorldGen.PlaceTile(tileX, tileY, PaintingPool.GetRandom(Random).Type, mute: true, forced: false, -1);
                                break;
                            }
                        case 1:
                            {
                                if(ChandelierEntry.IsValid)
                                {
                                    int num5 = room.Y + 1;
                                    WorldGen.PlaceTile(tileX, num5, ChandelierEntry.Type, mute: true, forced: false, -1, ChandelierEntry.GetStyle());

                                    // Turn on chandelier
                                    for (int j = -1; j < 2; j++)
                                    {
                                        for (int k = 0; k < 3; k++)
                                        {
                                            Tiles[j + tileX, k + num5].TileFrameX += 54;
                                        }
                                    }
                                }
                                
                                break;
                            }
                    }
                }

                int floor = room.Width / 8 + 3;

                while (floor > 0)
                {
                    int tileX = Random.Next(room.Width - 3) + 1 + room.X;
                    int tileY = room.Y + room.Height - 2;
                    switch (Random.Next(3))
                    {
                        case 0:
                            if (SmallPileEntry.IsValid)
                                WorldGen.PlaceTile(tileX, tileY, SmallPileEntry.Type, true, style: SmallPileEntry.GetStyle());
                            break;
                        case 1:
                            if (MediumPileEntry.IsValid)
                                WorldGen.PlaceTile(tileX, tileY, MediumPileEntry.Type, true, style: MediumPileEntry.GetStyle());
                            break;

                        case 2:
                            if (ExtraFurniturePool.Any())
                            {
                                TileTypeStylePair entry = ExtraFurniturePool.GetRandom(Random);
                                WorldGen.PlaceTile(tileX, tileY, entry.Type, mute: true, forced: false, -1, entry.GetStyle());
                            }
                            break;
                    }

                    floor--;
                }
            }
        }

        private void PlaceStairs()
        {
            if (!PlatformEntry.IsValid)
                return;

            foreach (Tuple<Point, Point> item3 in CreateStairsList())
            {
                Point item = item3.Item1;
                Point item2 = item3.Item2;
                int num = ((item2.X > item.X) ? 1 : (-1));
                ShapeData shapeData = new();
                for (int i = 0; i < item2.Y - item.Y; i++)
                {
                    shapeData.Add(num * (i + 1), i);
                }

                WorldUtils.Gen(item, new ModShapes.All(shapeData), Actions.Chain(new Actions.PlaceTile(PlatformEntry.Type, PlatformEntry.GetStyle()), new Actions.SetSlope((num == 1) ? 1 : 2), new Actions.SetFrames(frameNeighbors: true)));
                WorldUtils.Gen(new Point(item.X + ((num == 1) ? 1 : (-4)), item.Y - 1), new Shapes.Rectangle(4, 1), Actions.Chain(new Actions.Clear(), new Actions.PlaceWall(WallType), new Actions.PlaceTile(PlatformEntry.Type, PlatformEntry.GetStyle()), new Actions.SetFrames(frameNeighbors: true)));
            }
        }

        private List<Tuple<Point, Point>> CreateStairsList()
        {
            List<Tuple<Point, Point>> list = new();
            for (int i = 1; i < Rooms.Count; i++)
            {
                Rectangle room = Rooms[i];
                Rectangle prevRoom = Rooms[i - 1];
                if (prevRoom.X - room.X > room.X + room.Width - (prevRoom.X + prevRoom.Width))
                    list.Add(new Tuple<Point, Point>(new Point(room.X + room.Width - 1, room.Y + 1), new Point(room.X + room.Width - room.Height + 1, room.Y + room.Height - 1)));
                else
                    list.Add(new Tuple<Point, Point>(new Point(room.X, room.Y + 1), new Point(room.X + room.Height - 1, room.Y + room.Height - 1)));
            }

            return list;
        }

        private void PlaceDoors()
        {
            if (!DoorEntry.IsValid)
                return;

            foreach (Point item in CreateDoorList())
            {
                Tile tileBelow = Main.tile[item.X, item.Y + 3];
                tileBelow.Slope = SlopeType.Solid;

                WorldUtils.Gen(item, new Shapes.Rectangle(1, 3), new Actions.ClearTile(frameNeighbors: true));
                WorldGen.PlaceTile(item.X, item.Y, DoorEntry.Type, mute: true, forced: true, -1, DoorEntry.GetStyle());
            }
        }

        private List<Point> CreateDoorList()
        {
            List<Point> list = new();
            foreach (Rectangle room in Rooms)
            {
                if (FindSideExit(new Rectangle(room.X + room.Width, room.Y + 1, 1, room.Height - 2), isLeft: false, out var exitY))
                    list.Add(new Point(room.X + room.Width - 1, exitY));

                if (FindSideExit(new Rectangle(room.X, room.Y + 1, 1, room.Height - 2), isLeft: true, out exitY))
                    list.Add(new Point(room.X, exitY));
            }

            return list;
        }

        private void PlacePlatforms()
        {
            if (!PlatformEntry.IsValid)
                return;

            foreach (Point item in CreatePlatformsList())
            {
                WorldUtils.Gen(item, new Shapes.Rectangle(3, 1), Actions.Chain(new Actions.ClearMetadata(), new Actions.PlaceTile(PlatformEntry.Type, PlatformEntry.GetStyle()), new Actions.SetFrames(frameNeighbors: true)));
            }
        }

        private List<Point> CreatePlatformsList()
        {
            List<Point> list = new();
            if (FindVerticalExit(new Rectangle(TopRoom.X + 2, TopRoom.Y, TopRoom.Width - 4, 1), isUp: true, out var exitX))
                list.Add(new Point(exitX, TopRoom.Y));

            if (FindVerticalExit(new Rectangle(BottomRoom.X + 2, BottomRoom.Y + BottomRoom.Height - 1, BottomRoom.Width - 4, 1), isUp: false, out exitX))
                list.Add(new Point(exitX, BottomRoom.Y + BottomRoom.Height - 1));

            return list;
        }

        private void PlaceSupportBeams()
        {
            if (BeamType == ushort.MaxValue)
                return;

            foreach (Rectangle item in CreateSupportBeamList())
            {
                if (item.Height > 1 && Tiles[item.X, item.Y - 1].TileType != 19)
                {
                    WorldUtils.Gen(new Point(item.X, item.Y), new Shapes.Rectangle(item.Width, item.Height), Actions.Chain(new Actions.SetTileKeepWall(BeamType), new Actions.SetFrames(frameNeighbors: true)));
                    Tile tile = Tiles[item.X, item.Y + item.Height];
                    tile.Slope = Terraria.ID.SlopeType.Solid;
                    tile.IsHalfBlock = false;
                }
            }
        }

        private List<Rectangle> CreateSupportBeamList()
        {
            List<Rectangle> list = new List<Rectangle>();
            int num = Rooms.Min((Rectangle room) => room.Left);
            int num2 = Rooms.Max((Rectangle room) => room.Right) - 1;
            int num3 = 6;
            while (num3 > 4 && (num2 - num) % num3 != 0)
            {
                num3--;
            }

            for (int i = num; i <= num2; i += num3)
            {
                for (int j = 0; j < Rooms.Count; j++)
                {
                    Rectangle rectangle = Rooms[j];
                    if (i < rectangle.X || i >= rectangle.X + rectangle.Width)
                        continue;

                    int num4 = rectangle.Y + rectangle.Height;
                    int num5 = 50;
                    for (int k = j + 1; k < Rooms.Count; k++)
                    {
                        if (i >= Rooms[k].X && i < Rooms[k].X + Rooms[k].Width)
                            num5 = Math.Min(num5, Rooms[k].Y - num4);
                    }

                    if (num5 > 0)
                    {
                        Point result;
                        bool flag = WorldUtils.Find(new Point(i, num4), Searches.Chain(new Searches.Down(num5), new Conditions.IsSolid()), out result);
                        if (num5 < 50)
                        {
                            flag = true;
                            result = new Point(i, num4 + num5);
                        }

                        if (flag)
                            list.Add(new Rectangle(i, num4, 1, result.Y - num4));
                    }
                }
            }

            return list;
        }

        private static bool FindVerticalExit(Rectangle wall, bool isUp, out int exitX)
        {
            bool result2 = WorldUtils.Find(new Point(wall.X + wall.Width - 3, wall.Y + (isUp ? (-5) : 0)), Searches.Chain(new Searches.Left(wall.Width - 3), new Conditions.IsSolid().Not().AreaOr(3, 5)), out Point result);
            exitX = result.X;
            return result2;
        }

        private static bool FindSideExit(Rectangle wall, bool isLeft, out int exitY)
        {
            Point result;
            bool result2 = WorldUtils.Find(new Point(wall.X + (isLeft ? (-4) : 0), wall.Y + wall.Height - 3), Searches.Chain(new Searches.Up(wall.Height - 3), new Conditions.IsSolid().Not().AreaOr(4, 3)), out result);
            exitY = result.Y;
            return result2;
        }

        private void PlaceChests()
        {
            if (!ChestEntry.data.IsValid || Random.NextDouble() > ChestEntry.chance)
                return;

            bool flag = false;
            foreach (Rectangle room in Rooms)
            {
                int num = room.Height - 1 + room.Y;
                bool num2 = num > (int)Main.worldSurface;

                for (int i = 0; i < 10; i++)
                {
                    if (flag = WorldGen.AddBuriedChest(Random.Next(2, room.Width - 2) + room.X, num, 0, notNearOtherChests: false, ChestEntry.data.GetStyle(), trySlope: false, ChestEntry.data.Type))
                        break;
                }

                if (flag)
                    break;

                for (int j = room.X + 2; j <= room.X + room.Width - 2; j++)
                {
                    if (flag = WorldGen.AddBuriedChest(j, num, 0, notNearOtherChests: false, ChestEntry.data.GetStyle(), trySlope: false, ChestEntry.data.Type))
                        break;
                }

                if (flag)
                    break;
            }

            if (!flag)
            {
                foreach (Rectangle room2 in Rooms)
                {
                    int num3 = room2.Y - 1;
                    bool num4 = num3 > (int)Main.worldSurface;

                    for (int k = 0; k < 10; k++)
                    {
                        if (flag = WorldGen.AddBuriedChest(Random.Next(2, room2.Width - 2) + room2.X, num3, 0, notNearOtherChests: false, ChestEntry.data.GetStyle(), trySlope: false, ChestEntry.data.Type))
                            break;
                    }

                    if (flag)
                        break;

                    for (int l = room2.X + 2; l <= room2.X + room2.Width - 2; l++)
                    {
                        if (flag = WorldGen.AddBuriedChest(l, num3, 0, notNearOtherChests: false, ChestEntry.data.GetStyle(), trySlope: false, ChestEntry.data.Type))
                            break;
                    }

                    if (flag)
                        break;
                }
            }

            if (flag)
                return;

            for (int m = 0; m < 1000; m++)
            {
                int i2 = Random.Next(Rooms[0].X - 30, Rooms[0].X + 30);
                int num5 = Random.Next(Rooms[0].Y - 30, Rooms[0].Y + 30);
                if (WorldGen.AddBuriedChest(i2, num5, 0, notNearOtherChests: false, ChestEntry.data.GetStyle(), trySlope: false, ChestEntry.data.Type))
                    break;
            }
        }
    }

}

