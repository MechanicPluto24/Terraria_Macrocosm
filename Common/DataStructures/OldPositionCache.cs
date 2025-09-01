using Microsoft.Xna.Framework;

namespace Macrocosm.Common.DataStructures
{
    internal struct OldPositionCache
    {
        public Vector2[] Positions { get; private set; }
        public readonly int Count => Positions.Length;

        public OldPositionCache(int length, Vector2 init = default)
        {
            Positions = new Vector2[length];
            for (var i = 0; i < length; i++)
            {
                Positions[i] = init;
            }
        }

        public readonly void Add(Vector2 position)
        {
            for (int i = Positions.Length - 1; i > 0; i--)
            {
                Positions[i] = Positions[i - 1];
            }

            Positions[0] = position;
        }
    }
}
