using Macrocosm.Common.Bases.Projectiles;
using Macrocosm.Common.Drawing.Particles;
using Macrocosm.Content.Particles;
using Macrocosm.Content.Projectiles.Friendly.Ranged;
using Macrocosm.Content.Rarities;
using Macrocosm.Content.Sounds;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
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
            Item.DefaultToRangedWeapon(baseProjType: 10, ammoID: AmmoID.Bullet, singleShotTime: 28, shotVelocity: 32f, hasAutoReuse: true);

            Item.width = 50;
            Item.height = 32;

            Item.damage = 290;
            Item.DamageType = DamageClass.Ranged;
            Item.knockBack = 0; 

            Item.value = Item.sellPrice(silver: 700);
            Item.rare = ModContent.RarityType<MoonRarity2>();
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback, GunHeldProjectile heldProjectile)
        {
            if (!Main.dedServ)
                SoundEngine.PlaySound(SFX.DesertEagleShot with { Volume = 0.1f }, position);

            Particle.Create<GunFireRing>(position + new Vector2(14, 0).RotatedBy(velocity.ToRotation()), velocity * 0.25f, new(1f), velocity.ToRotation(), false);
            return true;
        }


        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            position += new Vector2(44, -4 * Math.Sign(velocity.X)).RotatedBy(velocity.ToRotation());

            if (type == ProjectileID.Bullet)
                type = ModContent.ProjectileType<ArmorPiercingBullet>();
        }

        public override Vector2? HoldoutOffset() => new Vector2(4, 0);

        public override void AddRecipes()
        {
        }
    }
}
