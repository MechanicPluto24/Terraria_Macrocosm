using Macrocosm.Common.Enums;
using Macrocosm.Common.Systems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Drawing;
using Terraria.ID;
using Terraria.Map;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace Macrocosm.Common.Utils
{
    public static partial class Utility
    {
        /// <summary> Get the ModTile of a <see cref="Tile"/>. Returns <langword>null</langword> if it doesn't exist. </summary>
        public static ModTile GetModTile(this Tile tile) => TileLoader.GetTile(tile.TileType);

        public static Vector2 ToWorldCoordinates(this Point point) => new(point.X * 16f, point.Y * 16f);
        public static Vector2 ToWorldCoordinates(this Point16 point) => new(point.X * 16f, point.Y * 16f);
        public static bool IsSloped(this Tile tile) => (int)tile.BlockType > 1;
        public static bool AnyWire(this Tile tile) => tile.RedWire || tile.BlueWire || tile.GreenWire || tile.YellowWire;

        public static ulong GetTileFrameSeed(int i, int j) => Main.TileFrameSeed ^ (ulong)((long)j << 32 | (long)(uint)i); // Don't remove any casts.

        /// <summary> Extension method to set the tile frame values </summary>
        public static void SetFrame(this Tile tile, int x, int y)
        {
            tile.TileFrameX = (short)x;
            tile.TileFrameY = (short)y;
        }

        /// <summary>
        /// Gets the top-left tile coordinates of a multitile
        /// </summary>
        /// <param name="i">The tile X-coordinate</param>
        /// <param name="j">The tile Y-coordinate</param>
        public static Point16 GetMultitileTopLeft(int i, int j)
        {
            Tile tile = Main.tile[i, j];

            int frameX = 0;
            int frameY = 0;

            if (tile.HasTile)
            {
                int style = 0, alt = 0;
                TileObjectData.GetTileInfo(tile, ref style, ref alt);
                TileObjectData data = TileObjectData.GetTileData(tile.TileType, style, alt);

                if (data != null)
                {
                    int padding = 16 + data.CoordinatePadding;

                    frameX = tile.TileFrameX % (padding * data.Width) / padding;
                    frameY = tile.TileFrameY % (padding * data.Height) / padding;
                }
            }

            return new Point16(i - frameX, j - frameY);
        }

        /// <summary>
        /// Uses <seealso cref="GetMultitileTopLeft(int, int)"/> to try to get the entity bound to the multitile at (<paramref name="i"/>, <paramref name="j"/>).
        /// </summary>
        /// <typeparam name="T">The type to get the entity as</typeparam>
        /// <param name="i">The tile X-coordinate</param>
        /// <param name="j">The tile Y-coordinate</param>
        /// <param name="entity">The found <typeparamref name="T"/> instance, if there was one.</param>
        /// <returns><see langword="true"/> if there was a <typeparamref name="T"/> instance, or <see langword="false"/> if there was no entity present OR the entity was not a <typeparamref name="T"/> instance.</returns>
        public static bool TryGetTileEntityAs<T>(int i, int j, out T entity) where T : TileEntity
        {
            Point16 origin = GetMultitileTopLeft(i, j);

            //TileEntity.ByPosition is a Dictionary<Point16, TileEntity> which contains all placed TileEntity instances in the world
            //TryGetValue is used to both check if the dictionary has the key, origin, and get the value from that key if it's there
            if (TileEntity.ByPosition.TryGetValue(origin, out TileEntity existing) && existing is T existingAsT)
            {
                entity = existingAsT;
                return true;
            }

            entity = null;
            return false;
        }

        /// <summary>
        /// Sets the tile <paramref name="style"/> and <paramref name="alternate"/> placement at the specified <paramref name="x"/> and <paramref name="y"/> coordinates.
        /// Uses <see cref="WorldGen.PlaceObject"/> to set the tile, ensuring multi-tile structures are placed at their origin.
        /// </summary>
        public static void SetTileStyle(int x, int y, int style, int alternate = 0)
        {
            Tile setTile = Main.tile[x, y];
            if (!setTile.HasTile)
                return;

            int type = setTile.TileType;
            if (type < 0)
                return;

            var data = TileObjectData.GetTileData(setTile);
            if (data is null)
                return;

            // Get the top-left corner of the multi-tile 
            Point16 topLeft = new(x, y);
            if (data.Width > 1 || data.Height > 1)
                topLeft = GetMultitileTopLeft(x, y);

            WorldGen.KillTile(topLeft.X, topLeft.Y, noItem: true);
            WorldGen.PlaceObject(topLeft.X + data.Origin.X, topLeft.Y + data.Origin.Y, type, mute: true, style, alternate);
        }

        public static Color GetPaintColor(Point coords) => WorldGen.paintColor(Main.tile[coords].TileColor);
        public static Color GetPaintColor(int i, int j) => WorldGen.paintColor(Main.tile[i, j].TileColor);
        public static Color GetPaintColor(this Tile tile) => WorldGen.paintColor(tile.TileColor);

        public static Color GetTileColor(Point coords) => GetTileColor(coords.X, coords.Y);

        public static Color GetTileColor(int i, int j)
        {
            if (CoordinatesOutOfBounds(i, j))
                return default;

            if (!Main.tile[i, j].HasTile)
                return default;

            Color[] colorLookup = MapTileSystem.GetMapColorLookup();

            if (colorLookup is null)
                return default;

            MapTile mapTile = MapHelper.CreateMapTile(i, j, 255);
            return colorLookup[mapTile.Type];
        }

        public static SpriteEffects GetTileSpriteEffects(int i, int j)
        {
            Tile tile = Main.tile[i, j];
            short drawFrameX = tile.TileFrameX;
            short drawFrameY = tile.TileFrameY;
            Main.instance.TilesRenderer.GetTileDrawData(
                i, j, tile, tile.TileType, ref drawFrameX, ref drawFrameY,
                out _, out _, out _, out _,
                out _, out _,
                out SpriteEffects effect,
                out _, out _, out _
            );

            return effect;
        }

        public static void GetTileDrawPositions(int i, int j, out int tileWidth, out int offsetY, out int tileHeight, out short drawFrameX, out short drawFrameY)
        {
            Tile tile = Main.tile[i, j];
            drawFrameX = tile.TileFrameX;
            drawFrameY = tile.TileFrameY;
            Main.instance.TilesRenderer.GetTileDrawData(
                i, j, tile, tile.TileType, ref drawFrameX, ref drawFrameY,
                out tileWidth, out tileHeight, out offsetY, out _,
                out _, out _,
                out _, out _, out _, out _
            );
        }

        public static void GetTileFrameOffset(int i, int j, out int addFrameX, out int addFrameY)
        {
            Tile tile = Main.tile[i, j];
            short drawFrameX = tile.TileFrameX;
            short drawFrameY = tile.TileFrameY;
            Main.instance.TilesRenderer.GetTileDrawData(
                i, j, tile, tile.TileType, ref drawFrameX, ref drawFrameY,
                out _, out _, out _, out _,
                out addFrameX, out addFrameY,
                out _, out _, out _, out _
            );
        }

        public static void DrawTileExtraTexture(int i, int j, SpriteBatch spriteBatch, Asset<Texture2D> texture, Vector2 drawOffset = default, Color? drawColor = null)
        {
            Tile tile = Main.tile[i, j];

            if (!TileDrawing.IsVisible(tile))
                return;

            Vector2 zero = Main.drawToScreen ? Vector2.Zero : new Vector2(Main.offScreenRange);

            GetTileFrameOffset(i, j, out int addFrameX, out int addFrameY);

            var data = TileObjectData.GetTileData(tile);
            int tileSize = 16 + data.CoordinatePadding;
            int width = data.CoordinateWidth;

            if (tile.TileFrameY < 0)
                return;

            int height = data.CoordinateHeights[tile.TileFrameY / tileSize % data.Height];

            drawOffset += new Vector2(data.DrawXOffset, data.DrawYOffset);

            drawColor ??= Color.White;

            Vector2 position = new Vector2(i, j) * 16 + zero + drawOffset - Main.screenPosition;

            spriteBatch.Draw(
                texture.Value,
                position,
                new Rectangle(tile.TileFrameX + addFrameX, tile.TileFrameY + addFrameY, width, height),
                drawColor.Value, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f
            );
        }

        public static void TileRenderer_AddSpecialPoint(int x, int y, string tileCounterType)
        {
            Type tileCounterTypeEnum = typeof(TileDrawing).Assembly.GetType("Terraria.GameContent.Drawing.TileDrawing+TileCounterType")
                ?? throw new InvalidOperationException("TileCounterType enum not found.");

            object enumValue = Enum.Parse(tileCounterTypeEnum, tileCounterType);
            Utility.InvokeMethod(Main.instance.TilesRenderer, "AddSpecialPoint", x, y, enumValue);
        }

        public static bool AnyConnectedSlope(int i, int j)
        {
            Tile tileTop = Main.tile[i, j - 1];
            Tile tileBottom = Main.tile[i, j + 1];
            Tile tileLeft = Main.tile[i - 1, j];
            Tile tileRight = Main.tile[i + 1, j];

            bool top = tileTop.IsSloped() && tileTop.BottomSlope;
            bool bottom = tileBottom.IsSloped() && tileBottom.TopSlope;
            bool left = tileLeft.IsSloped() && tileLeft.RightSlope;
            bool right = tileRight.IsSloped() && tileRight.LeftSlope;

            return top || bottom || left || right;
        }

        public static bool AnyConnectedSlopeOrHalfBlock(int i, int j)
        {
            return AnyConnectedSlope(i, j) || Main.tile[i, j + 1].IsHalfBlock;
        }

        public static bool HasBlendingFrame(int i, int j) => Main.tile[i, j].TileFrameX >= 234 || Main.tile[i, j].TileFrameY >= 90;
        public static bool HasBlendingFrame(this Tile tile) => tile.TileFrameX >= 234 || tile.TileFrameY >= 90;

        public static Point GetClosestTile(Vector2 worldPosition, int type, int distance = 25, Func<Tile, bool> addTile = null) =>
            GetClosestTile((int)(worldPosition.X / 16f), (int)(worldPosition.Y / 16f), type, distance, addTile);

        public static Point GetClosestTile(Point tileCoord, int type, int distance = 25, Func<Tile, bool> addTile = null) =>
            GetClosestTile(tileCoord.X, tileCoord.Y, type, distance, addTile);

        /// <summary>
        /// Returns the position of closest tile of the given type nearby using the given distance.
        /// By GroxTheGreat @ BaseMod
        /// </summary>
        /// <param name="distance"> How far from the x, y coordinates in tiles to check. </param>
        /// <param name="addTile"> Action that can be used to have custom check parameters. </param>
        public static Point GetClosestTile(int x, int y, int type, int distance = 25, Func<Tile, bool> addTile = null)
        {
            Vector2 originalPos = new(x, y);
            int leftX = Math.Max(10, x - distance);
            int leftY = Math.Max(10, y - distance);
            int rightX = Math.Min(Main.maxTilesX - 10, x + distance);
            int rightY = Math.Min(Main.maxTilesY - 10, y + distance);

            Point pos = default;
            float dist = -1;
            for (int x1 = leftX; x1 < rightX; x1++)
            {
                for (int y1 = leftY; y1 < rightY; y1++)
                {
                    Tile tile = Framing.GetTileSafely(x1, y1);
                    if (tile is { HasTile: true } && (tile.TileType == type || type == -1) && (addTile == null || addTile(tile)) && (dist == -1 || Vector2.Distance(originalPos, new Vector2(x1, y1)) < dist))
                    {
                        dist = Vector2.Distance(originalPos, new Vector2(x1, y1));
                        if (type > 0 && (TileID.Sets.BasicChest[type] || (TileObjectData.GetTileData(tile.TileType, 0) != null && (TileObjectData.GetTileData(tile.TileType, 0).Width > 1 || TileObjectData.GetTileData(tile.TileType, 0).Height > 1))))
                        {
                            int x2 = x1; int y2 = y1;
                            if (TileID.Sets.BasicChest[type])
                            {
                                x2 -= tile.TileFrameX / 18 % 2;
                                y2 -= tile.TileFrameY / 18 % 2;
                            }
                            else
                            {
                                Vector2 top = GetMultitileTopLeft(x2, y2).ToVector2();
                                x2 = (int)top.X;
                                y2 = (int)top.Y;
                            }
                            pos = new(x2, y2);
                        }
                        else
                        {
                            pos = new(x1, y1);
                        }
                    }
                }
            }
            return pos;
        }

        /// <summary>
        /// Returns all tiles of the given type nearby using the given distance.
        /// By GroxTheGreat @ BaseMod
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="type"></param>
        /// <param name="distance"> How far from the x, y coordinates in tiles to check. </param>
        /// <param name="addTile"> Action that can be used to have custom check parameters. </param>
        public static Vector2[] GetTiles(int x, int y, int type, int distance = 25, Func<Tile, bool> addTile = null)
        {
            int leftX = Math.Max(10, x - distance);
            int leftY = Math.Max(10, y - distance);
            int rightX = Math.Min(Main.maxTilesX - 10, x + distance);
            int rightY = Math.Min(Main.maxTilesY - 10, y + distance);
            List<Vector2> tilePos = new();
            for (int x1 = leftX; x1 < rightX; x1++)
            {
                for (int y1 = leftY; y1 < rightY; y1++)
                {
                    Tile tile = Framing.GetTileSafely(x1, y1);
                    if (tile is { HasTile: true } && tile.TileType == type && (addTile == null || addTile(tile)))
                    {
                        if (type == 21 || TileObjectData.GetTileData(tile).Width > 1 || TileObjectData.GetTileData(tile).Height > 1)
                        {
                            int x2 = x1; int y2 = y1;
                            if (type == 21)
                            {
                                x2 -= tile.TileFrameX / 18 % 2;
                                y2 -= tile.TileFrameY / 18 % 2;
                            }
                            else
                            {
                                Point p = GetMultitileTopLeft(x2, y2).ToPoint();
                                x2 = p.X;
                                y2 = p.Y;
                            }
                            Vector2 topLeft = new(x2, y2);
                            if (tilePos.Contains(topLeft)) { continue; }
                            tilePos.Add(topLeft);
                        }
                        else
                        {
                            tilePos.Add(new Vector2(x1, y1));
                        }
                    }
                }
            }
            return tilePos.ToArray();
        }

        ///<summary>
        /// Returns the total count of the given liquid within the distance provided.
        /// By GroxTheGreat @ BaseMod
        ///</summary>
        public static int LiquidCount(int x, int y, int distance = 25, int liquidType = 0)
        {
            int liquidAmt = 0;
            int leftX = Math.Max(10, x - distance);
            int leftY = Math.Max(10, y - distance);
            int rightX = Math.Min(Main.maxTilesX - 10, x + distance);
            int rightY = Math.Min(Main.maxTilesY - 10, y + distance);
            for (int x1 = leftX; x1 < rightX; x1++)
            {
                for (int y1 = leftY; y1 < rightY; y1++)
                {
                    Tile tile = Framing.GetTileSafely(x1, y1);
                    if (tile is { LiquidAmount: > 0 } && (liquidType == 0 ? tile.LiquidType == 1 : liquidType == 1 ? tile.LiquidType == 2 : tile.LiquidType == 3))
                    {
                        liquidAmt += tile.LiquidAmount;
                    }
                }
            }
            return liquidAmt;
        }

        ///<summary>
        /// Returns true if the tile type acts similarly to a platform.
        /// By GroxTheGreat @ BaseMod
        ///</summary>
        public static bool IsPlatform(int type) => Main.tileSolid[type] && Main.tileSolidTop[type];

        public static bool HasWire(this Tile tile) => tile.RedWire || tile.BlueWire || tile.GreenWire || tile.YellowWire;

        public static bool HasWire(this Tile tile, WireType wireType)
        {
            return wireType switch
            {
                WireType.Red => tile.RedWire,
                WireType.Blue => tile.BlueWire,
                WireType.Green => tile.GreenWire,
                WireType.Yellow => tile.YellowWire,
                _ => false
            };
        }

        public static IEnumerable<Point16> GetWireNeighbors(int x, int y, WireType? wireType = null)
        {
            Point16[] directions =
            [
                new(-1, 0), // Left
                new(1, 0),  // Right
                new(0, -1), // Up
                new(0, 1)   // Down
            ];

            foreach (var dir in directions)
            {
                int nx = x + dir.X;
                int ny = y + dir.Y;

                if (!WorldGen.InWorld(nx, ny)) 
                    continue;

                Tile neighborTile = Main.tile[nx, ny];

                if(wireType.HasValue)
                {
                    if (HasWire(neighborTile, wireType.Value))
                        yield return new Point16(nx, ny);
                }
                else
                {
                    if (HasWire(neighborTile))
                        yield return new Point16(nx, ny);
                }
            }
        }


        public static bool AlchemyFlower(int type) { return type is 82 or 83 or 84; }

        ///<summary>
        /// Goes through a square area given by the x, y and width, height params, and returns true if they are all of the type given.
        /// By GroxTheGreat @ BaseMod
        ///</summary>
        public static bool IsType(int x, int y, int width, int height, int type)
        {
            for (int x1 = x; x1 < x + width; x1++)
                for (int y1 = y; y1 < y + height; y1++)
                {
                    Tile tile = Framing.GetTileSafely(x1, y1);
                    if (tile is not { HasTile: true } || tile.TileType != type)
                    {
                        return false;
                    }
                }
            return true;
        }

        /// <summary>
        /// Return the count of tiles and walls of the given types within the given distance.
        /// If a location has a tile and a wall it is only counted once.
        /// By GroxTheGreat @ BaseMod
        /// </summary>
        public static int GetTileAndWallCount(Vector2 tileCenter, int[] tileTypes, int[] wallTypes, int distance = 35)
        {
            int tileCount = 0;
            bool addedTile = false;
            for (int x = -distance; x < distance + 1; x++)
            {
                for (int y = -distance; y < distance + 1; y++)
                {
                    int x2 = (int)tileCenter.X + x;
                    int y2 = (int)tileCenter.Y + y;
                    if (x2 < 0 || y2 < 0 || x2 > Main.maxTilesX || y2 > Main.maxTilesY) { continue; }
                    Tile tile = Framing.GetTileSafely(x2, y2);
                    if (tile == null) { continue; }
                    addedTile = false;
                    if (tile.HasTile)
                    {
                        foreach (int i in tileTypes)
                        {
                            if (tile.TileType == i) { tileCount++; addedTile = true; break; }
                        }
                    }
                    if (!addedTile)
                    {
                        foreach (int i in wallTypes)
                        {
                            if (tile.WallType == i) { tileCount++; break; }
                        }
                    }
                    addedTile = false;
                }
            }
            return tileCount;
        }

        /// <summary>
        /// Return the count of walls of the given types within the given distance.
        /// By GroxTheGreat @ BaseMod
        /// </summary>
        public static int GetWallCount(Vector2 tileCenter, int[] wallTypes, int distance = 35)
        {
            int wallCount = 0;
            for (int x = -distance; x < distance + 1; x++)
            {
                for (int y = -distance; y < distance + 1; y++)
                {
                    int x2 = (int)tileCenter.X + x;
                    int y2 = (int)tileCenter.Y + y;
                    if (x2 < 0 || y2 < 0 || x2 > Main.maxTilesX || y2 > Main.maxTilesY) { continue; }
                    Tile tile = Framing.GetTileSafely(x2, y2);
                    if (tile == null) { continue; }
                    foreach (int i in wallTypes)
                    {
                        if (tile.WallType == i) { wallCount++; break; }
                    }
                }
            }
            return wallCount;
        }

        /// <summary>
        /// Return the count of tiles of the given types within the given distance.
        /// By GroxTheGreat @ BaseMod
        /// </summary>
        public static int GetTileCount(Vector2 tileCenter, int[] tileTypes, int distance = 35)
        {
            int tileCount = 0;
            for (int x = -distance; x < distance + 1; x++)
            {
                for (int y = -distance; y < distance + 1; y++)
                {
                    int x2 = (int)tileCenter.X + x;
                    int y2 = (int)tileCenter.Y + y;
                    if (x2 < 0 || y2 < 0 || x2 > Main.maxTilesX || y2 > Main.maxTilesY) { continue; }
                    Tile tile = Framing.GetTileSafely(x2, y2);
                    if (tile is not { HasTile: true }) { continue; }
                    foreach (int i in tileTypes)
                    {
                        if (tile.TileType == i) { tileCount++; break; }
                    }
                }
            }
            return tileCount;
        }
    }
}
