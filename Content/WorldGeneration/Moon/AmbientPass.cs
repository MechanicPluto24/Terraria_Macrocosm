using Macrocosm.Common.Base;
using Macrocosm.Content.Tiles.Ambient;
using Terraria;
using Terraria.IO;
using Terraria.ModLoader;
using Terraria.WorldBuilding;

namespace Macrocosm.Content.WorldGeneration.Moon
{
	public class AmbientPass : GenPass
	{
		public AmbientPass(string name, float loadWeight) : base(name, loadWeight) { }

		/// <summary>
		/// Randomly places ambient rocks on The Moon's surface 
		/// </summary>
		/// TODO: Medium and large (>1x1) variations, underground gen 
		protected override void ApplyPass(GenerationProgress progress, GameConfiguration configuration)
		{
			progress.Message = "Placing rocks on the moon...";

			int skipX = 0;
			for (int tileX = 1; tileX < Main.maxTilesX - 1; tileX++)
			{
				progress.Set(tileX / Main.maxTilesX);

				if (skipX > 0)
				{
					skipX--;
					continue;
				}

				if (WorldGen.genRand.NextBool(20))
				{
					for (int tileY = 1; tileY < Main.worldSurface; tileY++)
					{
						if (Main.tile[tileX, tileY].HasTile && Main.tile[tileX, tileY].BlockType == Terraria.ID.BlockType.Solid)
						{
							WorldGen.PlaceTile(tileX, tileY - 1, ModContent.TileType<RegolithRockSmall>(), style: WorldGen.genRand.Next(10), mute: true);
 							skipX = WorldGen.genRand.Next(5, 50);
							break;
						}
					}
				}
			}
		}
	}
}