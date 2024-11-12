using Microsoft.Xna.Framework;
using Terraria.ID;

namespace Macrocosm.Common.Sets
{
    /// <summary>
    /// Tile Sets for special behavior of some Tiles, useful for crossmod.
    /// Note: Only initalize sets with vanilla content here, add modded content to sets in SetStaticDefaults.
    /// </summary>
    internal class TileSets
    {
        /// <summary> This tile type counts for graveyard, where applicable (e.g. Earth) </summary>
        public static bool[] GraveyardTile { get; } = TileID.Sets.Factory.CreateBoolSet();

        public static int[] RandomStyles { get; } = TileID.Sets.Factory.CreateIntSet(defaultState: 1);

        /// <summary>
        /// For custom sized containers (different from 2x2 chests and 3x2 dressers) 
        /// Used for consistent tile interaction. 
        /// </summary>
        // TODO: add support for DefaultContainerName and net serializaton 
        public static Point[] CustomContainerSize { get; } = TileID.Sets.Factory.CreateCustomSet(defaultState: Point.Zero);
    }
}
