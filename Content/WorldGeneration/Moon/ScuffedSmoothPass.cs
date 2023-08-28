using Terraria;
using Terraria.ID;
using Terraria.IO;
using Terraria.Localization;
using Terraria.WorldBuilding;

namespace Macrocosm.Content.WorldGeneration.Moon
{
	internal class ScuffedSmoothPass : GenPass
	{
		public ScuffedSmoothPass(string name, float loadWeight) : base(name, loadWeight) { }

		protected override void ApplyPass(GenerationProgress progress, GameConfiguration configuration)
		{
			progress.Message = Language.GetTextValue("Mods.Macrocosm.WorldGen.Moon.ScuffedSmoothPass");
			// WARNING x WARNING x WARNING
			// Heavily nested code copied from decompiled code
			for (int tileX = 20; tileX < Main.maxTilesX - 20; tileX++)
			{
				float percentAcrossWorld = (float)tileX / (float)Main.maxTilesX;
				progress.Set(percentAcrossWorld);
				for (int tileY = 20; tileY < Main.maxTilesY - 20; tileY++)
				{
					if (Main.tile[tileX, tileY].TileType != 48 && Main.tile[tileX, tileY].TileType != 137 && Main.tile[tileX, tileY].TileType != 232 && Main.tile[tileX, tileY].TileType != 191 && Main.tile[tileX, tileY].TileType != 151 && Main.tile[tileX, tileY].TileType != 274)
					{
						if (!Main.tile[tileX, tileY - 1].HasTile)
						{
							if (WorldGen.SolidTile(tileX, tileY) && TileID.Sets.CanBeClearedDuringGeneration[Main.tile[tileX, tileY].TileType])
							{
								if (!Main.tile[tileX - 1, tileY].IsHalfBlock && !Main.tile[tileX + 1, tileY].IsHalfBlock && Main.tile[tileX - 1, tileY].Slope == SlopeType.Solid && Main.tile[tileX + 1, tileY].Slope == SlopeType.Solid)
								{
									if (WorldGen.SolidTile(tileX, tileY + 1))
									{
										if (!WorldGen.SolidTile(tileX - 1, tileY) && !Main.tile[tileX - 1, tileY + 1].IsHalfBlock && WorldGen.SolidTile(tileX - 1, tileY + 1) && WorldGen.SolidTile(tileX + 1, tileY) && !Main.tile[tileX + 1, tileY - 1].HasTile)
										{
											if (WorldGen.genRand.NextBool(2))
												WorldGen.SlopeTile(tileX, tileY, 2);
											else
												WorldGen.PoundTile(tileX, tileY);
										}
										else if (!WorldGen.SolidTile(tileX + 1, tileY) && !Main.tile[tileX + 1, tileY + 1].IsHalfBlock && WorldGen.SolidTile(tileX + 1, tileY + 1) && WorldGen.SolidTile(tileX - 1, tileY) && !Main.tile[tileX - 1, tileY - 1].HasTile)
										{
											if (WorldGen.genRand.NextBool(2))
												WorldGen.SlopeTile(tileX, tileY, 1);
											else
												WorldGen.PoundTile(tileX, tileY);
										}
										else if (WorldGen.SolidTile(tileX + 1, tileY + 1) && WorldGen.SolidTile(tileX - 1, tileY + 1) && !Main.tile[tileX + 1, tileY].HasTile && !Main.tile[tileX - 1, tileY].HasTile)
										{
											WorldGen.PoundTile(tileX, tileY);
										}

										if (WorldGen.SolidTile(tileX, tileY))
										{
											if (WorldGen.SolidTile(tileX - 1, tileY) && WorldGen.SolidTile(tileX + 1, tileY + 2) && !Main.tile[tileX + 1, tileY].HasTile && !Main.tile[tileX + 1, tileY + 1].HasTile && !Main.tile[tileX - 1, tileY - 1].HasTile)
											{
												WorldGen.KillTile(tileX, tileY);
											}
											else if (WorldGen.SolidTile(tileX + 1, tileY) && WorldGen.SolidTile(tileX - 1, tileY + 2) && !Main.tile[tileX - 1, tileY].HasTile && !Main.tile[tileX - 1, tileY + 1].HasTile && !Main.tile[tileX + 1, tileY - 1].HasTile)
											{
												WorldGen.KillTile(tileX, tileY);
											}
											else if (!Main.tile[tileX - 1, tileY + 1].HasTile && !Main.tile[tileX - 1, tileY].HasTile && WorldGen.SolidTile(tileX + 1, tileY) && WorldGen.SolidTile(tileX, tileY + 2))
											{
												if (WorldGen.genRand.NextBool(5))
													WorldGen.KillTile(tileX, tileY);
												else if (WorldGen.genRand.NextBool(5))
													WorldGen.PoundTile(tileX, tileY);
												else
													WorldGen.SlopeTile(tileX, tileY, 2);
											}
											else if (!Main.tile[tileX + 1, tileY + 1].HasTile && !Main.tile[tileX + 1, tileY].HasTile && WorldGen.SolidTile(tileX - 1, tileY) && WorldGen.SolidTile(tileX, tileY + 2))
											{
												if (WorldGen.genRand.NextBool(5))
													WorldGen.KillTile(tileX, tileY);
												else if (WorldGen.genRand.NextBool(5))
													WorldGen.PoundTile(tileX, tileY);
												else
													WorldGen.SlopeTile(tileX, tileY, 1);
											}
										}
									}

									if (WorldGen.SolidTile(tileX, tileY) && !Main.tile[tileX - 1, tileY].HasTile && !Main.tile[tileX + 1, tileY].HasTile)
										WorldGen.KillTile(tileX, tileY);
								}
							}
							else if (!Main.tile[tileX, tileY].HasTile && Main.tile[tileX, tileY + 1].TileType != 151 && Main.tile[tileX, tileY + 1].TileType != 274)
							{
								if (Main.tile[tileX + 1, tileY].TileType != 190 && Main.tile[tileX + 1, tileY].TileType != 48 && Main.tile[tileX + 1, tileY].TileType != 232 && WorldGen.SolidTile(tileX - 1, tileY + 1) && WorldGen.SolidTile(tileX + 1, tileY) && !Main.tile[tileX - 1, tileY].HasTile && !Main.tile[tileX + 1, tileY - 1].HasTile)
								{
									WorldGen.PlaceTile(tileX, tileY, Main.tile[tileX, tileY + 1].TileType, mute: true);
									if (WorldGen.genRand.NextBool(2))
										WorldGen.SlopeTile(tileX, tileY, 2);
									else
										WorldGen.PoundTile(tileX, tileY);
								}

								if (Main.tile[tileX - 1, tileY].TileType != 190 && Main.tile[tileX - 1, tileY].TileType != 48 && Main.tile[tileX - 1, tileY].TileType != 232 && WorldGen.SolidTile(tileX + 1, tileY + 1) && WorldGen.SolidTile(tileX - 1, tileY) && !Main.tile[tileX + 1, tileY].HasTile && !Main.tile[tileX - 1, tileY - 1].HasTile)
								{
									WorldGen.PlaceTile(tileX, tileY, Main.tile[tileX, tileY + 1].TileType, mute: true);
									if (WorldGen.genRand.NextBool(2))
										WorldGen.SlopeTile(tileX, tileY, 1);
									else
										WorldGen.PoundTile(tileX, tileY);
								}
							}
						}
						else if (!Main.tile[tileX, tileY + 1].HasTile && WorldGen.genRand.NextBool(2) && WorldGen.SolidTile(tileX, tileY) && !Main.tile[tileX - 1, tileY].IsHalfBlock && !Main.tile[tileX + 1, tileY].IsHalfBlock && Main.tile[tileX - 1, tileY].Slope == SlopeType.Solid && Main.tile[tileX + 1, tileY].Slope == SlopeType.Solid && WorldGen.SolidTile(tileX, tileY - 1))
						{
							if (WorldGen.SolidTile(tileX - 1, tileY) && !WorldGen.SolidTile(tileX + 1, tileY) && WorldGen.SolidTile(tileX - 1, tileY - 1))
								WorldGen.SlopeTile(tileX, tileY, 3);
							else if (WorldGen.SolidTile(tileX + 1, tileY) && !WorldGen.SolidTile(tileX - 1, tileY) && WorldGen.SolidTile(tileX + 1, tileY - 1))
								WorldGen.SlopeTile(tileX, tileY, 4);
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
					if (WorldGen.genRand.NextBool(2) && !Main.tile[tileX, tileY - 1].HasTile && Main.tile[tileX, tileY].TileType != 137 && Main.tile[tileX, tileY].TileType != 48 && Main.tile[tileX, tileY].TileType != 232 && Main.tile[tileX, tileY].TileType != 191 && Main.tile[tileX, tileY].TileType != 151 && Main.tile[tileX, tileY].TileType != 274 && Main.tile[tileX, tileY].TileType != 75 && Main.tile[tileX, tileY].TileType != 76 && WorldGen.SolidTile(tileX, tileY) && Main.tile[tileX - 1, tileY].TileType != 137 && Main.tile[tileX + 1, tileY].TileType != 137)
					{
						if (WorldGen.SolidTile(tileX, tileY + 1) && WorldGen.SolidTile(tileX + 1, tileY) && !Main.tile[tileX - 1, tileY].HasTile)
							WorldGen.SlopeTile(tileX, tileY, 2);

						if (WorldGen.SolidTile(tileX, tileY + 1) && WorldGen.SolidTile(tileX - 1, tileY) && !Main.tile[tileX + 1, tileY].HasTile)
							WorldGen.SlopeTile(tileX, tileY, 1);
					}

					if (Main.tile[tileX, tileY].Slope == SlopeType.SlopeDownLeft && !WorldGen.SolidTile(tileX - 1, tileY))
					{
						WorldGen.SlopeTile(tileX, tileY);
						WorldGen.PoundTile(tileX, tileY);
					}

					if (Main.tile[tileX, tileY].Slope == SlopeType.SlopeDownRight && !WorldGen.SolidTile(tileX + 1, tileY))
					{
						WorldGen.SlopeTile(tileX, tileY);
						WorldGen.PoundTile(tileX, tileY);
					}
				}
			}
		}
	}
}