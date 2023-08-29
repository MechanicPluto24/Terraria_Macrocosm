using Terraria;
using Terraria.IO;
using Terraria.Localization;
using Terraria.WorldBuilding;

namespace Macrocosm.Content.WorldGeneration.Moon
{
	public class FinishPass : GenPass
	{
		private string subworld;

		public FinishPass(string name, float loadWeight, string subworld) : base(name, loadWeight) 
		{ 
			this.subworld = subworld;
		}

		protected override void ApplyPass(GenerationProgress progress, GameConfiguration configuration)
		{
			progress.Message = Language.GetTextValue("Mods.Macrocosm.WorldGen.Moon.FinishPass");

			SetSpawnPoint();
			Main.dayTime = true;
			Main.time = 0;
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