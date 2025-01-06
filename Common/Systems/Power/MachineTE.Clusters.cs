using Macrocosm.Common.Utils;
using System.Collections.Generic;
using Terraria.DataStructures;
using Terraria;
using Macrocosm.Common.Config;
using System.Linq;
using System.Reflection.PortableExecutable;

namespace Macrocosm.Common.Systems.Power
{
    public partial class MachineTE
    {
        public virtual bool CanCluster => false;

        public List<Point16> Cluster { get; private set; } = null;
        public Point16 ClusterOrigin { get; private set; }

        public int ClusterSize => Cluster != null ? Cluster.Count : 1;    
        public bool IsClusterOrigin => Position == ClusterOrigin;
        public bool InactiveInCluster => Cluster != null && !IsClusterOrigin;


        private static int clusterTimer = 0;
        private static void BuildClusters()
        {
            if (clusterTimer++ >= (int)ServerConfig.Instance.ClusterFindUpdateRate)
            {
                RefreshClusters();
                BuildClusters_Internal();
            }

        }

        private static void RefreshClusters()
        {
            foreach (var te in ByID.Values)
            {
                if (te is MachineTE machine)
                {
                    machine.Cluster = null;
                }
            }
        }

        private static void BuildClusters_Internal()
        {
            foreach (var tileEntity in ByID.Values)
            {
                if (tileEntity is MachineTE machine && machine.CanCluster && machine.Cluster == null)
                {
                    List<Point16> cluster = FindCluster(machine.Position.X, machine.Position.Y, machine.MachineTile.Type);
                    Point16 clusterOrigin = cluster.MinBy(tile => (tile.Y, tile.X));
                    foreach (var position in cluster)
                    {
                        if (ByPosition.TryGetValue(position, out var other) && other is MachineTE otherMachine && machine.Type == otherMachine.Type)
                        {
                            otherMachine.Cluster = cluster;
                            otherMachine.ClusterOrigin = clusterOrigin;
                        }
                    }
                }
            }
        }

        private static List<Point16> FindCluster(int startX, int startY, int tileType)
        {
            var cluster = new List<Point16>();
            var toVisit = new Queue<Point16>();
            var visited = new HashSet<Point16>();

            toVisit.Enqueue(new Point16(startX, startY));

            while (toVisit.Count > 0)
            {
                var current = toVisit.Dequeue();
                if (visited.Contains(current))
                    continue;

                visited.Add(current);
                cluster.Add(current);

                foreach (var neighbor in GetNeighborOffsets())
                {
                    var neighborPos = new Point16(current.X + neighbor.X, current.Y + neighbor.Y);
                    if (visited.Contains(neighborPos))
                        continue;

                    if (Main.tile[neighborPos.X, neighborPos.Y].TileType == tileType)
                    {
                        toVisit.Enqueue(neighborPos);
                    }
                }
            }

            return cluster;
        }

        private static Point16[] GetNeighborOffsets() =>
        [
            new Point16(-1, 0),
            new Point16(1, 0), 
            new Point16(0, -1),
            new Point16(0, 1), 
        ];
    }
}
