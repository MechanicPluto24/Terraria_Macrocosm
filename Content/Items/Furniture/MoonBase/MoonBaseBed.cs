﻿using Macrocosm.Content.Items.Blocks;
using Macrocosm.Content.Items.Crafting;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Furniture.MoonBase
{
    public class MoonBaseBed : ModItem
    {
        public override void SetStaticDefaults()
        {
        }

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<Tiles.Furniture.MoonBase.MoonBaseBed>());
            Item.width = 34;
            Item.height = 24;
            Item.value = 500;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient<MoonBasePlating>(15)
            .AddIngredient(ItemID.Silk, 5)
            .AddTile<Tiles.Crafting.Fabricator>()
            .Register();
        }
    }
}