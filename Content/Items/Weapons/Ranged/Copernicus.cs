using Macrocosm.Common.Bases.Projectiles;
using Macrocosm.Common.Utils;
using Macrocosm.Content.Projectiles.Friendly.Ranged;
using Macrocosm.Content.Rarities;
using Macrocosm.Content.Sounds;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Weapons.Ranged
{
    public class Copernicus : GunHeldProjectileItem
    {
        public override void SetStaticDefaults()
        {
        }

        public override void SetDefaultsHeldProjectile()
        {
            Item.damage = 90;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 70;
            Item.height = 26;
            Item.useTime = 8;
            Item.useAnimation = 8;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.channel = true;
            Item.knockBack = 8f;
            Item.value = 10000;
            Item.rare = ModContent.RarityType<MoonRarityT2>();
            Item.shoot = Macrocosm.ItemShoot_UsesAmmo;
            Item.autoReuse = true;
            Item.shootSpeed = 20f;
            Item.useAmmo = AmmoID.Bullet;
        }

        public override GunHeldProjectileData GunHeldProjectileData => new()
        {
            GunBarrelPosition = new Vector2(26f, 7f),
            CenterYOffset = 9f,
            MuzzleOffset = 45f,
            RecoilDiminish = 0.9f
        };

        public override Vector2? HoldoutOffset() => new Vector2(-10, 0);

        public override bool? UseItem(Player player)
        {
            if (!Main.dedServ)
                SoundEngine.PlaySound(SFX.AssaultRifle with { Volume = 0.7f }, player.position);

            return true;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback, GunHeldProjectile heldProjectile)
        {
            var dummy = Item.Clone();
            dummy.useAmmo = AmmoID.Rocket;

            if(player.ItemUseCount(Type) % 5 == 0 && player.PickAmmo(dummy, out _, out float speed, out damage, out knockback, out _, dontConsume: false))
            {
                int projToShoot = Utility.GetRocketAmmoProjectileID(player, ItemID.RocketLauncher);
                Projectile.NewProjectile(source, position + new Vector2(30, 8 * player.direction).RotatedBy(velocity.ToRotation()), velocity, projToShoot, damage, knockback, player.whoAmI);
            }

            return true;
        }

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            position -= new Vector2(4 * player.direction, 2); // so bullets line up with the muzzle
        }
    }
}
