﻿using Macrocosm.Common.Enums;
using Macrocosm.Content.Items.Consumables.Throwable;
using Macrocosm.Content.Items.Blocks.Bricks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Furniture.Haemonova
{
    public class HaemonovaCandle : ModItem
    {
        public override void SetStaticDefaults()
        {
        }

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<Tiles.Furniture.Luminite.LuminiteCandle>(), (int)LuminiteStyle.Haemonova);
            Item.width = 16;
            Item.height = 16;
            Item.value = 150;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<HaemonovaBrick>( 4)
                .AddIngredient<LunarCrystal>(1)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }
}