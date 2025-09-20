using Macrocosm.Common.Config;
using Macrocosm.Common.Utils;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.DataStructures;

namespace Macrocosm.Common.Systems.Power;

public partial class MachineTE
{
    public virtual bool CanCluster => false;

    public List<Point16> Cluster { get; private set; } = null;
    public int ClusterSize => Cluster != null ? Cluster.Count : 1;

    /// <summary> 
    /// Used for checking for new clusters when a tile is broken
    /// <br/> Run on the template instance
    /// </summary>
    public void KillTile_ClusterCheck(Point16 position)
    {
        if (CanCluster)
        {
            foreach (var neighbor in GetNeighborOffsets())
            {
                var neighborPosition = new Point16(position.X + neighbor.X, position.Y + neighbor.Y);
                Tile tile = Main.tile[neighborPosition];
                if (tile.HasTile && tile.TileType == MachineTile.Type && !ByPosition.ContainsKey(neighborPosition))
                    BlockPlacement(neighborPosition.X, neighborPosition.Y);
            }
        }
    }

    private static int clusterTimer = 0;
    private static void BuildClusters()
    {
        try
        {
            if (clusterTimer++ >= (int)ServerConfig.Instance.ClusterFindUpdateRate)
            {
                IEnumerable<MachineTE> clusterable = ByID.Values.Where(te => te is MachineTE m && m.CanCluster).Cast<MachineTE>();

                foreach (MachineTE machine in clusterable)
                    machine.Cluster = null;

                foreach (MachineTE machine in clusterable)
                {
                    // Get all tile coordinates where there is a connected tile of the same type as this machine's
                    List<Point16> cluster = FindCluster(machine.Position.X, machine.Position.Y, machine.MachineTile.Type);
                    if (cluster.Count > 0)
                    {
                        machine.Cluster = cluster;
                        Point16 clusterOrigin = cluster.MinBy(tile => (tile.Y, tile.X));

                        // Note: Tile type checks would be redundant here
                        foreach (var position in cluster)
                        {
                            if (position == clusterOrigin)
                            {
                                // On the origin 
                                if (!TileEntity.TryGet<MachineTE>(position.X, position.Y, out _))
                                    machine.Place(position.X, position.Y);
                            }
                            else
                            {
                                // Kill other TEs of the same type that are part of this cluster 
                                if (TileEntity.TryGet(position, out MachineTE other) && machine.Type == other.Type)
                                    other.Kill(position.X, position.Y);
                            }
                        }
                    }
                }
            }
        }
        catch 
        {
            // idk
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

                Tile tile = Main.tile[neighborPos.X, neighborPos.Y];
                if (tile.HasTile && tile.TileType == tileType)
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
