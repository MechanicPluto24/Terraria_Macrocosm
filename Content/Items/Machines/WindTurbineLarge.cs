﻿using Macrocosm.Common.Systems.Power;
using Terraria;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Machines
{
    public class WindTurbineLarge : ModItem
    {
        public override void SetStaticDefaults()
        {
        }

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<Content.Machines.WindTurbineLarge>());
            Item.width = 20;
            Item.height = 48;
            Item.value = 500;
            Item.mech = true;
        }
    }
}