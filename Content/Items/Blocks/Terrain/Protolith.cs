﻿using Macrocosm.Content.Items.Walls;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

namespace Macrocosm.Content.Items.Blocks.Terrain
{
    public class Protolith : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 100;
        }

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<Tiles.Blocks.Terrain.Protolith>());
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<ProtolithWall>(4)
                .Register();

            Recipe.Create(ItemID.SolarBrick, 1)
                .AddIngredient(Type, 10)
                .AddIngredient(ItemID.FragmentSolar, 1)
                .Register();
            Recipe.Create(ItemID.LunarBlockSolar, 1)
                .AddIngredient(Type, 5)
                .AddIngredient(ItemID.FragmentSolar, 1)
                .Register();

            Recipe.Create(ItemID.VortexBrick, 1)
                .AddIngredient(Type, 10)
                .AddIngredient(ItemID.FragmentVortex, 1)
                .Register();
            Recipe.Create(ItemID.LunarBlockVortex, 1)
                .AddIngredient(Type, 5)
                .AddIngredient(ItemID.FragmentVortex, 1)
                .Register();

            Recipe.Create(ItemID.NebulaBrick, 1)
                .AddIngredient(Type, 10)
                .AddIngredient(ItemID.FragmentNebula, 1)
                .Register();
            Recipe.Create(ItemID.LunarBlockNebula, 1)
                .AddIngredient(Type, 5)
                .AddIngredient(ItemID.FragmentNebula, 1)
                .Register();

            Recipe.Create(ItemID.StardustBrick, 1)
                .AddIngredient(Type, 10)
                .AddIngredient(ItemID.FragmentStardust, 1)
                .Register();
            Recipe.Create(ItemID.LunarBlockStardust, 1)
                .AddIngredient(Type, 5)
                .AddIngredient(ItemID.FragmentStardust, 1)
                .Register();
            
        }
    }
}