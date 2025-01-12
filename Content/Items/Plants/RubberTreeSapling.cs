using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Plants
{
    public class RubberTreeSapling : ModItem
    {
        public override void SetDefaults() 
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<Tiles.Trees.RubberTreeSapling>(), 0);
            Item.width = 16;
            Item.height = 36;
            Item.value = 10;
        }
    }
}
