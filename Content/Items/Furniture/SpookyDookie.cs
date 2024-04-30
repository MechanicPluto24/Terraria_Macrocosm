using System;
using System.Collections.Generic;
using Terraria.ModLoader;
using Terraria;

namespace Macrocosm.Content.Items.Furniture
{
    internal class SpookyDookie : ModItem
    {
        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<Tiles.Furniture.SpookyDookie>());
            Item.width = 16;
            Item.height = 22;
            Item.value = 150;
        }

        public override void AddRecipes()
        {
        }
    }
}
