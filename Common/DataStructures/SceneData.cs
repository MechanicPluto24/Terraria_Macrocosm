using Macrocosm.Common.Systems;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;

namespace Macrocosm.Common.DataStructures
{
    /// <summary>
    /// Data structure that lets you analyze SOME vanilla and modded scene metrics at any position, even on the server. 
    /// <br/> It includes NearbyEffects of modded tiles, only when !closer (gameplay effects)
    /// <br/> This includes most biome-related info, but <b>excludes</b> visual related things such as monoliths 
    /// </summary>
    public class SceneData
    {
        private readonly SceneMetrics sceneMetrics;
        private Vector2 scanCenterWorldCoordinates;

        public TileCounts Macrocosm => TileCounts.Instance;

        public Vector2 Position => scanCenterWorldCoordinates;
        public Point TilePosition => scanCenterWorldCoordinates.ToTileCoordinates();

        public bool ZoneUnderworldHeight => TilePosition.Y > Main.UnderworldLayer;
        public bool ZoneRockLayerHeight => TilePosition.Y <= Main.UnderworldLayer && TilePosition.Y > Main.rockLayer;
        public bool ZoneDirtLayerHeight => TilePosition.Y <= Main.rockLayer && TilePosition.Y > Main.worldSurface;
        public bool ZoneOverworldHeight => TilePosition.Y <= Main.worldSurface && TilePosition.Y > Main.worldSurface * 0.3499999940395355;
        public bool ZoneSkyHeight => TilePosition.Y <= Main.worldSurface * 0.3499999940395355;
        public bool ZoneBeach => WorldGen.oceanDepths(TilePosition.X, TilePosition.Y);
        public bool ZoneRain => Main.raining && TilePosition.Y <= Main.worldSurface;

        public bool ZoneShimmer => sceneMetrics.EnoughTilesForShimmer;
		public bool ZoneCorrupt => sceneMetrics.EnoughTilesForCorruption;
		public bool ZoneCrimson => sceneMetrics.EnoughTilesForCrimson;
		public bool ZoneHallow => sceneMetrics.EnoughTilesForHallow;
		public bool ZoneJungle => sceneMetrics.EnoughTilesForJungle && Position.Y / 16f < Main.UnderworldLayer;
        public bool ZoneSnow => sceneMetrics.EnoughTilesForSnow;
		public bool ZoneDesert => sceneMetrics.EnoughTilesForDesert;
		public bool ZoneGlowshroom => sceneMetrics.EnoughTilesForGlowingMushroom;
		public bool ZoneMeteor => sceneMetrics.EnoughTilesForMeteor;
		public bool ZoneWaterCandle => sceneMetrics.WaterCandleCount > 0;
		public bool ZonePeaceCandle => sceneMetrics.PeaceCandleCount > 0;
		public bool ZoneShadowCandle => sceneMetrics.ShadowCandleCount > 0;
		public bool ZoneGraveyard => sceneMetrics.EnoughTilesForGraveyard;

        public SceneData(Vector2 scanCenterWorldCoordinates)
        {
            sceneMetrics = new();
            this.scanCenterWorldCoordinates = scanCenterWorldCoordinates;
            Scan();
        }

        public SceneData(Point TilePosition) : this(TilePosition.ToWorldCoordinates()) { }
        public SceneData(Point16 TilePosition) : this(TilePosition.ToPoint()) { }

        public void Recenter(Vector2 scanCenterWorldCoordinates) => this.scanCenterWorldCoordinates = scanCenterWorldCoordinates;

        public void Scan(Vector2 scanCenterWorldCoordinates)
        {
            Recenter(scanCenterWorldCoordinates);
            Scan();
        }

        // TODO: reimplement this logic, this calling NearbyEffects could be problematic
        public void Scan()
        {
            SceneMetricsScanSettings settings = new() 
            {
                VisualScanArea = null, // Exclude visuals (e.g. monoliths)
                BiomeScanCenterPositionInWorld = scanCenterWorldCoordinates, 
                ScanOreFinderData = false // No ore finder info
            };

            sceneMetrics.ScanAndExportToMain(settings);
        }
    }
}
