using Macrocosm.Common.Sets;
using Macrocosm.Content.Tiles.Ambient;
using Macrocosm.Content.Tiles.Blocks.Terrain;
using System;
using System.Collections.Generic;
using Terraria.ModLoader;

namespace Macrocosm.Common.Systems
{
    public class TileCounts : ModSystem
    {
        public static TileCounts Instance => ModContent.GetInstance<TileCounts>();

        public int RegolithCount { get; private set; } = 0;
        public int IrradiatedRockCount { get; private set; } = 0;
        public int GraveyardModTileCount { get; private set; } = 0;
        public int MonolithCount { get; private set; } = 0;
        private int[] graveyardTileTypes;

        public override void PostSetupContent()
        {
            List<int> graveyardtypes = new();
            for (int type = 0; type < TileLoader.TileCount; type++)
                if (TileSets.GraveyardTile[type])
                    graveyardtypes.Add(type);
            graveyardTileTypes = graveyardtypes.ToArray();
        }

        public override void TileCountsAvailable(ReadOnlySpan<int> tileCounts)
        {
            RegolithCount = tileCounts[ModContent.TileType<Regolith>()];
            IrradiatedRockCount = tileCounts[ModContent.TileType<IrradiatedRock>()];
            MonolithCount = tileCounts[ModContent.TileType<Monolith>()];

            foreach (int type in graveyardTileTypes)
                GraveyardModTileCount += tileCounts[type];
        }

        public override void ResetNearbyTileEffects()
        {
            RegolithCount = 0;
            IrradiatedRockCount = 0;
            GraveyardModTileCount = 0;
            MonolithCount = 0;
        }
    }
}
