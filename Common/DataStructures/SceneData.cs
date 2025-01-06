using Macrocosm.Common.Systems;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;

namespace Macrocosm.Common.DataStructures
{
    public class SceneData
    {
        private readonly SceneMetrics sceneMetrics;
        private Vector2 scanCenterWorldCoordinates;

        public SceneMetrics Terraria => sceneMetrics;
        public TileCounts Macrocosm => TileCounts.Instance;


        public Vector2 Position => scanCenterWorldCoordinates;
        public Point TilePosition => scanCenterWorldCoordinates.ToTileCoordinates();

        public bool ZoneUnderworldHeight => TilePosition.Y > Main.UnderworldLayer;
        public bool ZoneRockLayerHeight => TilePosition.Y <= Main.UnderworldLayer && (double)TilePosition.Y > Main.rockLayer;
        public bool ZoneDirtLayerHeight => (double)TilePosition.Y <= Main.rockLayer && (double)TilePosition.Y > Main.worldSurface;
        public bool ZoneOverworldHeight => (double)TilePosition.Y <= Main.worldSurface && (double)TilePosition.Y > Main.worldSurface * 0.3499999940395355;
        public bool ZoneSkyHeight => (double)TilePosition.Y <= Main.worldSurface * 0.3499999940395355;
        public bool ZoneBeach => WorldGen.oceanDepths(TilePosition.X, TilePosition.Y);
        public bool ZoneRain => Main.raining && (double)TilePosition.Y <= Main.worldSurface;

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

        public void Scan()
        {
            SceneMetricsScanSettings settings = new() { BiomeScanCenterPositionInWorld = scanCenterWorldCoordinates };
            sceneMetrics.ScanAndExportToMain(settings);
        }
    }
}
