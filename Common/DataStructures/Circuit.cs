using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Macrocosm.Common.DataStructures
{
    // TODO: expand this & make this some kind of graph?
    public abstract class Circuit<T> : IEnumerable<T>
    {
        protected readonly HashSet<T> nodes = new();
        public int NodeCount => nodes.Count;

        public void Add(T node)
        {
            nodes.Add(node);
        }

        public void Remove(T node)
        {
            nodes.Remove(node);
        }

        public void Clear()
        {
            nodes.Clear();
        }

        public bool Contains(T node)
        {
            return nodes.Contains(node);
        }

        public bool IsEmpty => nodes.Count == 0;

        public abstract void Merge(Circuit<T> other);

        public abstract void Solve(int updateRate);

        public IEnumerator<T> GetEnumerator()
        {
            return nodes.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
