using Macrocosm.Content.Tiles;
using Terraria;
using Terraria.IO;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.WorldBuilding;

namespace Macrocosm.Content.WorldGeneration.Moon
{
	public class OrePass : GenPass
	{
		public OrePass(string name, float loadWeight) : base(name, loadWeight) { }
		
		protected override void ApplyPass(GenerationProgress progress, GameConfiguration configuration)
		{
			progress.Message = "Mineralizing the Moon...";
			#region Generate ore veins
			GenerateOre(ModContent.TileType<ArtemiteOre>(), 0.0001, WorldGen.genRand.Next(5, 9), WorldGen.genRand.Next(5, 9));
			GenerateOre(ModContent.TileType<ChandriumOre>(), 0.0001, WorldGen.genRand.Next(5, 9), WorldGen.genRand.Next(5, 9));
			GenerateOre(ModContent.TileType<DianiteOre>(), 0.0001, WorldGen.genRand.Next(5, 9), WorldGen.genRand.Next(5, 9));
			GenerateOre(ModContent.TileType<SeleniteOre>(), 0.0001, WorldGen.genRand.Next(5, 9), WorldGen.genRand.Next(5, 9));

			GenerateOre(TileID.LunarOre, 0.00005, WorldGen.genRand.Next(9, 15), WorldGen.genRand.Next(9, 15));
			#endregion
		}

		void GenerateOre(int TileType, double percent, int strength, int steps)
		{
			for (int k = 0; k < (int)((Main.maxTilesX * Main.maxTilesY) * percent); k++)
			{
				int x = WorldGen.genRand.Next(0, Main.maxTilesX);
				int y = WorldGen.genRand.Next(0, Main.maxTilesY);
				if (Main.tile[x, y].HasTile && Main.tile[x, y].TileType == ModContent.TileType<Tiles.Protolith>())
				{
					WorldGen.TileRunner(x, y, strength, steps, TileType);
				}
			}
		}
	}
}