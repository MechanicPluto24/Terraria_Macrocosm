using System;
using Terraria.ModLoader;
using Macrocosm.Content.Tiles;

namespace Macrocosm.Content.Systems
{
	class TileCountSystem : ModSystem
	{
		public int RegolithCount = 0;

		public static TileCountSystem TileCounts => ModContent.GetInstance<TileCountSystem>();

		public override void TileCountsAvailable(ReadOnlySpan<int> tileCounts)
		{
			RegolithCount = tileCounts[ModContent.TileType<Regolith>()];
		}
		public override void ResetNearbyTileEffects()
		{
			RegolithCount = 0;
		}
	}
}
