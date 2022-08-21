using Terraria;
using Terraria.IO;
using Terraria.WorldBuilding;

namespace Macrocosm.Content.WorldGeneration.Moon
{
	public class CavePass : GenPass
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
			progress.Message = "Carving the Moon...";
			for (int currentCaveSpot = 0; currentCaveSpot < (int)((double)(Main.maxTilesX * Main.maxTilesY) * 0.00013); currentCaveSpot++)
			{
				float percentDone = (float)((double)currentCaveSpot / ((double)(Main.maxTilesX * Main.maxTilesY) * 0.00013));
				progress.Set(percentDone);
				if (rockLayerHigh <= (double)Main.maxTilesY)
				{
					int airTileType = -1;
					WorldGen.TileRunner(WorldGen.genRand.Next(0, Main.maxTilesX), WorldGen.genRand.Next((int)rockLayerLow, Main.maxTilesY), WorldGen.genRand.Next(6, 20), WorldGen.genRand.Next(50, 300), airTileType);
				}
			}
		}
	}
}