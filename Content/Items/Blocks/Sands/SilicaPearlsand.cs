﻿using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Blocks.Sands
{
    public class SilicaPearlsand : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 200;
            ItemID.Sets.ExtractinatorMode[Type] = ModContent.ItemType<SilicaSand>();
            ItemID.Sets.SandgunAmmoProjectileData[Type] = new(ModContent.ProjectileType<Projectiles.Environment.Sands.SilicaPearlsandBall>(), BonusDamage: 20);
        }

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<Tiles.Blocks.Sands.SilicaPearlsand>());
            Item.width = 12;
            Item.height = 12;
            Item.ammo = AmmoID.Sand;
            Item.notAmmo = true;
        }

        public override void AddRecipes()
        {
        }
    }
}