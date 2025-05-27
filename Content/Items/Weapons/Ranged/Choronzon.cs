using Macrocosm.Common.Bases.Projectiles;
using Macrocosm.Common.CrossMod;
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
    public class Choronzon : GunHeldProjectileItem
    {
        public override GunHeldProjectileData GunHeldProjectileData => new()
        {
            GunBarrelPosition = new Vector2(28f, 6f),
            CenterYOffset = 9f,
            MuzzleOffset = 45f,
            Recoil = (4, 0.01f),
            RecoilDiminish = 0.8f
        };

        public override void SetStaticDefaults()
        {
        }

        public override void SetDefaultsHeldProjectile()
        {
            Item.damage = 100;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 70;
            Item.height = 26;
            Item.useTime = 25;
            Item.useAnimation = 25;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.channel = true;
            Item.knockBack = 8f;
            Item.value = 10000;
            Item.rare = ModContent.RarityType<MoonRarity2>();
            Item.shoot = Macrocosm.ItemShoot_UsesAmmo;
            Item.autoReuse = true;
            Item.shootSpeed = 12f;
            Item.useAmmo = ModContent.ItemType<Ammo.RailgunBolt>();
            Item.UseSound = SFX.RailgunShot;
        }

        public override Vector2? HoldoutOffset() => new Vector2(-10, 0);

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback, GunHeldProjectile heldProjectile)
        {
            return true;
        }

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            position -= velocity * 6;
        }
    }
}
