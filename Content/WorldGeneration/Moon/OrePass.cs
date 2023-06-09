using Terraria;
using Terraria.IO;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.WorldBuilding;
using Macrocosm.Common.Utils;
using Macrocosm.Content.Tiles.Ores;

namespace Macrocosm.Content.WorldGeneration.Moon
{
    public class OrePass : GenPass
	{
		public OrePass(string name, float loadWeight) : base(name, loadWeight) { }
		
		protected override void ApplyPass(GenerationProgress progress, GameConfiguration configuration)
		{
			progress.Message = "Mineralizing the Moon...";
			#region Generate ore veins
			int protolithType = ModContent.TileType<Tiles.Blocks.Protolith>();
			Utility.GenerateOre(ModContent.TileType<ArtemiteOre>(), 0.0001, WorldGen.genRand.Next(5, 9), WorldGen.genRand.Next(5, 9), protolithType);
			Utility.GenerateOre(ModContent.TileType<ChandriumOre>(), 0.0001, WorldGen.genRand.Next(5, 9), WorldGen.genRand.Next(5, 9), protolithType);
			Utility.GenerateOre(ModContent.TileType<DianiteOre>(), 0.0001, WorldGen.genRand.Next(5, 9), WorldGen.genRand.Next(5, 9), protolithType);
			Utility.GenerateOre(ModContent.TileType<SeleniteOre>(), 0.0001, WorldGen.genRand.Next(5, 9), WorldGen.genRand.Next(5, 9), protolithType);

			Utility.GenerateOre(TileID.LunarOre, 0.00005, WorldGen.genRand.Next(9, 15), WorldGen.genRand.Next(9, 15), protolithType);
			#endregion
		}
	}
}