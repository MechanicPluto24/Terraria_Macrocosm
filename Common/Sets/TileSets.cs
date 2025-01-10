using Microsoft.Xna.Framework;
using Terraria.GameContent;
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
        /// </summary>
        // TODO: add support for net serializaton (and proper unloading but this needs to be in tML directly) 
        public static Point[] CustomContainerSize { get; } = TileID.Sets.Factory.CreateCustomSet(defaultState: Point.Zero);

        public static TreePaintingSettings[] PaintingSettings { get; } = TileID.Sets.Factory.CreateCustomSet<TreePaintingSettings>(defaultState: null);
    }
}
