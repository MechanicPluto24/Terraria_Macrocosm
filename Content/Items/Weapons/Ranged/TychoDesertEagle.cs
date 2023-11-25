using Macrocosm.Common.Bases;
using Macrocosm.Content.Projectiles.Friendly.Ranged;
using Macrocosm.Content.Rarities;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Weapons.Ranged
{
    public class TychoDesertEagle : GunHeldProjectileItem
    {
        public override GunHeldProjectileData GunHeldProjectileData => new()
        {
            GunBarrelPosition = new(22, 6),
            CenterYOffset = 9,
            MuzzleOffset = 30,
            Recoil = (8, 0.75f),
            RecoilStartFrame = 4,
            UseBackArm = false
        };

        public override float? ProjectileScale => 0.75f;

        public override void SetStaticDefaults()
        {

        }

        public override void SetDefaultsHeldProjectile()
        {
            Item.damage = 150;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 50;
            Item.height = 32;
            Item.useTime = 20;
            Item.useAnimation = 20; // The length of the item's use animation in ticks (60 ticks == 1 second.)
            Item.useStyle = ItemUseStyleID.Shoot; // how you use the item (swinging, holding out, etc)
            Item.noMelee = true; //so the item's animation doesn't do damage
            Item.knockBack = 0; // Sets the item's knockback. Note that projectiles shot by this weapon will use its and the used ammunition's knockback added together.
            Item.value = Item.sellPrice(silver: 700);
            Item.rare = ModContent.RarityType<MoonRarityT2>();
            Item.autoReuse = true; // if you can hold click to automatically use it again
            Item.shoot = ModContent.ProjectileType<TychoBullet>(); //this gun uses a holdout projectile
            Item.shootSpeed = 32f; // the speed of the projectile (measured in pixels per frame)
            Item.DamageType = DamageClass.Ranged; //deals melee damage
            Item.reuseDelay = 8;
            Item.useAmmo = AmmoID.Bullet; //uses bullets as ammunition
        }

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            position += new Vector2(44, -4 * Math.Sign(velocity.X)).RotatedBy(velocity.ToRotation());
            type = ModContent.ProjectileType<TychoBullet>();
        }


        public override Vector2? HoldoutOffset() => new Vector2(4, 0);

        public override void AddRecipes()
        {

        }
    }
}
