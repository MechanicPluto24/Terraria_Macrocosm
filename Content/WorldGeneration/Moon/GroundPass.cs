using Terraria;
using Terraria.IO;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.WorldBuilding;

namespace Macrocosm.Content.WorldGeneration.Moon
{
	internal class GroundPass : GenPass
	{
		private int subworldWidth;
		private int subworldHeight;

		private double surfaceLayer = 200.0;
		private double rockLayerLow = 0.0;
		private double rockLayerHigh = 0.0;

		private int maxSurface = 0;

		public GroundPass(string name, float loadWeight, int subworldWidth, int subworldHeight) : base(name, loadWeight)
		{
			this.subworldWidth = subworldWidth;
			this.subworldHeight = subworldHeight;
		}

		public double RockLayerLow { get => rockLayerLow; set => rockLayerLow = value; }
		public double RockLayerHigh { get => rockLayerHigh; set => rockLayerHigh = value; }

		protected override void ApplyPass(GenerationProgress progress, GameConfiguration configuration)
		{
			progress.Message = Language.GetTextValue("Mods.Macrocosm.WorldGen.Moon.GroundPass");
			Main.worldSurface = surfaceLayer + 40;  
			Main.rockLayer = surfaceLayer + 222;  

			int surfaceHeight = (int)surfaceLayer; // If the moon's world size is variable, this probably should depend on that
			rockLayerLow = surfaceHeight;
			rockLayerHigh = surfaceHeight;

			ushort protolithType = (ushort)ModContent.TileType<Tiles.Blocks.Protolith>();

			// Generate base ground
			for (int i = 0; i < Main.maxTilesX; i++)
			{
				// Here, we just focus on the progress along the x-axis
				progress.Set(i / (float)(Main.maxTilesX - 1)); // Controls the progress bar, should only be set between 0f and 1f
				for (int j = surfaceHeight; j < subworldHeight; j++)
				{
					WorldGen.PlaceTile(i, j, protolithType, true, true);
				}

				if (WorldGen.genRand.Next(0, 10) == 0) // Not much deviation here
				{
					surfaceHeight += WorldGen.genRand.Next(-1, 2);
					if (WorldGen.genRand.Next(0, 10) == 0)
					{
						surfaceHeight += WorldGen.genRand.Next(-2, 3);
					}
				}

				if (maxSurface < surfaceHeight)
					maxSurface = surfaceHeight;

				if (surfaceHeight < rockLayerLow)
					rockLayerLow = surfaceHeight;

				if (surfaceHeight > rockLayerHigh)
					rockLayerHigh = surfaceHeight;
			}

			// keep the underground backwall below the surface lowest point 
			Main.worldSurface = maxSurface + 40;
		}
	}
}