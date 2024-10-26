﻿using Terraria;
using Terraria.ModLoader;
using Macrocosm.Content.Items.Bars;
using Macrocosm.Content.Items.Refined;
using Terraria.ID;

namespace Macrocosm.Content.Items.Machines
{
    public class OreExcavator : ModItem
    {
        public override void SetStaticDefaults()
        {
        }

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<Content.Machines.OreExcavator>());
            Item.width = 60;
            Item.height = 84;
            Item.value =  Item.sellPrice(gold:2);
            Item.mech = true;
        }

        public override void AddRecipes()
        {
             CreateRecipe()
                .AddIngredient<SteelBar>(20)
                .AddIngredient(ItemID.Wire, 50)
                .AddIngredient(ItemID.AdamantiteBar, 10)
                .AddIngredient<AluminumBar>(20)
                .AddIngredient(ItemID.Diamond, 50)
                .AddTile<Tiles.Crafting.Fabricator>()
                .Register();
            CreateRecipe()
                .AddIngredient<SteelBar>(20)
                .AddIngredient(ItemID.Wire, 50)
                .AddIngredient(ItemID.TitaniumBar, 10)
                .AddIngredient<AluminumBar>(20)
                .AddIngredient(ItemID.Diamond, 50)
                .AddTile<Tiles.Crafting.Fabricator>()
                .Register();
        }
    }
}