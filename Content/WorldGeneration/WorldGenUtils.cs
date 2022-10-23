using Macrocosm.Content.Tiles;
using Terraria;
using Terraria.IO;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.WorldBuilding;
using Microsoft.Xna.Framework;
using System;

namespace Macrocosm.Content.WorldGeneration
{
	public static class WorldGenUtils  
	{
		public static void GenerateOre(int TileType, double percent, int strength, int steps, int replaceTileType = -1)
		{
			for (int k = 0; k < (int)((Main.maxTilesX * Main.maxTilesY) * percent); k++)
			{
				int x = WorldGen.genRand.Next(0, Main.maxTilesX);
				int y = WorldGen.genRand.Next(0, Main.maxTilesY);
				if (Main.tile[x, y].HasTile && Main.tile[x, y].TileType == replaceTileType || replaceTileType == -1)
				{
					WorldGen.TileRunner(x, y, strength, steps, TileType);
				}
			}
		}

		public static bool CheckTile6WayBelow(int tileX, int tileY)
			=> Main.tile[tileX    , tileY    ].HasTile &&  // Current tile is active
			   Main.tile[tileX - 1, tileY    ].HasTile &&  // Left tile is active
			   Main.tile[tileX + 1, tileY    ].HasTile &&  // Right tile is active
			   Main.tile[tileX    , tileY + 1].HasTile &&  // Bottom tile is active
			   Main.tile[tileX - 1, tileY + 1].HasTile &&  // Bottom-left tile is active
			   Main.tile[tileX + 1, tileY + 1].HasTile &&  // Bottom-right tile is active						 
			   Main.tile[tileX    , tileY - 2].HasTile; // Top tile is active (will help to make the walls slightly lower than the terrain)

		#region Custom TileRunners
		public static void TileRunner(int i, int j, double strength, int steps, int tileType, bool addTile = false, int wallType = 0, bool addWall = false, float speedX = 0.0f, float speedY = 0.0f, bool noYChange = false, int ignoreTileType = -1) { 
			
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

				num = strength * (num2 / (double)steps);
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
						if ((ignoreTileType >= 0 && Main.tile[k, l].HasTile && Main.tile[k, l].TileType == ignoreTileType) || !(Math.Abs((double)k - val.X) + Math.Abs((double)l - val.Y) < strength * 0.5 * (1.0 + (double)WorldGen.genRand.Next(-10, 11) * 0.015)))
 							continue;

						if (tileType < 0)
 							Main.tile[k, l].ClearTile();
						else if (addTile || Main.tile[k, l].HasTile)
							WorldGen.PlaceTile(k, l, tileType, true, true);

						if (addWall)
						{
							Main.tile[k, l].Clear(Terraria.DataStructures.TileDataType.Wall);
						}
						else if (wallType > 0)
						{
							if(Main.tile[k, l].WallType != wallType)
								Main.tile[k, l].Clear(Terraria.DataStructures.TileDataType.Wall);

							WorldGen.PlaceWall(k, l, wallType, mute: true);
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