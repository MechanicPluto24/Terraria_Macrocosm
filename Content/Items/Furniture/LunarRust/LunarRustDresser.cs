﻿using Macrocosm.Common.Enums;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Furniture.LunarRust
{
    public class LunarRustDresser : ModItem
    {
        public override void SetStaticDefaults()
        {
        }

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<Tiles.Furniture.Luminite.LuminiteDresser>(), (int)LuminiteStyle.LunarRust);
            Item.width = 36;
            Item.height = 22;
            Item.value = 150;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(TileID.LunarRustBrick, 16)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }
}
