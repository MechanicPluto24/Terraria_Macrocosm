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
    public class StarDestroyer : GunHeldProjectileItem
    {
        private int starType = 0;

        public override void SetStaticDefaults()
        {
            ItemID.Sets.ItemsThatAllowRepeatedRightClick[Type] = true;
        }

        public override void SetDefaultsHeldProjectile()
        {
            Item.damage = 300;
            Item.knockBack = 4;
            Item.width = 76;
            Item.height = 30;
            Item.value = 10000;
            Item.rare = ModContent.RarityType<MoonRarityT3>();
            Item.UseSound = SoundID.Item9;
            Item.useAnimation = 10;
            Item.useTime = 10;
            Item.DamageType = DamageClass.Ranged;
            Item.noMelee = true;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.useAmmo = AmmoID.FallenStar;
            Item.shoot = Macrocosm.ItemShoot_UsesAmmo;
            Item.shootSpeed = 30f;
            Item.autoReuse = true;
        }

        public override GunHeldProjectileData GunHeldProjectileData => new()
        {
            GunBarrelPosition = new Vector2(26f, 7f),
            CenterYOffset = 9f,
            MuzzleOffset = 0f,
            RecoilDiminish = 0.9f
        };

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback, GunHeldProjectile gunHeldProjectile)
        {
            Projectile.NewProjectileDirect(source, position, velocity.RotatedByRandom(0.1) * 1.2f, ModContent.ProjectileType<StarDestroyerStar>(), damage, knockback, player.whoAmI, ai0: starType);

            if (starType++ > 1)
                starType = 0;

            for (int i = 0; i < Main.rand.Next(1, 3 + 1); i++)
                Projectile.NewProjectileDirect(source, position, velocity.RotatedByRandom(0.1) * Main.rand.NextFloat(0.9f, 1.8f), ModContent.ProjectileType<StarDestroyerBeam>(), damage / 2, knockback, player.whoAmI);

            return false;
        }
    }
}
