using Macrocosm.Common.Bases.Projectiles;
using Macrocosm.Content.Projectiles.Friendly.Ranged;
using Macrocosm.Content.Rarities;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Weapons.Ranged
{
    public class Cruithne : GunHeldProjectileItem
    {
        public override GunHeldProjectileData GunHeldProjectileData => new()
        {
            GunBarrelPosition = new Vector2(25, 6),
            CenterYOffset = 6,
            MuzzleOffset = 33,
            Recoil = (11, 0.8f),
            RecoilDiminish = 0.95f
        };

        public override void SetStaticDefaults()
        {
        }

        public override void SetDefaultsHeldProjectile()
        {
            Item.damage = 85;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 52;
            Item.height = 16;
            Item.useTime = 26;
            Item.useAnimation = 26;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 8f;
            Item.value = 10000;
            Item.rare = ModContent.RarityType<MoonRarityT2>();
            Item.UseSound = SoundID.Item38;
            Item.autoReuse = true;
            Item.shoot = Macrocosm.ItemShoot_UsesAmmo;
            Item.shootSpeed = 20f;
            Item.useAmmo = AmmoID.Bullet;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockBack, GunHeldProjectile heldProjectile)
        {
            Vector2 muzzleOffset = Vector2.Normalize(velocity) * 2f;
            if (Collision.CanHit(position, 0, 0, position + muzzleOffset, 0, 0))
                position += muzzleOffset;

            // Shoot slugs
            for (int i = 0; i < 6; i++)
            {
                // Shoot green slugs instead of musket balls
                type = type == ProjectileID.Bullet ? ModContent.ProjectileType<CruithneGreenSlug>() : type; 

                Vector2 perturbedSpeed = velocity.RotatedByRandom(MathHelper.ToRadians(12)) * Main.rand.NextFloat(0.7f, 100f);
                Projectile.NewProjectile(source, position.X, position.Y, perturbedSpeed.X, perturbedSpeed.Y, type, damage, knockBack, player.whoAmI);
            }

            // Shoot delirium shell
            damage = (int)(damage * 1.2f);
            position.Y -= 8;
            Projectile.NewProjectile(source, position.X, position.Y, velocity.X, velocity.Y, ModContent.ProjectileType<DeliriumShell>(), damage, knockBack, player.whoAmI);

            return false;
        }

        public override Vector2? HoldoutOffset() => new Vector2(-12, 0);
    }
}
