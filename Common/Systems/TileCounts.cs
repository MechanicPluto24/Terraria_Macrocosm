using Macrocosm.Content.Tiles.Blocks;
using Macrocosm.Content.Tiles.Tombstones;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria.ModLoader;

namespace Macrocosm.Common.Systems
{
	class TileCounts : ModSystem
	{
		public int RegolithCount = 0;
		public int IrradiatedRockCount = 0;
		public int GraveyardTileCount = 0;

		private int[] graveyardTileTypes;

        public static TileCounts Instance => ModContent.GetInstance<TileCounts>();

        public override void Load()
        {
			List<int> graveyardtypes = [];
			foreach(var tile in ModContent.GetContent<ModTile>().Where(item => item is ITombstoneTile))
                graveyardtypes.Add(tile.Type);
            graveyardTileTypes = [..graveyardtypes];	
        }

		public override void TileCountsAvailable(ReadOnlySpan<int> tileCounts)
		{
			RegolithCount = tileCounts[ModContent.TileType<Regolith>()];
			IrradiatedRockCount = tileCounts[ModContent.TileType<IrradiatedRock>()];

			foreach(int type in graveyardTileTypes)
                GraveyardTileCount += tileCounts[type];
        }

		public override void ResetNearbyTileEffects()
		{
			RegolithCount = 0;
			IrradiatedRockCount = 0;
            GraveyardTileCount = 0;
        }
	}
}
