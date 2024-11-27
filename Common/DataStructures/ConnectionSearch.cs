using System;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;

namespace Macrocosm.Common.DataStructures
{
    public class ConnectionSearch<T>
    {
        private readonly HashSet<Point16> visited;
        private readonly Queue<Point16> toProcess;
        private readonly HashSet<T> connectedNodes;
        private readonly Func<Point16, bool> connectionCheck;
        private readonly Func<Point16, T> retrieveNode;

        public ConnectionSearch(Func<Point16, bool> connectionCheck, Func<Point16, T> retrieveNode)
        {
            this.connectionCheck = connectionCheck;
            this.retrieveNode = retrieveNode;
            visited = new HashSet<Point16>();
            toProcess = new Queue<Point16>();
            connectedNodes = new HashSet<T>();
        }

        public HashSet<T> FindConnectedNodes(IEnumerable<Point16> startingPositions)
        {
            foreach (Point16 position in startingPositions)
            {
                if (connectionCheck(position))
                {
                    toProcess.Enqueue(position);
                    visited.Add(position);
                }
            }

            while (toProcess.Count > 0)
            {
                Point16 current = toProcess.Dequeue();

                var node = retrieveNode(current);
                if (node != null)
                {
                    connectedNodes.Add(node);
                }

                //if (Main.rand.NextBool(15))
                //    Dust.NewDustPerfect(current.ToWorldCoordinates(), DustID.Electric, Velocity: Main.rand.NextVector2Circular(2, 2), Scale: 0.35f);

                ProcessNeighbor(current.X, current.Y - 1); // Up
                ProcessNeighbor(current.X, current.Y + 1); // Down
                ProcessNeighbor(current.X - 1, current.Y); // Left
                ProcessNeighbor(current.X + 1, current.Y); // Right
            }

            return connectedNodes;
        }

        private void ProcessNeighbor(int x, int y)
        {
            if (!WorldGen.InWorld(x, y))
                return;

            Point16 position = new(x, y);
            if (visited.Contains(position))
                return;

            if (connectionCheck(position))
            {
                visited.Add(position);
                toProcess.Enqueue(position);
            }
        }
    }
}
