using System.Collections;
using System.Collections.Generic;

namespace Macrocosm.Common.DataStructures
{
    // TODO: expand this & make this some kind of graph?
    public abstract class Circuit<T> : IEnumerable<T>
    {
        protected readonly HashSet<T> nodes = new();
        public int NodeCount => nodes.Count;

        public virtual void Add(T node)
        {
            nodes.Add(node);
        }

        public virtual void Remove(T node)
        {
            nodes.Remove(node);
        }

        public virtual void Clear()
        {
            nodes.Clear();
        }

        public virtual bool Contains(T node)
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
