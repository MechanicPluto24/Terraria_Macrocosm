using Macrocosm.Content.Items.Materials;
using Macrocosm.Content.Projectiles.Friendly.Magic;
using Macrocosm.Content.Rarities;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Weapons.Magic
{
    public class DianiteFork : ModItem
    {
        public override void SetStaticDefaults()
        {

            Item.staff[Type] = true;
        }

        public override void SetDefaults()
        {
            Item.damage = 125;
            Item.DamageType = DamageClass.Magic;
            Item.mana = 30;
            Item.width = 80;
            Item.height = 80;
            Item.useTime = 12;
            Item.useAnimation = 10;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 5;
            Item.value = 10000;
            Item.rare = ModContent.RarityType<MoonRarityT1>();
            Item.UseSound = SoundID.Item20;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<DianiteForkProjectile>();
            Item.shootSpeed = 10f;
            Item.tileBoost = 50;
        }

        public override Vector2? HoldoutOrigin()
            => new Vector2(0, 0);


        public override void AddRecipes()
        {
            Recipe recipe = Recipe.Create(Type);
            recipe.AddIngredient<DianiteBar>(12);
            recipe.AddTile(TileID.WorkBenches);
            recipe.Register();
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockBack)
        {
            int projCount = 2;

            for (int i = 0; i < projCount; i++)
            {
                float angle = 360f / projCount * i;
                Projectile.NewProjectileDirect(source, position, velocity, ModContent.ProjectileType<DianiteForkProjectile>(), damage, knockBack, player.whoAmI, ai1: angle);
            }

            return false;
        }

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
        }
    }
}