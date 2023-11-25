using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace Macrocosm.Common.Utils
{
    public static partial class Utility
    {
        /// <summary>
        /// Gets the top-left tile of a multitile
        /// </summary>
        /// <param name="i">The tile X-coordinate</param>
        /// <param name="j">The tile Y-coordinate</param>
        public static Point16 GetTileOrigin(int i, int j)
        {
            //Framing.GetTileSafely ensures that the returned Tile instance is not null
            //Do note that neither this method nor Framing.GetTileSafely check if the wanted coordiates are in the world!
            Tile tile = Framing.GetTileSafely(i, j);

            Point16 coord = new Point16(i, j);
            Point16 frame = new Point16(tile.TileFrameX / 18, tile.TileFrameY / 18);

            return coord - frame;
        }

        /// <summary>
        /// Uses <seealso cref="GetTileOrigin(int, int)"/> to try to get the entity bound to the multitile at (<paramref name="i"/>, <paramref name="j"/>).
        /// </summary>
        /// <typeparam name="T">The type to get the entity as</typeparam>
        /// <param name="i">The tile X-coordinate</param>
        /// <param name="j">The tile Y-coordinate</param>
        /// <param name="entity">The found <typeparamref name="T"/> instance, if there was one.</param>
        /// <returns><see langword="true"/> if there was a <typeparamref name="T"/> instance, or <see langword="false"/> if there was no entity present OR the entity was not a <typeparamref name="T"/> instance.</returns>
        public static bool TryGetTileEntityAs<T>(int i, int j, out T entity) where T : TileEntity
        {
            Point16 origin = GetTileOrigin(i, j);

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

        public static Vector2 ToWorldCoordinates(this Point point)
            => new(point.X * 16f, point.Y * 16f);

        public static Vector2 ToWorldCoordinates(this Point16 point)
            => new(point.X * 16f, point.Y * 16f);

        public static bool IsSloped(this Tile tile)
        {
            return (int)tile.BlockType > 1;
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

        public static bool SandTileFrame(int i, int j, int projectileType)
        {
            if (WorldGen.noTileActions)
                return true;

            Tile above = Main.tile[i, j - 1];
            Tile below = Main.tile[i, j + 1];
            bool canFall = true;

            if (below == null || below.HasTile)
                canFall = false;

            if (above.HasTile && (TileID.Sets.BasicChest[above.TileType] || TileID.Sets.BasicChestFake[above.TileType] || above.TileType == TileID.PalmTree))
                canFall = false;

            if (canFall)
            {
                float positionX = i * 16 + 8;
                float positionY = j * 16 + 8;

                if (Main.netMode == NetmodeID.SinglePlayer)
                {
                    Main.tile[i, j].ClearTile();
                    int proj = Projectile.NewProjectile(new EntitySource_TileBreak(i, j), positionX, positionY, 0f, 0.41f, projectileType, 10, 0f, Main.myPlayer);
                    Main.projectile[proj].ai[0] = 1f;
                    WorldGen.SquareTileFrame(i, j);
                }
                else if (Main.netMode == NetmodeID.Server)
                {
                    Main.tile[i, j].ClearTile();
                    bool spawnProj = true;

                    for (int k = 0; k < 1000; k++)
                    {
                        Projectile otherProj = Main.projectile[k];

                        if (otherProj.active && otherProj.owner == Main.myPlayer && otherProj.type == projectileType && Math.Abs(otherProj.timeLeft - 3600) < 60 && otherProj.Distance(new Vector2(positionX, positionY)) < 4f)
                        {
                            spawnProj = false;
                            break;
                        }
                    }

                    if (spawnProj)
                    {
                        int proj = Projectile.NewProjectile(new EntitySource_TileBreak(i, j), positionX, positionY, 0f, 2.5f, projectileType, 10, 0f, Main.myPlayer);
                        Main.projectile[proj].velocity.Y = 0.5f;
                        Main.projectile[proj].position.Y += 2f;
                        Main.projectile[proj].netUpdate = true;
                    }

                    NetMessage.SendTileSquare(-1, i, j, 1);
                    WorldGen.SquareTileFrame(i, j);
                }
                return false;
            }
            return true;
        }

        public static void DrawTileGlowmask(int i, int j, SpriteBatch spriteBatch, Texture2D glowmask, Color color)
        {
            Tile tile = Main.tile[i, j];

            if (tile.BlockType != BlockType.Solid)
                return;

            Vector2 zero = Main.drawToScreen ? Vector2.Zero : new Vector2(Main.offScreenRange);

            spriteBatch.Draw
            (
                glowmask,
                new Vector2(i * 16 - (int)Main.screenPosition.X, j * 16 - (int)Main.screenPosition.Y) + zero,
                new Rectangle(tile.TileFrameX, tile.TileFrameY, 16, 16),
                color, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f
            );
        }

        #region BaseMod Utility
        //------------------------------------------------------//
        //---------------------- BASE TILE ---------------------//
        //------------------------------------------------------//
        // Contains methods dealing with tiles, except          //
        // generation. (for that, see BaseWorldGen/BaseGoreGen) //
        //------------------------------------------------------//
        //  Author(s): Grox the Great                           //
        //------------------------------------------------------//

        public static void AddMapEntry(ModTile tile, Color color)
        {
            tile.AddMapEntry(color);
        }

        public static void AddMapEntry(ModTile tile, Color color, string name)
        {
            LocalizedText name2 = tile.CreateMapEntryName();
            // name2.SetDefault(name);
            tile.AddMapEntry(color, name2);
        }

        public static void SetTileFrame(int x, int y, int tileWidth, int tileHeight, int frame, int tileFrameWidth = 16)
        {
            int type = Main.tile[x, y].TileType;
            int frameWidth = (tileFrameWidth + 2) * tileWidth;
            for (int x1 = 0; x1 < tileWidth; x1++)
            {
                for (int y1 = 0; y1 < tileHeight; y1++)
                {
                    int x2 = x + x1; int y2 = y + y1;
                    Main.tile[x2, y2].TileFrameX = (short)(frame * frameWidth + (tileFrameWidth + 2) * x1);
                }
            }
        }

        /*
         * Returns all tiles of the given type nearby using the given distance.
		 * 
		 * distance: how far from the x, y coordinates in tiles to check.
		 * addTile : action that can be used to have custom check parameters.
         */
        public static Vector2 GetClosestTile(int x, int y, int type, int distance = 25, Func<Tile, bool> addTile = null)
        {
            Vector2 originalPos = new(x, y);
            int leftX = Math.Max(10, x - distance);
            int leftY = Math.Max(10, y - distance);
            int rightX = Math.Min(Main.maxTilesX - 10, x + distance);
            int rightY = Math.Min(Main.maxTilesY - 10, y + distance);
            Vector2 pos = default;
            float dist = -1;
            for (int x1 = leftX; x1 < rightX; x1++)
            {
                for (int y1 = leftY; y1 < rightY; y1++)
                {
                    Tile tile = Framing.GetTileSafely(x1, y1);
                    if (tile is { HasTile: true } && tile.TileType == type && (addTile == null || addTile(tile)) && (dist == -1 || Vector2.Distance(originalPos, new Vector2(x1, y1)) < dist))
                    {
                        dist = Vector2.Distance(originalPos, new Vector2(x1, y1));
                        if (type == 21 || TileObjectData.GetTileData(tile.TileType, 0) != null && (TileObjectData.GetTileData(tile.TileType, 0).Width > 1 || TileObjectData.GetTileData(tile.TileType, 0).Height > 1))
                        {
                            int x2 = x1; int y2 = y1;
                            if (type == 21)
                            {
                                x2 -= tile.TileFrameX / 18 % 2;
                                y2 -= tile.TileFrameY / 18 % 2;
                            }
                            else
                            {
                                Vector2 top = FindTopLeft(x2, y2);
                                x2 = (int)top.X; y2 = (int)top.Y;
                            }
                            pos = new Vector2(x2, y2);
                        }
                        else
                        {
                            pos = new Vector2(x1, y1);
                        }
                    }
                }
            }
            return pos;
        }

        public static Point FindTopLeftPoint(int x, int y)
        {
            Vector2 v2 = FindTopLeft(x, y);
            return new Point((int)v2.X, (int)v2.Y);
        }

        public static Vector2 FindTopLeft(int x, int y)
        {
            Tile tile = Framing.GetTileSafely(x, y); if (tile == null) return new Vector2(x, y);
            TileObjectData data = TileObjectData.GetTileData(tile.TileType, 0);
            x -= tile.TileFrameX / 18 % data.Width;
            y -= tile.TileFrameY / 18 % data.Height;
            return new Vector2(x, y);
        }

        /*
         * Returns all tiles of the given type nearby using the given distance.
		 * 
		 * distance: how far from the x, y coordinates in tiles to check.
		 * addTile : action that can be used to have custom check parameters.
         */
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
                                Point p = FindTopLeftPoint(x2, y2); x2 = p.X; y2 = p.Y;
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
        ///</summary>
        public static bool IsPlatform(int type)
        {
            return Main.tileSolid[type] && Main.tileSolidTop[type];
        }

        public static bool AlchemyFlower(int type) { return type is 82 or 83 or 84; }

        ///<summary>
        /// Goes through a square area given by the x, y and width, height params, and returns true if they are all of the type given.
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

        /**
         * Return the count of tiles and walls of the given types within the given distance.
         * If a location has a tile and a wall it is only counted once.
         */
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

        /**
         * Return the count of walls of the given types within the given distance.
         */
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

        /**
         * Return the count of tiles of the given types within the given distance.
         */
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

        #endregion

    }
}
