using Macrocosm.Common.Sets;
using Macrocosm.Content.Tiles.Blocks.Terrain;
using Macrocosm.Content.Tiles.Misc;
using Microsoft.Xna.Framework;
using SubworldLibrary;
using System;
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

        public int IrradiatedRockCount { get; private set; } = 0;
        public int GraveyardTileCount { get; private set; } = 0;
        public int MonolithCount { get; private set; } = 0;

        public bool EnoughTilesForIrradiation => IrradiatedRockCount > 400;

        public bool HasMonolith => MonolithCount > 0;
        public bool EnoughPollution => PollutionLevel > PollutionLevelThreshold;

        public float PollutionLevel
        {
            get => pollutionLevel;
            set => pollutionLevel = MathHelper.Clamp(value, 0f, PollutionLevelMax);
        }
        private float pollutionLevel = 0f;
        public float PollutionLevelThreshold => 6f;
        public float PollutionLevelMax => 30f;

        public override void ResetNearbyTileEffects()
        {
            IrradiatedRockCount = 0;
            MonolithCount = 0;
            GraveyardTileCount = 0;

            PollutionLevel = 0f;
        }

        public override void TileCountsAvailable(ReadOnlySpan<int> tileCounts)
        {
            IrradiatedRockCount = tileCounts[ModContent.TileType<IrradiatedRock>()];
            MonolithCount = tileCounts[ModContent.TileType<Monolith>()];

            for (int type = 0; type < TileLoader.TileCount; type++)
            {
                if (TileSets.CountsForGraveyard[type])
                    GraveyardTileCount += tileCounts[type];
            }
        }

        public int GetModifiedGraveyardTileCount(int graveyardTileCount)
        {
            if (SubworldSystem.AnyActive<Macrocosm>())
                return 0;

            return graveyardTileCount + GraveyardTileCount;
        }

        private void On_SceneMetrics_ExportTileCountsToMain(On_SceneMetrics.orig_ExportTileCountsToMain orig, SceneMetrics self)
        {
            orig(self);
            self.GraveyardTileCount = GetModifiedGraveyardTileCount(self.GraveyardTileCount);
        }
    }
}
