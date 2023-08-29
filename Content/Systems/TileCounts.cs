using Macrocosm.Content.Tiles.Blocks;
using System;
using Terraria.ModLoader;

namespace Macrocosm.Content.Systems
{
	class TileCounts : ModSystem
	{
		public int RegolithCount = 0;
		public int IrradiatedRockCount = 0;

		public static TileCounts Instance 
			=> ModContent.GetInstance<TileCounts>();

		public override void TileCountsAvailable(ReadOnlySpan<int> tileCounts)
		{
			RegolithCount = tileCounts[ModContent.TileType<Regolith>()];
			IrradiatedRockCount = tileCounts[ModContent.TileType<IrradiatedRock>()];
		}
		public override void ResetNearbyTileEffects()
		{
			RegolithCount = 0;
			IrradiatedRockCount = 0;
		}
	}
}
