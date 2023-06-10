using Terraria;
using Terraria.IO;
using Terraria.WorldBuilding;

namespace Macrocosm.Content.WorldGeneration.Moon
{
	public class FinishPass : GenPass
	{
		public FinishPass(string name, float loadWeight) : base(name, loadWeight) { }

		protected override void ApplyPass(GenerationProgress progress, GameConfiguration configuration)
		{

			progress.Message = "Final touches...";

			SetSpawnPoint();
			Main.dayTime = true;

		}

		public void SetSpawnPoint()
		{
			int spawnTileX = Main.maxTilesX / 2;
			Main.spawnTileX = spawnTileX;
			for (int tileY = 0; tileY < Main.maxTilesY; tileY++)
			{
				if (Main.tile[spawnTileX, tileY].HasTile)
				{
					Main.spawnTileY = tileY - 2;
					break;
				}
			}
		}
	}
}