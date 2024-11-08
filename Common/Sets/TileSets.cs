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
    }
}
