﻿using Terraria;
using Terraria.ModLoader;

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
            Item.width = 21;
            Item.height = 32;
            Item.value = 500;
        }

        public override void AddRecipes()
        {
        }
    }
}