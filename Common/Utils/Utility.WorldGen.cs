using Macrocosm.Content.Tiles;
using Terraria;
using Terraria.IO;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.WorldBuilding;
using Microsoft.Xna.Framework;
using System;
using Terraria.ObjectData;
using Terraria.Localization;
using Macrocosm.Content.WorldGeneration.Base;
using Macrocosm.Common.Bases;

namespace Macrocosm.Common.Utils
{
    public static partial class Utility
    {
        public static bool CoordinatesOutOfBounds(int i, int j) => i >= Main.maxTilesX || j >= Main.maxTilesY || i < 0 || j < 0;

        public static void ForEachInRectangle(Rectangle rectangle, Action<int, int> action)
        {
            for (int i = rectangle.X; i < rectangle.X + rectangle.Width; i++)
            {
                for (int j = rectangle.Y; j < rectangle.Y + rectangle.Height; j++)
                {
                    action(i, j);
                }
            }
        }

        public static void ForEachInRectangle(int i, int j, int width, int height, Action<int, int> action)
        {
            ForEachInRectangle(new Rectangle(i, j, width, height), action);
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

        public static void FastPlaceWall(int i, int j, ushort wallType)
        {
            if (CoordinatesOutOfBounds(i, j))
            {
                return;
            }

            Main.tile[i, j].WallType = wallType;
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

        public static void TileRunner(int i, int j, ushort tileType, Range repeatCount, Range sprayRadius, Range blobSize, float density = 0.5f, int smoothing = 4)
        {
            int sprayRandom = Main.rand.Next(repeatCount);

            int posI = i;
            int posJ = j;
            for (int x = 0; x < sprayRandom; x++)
            {
                posI += Main.rand.NextDirection(sprayRadius);
                posJ += Main.rand.NextDirection(sprayRadius);

                int radius = Main.rand.Next(blobSize);
                float densityClamped = Math.Clamp(density, 0f, 1f);
                ForEachInCircle(
                    posI,
                    posJ,
                    radius,
                    radius,
                    (i, j) =>
                    {
                        if (Main.rand.NextFloat() > densityClamped)
                        {
                            return;
                        }

                        FastPlaceTile(i, j, tileType);
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
                        int solidCount = new TileNeighbourInfo(i, j).Solid.Count;
                        if (solidCount > 4)
                        {
                            FastPlaceTile(i, j, tileType);
                        }
                        else if (solidCount < 4)
                        {
                            FastRemoveTile(i, j);
                        }
                    }
                );
                }
            }
        }

        public static void WallRunner(int i, int j, ushort wallType, Range repeatCount, Range sprayRadius, Range blobSize, float density = 0.5f, int smoothing = 4)
        {
            int sprayRandom = Main.rand.Next(repeatCount);

            int posI = i;
            int posJ = j;
            for (int x = 0; x < sprayRandom; x++)
            {
                posI += Main.rand.NextDirection(sprayRadius);
                posJ += Main.rand.NextDirection(sprayRadius);

                int radius = Main.rand.Next(blobSize);
                float densityClamped = Math.Clamp(density, 0f, 1f);
                ForEachInCircle(
                    posI,
                    posJ,
                    radius,
                    radius,
                    (i, j) =>
                    {
                        if (Main.rand.NextFloat() > densityClamped)
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
                            FastRemoveTile(i, j);
                        }
                    }
                );
                }
            }
        }

        public static void GenerateOre(int tileType, double percent, int strength, int steps, int replaceTileType = -1)
        {
            for (int k = 0; k < (int)(Main.maxTilesX * Main.maxTilesY * percent); k++)
            {
                int x = WorldGen.genRand.Next(0, Main.maxTilesX);
                int y = WorldGen.genRand.Next(0, Main.maxTilesY);
                if (Main.tile[x, y].HasTile && Main.tile[x, y].TileType == replaceTileType || replaceTileType == -1)
                {
                    WorldGen.TileRunner(x, y, strength, steps, tileType);
                }
            }
        }

        public static bool CheckTile6WayBelow(int tileX, int tileY)
            => Main.tile[tileX, tileY].HasTile &&  // Current tile is active
               Main.tile[tileX - 1, tileY].HasTile &&  // Left tile is active
               Main.tile[tileX + 1, tileY].HasTile &&  // Right tile is active
               Main.tile[tileX, tileY + 1].HasTile &&  // Bottom tile is active
               Main.tile[tileX - 1, tileY + 1].HasTile &&  // Bottom-left tile is active
               Main.tile[tileX + 1, tileY + 1].HasTile &&  // Bottom-right tile is active						 
               Main.tile[tileX, tileY - 2].HasTile; // Top tile is active (will help to make the walls slightly lower than the terrain)


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
			if (WorldGen.noTileActions) return;
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
				int i2 = WorldGen.genRand.Next(100, Main.maxTilesX - 100);
				int j2 = WorldGen.genRand.Next(heightLimit, Main.maxTilesY - 150);
				WorldGen.OreRunner(i2, j2, oreStrength, oreSteps, (ushort)tileType);
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
			if (!WorldGen.InWorld(x, startY)) return startY;
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
			if (!WorldGen.InWorld(x, startY)) return startY;
			for (int y = startY; y > 10; y--)
			{
				Tile tile = Framing.GetTileSafely(x, y);
				if (tile is { HasTile: true } && (!solid || Main.tileSolid[tile.TileType])) { return y; }
			}
			return 10;
		}


		public static int GetFirstTileSide(int startX, int y, bool left, bool solid = true)
		{
			if (!WorldGen.InWorld(startX, y)) return startX;
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

		/*
			* returns the first Y position below the possible spawning height of floating islands.
			*/
		public static int GetBelowFloatingIslandY()
		{
			int size = GetWorldSize();
			return (size == 1 ? 1200 : size == 2 ? 1600 : size == 3 ? 2000 : 1200) + 1;
		}

		/**
			* Returns the current world size.
			* 1 == small, 2 == medium, 3 == large.
			*/
		public static int GetWorldSize()
		{
			if (Main.maxTilesX == 4200) { return 1; }

			if (Main.maxTilesX == 6400) { return 2; }

			if (Main.maxTilesX == 8400) { return 3; }
			return 1; //unknown size, assume small
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
					if (!WorldGen.InWorld(x1, y1)) continue;
					if (dist < distRad && Main.tile[x1, y1] != null && Main.tile[x1, y1].HasTile)
					{
						int currentType = Main.tile[x1, y1].TileType;
						int index = 0;
						if (Utility.InArray(tiles, currentType, ref index))
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
					if (!WorldGen.InWorld(x1, y1)) continue;
					if (dist < distRad && Main.tile[x1, y1] != null)
					{
						int currentType = Main.tile[x1, y1].WallType;
						int index = 0;
						if (Utility.InArray(walls, currentType, ref index))
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

			if (!WorldGen.InWorld(x, y)) return;
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

				if (!WorldGen.InWorld(x, y)) return;
				TileObjectData data = tile <= -1 ? null : TileObjectData.GetTileData(tile, tileStyle);
				int width = data == null ? 1 : data.Width;
				int height = data == null ? 1 : data.Height;
				int tileWidth = tile == -1 || data == null ? 1 : data.Width;
				int tileHeight = tile == -1 || data == null ? 1 : data.Height;
				byte oldSlope = (byte)Main.tile[x, y].Slope;
				bool oldHalfBrick = Main.tile[x, y].IsHalfBlock;
				if (tile != -1)
				{
					WorldGen.destroyObject = true;
					if (width > 1 || height > 1)
					{
						int xs = x, ys = y;
						Vector2 newPos = Utility.FindTopLeft(xs, ys);
						for (int x1 = 0; x1 < width; x1++)
						{
							for (int y1 = 0; y1 < height; y1++)
							{
								int x2 = (int)newPos.X + x1;
								int y2 = (int)newPos.Y + y1;
								if (x1 == 0 && y1 == 0 && Main.tile[x2, y2].TileType == 21) //is a chest, special case to prevent dupe glitch
								{
									KillChestAndItems(x2, y2);
								}
								Main.tile[x, y].TileType = 0;
								if (!silent) { WorldGen.KillTile(x, y, false, false, true); }
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
								int x2 = (int)newPos.X + x1;
								int y2 = (int)newPos.Y + y1;
								WorldGen.SquareTileFrame(x2, y2);
								WorldGen.SquareWallFrame(x2, y2);
							}
						}
					}
					else
					if (!silent)
					{
						WorldGen.KillTile(x, y, false, false, true);
					}
					WorldGen.destroyObject = false;
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
							WorldGen.SquareTileFrame(x, y);
						}
						else
						{
							WorldGen.destroyObject = true;
							if (!silent)
							{
								for (int x1 = 0; x1 < tileWidth; x1++)
								{
									for (int y1 = 0; y1 < tileHeight; y1++)
									{
										WorldGen.KillTile(x + x1, y + y1, false, false, true);
									}
								}
							}
							WorldGen.destroyObject = false;
							int genX = x;
							int genY = tile == 10 ? y : y + height;
							WorldGen.PlaceTile(genX, genY, tile, true, true, -1, tileStyle);
							for (int x1 = 0; x1 < tileWidth; x1++)
							{
								for (int y1 = 0; y1 < tileHeight; y1++)
								{
									WorldGen.SquareTileFrame(x + x1, y + y1);
								}
							}
						}
					}
					else
					{
						Mtile.HasTile = false;
					}
				}
				if (wall != -1)
				{
					if (wall == -2) { wall = 0; }
					Main.tile[x, y].WallType = 0;
					WorldGen.PlaceWall(x, y, wall, true);
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
				Utility.LogFancy("TILEGEN ERROR:", e);
			}
		}

		#region worldgen

		/**
			*  Generates a line of tiles/walls from one point to another point.
			*  
			*  thickness: How thick to make the walls of the line.
			*  sync : If true, will sync the client and server.
			*/
		public static void GenerateLine(GenConditions gen, int x, int y, int endX, int endY, int thickness, bool sync = true)
		{
			if (gen == null) throw new Exception("GenConditions cannot be null!");
			if (endX < x) { int temp = x; x = endX; endX = temp; }
			bool negativeY = endY < y; if (negativeY) x += Math.Abs(endX - x); //move it back since this essentially flips it on the X axis
			if (x == endX && y == endY) //it's just one tile...lol
			{
				int tileID = gen.GetTile(0), wallID = gen.GetWall(0);
				if (tileID > -1 && gen.CanPlace != null && !gen.CanPlace(x, y, tileID, wallID) || wallID > -1 && gen.CanPlaceWall != null && !gen.CanPlaceWall(x, y, tileID, wallID)) return;
				GenerateTile(x, y, tileID, wallID, 0, tileID != -1, true, 0, false, sync);
				if (gen.slope) SmoothTiles(x, y, x, y);
			}
			else
			if (x == endX || y == endY) //check to see if it's a straight line. If it is, use the less expensive method of genning.
			{
				if (endY < y) { int temp = y; y = endY; endY = temp; }
				bool vertical = x == endX;
				int tileIndex = -1, wallIndex = -1;
				for (int m = 0; m < (vertical ? endY - y : endX - x); m++)
				{
					for (int n = 0; n < thickness; n++)
					{
						tileIndex = gen.tiles == null ? -1 : gen.orderTiles ? tileIndex + 1 : WorldGen.genRand.Next(gen.tiles.Length);
						wallIndex = gen.walls == null ? -1 : gen.orderWalls ? wallIndex + 1 : WorldGen.genRand.Next(gen.walls.Length);
						if (tileIndex != -1 && tileIndex >= gen.tiles.Length) tileIndex = 0;
						if (wallIndex != -1 && wallIndex >= gen.walls.Length) wallIndex = 0;
						int addonX = vertical ? n : m, addonY = vertical ? m : n;
						int x2 = x + addonX, y2 = y + addonY;

						bool tileValid = tileIndex == -1 || gen.CanPlace == null || gen.CanPlace(x2, y2, gen.GetTile(tileIndex), gen.GetWall(wallIndex));
						bool wallValid = wallIndex == -1 || gen.CanPlaceWall == null || gen.CanPlaceWall(x2, y2, gen.GetTile(tileIndex), gen.GetWall(wallIndex));
						if (tileValid && wallValid)
						{
							GenerateTile(x2, y2, gen.GetTile(tileIndex), gen.GetWall(wallIndex), 0, gen.GetTile(tileIndex) != -1, true, 0, false, false);
						}
					}
				}
				if (gen.slope)
				{
					//SmoothTiles(x, y, x + (vertical ? thickness : Math.Abs(endX - x)), y + (vertical ? Math.Abs(endY - y) : thickness));
				}
				if (sync && Main.netMode != NetmodeID.SinglePlayer)
				{
					int size = endY - y > endX - x ? endY - y : endX - x;
					if (thickness > size) size = thickness;
					NetMessage.SendData(MessageID.TileSquare, -1, -1, NetworkText.FromLiteral(""), size, x, y);
				}
			}
			else //genning a line that isn't straight
			{
				Vector2 start = new(x, y), end = new(endX, endY), dir = new Vector2(endX, endY) - new Vector2(x, y);
				dir.Normalize();
				float length = Vector2.Distance(start, end);
				float way = 0f;

				float rot = Utility.RotationTo(start, end); if (rot < 0f) rot = (float)(Math.PI * 2f) - Math.Abs(rot);
				float rotPercent = MathHelper.Lerp(0f, 1f, rot / (float)(Math.PI * 2f));
				bool horizontal = rotPercent is < 0.125f or > 0.375f and < 0.625f or > 0.825f;
				int tileIndex = -1, wallIndex = -1;
				int lastX = x, lastY = y;
				while (way < length)
				{
					Vector2 v = start + dir * way;
					Point point = new((int)v.X, (int)v.Y);
					for (int n = 0; n < thickness; n++)
					{
						tileIndex = gen.tiles == null ? -1 : gen.orderTiles ? tileIndex + 1 : WorldGen.genRand.Next(gen.tiles.Length);
						wallIndex = gen.walls == null ? -1 : gen.orderWalls ? wallIndex + 1 : WorldGen.genRand.Next(gen.walls.Length);
						if (tileIndex != -1 && tileIndex >= gen.tiles.Length) tileIndex = 0;
						if (wallIndex != -1 && wallIndex >= gen.walls.Length) wallIndex = 0;

						int addonX = horizontal ? 0 : n, addonY = horizontal ? n : 0;
						int x2 = point.X + addonX, y2 = negativeY ? point.Y - addonY : point.Y + addonY;

						bool tileValid = tileIndex == -1 || gen.CanPlace == null || gen.CanPlace(x2, y2, gen.GetTile(tileIndex), gen.GetWall(wallIndex));
						bool wallValid = wallIndex == -1 || gen.CanPlaceWall == null || gen.CanPlaceWall(x2, y2, gen.GetTile(tileIndex), gen.GetWall(wallIndex));
						if (tileValid && wallValid)
						{
							GenerateTile(x2, y2, gen.GetTile(tileIndex), gen.GetWall(wallIndex), 0, gen.GetTile(tileIndex) != -1, true, 0, false, false);
							//if (gen.slope) SmoothTiles(x2, y2, x2 + 1, y2 + 1);
						}
					}
					if (sync && Main.netMode != NetmodeID.SinglePlayer && (!horizontal && Math.Abs(lastY - point.Y) >= 5 || horizontal && Math.Abs(lastY - point.Y) >= 5 || way + 1 > length))
					{
						int size = Math.Max(5, thickness);
						NetMessage.SendData(MessageID.TileSection, -1, -1, NetworkText.FromLiteral(""), lastX, lastY, size, size);
						lastX = point.X; lastY = point.Y;
					}
					way += 1;
				}
			}
		}

		/**
			*  Generates a hollow hallway with (x, y) as the top left. 
			*  Note that (endX, endY) is NOT the actual end of the hallway, but the end of the inner wall.
			*  
			*  thickness: How thick to make the walls of the hallway.
			*  height: The height of the hallway. (width if it's going up/down)
			*  sync : If true, will sync the client and server.
			*/
		public static void GenerateHall(GenConditions gen, int x, int y, int endX, int endY, int thickness, int height, bool sync = true)
		{
			if (gen == null) throw new Exception("GenConditions cannot be null!");
			if (endX < x) { int temp = x; x = endX; endX = temp; }
			//if (endY < y) { int temp = y; y = endY; endY = temp; }
			bool negativeX = endX < x, negativeY = endY < y;
			int nx = negativeX ? -1 : 1, ny = negativeY ? -1 : 1;
			Vector2 start = new(x, y), end = new(endX, endY);
			float rotPercent = MathHelper.Lerp(0f, 1f, Utility.RotationTo(start, end) / (float)(Math.PI * 2f));
			bool horizontal = rotPercent is < 0.125f or > 0.375f and < 0.625f or > 0.825f;
			Vector2 topEnd = new(endX, endY);
			int[] clearInt = { -2 };
			Vector2 wallStart = new(horizontal ? x : x + 2 * nx, horizontal ? y + 2 * ny : y), wallEnd = new(horizontal ? endX : endX + 2 * nx, horizontal ? endY + 2 * ny : endY);
			Vector2 bottomStart = new(horizontal ? x : x + (thickness * 2 + height) * nx, horizontal ? y + (thickness * 2 + height) * ny : y), bottomEnd = new(horizontal ? endX : endX + (thickness * 2 + height) * nx, horizontal ? endY + (thickness * 2 + height) * ny : endY);
			int[] tiles = gen.tiles, walls = gen.walls;
			gen.tiles = null;
			GenerateLine(gen, (int)wallStart.X, (int)wallStart.Y, (int)wallEnd.X, (int)wallEnd.Y, thickness * 3 + height - 2, false);
			gen.tiles = tiles;
			gen.walls = null;
			GenerateLine(gen, x, y, (int)topEnd.X, (int)topEnd.Y, thickness, false);
			GenerateLine(gen, (int)bottomStart.X, (int)bottomStart.Y, (int)bottomEnd.X, (int)bottomEnd.Y, thickness, false);
			gen.walls = walls;
		}

		/**
			*  Generates a hollow trapezoid with (x, y) as the top left. 
			*  Note that (endX, endY) is NOT the actual end of the room, but the end of the inner wall.
			*  
			*  thickness: How thick to make the walls of the trapezoid.
			*  height: The height of the trapezoid.
			*  sync : If true, will sync the client and server.
			*/
		public static void GenerateTrapezoid(GenConditions gen, int x, int y, int endX, int endY, int thickness, int height, bool sync = true)
		{
			if (gen == null) throw new Exception("GenConditions cannot be null!");
			if (endX < x) { int temp = x; x = endX; endX = temp; }
			//if (endY < y) { int temp = y; y = endY; endY = temp; }
			Vector2 start = new(x, y), end = new(endX, endY);
			float rotPercent = MathHelper.Lerp(0f, 1f, Utility.RotationTo(start, end) / (float)(Math.PI * 2f));
			bool horizontal = rotPercent is < 0.125f or > 0.375f and < 0.625f or > 0.825f;
			Vector2 topEnd = new(endX, endY);
			Vector2 wallStart = new(x + thickness, y + thickness), wallEnd = new(horizontal ? endX : endX + thickness, horizontal ? endY + thickness : endY);
			Vector2 bottomStart = new(horizontal ? x : x + thickness * 2 + height, horizontal ? y + thickness * 2 + height : y), bottomEnd = new(horizontal ? endX : endX + thickness * 2 + height, horizontal ? endY + thickness * 2 + height : endY);
			int[] tiles = gen.tiles, walls = gen.walls;
			gen.tiles = null;
			GenerateLine(gen, (int)wallStart.X, (int)wallStart.Y, (int)wallEnd.X, (int)wallEnd.Y, thickness + height, false);
			gen.tiles = tiles;
			gen.walls = null;
			GenerateLine(gen, x, y, (int)topEnd.X, (int)topEnd.Y, thickness, false);
			GenerateLine(gen, (int)bottomStart.X, (int)bottomStart.Y, (int)bottomEnd.X, (int)bottomEnd.Y, thickness, false);
			GenerateLine(gen, x, y, (int)bottomStart.X, (int)bottomStart.Y, thickness, false);
			GenerateLine(gen, (int)topEnd.X, (int)topEnd.Y, horizontal ? (int)bottomEnd.X : (int)bottomEnd.X + thickness, horizontal ? (int)bottomEnd.Y + thickness : (int)bottomEnd.Y, thickness, false);
			gen.walls = walls;
		}

		#endregion

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
				if (randomAmounts) { amounts[m] = WorldGen.genRand.Next(1, 6); } else { amounts[m] = 1; }
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
			int num2 = WorldGen.PlaceChest(x - 1, y, (ushort)type, false, chestStyle);
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
			WorldGen.SquareTileFrame(x + 1, y);
			if (sync && Main.netMode != NetmodeID.SinglePlayer)
			{
				NetMessage.SendTileSquare(-1, x, y, 2);
			}
		}

		public static void SmoothTiles(int topX, int topY, int bottomX, int bottomY)
		{
			Main.tileSolid[137] = false;
			for (int x = topX; x < bottomX; x++)
			{
				for (int y = topY; y < bottomY; y++)
				{
					if (Main.tile[x, y].TileType != 48 && Main.tile[x, y].TileType != 137 && Main.tile[x, y].TileType != 232 && Main.tile[x, y].TileType != 191 && Main.tile[x, y].TileType != 151 && Main.tile[x, y].TileType != 274)
					{
						if (!Main.tile[x, y - 1].HasTile)
						{
							if (WorldGen.SolidTile(x, y))
							{
								if (!Main.tile[x - 1, y].IsHalfBlock && !Main.tile[x + 1, y].IsHalfBlock && Main.tile[x - 1, y].Slope == 0 && Main.tile[x + 1, y].Slope == 0)
								{
									if (WorldGen.SolidTile(x, y + 1))
									{
										if (!WorldGen.SolidTile(x - 1, y) && !Main.tile[x - 1, y + 1].IsHalfBlock && WorldGen.SolidTile(x - 1, y + 1) && WorldGen.SolidTile(x + 1, y) && !Main.tile[x + 1, y - 1].HasTile)
										{
											if (WorldGen.genRand.NextBool(2))
											{
												WorldGen.SlopeTile(x, y, 2);
											}
											else
											{
												WorldGen.PoundTile(x, y);
											}
										}
										else if (!WorldGen.SolidTile(x + 1, y) && !Main.tile[x + 1, y + 1].IsHalfBlock && WorldGen.SolidTile(x + 1, y + 1) && WorldGen.SolidTile(x - 1, y) && !Main.tile[x - 1, y - 1].HasTile)
										{
											if (WorldGen.genRand.NextBool(2))
											{
												WorldGen.SlopeTile(x, y, 1);
											}
											else
											{
												WorldGen.PoundTile(x, y);
											}
										}
										else if (WorldGen.SolidTile(x + 1, y + 1) && WorldGen.SolidTile(x - 1, y + 1) && !Main.tile[x + 1, y].HasTile && !Main.tile[x - 1, y].HasTile)
										{
											WorldGen.PoundTile(x, y);
										}
										if (WorldGen.SolidTile(x, y))
										{
											if (WorldGen.SolidTile(x - 1, y) && WorldGen.SolidTile(x + 1, y + 2) && !Main.tile[x + 1, y].HasTile && !Main.tile[x + 1, y + 1].HasTile && !Main.tile[x - 1, y - 1].HasTile)
											{
												WorldGen.KillTile(x, y);
											}
											else if (WorldGen.SolidTile(x + 1, y) && WorldGen.SolidTile(x - 1, y + 2) && !Main.tile[x - 1, y].HasTile && !Main.tile[x - 1, y + 1].HasTile && !Main.tile[x + 1, y - 1].HasTile)
											{
												WorldGen.KillTile(x, y);
											}
											else if (!Main.tile[x - 1, y + 1].HasTile && !Main.tile[x - 1, y].HasTile && WorldGen.SolidTile(x + 1, y) && WorldGen.SolidTile(x, y + 2))
											{
												if (WorldGen.genRand.NextBool(5)) WorldGen.KillTile(x, y);
												else if (WorldGen.genRand.NextBool(5)) WorldGen.PoundTile(x, y);
												else WorldGen.SlopeTile(x, y, 2);
											}
											else if (!Main.tile[x + 1, y + 1].HasTile && !Main.tile[x + 1, y].HasTile && WorldGen.SolidTile(x - 1, y) && WorldGen.SolidTile(x, y + 2))
											{
												if (WorldGen.genRand.NextBool(5))
												{
													WorldGen.KillTile(x, y);
												}
												else if (WorldGen.genRand.NextBool(5))
												{
													WorldGen.PoundTile(x, y);
												}
												else
												{
													WorldGen.SlopeTile(x, y, 1);
												}
											}
										}
									}
									if (WorldGen.SolidTile(x, y) && !Main.tile[x - 1, y].HasTile && !Main.tile[x + 1, y].HasTile)
									{
										WorldGen.KillTile(x, y);
									}
								}
							}
							else if (!Main.tile[x, y].HasTile && Main.tile[x, y + 1].TileType != 151 && Main.tile[x, y + 1].TileType != 274)
							{
								if (Main.tile[x + 1, y].TileType != 190 && Main.tile[x + 1, y].TileType != 48 && Main.tile[x + 1, y].TileType != 232 && WorldGen.SolidTile(x - 1, y + 1) && WorldGen.SolidTile(x + 1, y) && !Main.tile[x - 1, y].HasTile && !Main.tile[x + 1, y - 1].HasTile)
								{
									WorldGen.PlaceTile(x, y, Main.tile[x, y + 1].TileType);
									if (WorldGen.genRand.NextBool(2))
									{
										WorldGen.SlopeTile(x, y, 2);
									}
									else
									{
										WorldGen.PoundTile(x, y);
									}
								}
								if (Main.tile[x - 1, y].TileType != 190 && Main.tile[x - 1, y].TileType != 48 && Main.tile[x - 1, y].TileType != 232 && WorldGen.SolidTile(x + 1, y + 1) && WorldGen.SolidTile(x - 1, y) && !Main.tile[x + 1, y].HasTile && !Main.tile[x - 1, y - 1].HasTile)
								{
									WorldGen.PlaceTile(x, y, Main.tile[x, y + 1].TileType);
									if (WorldGen.genRand.NextBool(2))
									{
										WorldGen.SlopeTile(x, y, 1);
									}
									else
									{
										WorldGen.PoundTile(x, y);
									}
								}
							}
						}
						else if (!Main.tile[x, y + 1].HasTile && WorldGen.genRand.NextBool(2) && WorldGen.SolidTile(x, y) && !Main.tile[x - 1, y].IsHalfBlock && !Main.tile[x + 1, y].IsHalfBlock && Main.tile[x - 1, y].Slope == 0 && Main.tile[x + 1, y].Slope == 0 && WorldGen.SolidTile(x, y - 1))
						{
							if (WorldGen.SolidTile(x - 1, y) && !WorldGen.SolidTile(x + 1, y) && WorldGen.SolidTile(x - 1, y - 1))
							{
								WorldGen.SlopeTile(x, y, 3);
							}
							else if (WorldGen.SolidTile(x + 1, y) && !WorldGen.SolidTile(x - 1, y) && WorldGen.SolidTile(x + 1, y - 1))
							{
								WorldGen.SlopeTile(x, y, 4);
							}
						}
					}
				}
			}
			for (int x = topX; x < bottomX; x++)
			{
				for (int y = topY; y < bottomY; y++)
				{
					if (WorldGen.genRand.NextBool(2) && !Main.tile[x, y - 1].HasTile && Main.tile[x, y].TileType != 137 && Main.tile[x, y].TileType != 48 && Main.tile[x, y].TileType != 232 && Main.tile[x, y].TileType != 191 && Main.tile[x, y].TileType != 151 && Main.tile[x, y].TileType != 274 && Main.tile[x, y].TileType != 75 && Main.tile[x, y].TileType != 76 && WorldGen.SolidTile(x, y) && Main.tile[x - 1, y].TileType != 137 && Main.tile[x + 1, y].TileType != 137)
					{
						if (WorldGen.SolidTile(x, y + 1) && WorldGen.SolidTile(x + 1, y) && !Main.tile[x - 1, y].HasTile)
						{
							WorldGen.SlopeTile(x, y, 2);
						}
						if (WorldGen.SolidTile(x, y + 1) && WorldGen.SolidTile(x - 1, y) && !Main.tile[x + 1, y].HasTile)
						{
							WorldGen.SlopeTile(x, y, 1);
						}
					}
					if (Main.tile[x, y].Slope == SlopeType.SlopeDownLeft && !WorldGen.SolidTile(x - 1, y))
					{
						WorldGen.SlopeTile(x, y);
						WorldGen.PoundTile(x, y);
					}
					if (Main.tile[x, y].Slope == SlopeType.SlopeDownRight && !WorldGen.SolidTile(x + 1, y))
					{
						WorldGen.SlopeTile(x, y);
						WorldGen.PoundTile(x, y);
					}
				}
			}
			Main.tileSolid[137] = true;
		}

		#endregion

		#region Custom TileRunners
		/// <summary> Used to spread walls alongside tiles. FIXME: Y change is too high </summary>
		public static void TileWallRunner(int i, int j, double strength, int steps, int tileType, bool addTile = false, int wallType = 0, bool addWall = false, float speedX = 0.0f, float speedY = 0.0f, bool noYChange = false, int ignoreTileType = -1)
        {

            double num = strength;
            double num2 = steps;

            Vector2 val = default;
            Vector2 val2 = default;

            val.X = i;
            val.Y = j;

            val2.X = WorldGen.genRand.Next(-10, 11) * 0.1f;
            val2.Y = WorldGen.genRand.Next(-10, 11) * 0.1f;

            if (speedX != 0.0 || speedY != 0.0)
            {
                val2.X = speedX;
                val2.Y = speedY;
            }

            bool flag = tileType == 368;
            bool flag2 = tileType == 367;

            while (num > 0.0 && num2 > 0.0)
            {
                if (WorldGen.drunkWorldGen && WorldGen.genRand.NextBool(30))
                    val.X += WorldGen.genRand.Next(-100, 101) * 0.05f;
                val.Y += WorldGen.genRand.Next(-100, 101) * 0.05f;

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
                        if (ignoreTileType >= 0 && Main.tile[k, l].HasTile && Main.tile[k, l].TileType == ignoreTileType || !(Math.Abs((double)k - val.X) + Math.Abs((double)l - val.Y) < strength * 0.5 * (1.0 + WorldGen.genRand.Next(-10, 11) * 0.015)))
                            continue;

                        if (tileType < 0)
                            Main.tile[k, l].ClearTile();
                        else if (addTile || Main.tile[k, l].HasTile)
                            WorldGen.PlaceTile(k, l, tileType, true, true);

                        if (wallType == -1)
                        {
                            Main.tile[k, l].Clear(Terraria.DataStructures.TileDataType.Wall);
                        }
                        else if (wallType > 0)
                        {
                            if (addWall || !addWall && Main.tile[k, l].WallType != 0)
                            {
                                if (Main.tile[k, l].WallType != 0)
                                    Main.tile[k, l].Clear(Terraria.DataStructures.TileDataType.Wall);

                                WorldGen.PlaceWall(k, l, wallType, mute: true);
                            }
                        }
                    }
                }

                val += val2;

                if (!WorldGen.genRand.NextBool(3) && num > 50.0)
                {
                    val += val2;
                    num2 -= 1.0;
                    val2.Y += WorldGen.genRand.Next(-10, 11) * 0.05f;
                    val2.X += WorldGen.genRand.Next(-10, 11) * 0.05f;
                    if (num > 100.0)
                    {
                        val += val2;
                        num2 -= 1.0;
                        val2.Y += WorldGen.genRand.Next(-10, 11) * 0.05f;
                        val2.X += WorldGen.genRand.Next(-10, 11) * 0.05f;
                        if (num > 150.0)
                        {
                            val += val2;
                            num2 -= 1.0;
                            val2.Y += WorldGen.genRand.Next(-10, 11) * 0.05f;
                            val2.X += WorldGen.genRand.Next(-10, 11) * 0.05f;
                            if (num > 200.0)
                            {
                                val += val2;
                                num2 -= 1.0;
                                val2.Y += WorldGen.genRand.Next(-10, 11) * 0.05f;
                                val2.X += WorldGen.genRand.Next(-10, 11) * 0.05f;
                                if (num > 250.0)
                                {
                                    val += val2;
                                    num2 -= 1.0;
                                    val2.Y += WorldGen.genRand.Next(-10, 11) * 0.05f;
                                    val2.X += WorldGen.genRand.Next(-10, 11) * 0.05f;
                                    if (num > 300.0)
                                    {
                                        val += val2;
                                        num2 -= 1.0;
                                        val2.Y += WorldGen.genRand.Next(-10, 11) * 0.05f;
                                        val2.X += WorldGen.genRand.Next(-10, 11) * 0.05f;
                                        if (num > 400.0)
                                        {
                                            val += val2;
                                            num2 -= 1.0;
                                            val2.Y += WorldGen.genRand.Next(-10, 11) * 0.05f;
                                            val2.X += WorldGen.genRand.Next(-10, 11) * 0.05f;
                                            if (num > 500.0)
                                            {
                                                val += val2;
                                                num2 -= 1.0;
                                                val2.Y += WorldGen.genRand.Next(-10, 11) * 0.05f;
                                                val2.X += WorldGen.genRand.Next(-10, 11) * 0.05f;
                                                if (num > 600.0)
                                                {
                                                    val += val2;
                                                    num2 -= 1.0;
                                                    val2.Y += WorldGen.genRand.Next(-10, 11) * 0.05f;
                                                    val2.X += WorldGen.genRand.Next(-10, 11) * 0.05f;
                                                    if (num > 700.0)
                                                    {
                                                        val += val2;
                                                        num2 -= 1.0;
                                                        val2.Y += WorldGen.genRand.Next(-10, 11) * 0.05f;
                                                        val2.X += WorldGen.genRand.Next(-10, 11) * 0.05f;
                                                        if (num > 800.0)
                                                        {
                                                            val += val2;
                                                            num2 -= 1.0;
                                                            val2.Y += WorldGen.genRand.Next(-10, 11) * 0.05f;
                                                            val2.X += WorldGen.genRand.Next(-10, 11) * 0.05f;
                                                            if (num > 900.0)
                                                            {
                                                                val += val2;
                                                                num2 -= 1.0;
                                                                val2.Y += WorldGen.genRand.Next(-10, 11) * 0.05f;
                                                                val2.X += WorldGen.genRand.Next(-10, 11) * 0.05f;
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
                val2.X += WorldGen.genRand.Next(-10, 11) * 0.05f;

                if (val2.X > 1.0)
                    val2.X = 1.0f;

                if (val2.X < -1.0)
                    val2.X = -1.0f;

                if (!noYChange)
                {
                    val2.Y += WorldGen.genRand.Next(-10, 11) * 0.05f;
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