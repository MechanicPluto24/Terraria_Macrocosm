using Macrocosm.Common.Bases;
using Macrocosm.Common.Utils;
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

            ItemID.Sets.ItemsThatAllowRepeatedRightClick[Type] = true;
        }

        public override void SetDefaultsHeldProjectile()
        {
            Item.damage = 80;
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
            Item.shoot = ProjectileID.PurificationPowder;
            Item.shootSpeed = 20f;
            Item.useAmmo = AmmoID.Bullet;
        }

        public override bool AltFunctionUse(Player player) => true;

        public override bool CanUseItemHeldProjectile(Player player)
        {
            if (player.AltFunction())
            {
                Item.useTime = 30;
                Item.useAnimation = 30;
                Item.autoReuse = true;
                Item.shoot = ModContent.ProjectileType<DeliriumShell>();
            }
            else
            {
                Item.useTime = 20;
                Item.useAnimation = 20;
                Item.shoot = ModContent.ProjectileType<CruithneGreenSlug>();
            }

            return true;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockBack)
        {
            int numberProjectiles = player.AltFunction() ? 1 : 6;
            int degree = player.AltFunction() ? 0 : 12;

            for (int i = 0; i < numberProjectiles; i++)
            {
                Vector2 muzzleOffset = Vector2.Normalize(velocity) * 12f;
                if (Collision.CanHit(position, 0, 0, position + muzzleOffset, 0, 0))
                    position += muzzleOffset;
                Vector2 perturbedSpeed = velocity.RotatedByRandom(MathHelper.ToRadians(degree));
                Projectile.NewProjectile(source, position.X, position.Y, perturbedSpeed.X, perturbedSpeed.Y, Item.shoot, damage, knockBack, player.whoAmI);
            }
            return false;
        }

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            if (player.AltFunction())
            {
                damage = (int)(damage * 1.5f);
                velocity *= 0.5f;
                position.Y -= 4;
            }
        }

        public override Vector2? HoldoutOffset() => new Vector2(-12, 0);
    }
}
