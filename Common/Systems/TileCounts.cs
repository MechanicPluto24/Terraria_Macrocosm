using Macrocosm.Common.Sets;
using Macrocosm.Content.Tiles.Ambient;
using Macrocosm.Content.Tiles.Blocks.Terrain;
using SubworldLibrary;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace Macrocosm.Common.Systems
{
    public class TileCounts : ModSystem
    {
        public static TileCounts Instance => ModContent.GetInstance<TileCounts>();

        public override void Load()
        {
            On_SceneMetrics.ExportTileCountsToMain += On_SceneMetrics_ExportTileCountsToMain;
        }

        public override void Unload()
        {
            On_SceneMetrics.ExportTileCountsToMain -= On_SceneMetrics_ExportTileCountsToMain;
        }

        public int RegolithCount { get; private set; } = 0;
        public int IrradiatedRockCount { get; private set; } = 0;
        public int GraveyardTileCount { get; private set; } = 0;
        public int MonolithCount { get; private set; } = 0;

        public bool EnoughTilesForIrradiation => IrradiatedRockCount > 400;
        public bool EnoughTilesForMonolith => IrradiatedRockCount > 0;


        public float PollutionLevel { get; set; } = 0f;
        public float MaxPollutionLevel => 30f;
        public bool EnoughLevelForPollution => PollutionLevel > 2f;


        private HashSet<int> graveyardTypes;
        public override void PostSetupContent()
        {
            graveyardTypes = new HashSet<int>();
            for (int type = 0; type < TileLoader.TileCount; type++)
            {
                if (TileSets.GraveyardTile[type])
                    graveyardTypes.Add(type);
            }
        }

        public override void ResetNearbyTileEffects()
        {
            RegolithCount = 0;
            IrradiatedRockCount = 0;
            MonolithCount = 0;
            GraveyardTileCount = 0;

            PollutionLevel = 0f;
        }

        public override void TileCountsAvailable(ReadOnlySpan<int> tileCounts)
        {
            RegolithCount = tileCounts[ModContent.TileType<Regolith>()];
            IrradiatedRockCount = tileCounts[ModContent.TileType<IrradiatedRock>()];
            MonolithCount = tileCounts[ModContent.TileType<Monolith>()];

            foreach (int type in graveyardTypes)
                GraveyardTileCount += tileCounts[type];
        }

        private void On_SceneMetrics_ExportTileCountsToMain(On_SceneMetrics.orig_ExportTileCountsToMain orig, SceneMetrics self)
        {
            orig(self);

            if(SubworldSystem.AnyActive<Macrocosm>())
                self.GraveyardTileCount = 0;
            else
                self.GraveyardTileCount += GraveyardTileCount;
        }
    }
}
