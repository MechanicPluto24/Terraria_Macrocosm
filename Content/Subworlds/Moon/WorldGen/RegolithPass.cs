using Terraria;
using Terraria.IO;
using Terraria.ModLoader;
using Terraria.WorldBuilding;

namespace Macrocosm.Content.Subworlds.Moon.Generation
{
	public class RegolithPass : GenPass
	{
		public RegolithPass(string name, float loadWeight) : base(name, loadWeight) { }

		protected override void ApplyPass(GenerationProgress progress, GameConfiguration configuration)
		{
			progress.Message = "Sending meteors to the Moon...";
			for (int tileX = 1; tileX < Main.maxTilesX - 1; tileX++)
			{
				float progressPercent = tileX / Main.maxTilesX;
				progress.Set(progressPercent / 2f);
				float regolithChance = 6;
				for (int tileY = 1; tileY < Main.maxTilesY; tileY++)
				{
					if (Main.tile[tileX, tileY].HasTile)
					{
						if (regolithChance > 0.1)
						{
							Main.tile[tileX, tileY].ClearTile();
							WorldGen.PlaceTile(tileX, tileY, (ushort)ModContent.TileType<Tiles.Regolith>(), true, true);
						}
						regolithChance -= 0.02f;
						if (regolithChance <= 0) break;
					}
				}
			}
			// Generate protolith veins
			for (int tileX = 1; tileX < Main.maxTilesX - 1; tileX++)
			{
				float progressPercent = tileX / Main.maxTilesX;
				progress.Set(0.5f + progressPercent / 2f);
				float regolithChance = 6;
				for (int tileY = 1; tileY < Main.maxTilesY; tileY++)
				{
					if (Main.tile[tileX, tileY].HasTile)
					{
						double veinChance = (6 - regolithChance) / 6f * 0.006;
						if (WorldGen.genRand.NextFloat() < veinChance || veinChance == 0)
						{
							WorldGen.TileRunner(tileX, tileY, WorldGen.genRand.Next((int)(6 * veinChance / 0.003), (int)(20 * veinChance / 0.003)), WorldGen.genRand.Next(5, 19), ModContent.TileType<Tiles.Protolith>());
						}
						regolithChance -= 0.02f;
						if (regolithChance < 0) break;
					}
				}
			}
		}
	}
}