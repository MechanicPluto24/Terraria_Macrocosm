﻿using Macrocosm.Common.Enums;
using Macrocosm.Content.Items.Consumables.Throwable;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Furniture.Mercury
{
    public class MercuryCandelabra : ModItem
    {
        public override void SetStaticDefaults()
        {
        }

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<Tiles.Furniture.Luminite.LuminiteCandelabra>(), (int)LuminiteStyle.Mercury);
            Item.width = 30;
            Item.height = 22;
            Item.value = 150;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.MercuryBrick, 5)
                .AddIngredient<LunarCrystal>(3)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }
}
