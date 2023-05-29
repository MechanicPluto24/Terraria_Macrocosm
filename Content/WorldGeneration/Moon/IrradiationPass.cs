using System;
using System.Collections.Generic;
using Macrocosm.Common.Utils;
using Macrocosm.Content.Tiles.Blocks;
using Macrocosm.Content.Tiles.Walls;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.IO;
using Terraria.ModLoader;
using Terraria.WorldBuilding;

namespace Macrocosm.Content.WorldGeneration.Moon
{
    public class IrradiationPass : GenPass
	{
		public int CenterX;

		private int holeWidth;
		private int holeHeight;
		private int biomeWidth;
		private int biomeHeight;

		private int halfX;
		private int halfY;
		private int quarterX;
		private int quarterY;

		private int cavesFrequency = 100;
		private int cavesFrequencyInitial = 200;

		public IrradiationPass(string name, float loadWeight) : base(name, loadWeight) { }

		protected override void ApplyPass(GenerationProgress progress, GameConfiguration configuration)
		{
			progress.Message = "Irradiating the Moon..."; //FIXME: change this lol 
			CenterX = GetBiomeCenterX();
			ClearMainHole(progress);
			AddBlocks(progress);
		}

		private int GetBiomeCenterX()
		{
			halfX = Main.maxTilesX / 2;
			quarterX = halfX / 2;

			halfY = Main.maxTilesY / 2;
			quarterY = halfY / 2;

			bool rightSide = WorldGen.genRand.NextBool();

			int centerX = rightSide ? halfX + quarterX : quarterX;
			centerX += WorldGen.genRand.Next(-quarterX/4, quarterX/4); // 1/16 world width variation

			return centerX;
		}

		/// <summary> Carves the main funnel entrance and caves </summary>
		private void ClearMainHole(GenerationProgress progress)
		{
			// get sizes 
			biomeWidth = quarterX / 4;
			holeWidth = (int)(biomeWidth * 0.25f);
			int initialHoleWidth = holeWidth;

			biomeHeight = halfY + quarterY;
			holeHeight = (int)(biomeHeight * 0.75f);

			int surfaceY = 0;

			// find surface
			while (!Main.tile[CenterX, surfaceY++].HasTile) { } 

			int startY = surfaceY - 50;
			int caveCounter = cavesFrequencyInitial;

			for (int tileY = startY; tileY < holeHeight; tileY++)
			{
				float percentY = (float)(tileY + startY) / (holeHeight + startY);

				progress.Set(0.3f * percentY);

				int startX = CenterX - holeWidth;
				int stopX = CenterX + holeWidth;

				for (int tileX = startX; tileX < stopX; tileX++)
				{
					// clear main funnel shaped hole  
					Main.tile[tileX, tileY].ClearEverything();

					// add deep caves 
					if ((tileX == startX || tileX == stopX - 1))
					{
						if (caveCounter-- == 0)
						{
							WorldGen.TileRunner(tileX, tileY, 15, 3000, -1);
							caveCounter = cavesFrequency;
						}
					}

					// add random small holes 
					if ((tileX == startX || tileX == stopX - 1) && WorldGen.genRand.NextBool(6))
						WorldGen.TileRunner(tileX, tileY, (int)(WorldGen.genRand.Next(15, 25) * (1.5f - percentY)), WorldGen.genRand.Next(35, 50), -1);
				}

				// narrow the funnel 
				holeWidth = (int)MathHelper.Lerp(initialHoleWidth, 1f, percentY);
			}
		}

		private void AddBlocks(GenerationProgress progress)
		{
			int trueSurfaceY = -1;
 
			int spreadCounter = (int)Main.worldSurface;

			int startX = CenterX - biomeWidth / 2;
			int stopX = CenterX + biomeWidth / 2;

			for (int tileX = startX; tileX < stopX; tileX++)
			{
				// FIXME: not smooth enough
				progress.Set(0.3f + 0.6f * (float)(tileX - startX) / (stopX - startX));  
			 
				for (int tileY = 1; tileY < biomeHeight; tileY++)
				{
					// TileRunner will place walls unconditionally only below the surface 
					int wallType = ModContent.WallType<IrradiatedRockWall>();
					bool addWall = tileY > (int)(Main.worldSurface * 1.4);

					// Spread the irradiated rocks periodically on biome edges  
					if ((tileX == startX || tileX == stopX - 1 || tileY == biomeHeight - 1) && spreadCounter-- == 0)
					{
						double strength = WorldGen.genRand.Next(50, 125) * MathHelper.Lerp(2.8f, 1.2f, Utils.GetLerpValue((float)Main.worldSurface, biomeHeight, tileY));
						int steps = (int)(WorldGen.genRand.Next(20, 40) * MathHelper.Lerp(1f, 1.2f, Utils.GetLerpValue((float)Main.worldSurface, biomeHeight, tileY)));

						// More spread at the very bottom
						if (tileY == biomeHeight - 1)
							strength += 40;

						Utility.TileWallRunner(tileX, tileY, strength, steps, ModContent.TileType<IrradiatedRock>(), addTile: false, wallType, addWall: addWall);
						spreadCounter = 80;
					}

					// detect the local surface level
					if(trueSurfaceY < 0 && tileX == startX && Main.tile[tileX, tileY].HasTile)
 						trueSurfaceY = tileY + 5;
 
					// clear all the walls the biome spread didn't reach
					if(Main.tile[tileX, tileY].WallType != ModContent.WallType<IrradiatedRockWall>())
 						Main.tile[tileX, tileY].Clear(Terraria.DataStructures.TileDataType.Wall);

					// place walls behind tiles or somewhere below the local surface level
					if ((Main.tile[tileX, tileY].HasTile && Utility.CheckTile6WayBelow(tileX, tileY)) || (tileY > trueSurfaceY + WorldGen.genRand.Next(1,3) && trueSurfaceY > 0))
						WorldGen.PlaceWall(tileX, tileY, ModContent.WallType<IrradiatedRockWall>(), true);
 
					// replace existing tiles 
					if(Main.tile[tileX, tileY].HasTile && Main.tile[tileX, tileY].TileType != ModContent.TileType<IrradiatedRock>())
						WorldGen.PlaceTile(tileX, tileY, ModContent.TileType<IrradiatedRock>(), true, true);
				}
			}
		}
	}
}