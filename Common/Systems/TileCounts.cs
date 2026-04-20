using Macrocosm.Common.Sets;
using Macrocosm.Content.Tiles.Blocks.Terrain;
using Macrocosm.Content.Tiles.Misc;
using Microsoft.Xna.Framework;
using SubworldLibrary;
using System;
using Terraria;
using Terraria.ModLoader;

namespace Macrocosm.Common.Systems;

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
    public int GraveyardExtraTileCount { get; private set; } = 0;
    public int MonolithCount { get; private set; } = 0;
    public int ApolloLanderCount { get; private set; } = 0;

    public bool EnoughTilesForIrradiation => IrradiatedRockCount > 400;

    public bool HasMonolith => MonolithCount > 0;
    public bool HasApolloLander => ApolloLanderCount > 0;
    public bool EnoughPollution => PollutionLevel > PollutionLevelThreshold;

    public float PollutionLevel
    {
        get => field;
        set => field = MathHelper.Clamp(value, 0f, PollutionLevelMax);
    }
    public float Pollution01 => PollutionLevelMax <= 0f ? 0f : PollutionLevel / PollutionLevelMax;
    public const float PollutionLevelMax = 500f;
    public const float PollutionLevelThreshold = 100f;

    public override void ResetNearbyTileEffects()
    {
        IrradiatedRockCount = 0;
        MonolithCount = 0;
        ApolloLanderCount = 0;
        GraveyardExtraTileCount = 0;

        PollutionLevel = 0f;
    }

    public override void TileCountsAvailable(ReadOnlySpan<int> tileCounts)
    {
        IrradiatedRockCount = tileCounts[ModContent.TileType<IrradiatedRock>()];
        MonolithCount = tileCounts[ModContent.TileType<Monolith>()];
        ApolloLanderCount = tileCounts[ModContent.TileType<ApolloLander>()];

        for (int type = 0; type < TileLoader.TileCount; type++)
        {
            if (TileSets.CountsForGraveyard[type])
                GraveyardExtraTileCount += tileCounts[type];
        }
    }

    public int GetModifiedGraveyardTileCount(int graveyardTileCount)
    {
        if (SubworldSystem.AnyActive<Macrocosm>())
            return 0;


        return graveyardTileCount + GraveyardExtraTileCount;
    }

    private void On_SceneMetrics_ExportTileCountsToMain(On_SceneMetrics.orig_ExportTileCountsToMain orig, SceneMetrics self)
    {
        orig(self);
        self.GraveyardTileCount = GetModifiedGraveyardTileCount(self.GraveyardTileCount);
    }
}
