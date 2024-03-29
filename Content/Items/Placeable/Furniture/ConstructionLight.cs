﻿using Macrocosm.Content.Items.Placeable.Blocks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Placeable.Furniture
{
    public class ConstructionLight : ModItem
    {
        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<Tiles.Furniture.ConstructionLight>());
            Item.width = 26;
            Item.height = 30;
            Item.value = 500;
        }

        public override void AddRecipes()
        {
        }
    }
}
