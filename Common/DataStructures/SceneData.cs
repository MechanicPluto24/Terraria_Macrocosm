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

        public SceneData(Vector2 scanCenterWorldCoordinates)
        {
            sceneMetrics = new();
            this.scanCenterWorldCoordinates = scanCenterWorldCoordinates;
            Scan();
        }

        public SceneData(Point scanCenterTileCoordinates) : this(scanCenterTileCoordinates.ToWorldCoordinates()) { }
        public SceneData(Point16 scanCenterTileCoordinates) : this(scanCenterTileCoordinates.ToPoint()) { }

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
