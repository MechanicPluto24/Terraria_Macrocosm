using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Macrocosm;
using SubworldLibrary;
using Terraria.World.Generation;
using Terraria.ModLoader;
using Terraria.ID;
using Macrocosm.Tiles;

namespace Macrocosm.Subworlds
{
	/// <summary>
	/// Moon terrain and crater generation by 4mbr0s3 2.
	/// I'll probably tweak it later when I have time...
	/// </summary>
	public class Moon : Subworld
	{
		public override int width => 2000;
		public override int height => 1000;

        public override ModWorld modWorld => null;

        public override bool saveSubworld => true;
        public override bool disablePlayerSaving => false;
		public override bool saveModData => true;
		public override List<GenPass> tasks => new List<GenPass>()
		{
			new SubworldGenPass(progress =>
			{
				progress.Message = "Landing on the Moon...";
				Main.worldSurface = Main.maxTilesY - 42; // Hides the underground layer just out of bounds
				Main.rockLayer = Main.maxTilesY; // Hides the cavern layer way out of bounds

				int surfaceHeight = 500; // If the moon's world size is variable, this probably should depend on that
				#region Base ground
				// Generate base ground
				for (int i = 0; i < Main.maxTilesX; i++)
				{
					// Here, we just focus on the progress along the x-axis
					progress.Set((i / (float)Main.maxTilesX - 1)); // Controls the progress bar, should only be set between 0f and 1f
					for (int j = surfaceHeight; j < Main.maxTilesY; j++)
					{
						Main.tile[i, j].active(true);
						Main.tile[i, j].type = (ushort)ModContent.TileType<Regolith>();
					}

					if (WorldGen.genRand.Next(0, 10) == 0) // Not much deviation here
                    {
						surfaceHeight += WorldGen.genRand.Next(-1, 2);
						if (WorldGen.genRand.Next(0, 10) == 0)
						{
							surfaceHeight += WorldGen.genRand.Next(-2, 3);
						}
					}
				}
				#endregion
			}),
			new SubworldGenPass(progress =>
			{
				progress.Message = "Sculpting the Moon...";
				#region Sculpt craters in two different sizes like in the real moon
				// We're having two passes; this time, it's for craters
				for (int craterPass = 0; craterPass < 2; craterPass++)
				{
					// Instantiate these off the scope so we can save these for the next passes
					// These are used to prevent overlapping craters of the same pass, which would make huge gaps
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
						progress.Set((i / (float)Main.maxTilesX - 1));
						// The moon has craters... Therefore, we gotta make some flat ellipses in the world gen code!
						if (WorldGen.genRand.Next(0, craterDenominatorChance) == 0 && i > lastMaxTileX + lastXRadius)
						{
							int craterJPosition = 0;
							// Look for a Y position to put the crater
							for (int lookupY = 0; lookupY < Main.maxTilesY; lookupY++)
							{
								if (Framing.GetTileSafely(i, lookupY).active())
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
											Main.tile[craterTileX, craterTileY].active(false);
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
										Main.tile[craterTileX, craterTileY].active(false);
									}
								}
							}
							lastMaxTileX = maxTileX;
							lastXRadius = diameterX / 2;
						}
					}
				}
				#endregion
			}),
			new SubworldGenPass(progress =>
			{
				progress.Message = "Smoothening the Moon...";
				#region Smooth the moon
				// Time to smoothen up the surface
				// We're going to scan through the moon and find the top most surface tiles
				for (int i = 0; i < Main.maxTilesX; i++)
                {
					progress.Set((i / (float)Main.maxTilesX - 1));
					int j = -1;
					for (int lookupJ = 0; lookupJ < Main.maxTilesY; lookupJ++)
					{
						// Framing.GetTileSafety() only checks if the tile is null and not when it's out of bounds... Let's use it anyway
                        if (Framing.GetTileSafely(i, lookupJ).active())
						{
							j = lookupJ;
							break;
						}
					}
					if (j >= 0 && i < Main.maxTilesX - 1 && i > 0) // Prevent edge out-of-bounds
                    {
						// If the tile to the left doesn't exist, slope to the left
						bool left = !Framing.GetTileSafely(i - 1, j).active();
						// If the tile to the right doesn't exist, slope to the right
						bool right = !Framing.GetTileSafely(i + 1, j).active();

						// If both doesn't exist, flatten the tile
						if (left && right)
                        {
							Main.tile[i, j].halfBrick(true);
                        }
						else if (left)
						{
							Main.tile[i, j].slope(2);
						}
						else if (right)
						{
							Main.tile[i, j].slope(1);
						}
                    }
				}
				#endregion
			})
		};

        public override void Load()
		{
			// One Terraria day = 86400
			SLWorld.noReturn = true;
			Main.dayTime = true;
			Main.spawnTileX = 1000;
			Main.spawnTileY = 500;
			Main.numClouds = 0;
		}
	}
}
