﻿using Macrocosm.Content.Items.Bars;
using Macrocosm.Content.Projectiles.Friendly.Summon;
using Macrocosm.Content.Rarities;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Weapons.Summon
{
    public class ChandriumWhip : ModItem
    {
        public override bool MeleePrefix() { return true; }
        public override void SetStaticDefaults()
        {

        }

        public override void SetDefaults()
        {
            Item.DefaultToWhip(ModContent.ProjectileType<ChandriumWhipProjectile>(), 180, 2, 4);

            Item.shootSpeed = 4;
            Item.rare = ModContent.RarityType<MoonRarityT1>();

            Item.channel = true;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient<ChandriumBar>(12)
            .AddTile(TileID.LunarCraftingStation)
            .Register();
        }
    }
}
