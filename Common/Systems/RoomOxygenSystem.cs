using Macrocosm.Common.Config;
using Macrocosm.Common.Systems.Power;
using Macrocosm.Common.Systems.Power.Oxygen;
using Macrocosm.Common.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SubworldLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Common.Systems
{
    public class RoomOxygenSystem : ModSystem
    {
        private struct RoomCacheEntry(Rectangle room, bool value)
        {
            public Rectangle Room = room;
            public bool Value = value;
        }

        private readonly static List<RoomCacheEntry> cache = new();
        private readonly static List<IOxygenSource> activeSources = new();

        private static int roomX1, roomX2, roomY1, roomY2;
        private static int numRoomTiles;
        private static bool canValidateRoom;

        private static int cacheResetTimer;

        private const int MaxCachedRooms = 32;
        public const int GlobalMaxRoomTiles = 5000;

        public override void PreUpdateEntities()
        {
            if (cacheResetTimer++ >= (int)ServerConfig.Instance.RoomPressureUpdateRate)
            {
                cacheResetTimer = 0;
                cache.Clear();
            }
        }

        public static bool CheckRoomOxygen(Point tileCoords) => CheckRoomOxygen(tileCoords.X, tileCoords.Y);
        public static bool CheckRoomOxygen(Vector2 worldPosition) => CheckRoomOxygen(worldPosition.ToTileCoordinates());
        public static bool CheckRoomOxygen(Entity entity) => CheckRoomOxygen(entity.Center.ToTileCoordinates());
        public static bool CheckRoomOxygen(int x, int y)
        {
            if (!WorldGen.InWorld(x, y))
                return false;

            if (!SubworldSystem.AnyActive<Macrocosm>())
                return true;

            foreach (var entry in cache)
            {
                if (entry.Room.Contains(x, y))
                    return entry.Value;
            }

            bool result = StartCheckRoom(x, y);
            CacheRoom(result);
            return result;
        }

        private static void CacheRoom(bool result)
        {
            Rectangle room = new(roomX1, roomY1, roomX2 - roomX1 + 1, roomY2 - roomY1 + 1);
            for (int i = 0; i < cache.Count; i++)
            {
                if (cache[i].Room == room)
                {
                    cache[i] = new(room, result);
                    return;
                }
            }

            if (cache.Count >= MaxCachedRooms)
                cache.RemoveAt(0);

            cache.Add(new RoomCacheEntry(room, result));
        }

        private static bool StartCheckRoom(int x, int y)
        {
            // Early exit if the start tile is invalid
            Tile tile = Main.tile[x, y];
            if (WorldGen.SolidTile(tile) || (tile.HasTile && tile.WallType == WallID.None))
                return false;

            activeSources.Clear();
            roomX1 = roomX2 = x;
            roomY1 = roomY2 = y;
            numRoomTiles = 0;
            canValidateRoom = true;

            HashSet<Point> visitedTiles = new();
            CheckTile(x, y, visitedTiles);

            if (!canValidateRoom || activeSources.Count <= 0)
                return false;

            int maxSize = activeSources.Select(s => s.MaxRoomSize).Max();
            if (maxSize <= 0 || numRoomTiles > maxSize)
                return false;

            return true;
        }

        private static void CheckTile(int x, int y, HashSet<Point> visitedTiles)
        {
            if (!canValidateRoom)
                return;

            if (!WorldGen.InWorld(x, y, 10))
                return;

            Point coords = new(x, y);
            if (visitedTiles.Contains(coords))
                return;

            visitedTiles.Add(coords);

            Tile tile = Main.tile[x, y];
            if (!tile.HasTile && tile.WallType <= 0)
            {
                canValidateRoom = false;
                return;
            }

            numRoomTiles++;
            if (numRoomTiles >= GlobalMaxRoomTiles)
            {
                canValidateRoom = false;
                return;
            }

            if (WorldGen.SolidTile(tile))
                return;

            if (Utility.TryGetTileEntityAs<MachineTE>(x, y, out var te) && te is IOxygenSource oxySrc)
            {
                if (oxySrc is IOxygenPassiveSource oxyPassive)
                    oxyPassive.TryActivateOxygen();

                if (oxySrc.IsProvidingOxygen)
                    activeSources.Add(oxySrc);
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
                        CheckTile(l, m, visitedTiles);
                }
            }
        }

        public override void PostDrawTiles()
        {
            if (!DebugConfig.Instance.RoomOxygenDebug)
                return;

            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, default, default, null, Main.GameViewMatrix.ZoomMatrix);
            foreach (var entry in cache)
            {
                Color color = entry.Value ? Color.Lime * 0.4f : Color.Red * 0.4f;
                var bounds = new Rectangle(
                    entry.Room.X * 16 - (int)Main.screenPosition.X,
                    entry.Room.Y * 16 - (int)Main.screenPosition.Y,
                    entry.Room.Width * 16,
                    entry.Room.Height * 16
                );

                Main.spriteBatch.Draw(TextureAssets.MagicPixel.Value, bounds, color);
            }
            Main.spriteBatch.End();
        }
    }
}
