using System;
using System.Collections.Generic;

namespace Macrocosm.Common.DataStructures
{
    public class UnionFind<T>
    {
        private readonly Dictionary<T, T> parent = new();
        private readonly Dictionary<T, int> rank = new();

        public void MakeSet(T item)
        {
            if (!parent.ContainsKey(item))
            {
                parent[item] = item;
                rank[item] = 0;
            }
        }

        public T Find(T item)
        {
            if (!parent.TryGetValue(item, out T parentItem))
            {
                parent[item] = item;
                rank[item] = 0;
                return item;
            }

            if (!EqualityComparer<T>.Default.Equals(parentItem, item))
            {
                parent[item] = Find(parentItem);
            }

            return parent[item];
        }

        public void Union(T itemA, T itemB)
        {
            T rootA = Find(itemA);
            T rootB = Find(itemB);

            if (EqualityComparer<T>.Default.Equals(rootA, rootB))
                return; 

            // Union by Rank
            int rankA = rank[rootA];
            int rankB = rank[rootB];

            if (rankA < rankB)
            {
                parent[rootA] = rootB;
            }
            else if (rankA > rankB)
            {
                parent[rootB] = rootA;
            }
            else
            {
                parent[rootB] = rootA;
                rank[rootA]++;
            }
        }

        public bool Contains(T item)
        {
            return parent.ContainsKey(item);
        }

        public void Remove(T item)
        {
            if (!parent.TryGetValue(item, out T value))
                return;

            T itemParent = value;
            foreach (var kvp in parent)
            {
                if (EqualityComparer<T>.Default.Equals(kvp.Value, item))
                {
                    parent[kvp.Key] = itemParent;
                }
            }

            parent.Remove(item);
            rank.Remove(item);
        }

        public void Clear()
        {
            parent.Clear();
            rank.Clear();
        }
    }
}
