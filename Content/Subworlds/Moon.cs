using System;
using System.Collections.Generic;
using Terraria;
using SubworldLibrary;
using Terraria.World.Generation;
using Terraria.ModLoader;
using Terraria.ID;
using Macrocosm.Content.Tiles;
using Terraria.UI;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Macrocosm.Common;
using Terraria.UI.Chat;

namespace Macrocosm.Content.Subworlds
{
	/// <summary>
	/// Moon terrain and crater generation by 4mbr0s3 2
	/// Why isn't anyone else working on this
	/// I have saved the day - Ryan
	/// </summary>
	public class Moon : Subworld
	{
		public override int width => 2000;
		public override int height => 1000;

		public override ModWorld modWorld => null;

		public override bool saveSubworld => true;
		public override bool disablePlayerSaving => false;
		public override bool saveModData => true;
		public static double rockLayerHigh = 0.0;
		public static double rockLayerLow = 0.0;
		public static double surfaceLayer = 200;
		public override List<GenPass> tasks => new List<GenPass>()
		{
			new SubworldGenPass(progress =>
			{
				progress.Message = "Landing on the Moon...";
				Main.worldSurface = surfaceLayer + 20; // Hides the underground layer just out of bounds
				Main.rockLayer = surfaceLayer + 60; // Hides the cavern layer way out of bounds

				int surfaceHeight = (int)surfaceLayer; // If the moon's world size is variable, this probably should depend on that
				rockLayerLow = surfaceHeight;
				rockLayerHigh = surfaceHeight;
				#region Base ground
				// Generate base ground
				for (int i = 0; i < Main.maxTilesX; i++)
				{
					// Here, we just focus on the progress along the x-axis
					progress.Set((i / (float)Main.maxTilesX - 1)); // Controls the progress bar, should only be set between 0f and 1f
					for (int j = surfaceHeight; j < height; j++)
					{
						Main.tile[i, j].active(true);
						Main.tile[i, j].type = (ushort)ModContent.TileType<Tiles.Protolith>();
					}

					if (WorldGen.genRand.Next(0, 10) == 0) // Not much deviation here
					{
						surfaceHeight += WorldGen.genRand.Next(-1, 2);
						if (WorldGen.genRand.Next(0, 10) == 0)
						{
							surfaceHeight += WorldGen.genRand.Next(-2, 3);
						}
					}

					if (surfaceHeight < rockLayerLow)
						rockLayerLow = surfaceHeight;

					if (surfaceHeight > rockLayerHigh)
						rockLayerHigh = surfaceHeight;

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
				// This generates before lunar caves so that there are overhangs
				progress.Message = "Backgrounding the Moon...";
				#region Generate regolith walls
				for (int tileX = 1; tileX < Main.maxTilesX - 1; tileX++) {
					int wall = 2;
					float progressPercent = tileX / Main.maxTilesX;
					progress.Set(progressPercent);
					bool surroundedTile = false;
					for (int tileY = 2; tileY < Main.maxTilesY - 1; tileY++) {
						if (Main.tile[tileX, tileY].active())
							wall = ModContent.WallType<Walls.RegolithWall>();

						if (surroundedTile)
							Main.tile[tileX, tileY].wall = (ushort)wall;

						if
						(
							Main.tile[tileX, tileY].active() // Current tile is active
							&& Main.tile[tileX - 1, tileY].active() // Left tile is active
							&& Main.tile[tileX + 1, tileY].active() // Right tile is active
							&& Main.tile[tileX, tileY + 1].active() // Bottom tile is active
							&& Main.tile[tileX - 1, tileY + 1].active() // Bottom-left tile is active
							&& Main.tile[tileX + 1, tileY + 1].active() // Bottom-right tile is active
							// The following will help to make the walls slightly lower than the terrain
							&& Main.tile[tileX, tileY - 2].active() // Top tile is active
						)
						{
							surroundedTile = true; // Set the rest of the walls down the column
						}
					}
				}
				#endregion
			}),
			new SubworldGenPass(progress =>
			{
				progress.Message = "Sending meteors to the Moon...";
				#region Generate regolith
				for (int tileX = 1; tileX < Main.maxTilesX - 1; tileX++) {
					float progressPercent = tileX / Main.maxTilesX;
					progress.Set(progressPercent / 2f);
					float regolithChance = 6;
					for (int tileY = 1; tileY < Main.maxTilesY; tileY++)
					{
						if (Main.tile[tileX, tileY].active())
						{
							if (regolithChance > 0.1)
							{
								Main.tile[tileX, tileY].type = (ushort)ModContent.TileType<Tiles.Regolith>();
							}
							regolithChance -= 0.02f;
							if (regolithChance <= 0) break;
						}
					}
				}
				// Generate protolith veins
				for (int tileX = 1; tileX < Main.maxTilesX - 1; tileX++) {
					float progressPercent = tileX / Main.maxTilesX;
					progress.Set(0.5f + progressPercent / 2f);
					float regolithChance = 6;
					for (int tileY = 1; tileY < Main.maxTilesY; tileY++)
					{
						if (Main.tile[tileX, tileY].active())
						{
							double veinChance = (6 - regolithChance) / 6f * 0.006;
							if (WorldGen.genRand.NextFloat() < veinChance || veinChance == 0)
							{
								//WorldGen.TileRunner(tileX, tileY, WorldGen.genRand.Next((int)(6 * veinChance / 0.005), (int)(20 * veinChance / 0.005)), WorldGen.genRand.Next(50, 300), ModContent.TileType<Tiles.Protolith>());
								WorldGen.TileRunner(tileX, tileY, WorldGen.genRand.Next((int)(6 * veinChance / 0.003), (int)(20 * veinChance / 0.003)), WorldGen.genRand.Next(5, 19), ModContent.TileType<Tiles.Protolith>());
							}
							regolithChance -= 0.02f;
							if (regolithChance < 0) break;
						}
					}
				}
				#endregion
			}),
			new SubworldGenPass(progress =>
			{
				void GenerateOre(int type, double percent, int strength, int steps)
				{
					for (int k = 0; k < (int)((Main.maxTilesX * Main.maxTilesY) * percent); k++)
					{
						int x = WorldGen.genRand.Next(0, Main.maxTilesX);
						int y = WorldGen.genRand.Next(0, Main.maxTilesY);
						if (Main.tile[x, y].active() && Main.tile[x, y].type == ModContent.TileType<Tiles.Protolith>())
						{
							WorldGen.TileRunner(x, y, strength, steps, type);
						}
					}
				}
				progress.Message = "Mineralizing the Moon...";
				#region Generate ore veins
				GenerateOre(ModContent.TileType<ArtemiteOre>(), 0.0001, WorldGen.genRand.Next(5, 9), WorldGen.genRand.Next(5, 9));
				GenerateOre(ModContent.TileType<ChandriumOre>(), 0.0001, WorldGen.genRand.Next(5, 9), WorldGen.genRand.Next(5, 9));
				GenerateOre(ModContent.TileType<DianiteOre>(), 0.0001, WorldGen.genRand.Next(5, 9), WorldGen.genRand.Next(5, 9));
				GenerateOre(ModContent.TileType<SeleniteOre>(), 0.0001, WorldGen.genRand.Next(5, 9), WorldGen.genRand.Next(5, 9));
				#endregion
			}),
			new SubworldGenPass(progress =>
			{
				progress.Message = "Carving the Moon...";
				for (int currentCaveSpot = 0; currentCaveSpot < (int)((double)(Main.maxTilesX * Main.maxTilesY) * 0.00013); currentCaveSpot++) {
					float percentDone = (float)((double)currentCaveSpot / ((double)(Main.maxTilesX * Main.maxTilesY) * 0.00013));
					progress.Set(percentDone);
					if (rockLayerHigh <= (double)Main.maxTilesY) {
						int airType = -1;
						WorldGen.TileRunner(WorldGen.genRand.Next(0, Main.maxTilesX), WorldGen.genRand.Next((int)rockLayerLow, Main.maxTilesY), WorldGen.genRand.Next(6, 20), WorldGen.genRand.Next(50, 300), airType);
					}
				}
				//float iterations = Main.maxTilesX / 4200;
				//for (int iterationIndex = 0; (float)iterationIndex < 5f * iterations; iterationIndex++) {
				//	try
				//	{
				//		// Randomly carve interconnected tunnels
				//		WorldGen.Caverer(WorldGen.genRand.Next(100, Main.maxTilesX - 100), WorldGen.genRand.Next((int)surfaceLayer, Main.maxTilesY - 50));
				//	}
				//	catch {}
				//}
			}),
			new SubworldGenPass(progress =>
			{
				progress.Message = "Smoothening the Moon...";
				// Adopted (copy-pasted) from "Smooth World" gen pass
				for (int tileX = 20; tileX < Main.maxTilesX - 20; tileX++) {
					float percentAcrossWorld = (float)tileX / (float)Main.maxTilesX;
					progress.Set(percentAcrossWorld);
					for (int tileY = 20; tileY < Main.maxTilesY - 20; tileY++) {
						if (Main.tile[tileX, tileY].type != 48 && Main.tile[tileX, tileY].type != 137 && Main.tile[tileX, tileY].type != 232 && Main.tile[tileX, tileY].type != 191 && Main.tile[tileX, tileY].type != 151 && Main.tile[tileX, tileY].type != 274) {
							if (!Main.tile[tileX, tileY - 1].active()) {
								if (WorldGen.SolidTile(tileX, tileY) && TileID.Sets.CanBeClearedDuringGeneration[Main.tile[tileX, tileY].type]) {
									if (!Main.tile[tileX - 1, tileY].halfBrick() && !Main.tile[tileX + 1, tileY].halfBrick() && Main.tile[tileX - 1, tileY].slope() == 0 && Main.tile[tileX + 1, tileY].slope() == 0) {
										if (WorldGen.SolidTile(tileX, tileY + 1)) {
											if (!WorldGen.SolidTile(tileX - 1, tileY) && !Main.tile[tileX - 1, tileY + 1].halfBrick() && WorldGen.SolidTile(tileX - 1, tileY + 1) && WorldGen.SolidTile(tileX + 1, tileY) && !Main.tile[tileX + 1, tileY - 1].active()) {
												if (WorldGen.genRand.Next(2) == 0)
													WorldGen.SlopeTile(tileX, tileY, 2);
												else
													WorldGen.PoundTile(tileX, tileY);
											}
											else if (!WorldGen.SolidTile(tileX + 1, tileY) && !Main.tile[tileX + 1, tileY + 1].halfBrick() && WorldGen.SolidTile(tileX + 1, tileY + 1) && WorldGen.SolidTile(tileX - 1, tileY) && !Main.tile[tileX - 1, tileY - 1].active()) {
												if (WorldGen.genRand.Next(2) == 0)
													WorldGen.SlopeTile(tileX, tileY, 1);
												else
													WorldGen.PoundTile(tileX, tileY);
											}
											else if (WorldGen.SolidTile(tileX + 1, tileY + 1) && WorldGen.SolidTile(tileX - 1, tileY + 1) && !Main.tile[tileX + 1, tileY].active() && !Main.tile[tileX - 1, tileY].active()) {
												WorldGen.PoundTile(tileX, tileY);
											}

											if (WorldGen.SolidTile(tileX, tileY)) {
												if (WorldGen.SolidTile(tileX - 1, tileY) && WorldGen.SolidTile(tileX + 1, tileY + 2) && !Main.tile[tileX + 1, tileY].active() && !Main.tile[tileX + 1, tileY + 1].active() && !Main.tile[tileX - 1, tileY - 1].active()) {
													WorldGen.KillTile(tileX, tileY);
												}
												else if (WorldGen.SolidTile(tileX + 1, tileY) && WorldGen.SolidTile(tileX - 1, tileY + 2) && !Main.tile[tileX - 1, tileY].active() && !Main.tile[tileX - 1, tileY + 1].active() && !Main.tile[tileX + 1, tileY - 1].active()) {
													WorldGen.KillTile(tileX, tileY);
												}
												else if (!Main.tile[tileX - 1, tileY + 1].active() && !Main.tile[tileX - 1, tileY].active() && WorldGen.SolidTile(tileX + 1, tileY) && WorldGen.SolidTile(tileX, tileY + 2)) {
													if (WorldGen.genRand.Next(5) == 0)
														WorldGen.KillTile(tileX, tileY);
													else if (WorldGen.genRand.Next(5) == 0)
														WorldGen.PoundTile(tileX, tileY);
													else
														WorldGen.SlopeTile(tileX, tileY, 2);
												}
												else if (!Main.tile[tileX + 1, tileY + 1].active() && !Main.tile[tileX + 1, tileY].active() && WorldGen.SolidTile(tileX - 1, tileY) && WorldGen.SolidTile(tileX, tileY + 2)) {
													if (WorldGen.genRand.Next(5) == 0)
														WorldGen.KillTile(tileX, tileY);
													else if (WorldGen.genRand.Next(5) == 0)
														WorldGen.PoundTile(tileX, tileY);
													else
														WorldGen.SlopeTile(tileX, tileY, 1);
												}
											}
										}

										if (WorldGen.SolidTile(tileX, tileY) && !Main.tile[tileX - 1, tileY].active() && !Main.tile[tileX + 1, tileY].active())
											WorldGen.KillTile(tileX, tileY);
									}
								}
								else if (!Main.tile[tileX, tileY].active() && Main.tile[tileX, tileY + 1].type != 151 && Main.tile[tileX, tileY + 1].type != 274) {
									if (Main.tile[tileX + 1, tileY].type != 190 && Main.tile[tileX + 1, tileY].type != 48 && Main.tile[tileX + 1, tileY].type != 232 && WorldGen.SolidTile(tileX - 1, tileY + 1) && WorldGen.SolidTile(tileX + 1, tileY) && !Main.tile[tileX - 1, tileY].active() && !Main.tile[tileX + 1, tileY - 1].active()) {
										WorldGen.PlaceTile(tileX, tileY, Main.tile[tileX, tileY + 1].type);
										if (WorldGen.genRand.Next(2) == 0)
											WorldGen.SlopeTile(tileX, tileY, 2);
										else
											WorldGen.PoundTile(tileX, tileY);
									}

									if (Main.tile[tileX - 1, tileY].type != 190 && Main.tile[tileX - 1, tileY].type != 48 && Main.tile[tileX - 1, tileY].type != 232 && WorldGen.SolidTile(tileX + 1, tileY + 1) && WorldGen.SolidTile(tileX - 1, tileY) && !Main.tile[tileX + 1, tileY].active() && !Main.tile[tileX - 1, tileY - 1].active()) {
										WorldGen.PlaceTile(tileX, tileY, Main.tile[tileX, tileY + 1].type);
										if (WorldGen.genRand.Next(2) == 0)
											WorldGen.SlopeTile(tileX, tileY, 1);
										else
											WorldGen.PoundTile(tileX, tileY);
									}
								}
							}
							else if (!Main.tile[tileX, tileY + 1].active() && WorldGen.genRand.Next(2) == 0 && WorldGen.SolidTile(tileX, tileY) && !Main.tile[tileX - 1, tileY].halfBrick() && !Main.tile[tileX + 1, tileY].halfBrick() && Main.tile[tileX - 1, tileY].slope() == 0 && Main.tile[tileX + 1, tileY].slope() == 0 && WorldGen.SolidTile(tileX, tileY - 1)) {
								if (WorldGen.SolidTile(tileX - 1, tileY) && !WorldGen.SolidTile(tileX + 1, tileY) && WorldGen.SolidTile(tileX - 1, tileY - 1))
									WorldGen.SlopeTile(tileX, tileY, 3);
								else if (WorldGen.SolidTile(tileX + 1, tileY) && !WorldGen.SolidTile(tileX - 1, tileY) && WorldGen.SolidTile(tileX + 1, tileY - 1))
									WorldGen.SlopeTile(tileX, tileY, 4);
							}

							if (TileID.Sets.Conversion.Sand[Main.tile[tileX, tileY].type])
								Tile.SmoothSlope(tileX, tileY, applyToNeighbors: false);
						}
					}
				}

				for (int tileX = 20; tileX < Main.maxTilesX - 20; tileX++) {
					for (int tileY = 20; tileY < Main.maxTilesY - 20; tileY++) {
						if (WorldGen.genRand.Next(2) == 0 && !Main.tile[tileX, tileY - 1].active() && Main.tile[tileX, tileY].type != 137 && Main.tile[tileX, tileY].type != 48 && Main.tile[tileX, tileY].type != 232 && Main.tile[tileX, tileY].type != 191 && Main.tile[tileX, tileY].type != 151 && Main.tile[tileX, tileY].type != 274 && Main.tile[tileX, tileY].type != 75 && Main.tile[tileX, tileY].type != 76 && WorldGen.SolidTile(tileX, tileY) && Main.tile[tileX - 1, tileY].type != 137 && Main.tile[tileX + 1, tileY].type != 137) {
							if (WorldGen.SolidTile(tileX, tileY + 1) && WorldGen.SolidTile(tileX + 1, tileY) && !Main.tile[tileX - 1, tileY].active())
								WorldGen.SlopeTile(tileX, tileY, 2);

							if (WorldGen.SolidTile(tileX, tileY + 1) && WorldGen.SolidTile(tileX - 1, tileY) && !Main.tile[tileX + 1, tileY].active())
								WorldGen.SlopeTile(tileX, tileY, 1);
						}

						if (Main.tile[tileX, tileY].slope() == 1 && !WorldGen.SolidTile(tileX - 1, tileY)) {
							WorldGen.SlopeTile(tileX, tileY);
							WorldGen.PoundTile(tileX, tileY);
						}

						if (Main.tile[tileX, tileY].slope() == 2 && !WorldGen.SolidTile(tileX + 1, tileY)) {
							WorldGen.SlopeTile(tileX, tileY);
							WorldGen.PoundTile(tileX, tileY);
						}
					}
				}
			}),
		};
		public override UIState loadingUIState => new MoonSubworldLoadUI();
		public class MoonSubworldLoadUI : UIDefaultSubworldLoad
		{
			bool toEarth;
			double animationTimer = 0;
			Texture2D lunaBackground;
			Texture2D earthBackground;

			private string _chosenMessage;

			string[] moonMessages =
			{
				"No atmosphere does not mean no life. Always remain on guard.",
				"The Moon takes much longer to rotate than the Earth. Make sure you have enough supplies to last\nthrough the night.",
				"Take advantage of the Moon's low gravity, but remember that your enemies will do the same.",
				"We feared the Blood Moon then, we fear it now. Heed its crimson glare.",
				"The monsters that gather on our own satellite intrigue many."
			};
			string[] earthMessages =
			{
				"Returning for a beer?",
				"Did you know that there are more than 1 quadrillion ants roaming our Earth?",
				"Normal gravity can be quite punishing after a long trek in zero-g. You'll get used to it.",
				"Nearly seven billion deaths within the first 24 hours.",
				"You are never truly safe."
			};
			public override void OnInitialize()
			{
				toEarth = IsActive<Moon>();
				lunaBackground = ModContent.GetTexture($"{nameof(Macrocosm)}/Content/Subworlds/LoadingBackgrounds/Luna");
				earthBackground = ModContent.GetTexture($"{nameof(Macrocosm)}/Content/Subworlds/LoadingBackgrounds/Earth");
				if (toEarth)
					_chosenMessage = RandomHelper.PickRandom(earthMessages);
				else
					_chosenMessage = RandomHelper.PickRandom(moonMessages);
			}
			protected override void DrawSelf(SpriteBatch spriteBatch)
			{
				spriteBatch.End();
				Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, null, null, null, null, Main.UIScaleMatrix);
				if (toEarth)
				{
					spriteBatch.Draw
					(
						earthBackground,
						new Rectangle(Main.screenWidth - earthBackground.Width, Main.screenHeight - earthBackground.Height + 50 - (int)(animationTimer * 10), earthBackground.Width, earthBackground.Height),
						null,
						Color.White * (float)(animationTimer / 5) * 0.8f
					);
					string msgToPlayer = "Earth"; // Title
					Vector2 messageSize = Main.fontDeathText.MeasureString(msgToPlayer) * 1f;
					ChatManager.DrawColorCodedStringWithShadow(Main.spriteBatch, Main.fontDeathText, msgToPlayer, new Vector2(Main.screenWidth / 2f - messageSize.X / 2f, messageSize.Y), new Color(94,150,255), 0f, Vector2.Zero, Vector2.One);
					Vector2 messageSize2 = Main.fontDeathText.MeasureString(_chosenMessage) * 0.7f;
					ChatManager.DrawColorCodedStringWithShadow(Main.spriteBatch, Main.fontDeathText, _chosenMessage, new Vector2(Main.screenWidth / 2f - messageSize2.X / 2f, Main.screenHeight - messageSize2.Y - 20), Color.White, 0f, Vector2.Zero, new Vector2(0.7f));
				}
				else
				{
					spriteBatch.Draw
					(
						lunaBackground,
						new Rectangle(Main.screenWidth - lunaBackground.Width, Main.screenHeight - lunaBackground.Height + 50 - (int)(animationTimer * 10), lunaBackground.Width, lunaBackground.Height),
						null,
						Color.White * (float)(animationTimer / 5) * 0.8f
					);
					string msgToPlayer = "Earth's Moon"; // Title
					Vector2 messageSize = Main.fontDeathText.MeasureString(msgToPlayer) * 1f;
					ChatManager.DrawColorCodedStringWithShadow(Main.spriteBatch, Main.fontDeathText, msgToPlayer, new Vector2(Main.screenWidth / 2f - messageSize.X / 2f, messageSize.Y), Color.White, 0f, Vector2.Zero, Vector2.One);
					Vector2 messageSize2 = Main.fontDeathText.MeasureString(_chosenMessage) * 0.7f;
					ChatManager.DrawColorCodedStringWithShadow(Main.spriteBatch, Main.fontDeathText, _chosenMessage, new Vector2(Main.screenWidth / 2f - messageSize2.X / 2f, Main.screenHeight - messageSize2.Y - 20), Color.White, 0f, Vector2.Zero, new Vector2(0.7f));
				}
				spriteBatch.End();
				Main.spriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, null, null, Main.UIScaleMatrix);
				
				base.DrawSelf(spriteBatch);
			}
			public override void Update(GameTime gameTime)
			{
				animationTimer += gameTime.ElapsedGameTime.TotalSeconds;
				if (animationTimer > 5) 
					animationTimer = 5;
			}
		}

		public override void Load()
		{
			// One Terraria day = 86400
			SLWorld.drawUnderworldBackground = false;
			SLWorld.noReturn = true;
			Main.dayTime = true;
			Main.spawnTileX = 1000;
			for (int tileY = 0; tileY < Main.maxTilesY; tileY++)
			{
				if (Main.tile[1000, tileY].active())
				{
					Main.spawnTileY = tileY;
					break;
				}
			}
			Main.numClouds = 0;
		}
	}
}
