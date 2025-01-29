using Macrocosm.Common.Config;
using Macrocosm.Common.Systems.Power;
using Macrocosm.Common.Utils;
using Microsoft.Xna.Framework;
using SubworldLibrary;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Common.Systems
{
    public class RoomOxygenSystem : ModSystem
    {
        private static int roomX1, roomX2, roomY1, roomY2;
        private static bool resultCache;
        private static bool cacheValid = false;
        private static int cacheResetTimer;

        private static int numRoomTiles;
        private const int MaxRoomTiles = 1500;
        private static bool canValidateRoom;
        private static bool hasOxygenSource;

        public override void PreUpdateEntities()
        {
            if (cacheResetTimer++ >= (int)ServerConfig.Instance.RoomPressureUpdateRate)
            {
                cacheResetTimer = 0;
                cacheValid = false;
            }
        }

        public static bool IsRoomPressurized(Point tileCoords) => IsRoomPressurized(tileCoords.X, tileCoords.Y);
        public static bool IsRoomPressurized(Vector2 worldPosition) => IsRoomPressurized(worldPosition.ToTileCoordinates());
        public static bool IsRoomPressurized(Entity entity) => IsRoomPressurized(entity.Center.ToTileCoordinates());
        public static bool IsRoomPressurized(int x, int y)
        {
            if (!WorldGen.InWorld(x, y))
                return false;

            if (!SubworldSystem.AnyActive<Macrocosm>())
                return true;

            // Return cached result if this room was sampled
            bool cacheInBounds = x >= roomX1 && x <= roomX2 && y >= roomY1 && y <= roomY2;
            if (cacheValid && cacheInBounds)
                return resultCache;

            if (!StartRoomCheck(x, y))
                return false;

            cacheValid = true;
            return resultCache;
        }

        private static bool StartRoomCheck(int x, int y)
        {
            roomX1 = roomX2 = x;
            roomY1 = roomY2 = y;

            Tile tile = Main.tile[x, y];

            numRoomTiles = 0;
            canValidateRoom = true;
            hasOxygenSource = false;

            HashSet<Point> visitedTiles = new();
            if (WorldGen.SolidTile(tile) || (tile.HasTile && tile.WallType <= 0))
            {
                canValidateRoom = false;
                return false;
            }

            CheckRoom(x, y, visitedTiles);

            resultCache = canValidateRoom && hasOxygenSource;
            return resultCache;
        }

        private static void CheckRoom(int x, int y, HashSet<Point> visitedTiles)
        {
            if (!canValidateRoom)
                return;

            if (!WorldGen.InWorld(x, y, 10))
                return;

            Point tilePoint = new(x, y);
            if (visitedTiles.Contains(tilePoint))
                return;

            visitedTiles.Add(tilePoint);

            Tile tile = Main.tile[x, y];
            if (!tile.HasTile && tile.WallType <= 0)
            {
                canValidateRoom = false;
                return;
            }

            numRoomTiles++;
            if (numRoomTiles >= MaxRoomTiles)
            {
                canValidateRoom = false;
                return;
            }

            if (WorldGen.SolidTile(tile))
                return;

            if (Utility.TryGetTileEntityAs<MachineTE>(x, y, out var te) && te is IOxygenSource oxygenSource && oxygenSource.IsProvidingOxygen)
            {
                hasOxygenSource = true;
            }

            roomX1 = Math.Min(roomX1, x);
            roomX2 = Math.Max(roomX2, x);
            roomY1 = Math.Min(roomY1, y);
            roomY2 = Math.Max(roomY2, y);

            for (int l = x - 1; l <= x + 1; l++)
            {
                for (int m = y - 1; m <= y + 1; m++)
                {
                    if (l != x || m != y)
                        CheckRoom(l, m, visitedTiles);
                }
            }
        }
    }
}
