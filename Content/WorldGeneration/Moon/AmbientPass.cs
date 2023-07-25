using Macrocosm.Content.Tiles.Ambient;
using Terraria;
using Terraria.IO;
using Terraria.Localization;
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
		/// TODO: large variations, underground gen 
		protected override void ApplyPass(GenerationProgress progress, GameConfiguration configuration)
		{
			progress.Message = Language.GetTextValue("Mods.Macrocosm.WorldGen.Moon.AmbientPass");

			int skipX = 0;
			for (int tileX = 1; tileX < Main.maxTilesX - 1; tileX++)
			{
				progress.Set(tileX / Main.maxTilesX);

				if (skipX > 0)
				{
					skipX--;
					continue;
				}

				// rocks on the surface 
				if (WorldGen.genRand.NextBool(15))
				{
					// small rocks
					if (!WorldGen.genRand.NextBool(5))
					{
						for (int tileY = 1; tileY < Main.worldSurface; tileY++)
						{
							if (Main.tile[tileX, tileY].HasTile && Main.tile[tileX, tileY].BlockType == Terraria.ID.BlockType.Solid)
							{
								WorldGen.PlaceTile(tileX, tileY - 1, ModContent.TileType<RegolithRockSmallNatural>(), style: WorldGen.genRand.Next(10), mute: true);
								skipX = WorldGen.genRand.Next(2, 25);
								break;
							}
						}
					}
					// medium rocks
					else
					{
						for (int tileY = 1; tileY < Main.worldSurface; tileY++)
						{
							if (Main.tile[tileX, tileY].HasTile && Main.tile[tileX, tileY].BlockType == Terraria.ID.BlockType.Solid &&
							    Main.tile[tileX + 1, tileY].HasTile && Main.tile[tileX + 1, tileY].BlockType == Terraria.ID.BlockType.Solid)
							{
								WorldGen.PlaceTile(tileX, tileY - 1, ModContent.TileType<RegolithRockMediumNatural>(), style: WorldGen.genRand.Next(6), mute: true);
								skipX = WorldGen.genRand.Next(2, 25);
								break;
							}
						}
					}
				}
			}
		}
	}
}