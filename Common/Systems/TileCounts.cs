using Macrocosm.Common.Bases;
using Macrocosm.Content.Tiles.Blocks;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria.ModLoader;

namespace Macrocosm.Common.Systems
{
    class TileCounts : ModSystem
	{
		public int RegolithCount { get; private set; } = 0;
		public int IrradiatedRockCount { get; private set; } = 0;
		public int GraveyardModTileCount { get; private set; } = 0;

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
			{
				ModTile tile = TileLoader.GetTile(type);
				if(tile is ITombstoneTile tombstone && tombstone.AllowGrayeyardOnEarth)
					GraveyardModTileCount += tileCounts[type];
			}
        }

		public override void ResetNearbyTileEffects()
		{
			RegolithCount = 0;
			IrradiatedRockCount = 0;
            GraveyardModTileCount = 0;
        }
	}
}
