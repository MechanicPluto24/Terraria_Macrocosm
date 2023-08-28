using System.Collections.Generic;
using Macrocosm.Content.Tiles.Walls;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.IO;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.WorldBuilding;

namespace Macrocosm.Content.WorldGeneration.Moon
{
    internal class CavePass : GenPass
	{
		private double rockLayerHigh = 0.0;
		private double rockLayerLow = 0.0;

		public CavePass(string name, float loadWeight, double rockLayerHigh, double rockLayerLow) : base(name, loadWeight)
		{
			this.rockLayerHigh = rockLayerHigh;
			this.rockLayerLow = rockLayerLow;
		}

		protected override void ApplyPass(GenerationProgress progress, GameConfiguration configuration)
		{
			progress.Message = Language.GetTextValue("Mods.Macrocosm.WorldGen.Moon.CavePass");

			double smallCaveFreq = 0.0009;
			double largeCaveFreq = 0.00013;

			int smallCaveWallChance = 5; // 1/x chance to spread walls  
			int largeCaveChance = 10; // 1/x chance to spread walls 
			int wallGenHeight = (int)Main.rockLayer + 100;

			int airTileType = -1;
			int protolithWallType = ModContent.WallType<ProtolithWall>();

			List<Point> smallCaves = new();
			List<Point> largeCaves = new();
 

			// generate small caves in the protolith layer 
			for (int smallCaveSpot = 0; smallCaveSpot < (int)((double)(Main.maxTilesX * Main.maxTilesY) * smallCaveFreq); smallCaveSpot++)
			{
				float percentDone = (float)((double)smallCaveSpot / ((double)(Main.maxTilesX * Main.maxTilesY) * smallCaveFreq));
				progress.Set(percentDone * 0.5f);

				int tileX = WorldGen.genRand.Next(0, Main.maxTilesX);
				int tileY = WorldGen.genRand.Next((int)Main.rockLayer, Main.maxTilesY);

				// really small holes 
				WorldGen.TileRunner(tileX, tileY, WorldGen.genRand.Next(2, 5), WorldGen.genRand.Next(2, 20), airTileType);

				tileX = WorldGen.genRand.Next(0, Main.maxTilesX);
				tileY = WorldGen.genRand.Next((int)Main.rockLayer, Main.maxTilesY);
				while (((double)tileY < Main.rockLayer) || ((double)tileX > (double)Main.maxTilesX * 0.45 && (double)tileX < (double)Main.maxTilesX * 0.55 && (double)tileY < Main.rockLayer))
				{
					tileX = WorldGen.genRand.Next(0, Main.maxTilesX);
					tileY = WorldGen.genRand.Next((int)Main.rockLayer, Main.maxTilesY);
				}

				// small caves 
				smallCaves.Add(new Point(tileX, tileY));
				WorldGen.TileRunner(tileX, tileY, WorldGen.genRand.Next(8, 15), WorldGen.genRand.Next(7, 30), airTileType);
			}
 
			//generate large caves 
			for (int largeCaveSpot = 0; largeCaveSpot < (int)((double)(Main.maxTilesX * Main.maxTilesY) * largeCaveFreq); largeCaveSpot++)
			{
				float percentDone = (float)((double)largeCaveSpot / ((double)(Main.maxTilesX * Main.maxTilesY) * largeCaveFreq));
				progress.Set(0.5f + percentDone * 0.5f);
				if (rockLayerHigh <= (double)Main.maxTilesY)
				{
					int tileX = WorldGen.genRand.Next(0, Main.maxTilesX);
					int tileY = WorldGen.genRand.Next((int)rockLayerLow, Main.maxTilesY);
					largeCaves.Add(new Point(tileX, tileY));
 					WorldGen.TileRunner(tileX, tileY, WorldGen.genRand.Next(6, 20), WorldGen.genRand.Next(50, 300), airTileType);
				}
			}

			foreach(Point cave in smallCaves)
			{
				if (cave.Y > wallGenHeight)
				{
					if (WorldGen.genRand.NextBool(smallCaveWallChance))
						WorldGen.Spread.Wall(cave.X, cave.Y, protolithWallType); // unlimited spread
				}
			}

			foreach (Point cave in largeCaves)
			{
				if (cave.Y > wallGenHeight)
				{
					WorldGen.maxWallOut2 *= 3;

					if (WorldGen.genRand.NextBool(largeCaveChance))
						WorldGen.Spread.Wall2(cave.X, cave.Y, protolithWallType); // limited spread
					else
						WorldGen.Spread.Wall2(cave.X, cave.Y, 0);

					WorldGen.maxWallOut2 /= 3;
 				}
			}
		}
	}
}