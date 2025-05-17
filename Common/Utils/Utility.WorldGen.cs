using Macrocosm.Common.Bases.Walls;
using Macrocosm.Common.DataStructures;
using Macrocosm.Common.Enums;
using Microsoft.Xna.Framework;
using ReLogic.Utilities;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;
using Terraria.WorldBuilding;
using static Terraria.WorldGen;

namespace Macrocosm.Common.Utils
{
    public static partial class Utility
    {
        public static bool CoordinatesOutOfBounds(int i, int j) => !InWorld(i, j);

        public static void ForEachInRectangle(Rectangle rectangle, Action<int, int> action, int addI = 1, int addJ = 1, bool boundsCheck = true)
        {
            for (int i = rectangle.X; i < rectangle.X + rectangle.Width; i += addI)
            {
                for (int j = rectangle.Y; j < rectangle.Y + rectangle.Height; j += addJ)
                {
                    if (!(boundsCheck && CoordinatesOutOfBounds(i, j)))
                        action(i, j);
                }
            }
        }

        public static void ForEachInRectangle(int i, int j, int width, int height, Action<int, int> action, int addI = 1, int addJ = 1, bool boundsCheck = true)
        {
            ForEachInRectangle(new Rectangle(i, j, width, height), action, addI, addJ, boundsCheck);
        }

        public static void FastPlaceTile(int i, int j, ushort tileType)
        {
            if (CoordinatesOutOfBounds(i, j))
            {
                return;
            }

            Tile tile = Main.tile[i, j];
            tile.TileType = tileType;
            tile.Get<TileWallWireStateData>().HasTile = true;
        }

        public static void FastRemoveTile(int i, int j)
        {
            if (CoordinatesOutOfBounds(i, j))
            {
                return;
            }

            Main.tile[i, j].Get<TileWallWireStateData>().HasTile = false;
        }

        public static void FastRemoveWall(int i, int j)
        {
            if (CoordinatesOutOfBounds(i, j))
            {
                return;
            }

            Main.tile[i, j].WallType = WallID.None;
        }

        public static void FastPlaceWall(int i, int j, int wallType)
        {
            if (CoordinatesOutOfBounds(i, j))
            {
                return;
            }

            Main.tile[i, j].WallType = (ushort)wallType;
        }

        public static void ForEachInCircle(int i, int j, int width, int height, Action<int, int> action)
        {
            ForEachInRectangle(
                i - (int)width / 2,
                j - (int)height / 2,
                width,
                height,
                (iLocal, jLocal) =>
                {
                    if (MathF.Pow((iLocal - i) / (width * 0.5f), 2) + MathF.Pow((jLocal - j) / (height * 0.5f), 2) - 1 >= 0)
                    {
                        return;
                    }

                    action(iLocal, jLocal);
                }
            );
        }

        public static void ForEachInCircle(int i, int j, int radius, Action<int, int> action)
        {
            ForEachInCircle(i, j, radius * 2, radius * 2, action);
        }

        public static bool TryPlaceObject<T>(int i, int j, int style = 0, int alternate = 0, bool sync = false) where T : ModTile
            => TryPlaceObject(i, j, ModContent.TileType<T>(), style, alternate, sync);

        public static bool TryPlaceObject(int i, int j, int type, int style = 0, int alternate = 0, bool sync = false)
        {
            TileObjectData data = TileObjectData.GetTileData(type, style, alternate);

            if (data is null)
                return false;

            int x = i - data.Origin.X;
            int y = j - data.Origin.Y;

            if (!InWorld(x, y, 10) || !InWorld(x + data.Width - 1, y + data.Height - 1))
                return false;

            for (int dx = 0; dx < data.Width; dx++)
            {
                for (int dy = 0; dy < data.Height; dy++)
                {
                    Tile tile = Main.tile[i + dx, j + dy];
                    if (tile.HasTile)
                        return false;
                }
            }

            bool result = PlaceObject(x, y, type, true, style, alternate);

            if (sync)
                NetMessage.SendObjectPlacement(-1, i, j, type, 0, 0, -1, -1);

            return result;
        }

        /// <summary>
        /// Convert a single wall tile to its specified variant (Normal, Unsafe, or Natural).
        /// </summary>
        /// <param name="x">The x-coordinate of the tile.</param>
        /// <param name="y">The y-coordinate of the tile.</param>
        /// <param name="variant">The variant type to convert to.</param>
        public static void ConvertWallSafety(int x, int y, WallSafetyType variant)
        {
            Tile tile = Main.tile[x, y];
            tile.WallType = (ushort)VariantWall.GetWallVariantType(tile.WallType, variant);
        }

        /// <summary>
        /// Convert a rectangular area of walls to their specified variant.
        /// </summary>
        /// <param name="startX">The starting x-coordinate of the area.</param>
        /// <param name="startY">The starting y-coordinate of the area.</param>
        /// <param name="width">The width of the area.</param>
        /// <param name="height">The height of the area.</param>
        /// <param name="variant">The variant type to convert to.</param>
        public static void ConvertWallSafetyInArea(int startX, int startY, int width, int height, WallSafetyType variant)
        {
            for (int x = startX; x < startX + width; x++)
            {
                for (int y = startY; y < startY + height; y++)
                {
                    ConvertWallSafety(x, y, variant);
                }
            }
        }

        /// <summary>
        /// Convert a rectangular area of walls to their specified variant, using a Rectangle.
        /// </summary>
        /// <param name="area">The Rectangle defining the area to convert.</param>
        /// <param name="variant">The variant type to convert to.</param>
        public static void ConvertWallSafetyInArea(Rectangle area, WallSafetyType variant) => ConvertWallSafetyInArea(area.X, area.Y, area.Width, area.Height, variant);

        /// <summary> This implementation also allows for tile removal (<paramref name="type"/> = -1) </summary>
        /// <inheritdoc cref="WorldGen.OreRunner(int, int, double, int, ushort)"/>
        public static void SafeTileRunner(int i, int j, double strength, int steps, int type)
        {
            double stepsD = steps;
            Vector2D position = new Vector2D(i, j);
            Vector2D movement = new((double)genRand.Next(-10, 11) * 0.1, (double)genRand.Next(-10, 11) * 0.1);

            while (strength > 0.0 && stepsD > 0.0)
            {
                if (position.Y < 0.0 && stepsD > 0.0 && type == 59)
                    stepsD = 0.0;

                strength = strength * (stepsD / (double)steps);
                stepsD -= 1.0;
                int left = (int)(position.X - strength * 0.5);
                int right = (int)(position.X + strength * 0.5);
                int top = (int)(position.Y - strength * 0.5);
                int bottom = (int)(position.Y + strength * 0.5);

                if (left < 0)
                    left = 0;

                if (right > Main.maxTilesX)
                    right = Main.maxTilesX;

                if (top < 0)
                    top = 0;

                if (bottom > Main.maxTilesY)
                    bottom = Main.maxTilesY;

                for (int k = left; k < right; k++)
                {
                    for (int l = top; l < bottom; l++)
                    {
                        if (Math.Abs((double)k - position.X) + Math.Abs((double)l - position.Y) < strength * 0.5 * (1.0 + (double)genRand.Next(-10, 11) * 0.015) && Main.tile[k, l].HasTile && (TileID.Sets.CanBeClearedDuringOreRunner[Main.tile[k, l].TileType] || (Main.remixWorld && Main.tile[k, l].TileType == 230) || (Main.tile[k, l].TileType == 225 && Main.tile[k, l].TileType != 108)))
                        {
                            if (type > 0)
                            {
                                Main.tile[k, l].TileType = (ushort)type;
                            }
                            else
                            {
                                WorldUtils.ClearTile(k, l, false);
                            }

                            Main.tile[k, l].ClearBlockPaintAndCoating();
                            SquareTileFrame(k, l);
                            if (Main.netMode == NetmodeID.Server)
                                NetMessage.SendTileSquare(-1, k, l);
                        }
                    }
                }

                position += movement;
                movement.X += (double)genRand.Next(-10, 11) * 0.05;
                if (movement.X > 1.0)
                    movement.X = 1.0;

                if (movement.X < -1.0)
                    movement.X = -1.0;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="i"></param>
        /// <param name="j"></param>
        /// <param name="tileType">if set to < 0 removes tiles.</param>
        /// <param name="repeatCount"></param>
        /// <param name="sprayRadius"></param>
        /// <param name="blobSize"></param>
        /// <param name="density"></param>
        /// <param name="smoothing"></param>
        public static void BlobTileRunner(int i, int j, int tileType, Range repeatCount, Range sprayRadius, Range blobSize, float density = 0.5f, int smoothing = 4, Func<int, int, bool> perTileCheck = null, ushort wallType = 0)
        {
            int sprayRandom = genRand.Next(repeatCount);

            Dictionary<(int, int), ushort> replacedTypes = new();

            int posI = i;
            int posJ = j;
            for (int x = 0; x < sprayRandom; x++)
            {
                posI += genRand.NextDirection(sprayRadius);
                posJ += genRand.NextDirection(sprayRadius);

                int radius = genRand.Next(blobSize);
                float densityClamped = Math.Clamp(density, 0f, 1f);
                ForEachInCircle(
                    posI,
                    posJ,
                    radius,
                    radius,
                    (i, j) =>
                    {
                        if (CoordinatesOutOfBounds(i, j) || genRand.NextFloat() > densityClamped)
                        {
                            return;
                        }

                        if (perTileCheck is null || perTileCheck(i, j))
                        {
                            if (Main.tile[i, j].HasTile && !replacedTypes.ContainsKey((i, j)))
                            {
                                replacedTypes.Add((i, j), Main.tile[i, j].TileType);
                            }

                            if (tileType < 0)
                            {
                                FastRemoveTile(i, j);
                            }
                            else
                            {
                                FastPlaceTile(i, j, (ushort)tileType);
                            }
                            if (wallType > 0)
                            {
                                FastPlaceWall(i, j, (ushort)wallType);
                            }
                        }
                    }
                );

                for (int y = 0; y < smoothing; y++)
                {
                    ForEachInCircle(
                        posI,
                        posJ,
                        radius,
                        radius,
                        (i, j) =>
                        {
                            if (tileType < 0)
                            {
                                int solidCountRemover = new TileNeighbourInfo(i, j).Solid.Count;
                                if (solidCountRemover > 4 && replacedTypes.TryGetValue((i, j), out ushort replacedType))
                                {
                                    FastPlaceTile(i, j, replacedType);
                                }
                                else if (solidCountRemover < 4)
                                {
                                    FastRemoveTile(i, j);
                                }

                                return;
                            }

                            int solidCount = new TileNeighbourInfo(i, j).IsType((ushort)tileType).Count;
                            if (solidCount > 4)
                            {
                                FastPlaceTile(i, j, (ushort)tileType);
                            }
                            else if (solidCount < 4)
                            {
                                if (replacedTypes.TryGetValue((i, j), out ushort replacedType))
                                {
                                    FastPlaceTile(i, j, replacedType);
                                }
                                else if (Main.tile[i, j].HasTile && Main.tile[i, j].TileType == tileType)
                                {
                                    FastRemoveTile(i, j);
                                }
                            }
                        }
                    );
                }
            }
        }
        //Why wasn't this a thing already
        public static void BlobLiquidTileRunner(int i, int j, int liquidType, Range repeatCount, Range sprayRadius, Range blobSize, float density = 0.5f, int smoothing = 4, Func<int, int, bool> perTileCheck = null)
        {
            int sprayRandom = genRand.Next(repeatCount);

            Dictionary<(int, int), ushort> replacedTypes = new();

            int posI = i;
            int posJ = j;
            for (int x = 0; x < sprayRandom; x++)
            {
                posI += genRand.NextDirection(sprayRadius);
                posJ += genRand.NextDirection(sprayRadius);

                int radius = genRand.Next(blobSize);
                float densityClamped = Math.Clamp(density, 0f, 1f);
                ForEachInCircle(
                    posI,
                    posJ,
                    radius,
                    radius,
                    (i, j) =>
                    {
                        if (CoordinatesOutOfBounds(i, j) || genRand.NextFloat() > densityClamped)
                        {
                            return;
                        }

                        if (perTileCheck is null || perTileCheck(i, j))
                        {
                            if (Main.tile[i, j].HasTile && !replacedTypes.ContainsKey((i, j)))
                            {
                                replacedTypes.Add((i, j), Main.tile[i, j].TileType);
                            }
                            FastRemoveTile(i, j);
                            Tile tile = Main.tile[i, j];
                            tile.LiquidAmount = 255;
                            tile.LiquidType = liquidType;

                        }
                    }
                );

            }
        }

        public static void BlobWallRunner(int i, int j, ushort wallType, Range repeatCount, Range sprayRadius, Range blobSize, float density = 0.5f, int smoothing = 4)
        {
            int sprayRandom = genRand.Next(repeatCount);

            int posI = i;
            int posJ = j;
            for (int x = 0; x < sprayRandom; x++)
            {
                posI += genRand.NextDirection(sprayRadius);
                posJ += genRand.NextDirection(sprayRadius);

                int radius = genRand.Next(blobSize);
                float densityClamped = Math.Clamp(density, 0f, 1f);
                ForEachInCircle(
                    posI,
                    posJ,
                    radius,
                    radius,
                    (i, j) =>
                    {
                        if (genRand.NextFloat() > densityClamped)
                        {
                            return;
                        }

                        FastPlaceWall(i, j, wallType);
                    }
                );

                for (int y = 0; y < smoothing; y++)
                {
                    ForEachInCircle(
                    posI,
                    posJ,
                    radius,
                    radius,
                    (i, j) =>
                    {
                        int wallCount = new TileNeighbourInfo(i, j).Wall.Count;
                        if (wallCount > 4)
                        {
                            FastPlaceWall(i, j, wallType);
                        }
                        else if (wallCount < 4)
                        {
                            FastRemoveWall(i, j);
                        }
                    }
                );
                }
            }
        }

        public static void SafePoundTile(int i, int j)
        {
            Tile tile = Main.tile[i, j];

            if (CoordinatesOutOfBounds(i, j) || !tile.HasTile)
                return;

            var info = new TileNeighbourInfo(i, j).HasTile;

            if (
                !info.Bottom ||
                info.Top ||
                (info.Right && info.Left)
                )
            {
                return;
            }

            tile.BlockType = BlockType.HalfBlock;
        }

        public static void SafeSlopeTile(int i, int j)
        {
            Tile tile = Main.tile[i, j];

            if (CoordinatesOutOfBounds(i, j) || !tile.HasTile)
                return;

            var info = new TileNeighbourInfo(i, j).HasTile;

            if (
                (info.Top && info.Right && info.Bottom) ||
                (info.Right && info.Bottom && info.Left) ||
                (info.Bottom && info.Left && info.Top) ||
                (info.Right && info.Left) ||
                (info.Top && info.Bottom)
                )
            {
                return;
            }

            if (info.Top && info.Right)
            {
                tile.BlockType = BlockType.SlopeUpRight;
                return;
            }

            if (info.Right && info.Bottom)
            {
                tile.BlockType = BlockType.SlopeDownRight;
                return;
            }

            if (info.Bottom && info.Left)
            {
                tile.BlockType = BlockType.SlopeDownLeft;
                return;
            }

            if (info.Left && info.Top)
            {
                tile.BlockType = BlockType.SlopeUpLeft;
                return;
            }
        }

        public static void GenerateOre(int tileType, double percent, int strength, int steps, int replaceTileType = -1)
        {
            for (int k = 0; k < (int)(Main.maxTilesX * Main.maxTilesY * percent); k++)
            {
                int x = genRand.Next(0, Main.maxTilesX);
                int y = genRand.Next(0, Main.maxTilesY);
                if (Main.tile[x, y].HasTile && Main.tile[x, y].TileType == replaceTileType || replaceTileType == -1)
                {
                    TileRunner(x, y, strength, steps, (ushort)tileType);
                }
            }
        }

        public static bool CheckEmptyWithSolidBottom(int x, int y, int width, int height)
        {
            for (int i = x; i < x + width; i++)
            {
                for (int j = y; j < y + height; j++)
                {
                    Tile tile = Main.tile[i, j];
                    if (tile.HasTile)
                        return false;
                }
            }

            for (int i = x; i < x + width; i++)
            {
                Tile tileBelow = Main.tile[i, y + height];
                if (!tileBelow.HasTile || !Main.tileSolid[tileBelow.TileType] || tileBelow.IsActuated)
                    return false;
            }

            return true;
        }

        public static bool CheckEmptyAboveWithSolidToTheRight(int x, int y, int width, int height)
        {
            for (int i = x; i < x + width; i++)
            {
                Tile tile = Main.tile[i, y];
                if (!tile.HasTile || !Main.tileSolid[tile.TileType] || tile.IsActuated)
                    return false;
            }

            for (int i = x; i < x + width; i++)
            {
                for (int j = y - height; j < y; j++)
                {
                    Tile tile = Main.tile[i, j];
                    if (tile.HasTile)
                        return false;
                }
            }

            return true;
        }


        /// <summary>
        /// Use with caution as it may break through the recursion limit if the maxCount is set too high.
        /// </summary>
        /// <param name="i"></param>
        /// <param name="j"></param>
        /// <param name="predicate"></param>
        /// <param name="maxCount"></param>
        /// <returns>true if maxCount is lower than the count of coordinates</returns>
        public static bool ConnectedTiles(int i, int j, Func<Tile, bool> predicate, out List<(int, int)> coordinates, int maxCount = 255)
        {
            coordinates = new() { (i, j) };
            ConnectedTilesRecursive(i, j, predicate, coordinates, maxCount);
            return coordinates.Count != maxCount;
        }

        private static void ConnectedTilesRecursive(int i, int j, Func<Tile, bool> predicate, List<(int, int)> coordinates, int maxCount)
        {
            void CheckCoordinate(int i, int j)
            {
                if (coordinates.Count == maxCount)
                {
                    return;
                }

                if (!CoordinatesOutOfBounds(i, j) && !coordinates.Contains((i, j)) && predicate.Invoke(Main.tile[i, j]))
                {
                    coordinates.Add((i, j));
                    ConnectedTilesRecursive(i, j, predicate, coordinates, maxCount);
                }
            }

            CheckCoordinate(i, j - 1);
            CheckCoordinate(i + 1, j);
            CheckCoordinate(i, j + 1);
            CheckCoordinate(i - 1, j);
        }

        public static int ConnectedTilesCount(int i, int j, Func<Tile, bool> predicate, int maxCount = 255)
        {
            ConnectedTiles(i, j, predicate, out List<(int, int)> coordinates, maxCount);
            return coordinates.Count;
        }

        public static bool TileInOuterThird(int tileX)
            => Math.Abs(tileX - Main.spawnTileX) > Main.maxTilesX / 3;

        public static bool TileInInnerThird(int tileX)
            => Math.Abs(tileX - Main.spawnTileX) > Main.maxTilesX / 3;

        public static bool TileAtOceanPosition(int tileX, int tileY)
            => oceanDepths(tileX, tileY);

        public static bool TileInDesertHive(int tileX, int tileY)
            => tileX >= GenVars.desertHiveLeft && tileX <= GenVars.desertHiveRight &&
               tileY >= GenVars.desertHiveHigh && tileY <= GenVars.desertHiveLow;


        #region BaseMod BaseWorldGen

        //------------------------------------------------------//
        //------------------- BASE WORLDGEN --------------------//
        //------------------------------------------------------//
        // Contains methods for generating various things into  //
        // the world.                                           //
        //------------------------------------------------------//
        //  Author(s): Grox the Great                           //
        //------------------------------------------------------//

        public static Tile GetTileSafely(Vector2 position)
        {
            return GetTileSafely((int)(position.X / 16f), (int)(position.Y / 16f));
        }

        public static Tile GetTileSafely(int x, int y)
        {
            if (x < 0 || x > Main.maxTilesX || y < 0 || y > Main.maxTilesY)
                return new Tile();
            return Framing.GetTileSafely(x, y);
        }

        public static void GenOre(int tileType, int amountInWorld = -1, float oreStrength = 5, int oreSteps = 5, int heightLimit = -1, bool mapDebug = false)
        {
            if (noTileActions) return;
            if (heightLimit == -1) heightLimit = (int)Main.worldSurface;
            if (amountInWorld == -1)
            {
                float oreCount = Main.maxTilesX / 4200;
                oreCount *= 50f;
                amountInWorld = (int)oreCount;
            }
            int count = 0;
            while (count < amountInWorld)
            {
                int i2 = genRand.Next(100, Main.maxTilesX - 100);
                int j2 = genRand.Next(heightLimit, Main.maxTilesY - 150);
                OreRunner(i2, j2, oreStrength, oreSteps, (ushort)tileType);
                count++;
            }
        }

        /*
			* Iterates downwards and returns the first Y position that has a tile in it.
			* startY : The y to begin iteration at.
			* solid : True if the tile must be solid.
			*/
        public static int GetFirstTileFloor(int x, int startY, bool solid = true)
        {
            if (!InWorld(x, startY)) return startY;
            for (int y = startY; y < Main.maxTilesY - 10; y++)
            {
                Tile tile = Framing.GetTileSafely(x, y);
                if (tile is { HasTile: true } && (!solid || Main.tileSolid[tile.TileType])) { return y; }
            }
            return Main.maxTilesY - 10;
        }

        /*
			* Iterates upwards and returns the first Y position that has a tile in it.
			* startY : The y to begin iteration at.
			* solid : True if the tile must be solid.
			*/
        public static int GetFirstTileCeiling(int x, int startY, bool solid = true)
        {
            if (!InWorld(x, startY)) return startY;
            for (int y = startY; y > 10; y--)
            {
                Tile tile = Framing.GetTileSafely(x, y);
                if (tile is { HasTile: true } && (!solid || Main.tileSolid[tile.TileType])) { return y; }
            }
            return 10;
        }


        public static int GetFirstTileSide(int startX, int y, bool left, bool solid = true)
        {
            if (!InWorld(startX, y)) return startX;
            if (left)
            {
                for (int x = startX; x > 10; x--)
                {
                    Tile tile = Framing.GetTileSafely(x, y);
                    if (tile is { HasTile: true } && (!solid || Main.tileSolid[tile.TileType])) { return x; }
                }
                return 10;
            }

            for (int x = startX; x < Main.maxTilesX - 10; x++)
            {
                Tile tile = Framing.GetTileSafely(x, y);
                if (tile is { HasTile: true } && (!solid || Main.tileSolid[tile.TileType])) { return x; }
            }
            return Main.maxTilesX - 10;
        }

        /**
			*  Replaces tiles within a certain radius with the replacements. (Circular)
			*
			*  position : the position of the center. (NOTE THIS IS NPC/PROJECTILE COORDS NOT TILE)
			*  radius : The radius from the position you want to replace to.
			*  tiles : the array of tiles you want to replace.
			*  replacements : the array of replacement tiles. (it goes by using the same index as tiles. Ie, tiles[0] will be replaced with replacements[0].)
			*  sync : the conditional over which of wether to sync or not.
			*  silent : If true, prevents sounds and dusts.
			*/
        public static void ReplaceTiles(Vector2 position, int radius, int[] tiles, int[] replacements, bool silent = false, bool sync = true)
        {
            int radiusLeft = (int)(position.X / 16f - radius);
            int radiusRight = (int)(position.X / 16f + radius);
            int radiusUp = (int)(position.Y / 16f - radius);
            int radiusDown = (int)(position.Y / 16f + radius);
            if (radiusLeft < 0) { radiusLeft = 0; }
            if (radiusRight > Main.maxTilesX) { radiusRight = Main.maxTilesX; }
            if (radiusUp < 0) { radiusUp = 0; }
            if (radiusDown > Main.maxTilesY) { radiusDown = Main.maxTilesY; }

            float distRad = radius * 16f;
            for (int x1 = radiusLeft; x1 <= radiusRight; x1++)
            {
                for (int y1 = radiusUp; y1 <= radiusDown; y1++)
                {
                    double dist = Vector2.Distance(new Vector2(x1 * 16f + 8f, y1 * 16f + 8f), position);
                    if (!InWorld(x1, y1)) continue;
                    if (dist < distRad && Main.tile[x1, y1] != null && Main.tile[x1, y1].HasTile)
                    {
                        int currentType = Main.tile[x1, y1].TileType;
                        int index = 0;
                        if (InArray(tiles, currentType, ref index))
                        {
                            GenerateTile(x1, y1, replacements[index], -1, 0, true, false, -2, silent, false);
                        }
                    }
                }
            }
            if (sync && Main.netMode != NetmodeID.SinglePlayer)
            {
                NetMessage.SendTileSquare(-1, (int)(position.X / 16f), (int)(position.Y / 16f), radius * 2 + 2);
            }
        }
        public static void ReplaceWalls(Vector2 position, int radius, int[] walls, int[] replacements, bool silent = false, bool sync = true)
        {
            int radiusLeft = (int)(position.X / 16f - radius);
            int radiusRight = (int)(position.X / 16f + radius);
            int radiusUp = (int)(position.Y / 16f - radius);
            int radiusDown = (int)(position.Y / 16f + radius);
            if (radiusLeft < 0) { radiusLeft = 0; }
            if (radiusRight > Main.maxTilesX) { radiusRight = Main.maxTilesX; }
            if (radiusUp < 0) { radiusUp = 0; }
            if (radiusDown > Main.maxTilesY) { radiusDown = Main.maxTilesY; }

            float distRad = radius * 16f;
            for (int x1 = radiusLeft; x1 <= radiusRight; x1++)
            {
                for (int y1 = radiusUp; y1 <= radiusDown; y1++)
                {
                    double dist = Vector2.Distance(new Vector2(x1 * 16f + 8f, y1 * 16f + 8f), position);
                    if (!InWorld(x1, y1)) continue;
                    if (dist < distRad && Main.tile[x1, y1] != null)
                    {
                        int currentType = Main.tile[x1, y1].WallType;
                        int index = 0;
                        if (InArray(walls, currentType, ref index))
                        {
                            GenerateTile(x1, y1, -1, replacements[index], 0, true, false, -2, silent, false);
                        }
                    }
                }
            }
            if (sync && Main.netMode != NetmodeID.SinglePlayer)
            {
                NetMessage.SendTileSquare(-1, (int)(position.X / 16f), (int)(position.Y / 16f), radius * 2 + 2);
            }
        }

        /**
			*  Completely kills a chest at X, Y and removes all items within it.
			*  (note this does not remove the tile itself)
			*/
        public static bool KillChestAndItems(int x, int y)
        {
            for (int i = 0; i < 1000; i++)
            {
                if (Main.chest[i] != null && Main.chest[i].x == x && Main.chest[i].y == y)
                {
                    Main.chest[i] = null;
                    return true;
                }
            }
            return false;
        }

        /**
			*  Generates a single tile of liquid.
			*  isLava == true if you want lava instead of water.
			*  updateFlow == true if you want the flow to update after placement. (almost definitely yes)
			*  liquidHeight is the height given to the liquid. (0 - 255)
			*/
        public static void GenerateLiquid(int x, int y, int liquidType, bool updateFlow = true, int liquidHeight = 255, bool sync = true)
        {
            Tile Mtile = Main.tile[x, y];

            if (!InWorld(x, y)) return;
            liquidHeight = (int)MathHelper.Clamp(liquidHeight, 0, 255);
            Main.tile[x, y].LiquidAmount = (byte)liquidHeight;
            if (liquidType == 0) { Mtile.LiquidType = LiquidID.Water; }
            else
            if (liquidType == 1) { Mtile.LiquidType = LiquidID.Lava; }
            else
            if (liquidType == 2) { Mtile.LiquidType = LiquidID.Honey; }
            if (updateFlow) { Liquid.AddWater(x, y); }
            if (sync && Main.netMode != NetmodeID.SinglePlayer) { NetMessage.SendTileSquare(-1, x, y, 1); }
        }

        /**
			*  Generates a width by height block of liquid with x, y being the top-left corner. 
			*  isLava == true if you want lava instead of water.
			*/
        public static void GenerateLiquid(int x, int y, int width, int height, int liquidType, bool updateFlow = true, int liquidHeight = 255, bool sync = true)
        {
            for (int x1 = 0; x1 < width; x1++)
            {
                for (int y1 = 0; y1 < height; y1++)
                {
                    GenerateLiquid(x1 + x, y1 + y, liquidType, updateFlow, liquidHeight, false);
                }
            }
            int size = width > height ? width : height;
            if (sync && Main.netMode != NetmodeID.SinglePlayer)
            {
                NetMessage.SendTileSquare(-1, x + (int)(width * 0.5F) - 1, y + (int)(height * 0.5F) - 1, size + 4);
            }
        }

        /*
			*  Generates a single tile and wall at the given coordinates. (if the tile is > 1 x 1 it assumes the passed in coordinate is the top left)
			*  tile : type of tile to place. -1 means don't do anything tile related, -2 is used in conjunction with active == false to make air.
			*  wall : type of wall to place. -1 means don't do anything wall related. -2 is used to remove the wall already there.
			*  tileStyle : the style of the given tile. 
			*  active : If false, will make the tile 'air' and show the wall only.
			*  removeLiquid : If true, it will remove liquids in the generating area.
			*  slope : if -2, keep the current slope. if -1, make it a halfbrick, otherwise make it the slope given.
			*  silent : If true, will not display dust nor sound.
			*  sync : If true, will sync the client and server.
			*/
        public static void GenerateTile(int x, int y, int tile, int wall, int tileStyle = 0, bool active = true, bool removeLiquid = true, int slope = -2, bool silent = false, bool sync = true)
        {
            try
            {
                Tile Mtile = Main.tile[x, y];

                if (!InWorld(x, y)) return;
                TileObjectData data = tile <= -1 ? null : TileObjectData.GetTileData(tile, tileStyle);
                int width = data == null ? 1 : data.Width;
                int height = data == null ? 1 : data.Height;
                int tileWidth = tile == -1 || data == null ? 1 : data.Width;
                int tileHeight = tile == -1 || data == null ? 1 : data.Height;
                byte oldSlope = (byte)Main.tile[x, y].Slope;
                bool oldHalfBrick = Main.tile[x, y].IsHalfBlock;
                if (tile != -1)
                {
                    destroyObject = true;
                    if (width > 1 || height > 1)
                    {
                        int xs = x, ys = y;
                        Point16 newPos = TileObjectData.TopLeft(xs, ys);
                        for (int x1 = 0; x1 < width; x1++)
                        {
                            for (int y1 = 0; y1 < height; y1++)
                            {
                                int x2 = newPos.X + x1;
                                int y2 = newPos.Y + y1;
                                if (x1 == 0 && y1 == 0 && Main.tile[x2, y2].TileType == 21) //is a chest, special case to prevent dupe glitch
                                {
                                    KillChestAndItems(x2, y2);
                                }
                                Main.tile[x, y].TileType = 0;
                                if (!silent) { KillTile(x, y, false, false, true); }
                                if (removeLiquid)
                                {
                                    GenerateLiquid(x2, y2, 0, true, 0, false);
                                }
                            }
                        }
                        for (int x1 = 0; x1 < width; x1++)
                        {
                            for (int y1 = 0; y1 < height; y1++)
                            {
                                int x2 = newPos.X + x1;
                                int y2 = newPos.Y + y1;
                                SquareTileFrame(x2, y2);
                                SquareWallFrame(x2, y2);
                            }
                        }
                    }
                    else
                    if (!silent)
                    {
                        KillTile(x, y, false, false, true);
                    }
                    destroyObject = false;
                    if (active)
                    {
                        if (tileWidth <= 1 && tileHeight <= 1 && !Main.tileFrameImportant[tile])
                        {
                            Main.tile[x, y].TileType = (ushort)tile;
                            Mtile.HasTile = true;
                            if (slope == -2 && oldHalfBrick) { Mtile.IsHalfBlock = true; }
                            else
                            if (slope == -1) { Mtile.IsHalfBlock = true; }
                            else
                            { Mtile.Slope = (SlopeType)(slope == -2 ? oldSlope : (byte)slope); }
                            SquareTileFrame(x, y);
                        }
                        else
                        {
                            destroyObject = true;
                            if (!silent)
                            {
                                for (int x1 = 0; x1 < tileWidth; x1++)
                                {
                                    for (int y1 = 0; y1 < tileHeight; y1++)
                                    {
                                        KillTile(x + x1, y + y1, false, false, true);
                                    }
                                }
                            }
                            destroyObject = false;
                            int genX = x;
                            int genY = tile == 10 ? y : y + height;
                            PlaceTile(genX, genY, tile, true, true, -1, tileStyle);
                            for (int x1 = 0; x1 < tileWidth; x1++)
                            {
                                for (int y1 = 0; y1 < tileHeight; y1++)
                                {
                                    SquareTileFrame(x + x1, y + y1);
                                }
                            }
                        }
                    }
                    else
                    {
                        Mtile.ClearTile();
                    }
                }
                if (wall != -1)
                {
                    if (wall == -2) { wall = 0; }
                    Main.tile[x, y].WallType = 0;
                    PlaceWall(x, y, wall, true);
                }
                if (sync && Main.netMode != NetmodeID.SinglePlayer)
                {
                    int sizeWidth = tileWidth + Math.Max(0, width - 1);
                    int sizeHeight = tileHeight + Math.Max(0, height - 1);
                    int size = sizeWidth > sizeHeight ? sizeWidth : sizeHeight;
                    NetMessage.SendTileSquare(-1, x + (int)(size * 0.5F), y + (int)(size * 0.5F), size + 1);
                }
            }
            catch (Exception e)
            {
                LogFancy("TILEGEN ERROR:", e);
            }
        }

        /**
			*  Generates a width by height hollow room with x, y being the top-left corner of the room, using wall as the walls in the space in the middle.
			*/
        public static void GenerateRoomOld(int x, int y, int width, int height, int tile, int wall)
        {
            GenerateRoomOld(x, y, width, height, tile, tile, tile, wall);
        }

        /**
			*  Generates a width by height hollow room with x, y being the top-left corner of the room, using wall as the walls in the space in the middle.
			*  Making any of the tile vars -1 will result in that piece of the structure not generating. (ie if tileSides == -1, both sides will be w/e was there
			*  before them.)
			*  wallEnds : true if you want every tile to have walls behind them instead of just the tileless ones.
			*/
        public static void GenerateRoomOld(int x, int y, int width, int height, int tileSides, int tileFloor, int tileCeiling, int wall, bool wallEnds = false, int sideThickness = 1, int floorThickness = 1, int ceilingThickness = 1, bool sync = true)
        {
            if (tileSides != -1 && sideThickness > 1) { width += sideThickness; x -= sideThickness / 2; }
            if (tileFloor != -1 && floorThickness > 1) { height += floorThickness; }
            if (tileCeiling != -1 && ceilingThickness > 1) { height += ceilingThickness; y -= ceilingThickness / 2; }
            for (int x1 = 0; x1 < width; x1++)
            {
                for (int y1 = 0; y1 < height; y1++)
                {
                    int x2 = x1 + x;
                    int y2 = y1 + y;
                    if ((wallEnds || tileCeiling != -1) && y1 < ceilingThickness) //ceiling
                    {
                        GenerateTile(x2, y2, tileCeiling, wallEnds && y1 == 0 ? wall : -1, 0, tileCeiling != -1 || !wallEnds, true, 0, false, false);
                    }
                    else
                    if ((wallEnds || tileFloor != -1) && y1 >= height - floorThickness) //floor
                    {
                        GenerateTile(x2, y2, tileFloor, wallEnds && y1 >= height - 1 ? wall : -1, 0, tileFloor == -1 ? !wallEnds : true, true, 0, false, false);
                    }
                    else
                    if ((wallEnds || tileSides != -1) && (x1 < sideThickness || x1 >= width - sideThickness)) //sides
                    {
                        GenerateTile(x2, y2, tileSides, wallEnds && x1 > 0 && x1 < width - 1 ? wall : -1, 0, tileSides == -1 ? !wallEnds : true, true, 0, false, false);
                    }
                    else
                    if (x1 >= sideThickness && x1 < width - sideThickness && y1 >= ceilingThickness && y1 < height - floorThickness)
                    {
                        GenerateTile(x2, y2, -1, wall, 0, false, true, 0, false, false);
                    }
                }
            }
            int size = width > height ? width : height;
            if (sync && Main.netMode != NetmodeID.SinglePlayer)
            {
                NetMessage.SendTileSquare(-1, x + (int)(width * 0.5F) - 1, y + (int)(height * 0.5F) - 1, size + 4);
            }
        }

        /**
			*  Generates a chest with the given item IDs. 
			*  randomAmounts should be true if the item(s) should have a random stack amount (between 1-5).
			*  randomPrefix should be true if the item(s) should get a random prefix.
			*/
        public static void GenerateChest(int x, int y, int type, int chestStyle, int[] stackIDs, bool randomAmounts = false, bool randomPrefix = false, bool sync = true)
        {
            int[] amounts = new int[20];
            for (int m = 0; m < amounts.Length; m++)
            {
                if (randomAmounts) { amounts[m] = genRand.Next(1, 6); } else { amounts[m] = 1; }
            }
            GenerateChest(x, y, type, chestStyle, stackIDs, amounts, randomPrefix, sync);
        }

        /**
			*  Generates a chest with the given item IDs and stack amounts. 
			*  randomPrefix should be true if the item should get a random prefix.
			*/
        public static void GenerateChest(int x, int y, int type, int chestStyle, int[] stackIDs, int[] stackAmounts, bool randomPrefix = false, bool sync = true)
        {
            int[] prefixes = new int[20];
            for (int m = 0; m < prefixes.Length; m++)
            {
                if (randomPrefix) { prefixes[m] = -1; } else { prefixes[m] = -10; }
            }
            GenerateChest(x, y, type, chestStyle, stackIDs, stackAmounts, prefixes, sync);
        }

        /**
			*  Generates a chest with the given item ids, prefixes and stack amounts.
			*/
        public static void GenerateChest(int x, int y, int type, int chestStyle, int[] stackIDs, int[] stackAmounts, int[] stackPrefixes, bool sync = true)
        {
            int num2 = PlaceChest(x - 1, y, (ushort)type, false, chestStyle);
            if (num2 >= 0)
            {
                for (int m = 0; m < Main.chest[num2].item.Length; m++)
                {
                    if (stackIDs == null || stackIDs.Length <= m) break;
                    Main.chest[num2].item[m].SetDefaults(stackIDs[m], false);
                    Main.chest[num2].item[m].stack = stackAmounts[m];
                    if (stackPrefixes[m] != -10) { Main.chest[num2].item[m].Prefix(stackPrefixes[m]); }
                }
            }
            SquareTileFrame(x + 1, y);
            if (sync && Main.netMode != NetmodeID.SinglePlayer)
            {
                NetMessage.SendTileSquare(-1, x, y, 2);
            }
        }

        public static void SmoothTiles(int topX, int topY, int bottomX, int bottomY)
        {
            Main.tileSolid[TileID.Traps] = false;
            for (int x = topX; x < bottomX; x++)
            {
                for (int y = topY; y < bottomY; y++)
                {
                    if (Main.tile[x, y].TileType != TileID.Spikes && Main.tile[x, y].TileType != TileID.Traps && Main.tile[x, y].TileType != TileID.WoodenSpikes && Main.tile[x, y].TileType != TileID.LivingWood && Main.tile[x, y].TileType != TileID.SandstoneBrick && Main.tile[x, y].TileType != TileID.SandStoneSlab)
                    {
                        if (!Main.tile[x, y - 1].HasTile)
                        {
                            if (SolidTile(x, y))
                            {
                                if (!Main.tile[x - 1, y].IsHalfBlock && !Main.tile[x + 1, y].IsHalfBlock && Main.tile[x - 1, y].Slope == 0 && Main.tile[x + 1, y].Slope == 0)
                                {
                                    if (SolidTile(x, y + 1))
                                    {
                                        if (!SolidTile(x - 1, y) && !Main.tile[x - 1, y + 1].IsHalfBlock && SolidTile(x - 1, y + 1) && SolidTile(x + 1, y) && !Main.tile[x + 1, y - 1].HasTile)
                                        {
                                            if (genRand.NextBool(2))
                                            {
                                                SlopeTile(x, y, 2);
                                            }
                                            else
                                            {
                                                PoundTile(x, y);
                                            }
                                        }
                                        else if (!SolidTile(x + 1, y) && !Main.tile[x + 1, y + 1].IsHalfBlock && SolidTile(x + 1, y + 1) && SolidTile(x - 1, y) && !Main.tile[x - 1, y - 1].HasTile)
                                        {
                                            if (genRand.NextBool(2))
                                            {
                                                SlopeTile(x, y, 1);
                                            }
                                            else
                                            {
                                                PoundTile(x, y);
                                            }
                                        }
                                        else if (SolidTile(x + 1, y + 1) && SolidTile(x - 1, y + 1) && !Main.tile[x + 1, y].HasTile && !Main.tile[x - 1, y].HasTile)
                                        {
                                            PoundTile(x, y);
                                        }
                                        if (SolidTile(x, y))
                                        {
                                            if (SolidTile(x - 1, y) && SolidTile(x + 1, y + 2) && !Main.tile[x + 1, y].HasTile && !Main.tile[x + 1, y + 1].HasTile && !Main.tile[x - 1, y - 1].HasTile)
                                            {
                                                KillTile(x, y);
                                            }
                                            else if (SolidTile(x + 1, y) && SolidTile(x - 1, y + 2) && !Main.tile[x - 1, y].HasTile && !Main.tile[x - 1, y + 1].HasTile && !Main.tile[x + 1, y - 1].HasTile)
                                            {
                                                KillTile(x, y);
                                            }
                                            else if (!Main.tile[x - 1, y + 1].HasTile && !Main.tile[x - 1, y].HasTile && SolidTile(x + 1, y) && SolidTile(x, y + 2))
                                            {
                                                if (genRand.NextBool(5)) KillTile(x, y);
                                                else if (genRand.NextBool(5)) PoundTile(x, y);
                                                else SlopeTile(x, y, 2);
                                            }
                                            else if (!Main.tile[x + 1, y + 1].HasTile && !Main.tile[x + 1, y].HasTile && SolidTile(x - 1, y) && SolidTile(x, y + 2))
                                            {
                                                if (genRand.NextBool(5))
                                                {
                                                    KillTile(x, y);
                                                }
                                                else if (genRand.NextBool(5))
                                                {
                                                    PoundTile(x, y);
                                                }
                                                else
                                                {
                                                    SlopeTile(x, y, 1);
                                                }
                                            }
                                        }
                                    }
                                    if (SolidTile(x, y) && !Main.tile[x - 1, y].HasTile && !Main.tile[x + 1, y].HasTile)
                                    {
                                        KillTile(x, y);
                                    }
                                }
                            }
                            else if (!Main.tile[x, y].HasTile && Main.tile[x, y + 1].TileType != TileID.SandstoneBrick && Main.tile[x, y + 1].TileType != TileID.SandStoneSlab)
                            {
                                if (Main.tile[x + 1, y].TileType != TileID.MushroomBlock && Main.tile[x + 1, y].TileType != TileID.Spikes && Main.tile[x + 1, y].TileType != TileID.WoodenSpikes && SolidTile(x - 1, y + 1) && SolidTile(x + 1, y) && !Main.tile[x - 1, y].HasTile && !Main.tile[x + 1, y - 1].HasTile)
                                {
                                    PlaceTile(x, y, Main.tile[x, y + 1].TileType);
                                    if (genRand.NextBool(2))
                                    {
                                        SlopeTile(x, y, 2);
                                    }
                                    else
                                    {
                                        PoundTile(x, y);
                                    }
                                }
                                if (Main.tile[x - 1, y].TileType != TileID.MushroomBlock && Main.tile[x - 1, y].TileType != TileID.Spikes && Main.tile[x - 1, y].TileType != TileID.WoodenSpikes && SolidTile(x + 1, y + 1) && SolidTile(x - 1, y) && !Main.tile[x + 1, y].HasTile && !Main.tile[x - 1, y - 1].HasTile)
                                {
                                    PlaceTile(x, y, Main.tile[x, y + 1].TileType);
                                    if (genRand.NextBool(2))
                                    {
                                        SlopeTile(x, y, 1);
                                    }
                                    else
                                    {
                                        PoundTile(x, y);
                                    }
                                }
                            }
                        }
                        else if (!Main.tile[x, y + 1].HasTile && genRand.NextBool(2) && SolidTile(x, y) && !Main.tile[x - 1, y].IsHalfBlock && !Main.tile[x + 1, y].IsHalfBlock && Main.tile[x - 1, y].Slope == 0 && Main.tile[x + 1, y].Slope == 0 && SolidTile(x, y - 1))
                        {
                            if (SolidTile(x - 1, y) && !SolidTile(x + 1, y) && SolidTile(x - 1, y - 1))
                            {
                                SlopeTile(x, y, 3);
                            }
                            else if (SolidTile(x + 1, y) && !SolidTile(x - 1, y) && SolidTile(x + 1, y - 1))
                            {
                                SlopeTile(x, y, 4);
                            }
                        }
                    }
                }
            }
            for (int x = topX; x < bottomX; x++)
            {
                for (int y = topY; y < bottomY; y++)
                {
                    if (genRand.NextBool(2) && !Main.tile[x, y - 1].HasTile && Main.tile[x, y].TileType != TileID.Traps && Main.tile[x, y].TileType != TileID.Spikes && Main.tile[x, y].TileType != TileID.WoodenSpikes && Main.tile[x, y].TileType != TileID.LivingWood && Main.tile[x, y].TileType != TileID.SandstoneBrick && Main.tile[x, y].TileType != TileID.SandStoneSlab && Main.tile[x, y].TileType != TileID.ObsidianBrick && Main.tile[x, y].TileType != TileID.HellstoneBrick && SolidTile(x, y) && Main.tile[x - 1, y].TileType != TileID.Traps && Main.tile[x + 1, y].TileType != TileID.Traps)
                    {
                        if (SolidTile(x, y + 1) && SolidTile(x + 1, y) && !Main.tile[x - 1, y].HasTile)
                        {
                            SlopeTile(x, y, 2);
                        }
                        if (SolidTile(x, y + 1) && SolidTile(x - 1, y) && !Main.tile[x + 1, y].HasTile)
                        {
                            SlopeTile(x, y, 1);
                        }
                    }
                    if (Main.tile[x, y].Slope == SlopeType.SlopeDownLeft && !SolidTile(x - 1, y))
                    {
                        SlopeTile(x, y);
                        PoundTile(x, y);
                    }
                    if (Main.tile[x, y].Slope == SlopeType.SlopeDownRight && !SolidTile(x + 1, y))
                    {
                        SlopeTile(x, y);
                        PoundTile(x, y);
                    }
                }
            }
            Main.tileSolid[TileID.Traps] = true;
        }

        public static void SmoothWorld(GenerationProgress progress)
        {
            // WARNING x WARNING x WARNING
            // Heavily nested code copied from decompiled code
            for (int tileX = 20; tileX < Main.maxTilesX - 20; tileX++)
            {
                float percentAcrossWorld = (float)tileX / (float)Main.maxTilesX;
                progress.Set(percentAcrossWorld);
                for (int tileY = 20; tileY < Main.maxTilesY - 20; tileY++)
                {
                    if (Main.tile[tileX, tileY].TileType != TileID.Spikes && Main.tile[tileX, tileY].TileType != TileID.Traps && Main.tile[tileX, tileY].TileType != TileID.WoodenSpikes && Main.tile[tileX, tileY].TileType != TileID.LivingWood && Main.tile[tileX, tileY].TileType != TileID.SandstoneBrick && Main.tile[tileX, tileY].TileType != TileID.SandStoneSlab)
                    {
                        if (!Main.tile[tileX, tileY - 1].HasTile)
                        {
                            if (SolidTile(tileX, tileY) && TileID.Sets.CanBeClearedDuringGeneration[Main.tile[tileX, tileY].TileType])
                            {
                                if (!Main.tile[tileX - 1, tileY].IsHalfBlock && !Main.tile[tileX + 1, tileY].IsHalfBlock && Main.tile[tileX - 1, tileY].Slope == SlopeType.Solid && Main.tile[tileX + 1, tileY].Slope == SlopeType.Solid)
                                {
                                    if (SolidTile(tileX, tileY + 1))
                                    {
                                        if (!SolidTile(tileX - 1, tileY) && !Main.tile[tileX - 1, tileY + 1].IsHalfBlock && SolidTile(tileX - 1, tileY + 1) && SolidTile(tileX + 1, tileY) && !Main.tile[tileX + 1, tileY - 1].HasTile)
                                        {
                                            if (genRand.NextBool(2))
                                                SlopeTile(tileX, tileY, 2);
                                            else
                                                PoundTile(tileX, tileY);
                                        }
                                        else if (!SolidTile(tileX + 1, tileY) && !Main.tile[tileX + 1, tileY + 1].IsHalfBlock && SolidTile(tileX + 1, tileY + 1) && SolidTile(tileX - 1, tileY) && !Main.tile[tileX - 1, tileY - 1].HasTile)
                                        {
                                            if (genRand.NextBool(2))
                                                SlopeTile(tileX, tileY, 1);
                                            else
                                                PoundTile(tileX, tileY);
                                        }
                                        else if (SolidTile(tileX + 1, tileY + 1) && SolidTile(tileX - 1, tileY + 1) && !Main.tile[tileX + 1, tileY].HasTile && !Main.tile[tileX - 1, tileY].HasTile)
                                        {
                                            PoundTile(tileX, tileY);
                                        }

                                        if (SolidTile(tileX, tileY))
                                        {
                                            if (SolidTile(tileX - 1, tileY) && SolidTile(tileX + 1, tileY + 2) && !Main.tile[tileX + 1, tileY].HasTile && !Main.tile[tileX + 1, tileY + 1].HasTile && !Main.tile[tileX - 1, tileY - 1].HasTile)
                                            {
                                                KillTile(tileX, tileY);
                                            }
                                            else if (SolidTile(tileX + 1, tileY) && SolidTile(tileX - 1, tileY + 2) && !Main.tile[tileX - 1, tileY].HasTile && !Main.tile[tileX - 1, tileY + 1].HasTile && !Main.tile[tileX + 1, tileY - 1].HasTile)
                                            {
                                                KillTile(tileX, tileY);
                                            }
                                            else if (!Main.tile[tileX - 1, tileY + 1].HasTile && !Main.tile[tileX - 1, tileY].HasTile && SolidTile(tileX + 1, tileY) && SolidTile(tileX, tileY + 2))
                                            {
                                                if (genRand.NextBool(5))
                                                    KillTile(tileX, tileY);
                                                else if (genRand.NextBool(5))
                                                    PoundTile(tileX, tileY);
                                                else
                                                    SlopeTile(tileX, tileY, 2);
                                            }
                                            else if (!Main.tile[tileX + 1, tileY + 1].HasTile && !Main.tile[tileX + 1, tileY].HasTile && SolidTile(tileX - 1, tileY) && SolidTile(tileX, tileY + 2))
                                            {
                                                if (genRand.NextBool(5))
                                                    KillTile(tileX, tileY);
                                                else if (genRand.NextBool(5))
                                                    PoundTile(tileX, tileY);
                                                else
                                                    SlopeTile(tileX, tileY, 1);
                                            }
                                        }
                                    }

                                    if (SolidTile(tileX, tileY) && !Main.tile[tileX - 1, tileY].HasTile && !Main.tile[tileX + 1, tileY].HasTile)
                                        KillTile(tileX, tileY);
                                }
                            }
                            else if (!Main.tile[tileX, tileY].HasTile && Main.tile[tileX, tileY + 1].TileType != TileID.SandstoneBrick && Main.tile[tileX, tileY + 1].TileType != TileID.SandStoneSlab)
                            {
                                if (Main.tile[tileX + 1, tileY].TileType != TileID.MushroomBlock && Main.tile[tileX + 1, tileY].TileType != TileID.Spikes && Main.tile[tileX + 1, tileY].TileType != TileID.WoodenSpikes && SolidTile(tileX - 1, tileY + 1) && SolidTile(tileX + 1, tileY) && !Main.tile[tileX - 1, tileY].HasTile && !Main.tile[tileX + 1, tileY - 1].HasTile)
                                {
                                    PlaceTile(tileX, tileY, Main.tile[tileX, tileY + 1].TileType, mute: true);
                                    if (genRand.NextBool(2))
                                        SlopeTile(tileX, tileY, 2);
                                    else
                                        PoundTile(tileX, tileY);
                                }

                                if (Main.tile[tileX - 1, tileY].TileType != TileID.MushroomBlock && Main.tile[tileX - 1, tileY].TileType != TileID.Spikes && Main.tile[tileX - 1, tileY].TileType != TileID.WoodenSpikes && SolidTile(tileX + 1, tileY + 1) && SolidTile(tileX - 1, tileY) && !Main.tile[tileX + 1, tileY].HasTile && !Main.tile[tileX - 1, tileY - 1].HasTile)
                                {
                                    PlaceTile(tileX, tileY, Main.tile[tileX, tileY + 1].TileType, mute: true);
                                    if (genRand.NextBool(2))
                                        SlopeTile(tileX, tileY, 1);
                                    else
                                        PoundTile(tileX, tileY);
                                }
                            }
                        }
                        else if (!Main.tile[tileX, tileY + 1].HasTile && genRand.NextBool(2) && SolidTile(tileX, tileY) && !Main.tile[tileX - 1, tileY].IsHalfBlock && !Main.tile[tileX + 1, tileY].IsHalfBlock && Main.tile[tileX - 1, tileY].Slope == SlopeType.Solid && Main.tile[tileX + 1, tileY].Slope == SlopeType.Solid && SolidTile(tileX, tileY - 1))
                        {
                            if (SolidTile(tileX - 1, tileY) && !SolidTile(tileX + 1, tileY) && SolidTile(tileX - 1, tileY - 1))
                                SlopeTile(tileX, tileY, 3);
                            else if (SolidTile(tileX + 1, tileY) && !SolidTile(tileX - 1, tileY) && SolidTile(tileX + 1, tileY - 1))
                                SlopeTile(tileX, tileY, 4);
                        }

                        if (TileID.Sets.Conversion.Sand[Main.tile[tileX, tileY].TileType])
                            Tile.SmoothSlope(tileX, tileY, applyToNeighbors: false);
                    }
                }
            }

            for (int tileX = 20; tileX < Main.maxTilesX - 20; tileX++)
            {
                for (int tileY = 20; tileY < Main.maxTilesY - 20; tileY++)
                {
                    if (genRand.NextBool(2) && !Main.tile[tileX, tileY - 1].HasTile && Main.tile[tileX, tileY].TileType != TileID.Traps && Main.tile[tileX, tileY].TileType != TileID.Spikes && Main.tile[tileX, tileY].TileType != TileID.WoodenSpikes && Main.tile[tileX, tileY].TileType != TileID.LivingWood && Main.tile[tileX, tileY].TileType != TileID.SandstoneBrick && Main.tile[tileX, tileY].TileType != TileID.SandStoneSlab && Main.tile[tileX, tileY].TileType != TileID.ObsidianBrick && Main.tile[tileX, tileY].TileType != TileID.HellstoneBrick && SolidTile(tileX, tileY) && Main.tile[tileX - 1, tileY].TileType != TileID.Traps && Main.tile[tileX + 1, tileY].TileType != TileID.Traps)
                    {
                        if (SolidTile(tileX, tileY + 1) && SolidTile(tileX + 1, tileY) && !Main.tile[tileX - 1, tileY].HasTile)
                            SlopeTile(tileX, tileY, 2);

                        if (SolidTile(tileX, tileY + 1) && SolidTile(tileX - 1, tileY) && !Main.tile[tileX + 1, tileY].HasTile)
                            SlopeTile(tileX, tileY, 1);
                    }

                    if (Main.tile[tileX, tileY].Slope == SlopeType.SlopeDownLeft && !SolidTile(tileX - 1, tileY))
                    {
                        SlopeTile(tileX, tileY);
                        PoundTile(tileX, tileY);
                    }

                    if (Main.tile[tileX, tileY].Slope == SlopeType.SlopeDownRight && !SolidTile(tileX + 1, tileY))
                    {
                        SlopeTile(tileX, tileY);
                        PoundTile(tileX, tileY);
                    }
                }
            }
        }

        #endregion

        #region Custom TileRunners

        public static void TileRunnerButItDoesntIgnoreAir(int i, int j, double strength, int steps, int type, bool addTile = false, double speedX = 0.0, double speedY = 0.0, bool noYChange = false, bool overRide = true) // Get a better name for this lol
        {
            if (!GenVars.mudWall)
            {
                if (drunkWorldGen)
                {
                    strength *= 1.0 + (double)genRand.Next(-80, 81) * 0.01;
                    steps = (int)((double)steps * (1.0 + (double)genRand.Next(-80, 81) * 0.01));
                }
                else if (remixWorldGen)
                {
                    strength *= 1.0 + (double)genRand.Next(-50, 51) * 0.01;
                }
                else if (getGoodWorldGen && type != 57)
                {
                    strength *= 1.0 + (double)genRand.Next(-80, 81) * 0.015;
                    steps += genRand.Next(3);
                }
            }

            double num = strength;
            double num2 = steps;
            Vector2D vector2D = default(Vector2D);
            vector2D.X = i;
            vector2D.Y = j;
            Vector2D vector2D2 = default(Vector2D);
            vector2D2.X = (double)genRand.Next(-10, 11) * 0.1;
            vector2D2.Y = (double)genRand.Next(-10, 11) * 0.1;
            if (speedX != 0.0 || speedY != 0.0)
            {
                vector2D2.X = speedX;
                vector2D2.Y = speedY;
            }

            while (num > 0.0 && num2 > 0.0)
            {
                if (drunkWorldGen && genRand.NextBool(30))
                {
                    vector2D.X += (double)genRand.Next(-100, 101) * 0.05;
                    vector2D.Y += (double)genRand.Next(-100, 101) * 0.05;
                }

                if (vector2D.Y < 0.0 && num2 > 0.0 && type == 59)
                    num2 = 0.0;

                num = strength * (num2 / (double)steps);
                num2 -= 1.0;
                int num3 = (int)(vector2D.X - num * 0.5);
                int num4 = (int)(vector2D.X + num * 0.5);
                int num5 = (int)(vector2D.Y - num * 0.5);
                int num6 = (int)(vector2D.Y + num * 0.5);
                if (num3 < 1)
                    num3 = 1;

                if (num4 > Main.maxTilesX - 1)
                    num4 = Main.maxTilesX - 1;

                if (num5 < 1)
                    num5 = 1;

                if (num6 > Main.maxTilesY - 1)
                    num6 = Main.maxTilesY - 1;

                for (int k = num3; k < num4; k++)
                {

                    for (int l = num5; l < num6; l++)
                    {

                        if (addTile)
                            Main.tile[k, l].LiquidAmount = 0;

                        FastPlaceTile(k, l, (ushort)type);
                    }
                }

                vector2D += vector2D2;
                if ((!drunkWorldGen || !genRand.NextBool(3)) && num > 50.0)
                {
                    vector2D += vector2D2;
                    num2 -= 1.0;
                    vector2D2.Y += (double)genRand.Next(-10, 11) * 0.05;
                    vector2D2.X += (double)genRand.Next(-10, 11) * 0.05;
                    if (num > 100.0)
                    {
                        vector2D += vector2D2;
                        num2 -= 1.0;
                        vector2D2.Y += (double)genRand.Next(-10, 11) * 0.05;
                        vector2D2.X += (double)genRand.Next(-10, 11) * 0.05;
                        if (num > 150.0)
                        {
                            vector2D += vector2D2;
                            num2 -= 1.0;
                            vector2D2.Y += (double)genRand.Next(-10, 11) * 0.05;
                            vector2D2.X += (double)genRand.Next(-10, 11) * 0.05;
                            if (num > 200.0)
                            {
                                vector2D += vector2D2;
                                num2 -= 1.0;
                                vector2D2.Y += (double)genRand.Next(-10, 11) * 0.05;
                                vector2D2.X += (double)genRand.Next(-10, 11) * 0.05;
                                if (num > 250.0)
                                {
                                    vector2D += vector2D2;
                                    num2 -= 1.0;
                                    vector2D2.Y += (double)genRand.Next(-10, 11) * 0.05;
                                    vector2D2.X += (double)genRand.Next(-10, 11) * 0.05;
                                    if (num > 300.0)
                                    {
                                        vector2D += vector2D2;
                                        num2 -= 1.0;
                                        vector2D2.Y += (double)genRand.Next(-10, 11) * 0.05;
                                        vector2D2.X += (double)genRand.Next(-10, 11) * 0.05;
                                        if (num > 400.0)
                                        {
                                            vector2D += vector2D2;
                                            num2 -= 1.0;
                                            vector2D2.Y += (double)genRand.Next(-10, 11) * 0.05;
                                            vector2D2.X += (double)genRand.Next(-10, 11) * 0.05;
                                            if (num > 500.0)
                                            {
                                                vector2D += vector2D2;
                                                num2 -= 1.0;
                                                vector2D2.Y += (double)genRand.Next(-10, 11) * 0.05;
                                                vector2D2.X += (double)genRand.Next(-10, 11) * 0.05;
                                                if (num > 600.0)
                                                {
                                                    vector2D += vector2D2;
                                                    num2 -= 1.0;
                                                    vector2D2.Y += (double)genRand.Next(-10, 11) * 0.05;
                                                    vector2D2.X += (double)genRand.Next(-10, 11) * 0.05;
                                                    if (num > 700.0)
                                                    {
                                                        vector2D += vector2D2;
                                                        num2 -= 1.0;
                                                        vector2D2.Y += (double)genRand.Next(-10, 11) * 0.05;
                                                        vector2D2.X += (double)genRand.Next(-10, 11) * 0.05;
                                                        if (num > 800.0)
                                                        {
                                                            vector2D += vector2D2;
                                                            num2 -= 1.0;
                                                            vector2D2.Y += (double)genRand.Next(-10, 11) * 0.05;
                                                            vector2D2.X += (double)genRand.Next(-10, 11) * 0.05;
                                                            if (num > 900.0)
                                                            {
                                                                vector2D += vector2D2;
                                                                num2 -= 1.0;
                                                                vector2D2.Y += (double)genRand.Next(-10, 11) * 0.05;
                                                                vector2D2.X += (double)genRand.Next(-10, 11) * 0.05;
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                vector2D2.X += (double)genRand.Next(-10, 11) * 0.05;
                if (drunkWorldGen)
                    vector2D2.X += (double)genRand.Next(-10, 11) * 0.25;

                if (vector2D2.X > 1.0)
                    vector2D2.X = 1.0;

                if (vector2D2.X < -1.0)
                    vector2D2.X = -1.0;

                if (!noYChange)
                {
                    vector2D2.Y += (double)genRand.Next(-10, 11) * 0.05;
                    if (vector2D2.Y > 1.0)
                        vector2D2.Y = 1.0;

                    if (vector2D2.Y < -1.0)
                        vector2D2.Y = -1.0;
                }
                else if (type != 59 && num < 3.0)
                {
                    if (vector2D2.Y > 1.0)
                        vector2D2.Y = 1.0;

                    if (vector2D2.Y < -1.0)
                        vector2D2.Y = -1.0;
                }

                if (type == 59 && !noYChange)
                {
                    if (vector2D2.Y > 0.5)
                        vector2D2.Y = 0.5;

                    if (vector2D2.Y < -0.5)
                        vector2D2.Y = -0.5;

                    if (vector2D.Y < Main.rockLayer + 100.0)
                        vector2D2.Y = 1.0;

                    if (vector2D.Y > (double)(Main.maxTilesY - 300))
                        vector2D2.Y = -1.0;
                }
            }
        }

        /// <summary> Used to spread walls alongside tiles. FIXME: Y change is too high </summary>
        public static void TileWallRunner(int i, int j, double strength, int steps, int tileType, bool addTile = false, int wallType = 0, bool addWall = false, float speedX = 0.0f, float speedY = 0.0f, bool noYChange = false, int ignoreTileType = -1)
        {
            double num = strength;
            double num2 = steps;

            Vector2 val = default;
            Vector2 val2 = default;

            val.X = i;
            val.Y = j;

            val2.X = genRand.Next(-10, 11) * 0.1f;
            val2.Y = genRand.Next(-10, 11) * 0.1f;

            if (speedX != 0.0 || speedY != 0.0)
            {
                val2.X = speedX;
                val2.Y = speedY;
            }

            while (num > 0.0 && num2 > 0.0)
            {
                if (drunkWorldGen && genRand.NextBool(30))
                    val.X += genRand.Next(-100, 101) * 0.05f;
                val.Y += genRand.Next(-100, 101) * 0.05f;

                if (val.Y < 0.0 && num2 > 0.0 && tileType == 59)
                    num2 = 0.0;

                num = strength * (num2 / steps);
                num2 -= 1.0;
                int num3 = (int)(val.X - num * 0.5);
                int num4 = (int)(val.X + num * 0.5);
                int num5 = (int)(val.Y - num * 0.5);
                int num6 = (int)(val.Y + num * 0.5);

                if (num3 < 1)
                    num3 = 1;

                if (num4 > Main.maxTilesX - 1)
                    num4 = Main.maxTilesX - 1;

                if (num5 < 1)
                    num5 = 1;

                if (num6 > Main.maxTilesY - 1)
                    num6 = Main.maxTilesY - 1;

                for (int k = num3; k < num4; k++)
                {
                    for (int l = num5; l < num6; l++)
                    {
                        if (ignoreTileType >= 0 && Main.tile[k, l].HasTile && Main.tile[k, l].TileType == ignoreTileType || !(Math.Abs((double)k - val.X) + Math.Abs((double)l - val.Y) < strength * 0.5 * (1.0 + genRand.Next(-10, 11) * 0.015)))
                            continue;

                        if (tileType < 0)
                            Main.tile[k, l].ClearTile();
                        else if (addTile || Main.tile[k, l].HasTile)
                            PlaceTile(k, l, tileType, true, true);

                        if (wallType == -1)
                        {
                            Main.tile[k, l].Clear(TileDataType.Wall);
                        }
                        else if (wallType > 0)
                        {
                            if (addWall || !addWall && Main.tile[k, l].WallType != 0)
                            {
                                if (Main.tile[k, l].WallType != 0)
                                    Main.tile[k, l].Clear(TileDataType.Wall);

                                PlaceWall(k, l, wallType, mute: true);
                            }
                        }
                    }
                }

                val += val2;

                if (!genRand.NextBool(3) && num > 50.0)
                {
                    val += val2;
                    num2 -= 1.0;
                    val2.Y += genRand.Next(-10, 11) * 0.05f;
                    val2.X += genRand.Next(-10, 11) * 0.05f;
                    if (num > 100.0)
                    {
                        val += val2;
                        num2 -= 1.0;
                        val2.Y += genRand.Next(-10, 11) * 0.05f;
                        val2.X += genRand.Next(-10, 11) * 0.05f;
                        if (num > 150.0)
                        {
                            val += val2;
                            num2 -= 1.0;
                            val2.Y += genRand.Next(-10, 11) * 0.05f;
                            val2.X += genRand.Next(-10, 11) * 0.05f;
                            if (num > 200.0)
                            {
                                val += val2;
                                num2 -= 1.0;
                                val2.Y += genRand.Next(-10, 11) * 0.05f;
                                val2.X += genRand.Next(-10, 11) * 0.05f;
                                if (num > 250.0)
                                {
                                    val += val2;
                                    num2 -= 1.0;
                                    val2.Y += genRand.Next(-10, 11) * 0.05f;
                                    val2.X += genRand.Next(-10, 11) * 0.05f;
                                    if (num > 300.0)
                                    {
                                        val += val2;
                                        num2 -= 1.0;
                                        val2.Y += genRand.Next(-10, 11) * 0.05f;
                                        val2.X += genRand.Next(-10, 11) * 0.05f;
                                        if (num > 400.0)
                                        {
                                            val += val2;
                                            num2 -= 1.0;
                                            val2.Y += genRand.Next(-10, 11) * 0.05f;
                                            val2.X += genRand.Next(-10, 11) * 0.05f;
                                            if (num > 500.0)
                                            {
                                                val += val2;
                                                num2 -= 1.0;
                                                val2.Y += genRand.Next(-10, 11) * 0.05f;
                                                val2.X += genRand.Next(-10, 11) * 0.05f;
                                                if (num > 600.0)
                                                {
                                                    val += val2;
                                                    num2 -= 1.0;
                                                    val2.Y += genRand.Next(-10, 11) * 0.05f;
                                                    val2.X += genRand.Next(-10, 11) * 0.05f;
                                                    if (num > 700.0)
                                                    {
                                                        val += val2;
                                                        num2 -= 1.0;
                                                        val2.Y += genRand.Next(-10, 11) * 0.05f;
                                                        val2.X += genRand.Next(-10, 11) * 0.05f;
                                                        if (num > 800.0)
                                                        {
                                                            val += val2;
                                                            num2 -= 1.0;
                                                            val2.Y += genRand.Next(-10, 11) * 0.05f;
                                                            val2.X += genRand.Next(-10, 11) * 0.05f;
                                                            if (num > 900.0)
                                                            {
                                                                val += val2;
                                                                num2 -= 1.0;
                                                                val2.Y += genRand.Next(-10, 11) * 0.05f;
                                                                val2.X += genRand.Next(-10, 11) * 0.05f;
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                val2.X += genRand.Next(-10, 11) * 0.05f;

                if (val2.X > 1.0)
                    val2.X = 1.0f;

                if (val2.X < -1.0)
                    val2.X = -1.0f;

                if (!noYChange)
                {
                    val2.Y += genRand.Next(-10, 11) * 0.05f;
                    if (val2.Y > 1.0)
                        val2.Y = 1.0f;

                    if (val2.Y < -1.0)
                        val2.Y = -1.0f;

                }
                else if (num < 3.0)
                {
                    if (val2.Y > 1.0)
                        val2.Y = 1.0f;

                    if (val2.Y < -1.0)
                        val2.Y = -1.0f;
                }
                if (!noYChange)
                {
                    if (val2.Y > 0.5)
                        val2.Y = 0.5f;

                    if (val2.Y < -0.5)
                        val2.Y = -0.5f;

                    if (val.Y < Main.rockLayer + 100.0)
                        val2.Y = 1.0f;

                    if (val.Y > (double)(Main.maxTilesY - 300))
                        val2.Y = -1.0f;
                }
            }
        }
        #endregion
    }
}