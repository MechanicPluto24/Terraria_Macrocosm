using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.DataStructures;

namespace Macrocosm.Common.DataStructures
{
    public class UnionFind
    {
        private readonly Dictionary<Point16, Point16> parent = new();

        public Point16 Find(Point16 p)
        {
            if (!parent.TryGetValue(p, out Point16 value))
            {
                parent[p] = p;
                return p;
            }
            else if (value != p)
            {
                parent[p] = Find(value);
            }
            return parent[p];
        }

        public void Union(Point16 a, Point16 b)
        {
            Point16 rootA = Find(a);
            Point16 rootB = Find(b);
            if (rootA != rootB)
            {
                parent[rootB] = rootA;
            }
        }

        public bool Connected(Point16 a, Point16 b) => Find(a) == Find(b);

        public void Clear()
        {
            parent.Clear();
        }
    }
}
