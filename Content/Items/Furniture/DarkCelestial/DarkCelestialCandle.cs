﻿using Macrocosm.Common.Enums;
using Macrocosm.Content.Items.Consumables.Throwable;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Furniture.DarkCelestial
{
    public class DarkCelestialCandle : ModItem
    {
        public override void SetStaticDefaults()
        {
        }

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<Tiles.Furniture.Luminite.LuminiteCandle>(), (int)LuminiteStyle.DarkCelestial);
            Item.width = 16;
            Item.height = 16;
            Item.value = 150;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.DarkCelestialBrick, 4)
                .AddIngredient<LunarCrystal>(1)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }
}
