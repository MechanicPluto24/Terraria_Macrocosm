﻿using Macrocosm.Common.Enums;
using Macrocosm.Content.Items.Consumables.Throwable;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Furniture.StarRoyale
{
    public class StarRoyaleLantern : ModItem
    {
        public override void SetStaticDefaults()
        {
        }

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<Tiles.Furniture.Luminite.LuminiteLantern>(), (int)LuminiteStyle.StarRoyale);
            Item.width = 14;
            Item.height = 28;
            Item.value = 150;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.StarRoyaleBrick, 6)
                .AddIngredient<LunarCrystal>(1)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }
}
