using Macrocosm.Common.Bases.Projectiles;
using Macrocosm.Content.Projectiles.Friendly.Magic;
using Macrocosm.Content.Rarities;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
namespace Macrocosm.Content.Items.Weapons.Magic{
public class DualWaveGuns : ModItem
    {
     
        
        

        public override void SetDefaults()
        {
            Item.damage = 200;
            Item.DamageType = DamageClass.Magic;
            Item.mana = 10;
            Item.width = 54;
            Item.height = 36;
            Item.useTime = 17;
            Item.useAnimation = 17;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 10;
            Item.value = Item.sellPrice(0,10, 0, 0);
            Item.rare = ModContent.RarityType<MoonRarityT1>();
            Item.UseSound = SoundID.Item11;
            Item.autoReuse = true;
            Item.noUseGraphic = true;
            Item.shoot = ModContent.ProjectileType<WaveGunLaser>();
            Item.shootSpeed = 28f;
        }
        public override bool AltFunctionUse(Player player) => true;
         public override bool CanConsumeAmmo(Item ammo, Player player) =>false;
        public override bool CanUseItem(Player player) => player.ownedProjectileCounts[ModContent.ProjectileType<DualWaveGunsHeld>()] < 1;
         public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            float maxCharge = 90f * player.GetAttackSpeed(DamageClass.Ranged);
            Vector2 aim = velocity;
            Projectile.NewProjectileDirect(source, position, aim, ModContent.ProjectileType<DualWaveGunsHeld>(), damage, knockback, player.whoAmI, maxCharge);
            return false;
        }
        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient<WaveGunBlue>(1)
            .AddIngredient<WaveGunRed>(1)
            .AddTile(TileID.LunarCraftingStation)
            .Register();
        }
    }
}