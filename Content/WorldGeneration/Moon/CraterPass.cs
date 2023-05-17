using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.IO;
using Terraria.WorldBuilding;

namespace Macrocosm.Content.WorldGeneration.Moon
{
	// Crater generation by Ambrose
	public class CraterPass : GenPass
	{
		public CraterPass(string name, float loadWeight) : base(name, loadWeight) { }

		protected override void ApplyPass(GenerationProgress progress, GameConfiguration configuration)
		{
			GenerateCraters(progress);
			ErodeEdges(progress);
		}

		private void GenerateCraters(GenerationProgress progress)
		{
			progress.Message = "Sculpting the Moon...";
			for (int craterPass = 0; craterPass < 2; craterPass++)
			{
				int lastMaxTileX = 0;
				int lastXRadius = 0;
				int craterDenominatorChance = 20;
				switch (craterPass)
				{
					case 0:
						craterDenominatorChance = 5;
						break;
					case 1:
						craterDenominatorChance = 20;
						break;
					default:
						craterDenominatorChance = 30;
						break;
				}


				for (int i = 0; i < Main.maxTilesX; i++)
				{
					progress.Set(0.8f * (i / (float)(Main.maxTilesX - 1)));
					// The moon has craters... Therefore, we gotta make some flat ellipses in the world gen code!
					if (WorldGen.genRand.Next(0, craterDenominatorChance) == 0 && i > lastMaxTileX + lastXRadius)
					{
						int craterJPosition = 0;
						// Look for a Y position to put the crater
						for (int lookupY = 0; lookupY < Main.maxTilesY; lookupY++)
						{
							if (Framing.GetTileSafely(i, lookupY).HasTile)
							{
								craterJPosition = lookupY;
								break;
							}
						}
						// Create random-sized boxes in which our craters will be carved
						int radiusX;
						int radiusY;
						// Two passes for two different sizes of craters
						// (That's why the moon raggedy)
						// We could add more to roughen up the terrain more, but the terrain looks fine with two
						switch (craterPass)
						{
							// This is for the big, main craters
							case 0:
								radiusX = WorldGen.genRand.Next(8, 55);
								radiusY = WorldGen.genRand.Next(4, 10);
								break;
							case 1:
								radiusX = WorldGen.genRand.Next(8, 15);
								radiusY = WorldGen.genRand.Next(2, 4);
								break;
							default:
								radiusX = 0;
								radiusY = 0;
								break;
						}

						int minTileX = i - radiusX;
						int maxTileX = i + radiusX;
						int minTileY = craterJPosition - radiusY;
						int maxTileY = craterJPosition + radiusY;

						// Calculate diameter and center of ellipse based on the boundaries specified
						int diameterX = Math.Abs(minTileX - maxTileX);
						int diameterY = Math.Abs(minTileY - maxTileY);
						float centerX = (minTileX + maxTileX - 1) / 2f;
						float centerY = (minTileY + maxTileY - 1) / 2f;

						// Make the crater
						for (int craterTileX = minTileX; craterTileX < maxTileX; craterTileX++)
						{
							for (int craterTileY = minTileY; craterTileY < maxTileY; craterTileY++)
							{
								// This is the equation for the unit ellipse; we're dividing by squares of the diameters to scale along the axes
								if
								(
									(
										Math.Pow(craterTileX - centerX, 2) / Math.Pow(diameterX / 2, 2))
										+ (Math.Pow(craterTileY - centerY, 2) / Math.Pow(diameterY / 2, 2)
									) <= 1
								)
								{
									if (craterTileX < Main.maxTilesX && craterTileY < Main.maxTilesY && craterTileX >= 0 && craterTileY >= 0)
									{
										//Main.tile[craterTileX, craterTileY].ClearTile();
										Main.tile[craterTileX, craterTileY].ClearEverything();
									}
								}
							}
							// We're going to remove a chunk of tiles 
							// above the craters to prevent weird overhangs which clearly do not appear on the moon in real-life
							// It'll extend from the halfway mark of the ellipse to twenty tiles above the minTileY
							// Before: http://prntscr.com/toyj7u
							// After: http://prntscr.com/toylfa
							for (int craterTileY = minTileY - 20; craterTileY < maxTileY - diameterY / 2; craterTileY++)
							{
								if (craterTileX < Main.maxTilesX && craterTileY < Main.maxTilesY && craterTileX >= 0 && craterTileY >= 0)
								{
									Main.tile[craterTileX, craterTileY].ClearTile();
								}
							}
						}
						lastMaxTileX = maxTileX;
						lastXRadius = diameterX / 2;
					}
				}
			}
		}

		/// <summary> Some cleanup of rough edges </summary>
		private void ErodeEdges(GenerationProgress progress)
		{
			int numPasses = 2; // clear edges twice
			for (int clearPass = 0; clearPass < numPasses; clearPass++)
			{
				List<Point> list = new();
				for (int tileX = 2; tileX < Main.maxTilesX - 2; tileX++)
				{
					progress.Set(0.8f + (0.1f * clearPass) + (0.1f * (tileX / (float)(Main.maxTilesX - 1))));

					for (int tileY = 2; tileY < Main.maxTilesY - 2; tileY++)
					{
						// check if there's a tile, so it won't check all the surrounding tiles for nothing
						if (Main.tile[tileX, tileY].HasTile)
						{
							// if there are some tiles below..
							bool tilesBelow = Main.tile[tileX, tileY + 1].HasTile &&
										      Main.tile[tileX, tileY + 2].HasTile;

							// ..and no tiles above
							bool tilesAbove = Main.tile[tileX - 1, tileY - 1].HasTile &&
											  Main.tile[tileX    , tileY - 1].HasTile &&
											  Main.tile[tileX + 1, tileY - 1].HasTile;

							// check if there are tiles on the right and right-down
							bool tilesRight = Main.tile[tileX + 1, tileY    ].HasTile &&
											  Main.tile[tileX + 1, tileY + 1].HasTile;

							// check if there are tiles on the left and left-down
							bool tilesLeft = Main.tile[tileX - 1, tileY    ].HasTile &&
											 Main.tile[tileX - 1, tileY + 1].HasTile;

							// add the tile to a list if there are tiles on one side but not the other (XOR)
							if (tilesBelow && !tilesAbove && (tilesRight ^ tilesLeft))
								list.Add(new Point(tileX, tileY));

							break; // so it won't find a tile somewhere below the surface
						}
					}
				}

				foreach (Point p in list)
				{
					// clear the found tile 
					Main.tile[p.X, p.Y].ClearEverything();
				}

			}
		}
	}
}