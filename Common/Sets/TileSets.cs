using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
    }
}
